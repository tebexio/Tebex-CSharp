using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tebex.Common;
using Tebex.PluginAPI;

namespace Tebex.Plugin
{
    /// <summary>
    /// TebexCorePlugin defines the main logic and flows for a typical Tebex plugin. It can serve as both an example for
    /// implementing Tebex and a working plugin that processes command-based deliverables for Tebex packages on a game server.
    /// </summary>
    public class TebexCorePlugin
    {
        protected TebexPluginApi PluginApi;
        protected PluginAdapter PluginAdapter;
        
        /// <summary>
        /// Queue of player joins sent periodically to Tebex
        /// </summary>
        protected readonly List<JoinEvent> JoinEvents = new List<JoinEvent>();
        
        /// <summary>
        /// Queue of commands that ran successfully, periodically sent to Tebex.
        /// </summary>
        protected readonly List<Command> CompletedCommands = new List<Command>();
        
        /// <summary>
        /// Cached instance of the current store based on the secret key
        /// </summary>
        protected Store ConnectedStore;
        
        // Cached store components
        protected List<Package> Packages = new List<Package>();
        protected List<Category> Categories = new List<Category>();
        protected List<Coupon> Coupons = new List<Coupon>();
        protected List<CommunityGoal> Goals = new List<CommunityGoal>();
        protected List<Sale> Sales = new List<Sale>();
        
        // Recurring tasks to perform and their cancellation tokens
        private Task? _refreshStoreDataTask;
        private Task? _syncServerActivitiesTask;
        private Task? _queueCheckTask; // only assign in _setNextQueueCheck()
        private CancellationTokenSource _syncStoreDataCancellationToken = new CancellationTokenSource();
        private CancellationTokenSource _syncServerActivitiesCancellationToken = new CancellationTokenSource();
        private CancellationTokenSource _queueCheckCancellationToken = new CancellationTokenSource();
        
#pragma warning disable CS8618, CS9264
        private TebexCorePlugin() {} // Force using Initialize(), where non-nullable fields are set based on key
#pragma warning restore CS8618, CS9264

        /// <summary>
        /// Initializes a new instance of the <see cref="TebexCorePlugin"/> class with the specified adapter and secret key.
        /// This method connects to the Tebex store using the provided secret key and sets up necessary plugin data.
        /// Throws an exception if the connection to the Tebex store fails.
        /// </summary>
        /// <param name="adapter">The <see cref="PluginAdapter"/> instance used to interface with the plugin system.</param>
        /// <param name="key">The secret key used to authenticate and connect to the Tebex store.</param>
        /// <returns>The task which initializes the Plugin. The task result is the initialized <see cref="TebexCorePlugin"/> instance.</returns>
        public static Task<TebexCorePlugin> Initialize(PluginAdapter adapter, string key)
        {
            var plugin = new TebexCorePlugin
            {
                PluginAdapter = adapter,
                PluginApi = TebexPluginApi.Initialize(adapter, key)
            };

            var successfulStoreConnection = new TaskCompletionSource<bool>();
            plugin.PluginAdapter.LogDebug("Connecting to Tebex store...");
            plugin.PluginApi.GetInformation(store =>
            {
                plugin.PluginAdapter.LogDebug($"Successfully connected to {store.Account.Domain} as {store.Server.Name}");
                plugin.Start(store);
                successfulStoreConnection.SetResult(true);
            }, onPluginApiError => {
                successfulStoreConnection.SetResult(false);
            }, onServerError => {
                successfulStoreConnection.SetResult(false);
            }).Wait();

            if (successfulStoreConnection.Task.Result == false)
            {
                return Task.FromException<TebexCorePlugin>(new Exception("Did not successfully connect to the Tebex store. Is your secret key correct?"));
            }
            
            return Task.FromResult(plugin);
        }

        public void Start(Store storeInfo)
        {
            PluginAdapter.LogDebug("Tebex is starting...");
            ConnectedStore = storeInfo;
            SyncStoreData();
            _scheduleRecurringTasks();
        }
        
        /// <summary>
        /// Cancels all active tasks with their associated cancellation tokens.
        /// </summary>
        public void Stop()
        {
            PluginAdapter.LogDebug("Tebex is stopping...");
            _syncStoreDataCancellationToken.Cancel();
            _syncServerActivitiesCancellationToken.Cancel();
            _queueCheckCancellationToken.Cancel();
            
            _queueCheckTask?.Wait();
            _refreshStoreDataTask?.Wait();
            _syncServerActivitiesTask?.Wait();
            
            PluginAdapter.LogDebug("Tebex has stopped.");
        }
        
        private void _scheduleRecurringTasks()
        {
            // set up cancellation tokens - queue check is handled in its own func
            _syncStoreDataCancellationToken = new CancellationTokenSource();
            _syncServerActivitiesCancellationToken = new CancellationTokenSource();
            _queueCheckCancellationToken = new CancellationTokenSource();
            
            // Clear player joins and completed commands
            _syncServerActivitiesTask = Scheduler.ExecuteEvery(PluginAdapter, TimeSpan.FromSeconds(60), () =>
            {
                SendPlayerJoinEvents();
                DeleteCompletedCommands();
            }, _syncServerActivitiesCancellationToken.Token);

            // Periodically refresh store packages, sales, goals
            _refreshStoreDataTask = Scheduler.ExecuteEvery(PluginAdapter, TimeSpan.FromMinutes(30), SyncStoreData, 
                _syncStoreDataCancellationToken.Token);
            
            // Default meta.next_check is 120 seconds
            _queueCheckTask = Scheduler.ExecuteEvery(PluginAdapter,TimeSpan.FromSeconds(120), CheckCommandQueue, _queueCheckCancellationToken.Token);
        }
        
        /// <summary>
        /// Sends all events in JoinEvents to Tebex if they are present. Triggers default errors if a problem occurs.
        /// </summary>
        public void SendPlayerJoinEvents()
        {
            if (JoinEvents.Count == 0)
            {
                return;
            }
            
            PluginAdapter.LogDebug($"Sending {JoinEvents.Count} join events...");
            PluginApi.SendJoinEvents(JoinEvents, success =>
            {
                JoinEvents.Clear();
                PluginAdapter.LogDebug("Successfully cleared join events.");
            }, onPluginApiError =>
            {
                PluginAdapter.DefaultPluginError(onPluginApiError);
            }, onServerError =>
            {
                PluginAdapter.DefaultServerError(onServerError);
            });
        }

        /// <summary>
        /// The main queue check for commands. The QueueCheckTask is then updated to the received meta.next_check.
        /// </summary>
        public void CheckCommandQueue()
        {
            TaskCompletionSource<int> nextCheck = new TaskCompletionSource<int>();
            
            PluginAdapter.LogDebug("Checking command queue...");
            PluginApi.GetCommandQueue(queue =>
            {
                _handleCommandQueue(queue);
                nextCheck.SetResult(queue.Meta.NextCheck);
            }, onPluginApiError =>
            {
                nextCheck.SetException(new Exception(onPluginApiError.ErrorMessage));
            }, onServerError =>
            {
                nextCheck.SetException(new Exception(onServerError.Body));
            }).Wait();

            if (nextCheck.Task.IsFaulted) { // will try again at 2 minutes
                return;
            }

            // if successful reschedule based on the received next check
            PluginAdapter.LogDebug("Next check received was " + nextCheck.Task.Result + " seconds.");
        }

        /// <summary>
        /// Deletes all completed commands from the local cache and makes an API request to remove them from Tebex.
        /// </summary>
        public void DeleteCompletedCommands()
        {
            var commandIdsToDelete = new int[CompletedCommands.Count];
            for (var i = 0; i < CompletedCommands.Count; i++)
            {
                commandIdsToDelete[i] = CompletedCommands[i].Id;
            }

            if (commandIdsToDelete.Length == 0)
            {
                PluginAdapter.LogDebug("No completed commands to delete.");
                return;
            }
            
            PluginAdapter.LogDebug($"Deleting {CompletedCommands.Count} completed commands...");
            PluginApi.DeleteCommands(commandIdsToDelete, response => CompletedCommands.Clear(), PluginAdapter.DefaultPluginError, PluginAdapter.DefaultServerError);
        }

        /// <summary>
        /// Fetches all store information, including coupons, sales, packages, categories, and community goals.
        /// Handles API and server errors by utilizing the default error handling methods provided by the <see cref="PluginAdapter"/> instance.
        /// </summary>
        public void SyncStoreData()
        {
            PluginApi.GetAllCoupons(coupons =>
                {
                    Coupons = coupons.Data;
                    PluginAdapter.LogDebug($"Fetched {coupons.Data.Count} coupons");
                },
                onPluginApiError => PluginAdapter.DefaultPluginError(onPluginApiError),
                onServerError => PluginAdapter.DefaultServerError(onServerError));
            
            PluginApi.GetAllSales(sales =>
                {
                    
                    Sales = sales.Data;
                    PluginAdapter.LogDebug($"Fetched {sales.Data.Count} sales");
                },
                onPluginApiError => PluginAdapter.DefaultPluginError(onPluginApiError),
                onServerError => PluginAdapter.DefaultServerError(onServerError));
            
            PluginApi.GetAllPackages(true, packages =>
                {
                    Packages = packages;
                    PluginAdapter.LogDebug($"Fetched {packages.Count} packages");
                },
                onPluginApiError => PluginAdapter.DefaultPluginError(onPluginApiError),
                onServerError => PluginAdapter.DefaultServerError(onServerError));
            
            PluginApi.GetListings(response =>
                {
                    Categories = response.Categories;
                    PluginAdapter.LogDebug($"Fetched {response.Categories.Count} categories");
                },
                onPluginApiError => PluginAdapter.DefaultPluginError(onPluginApiError),
                onServerError => PluginAdapter.DefaultServerError(onServerError));
            
            PluginApi.GetAllCommunityGoals(goals =>
                {
                    Goals = goals;
                    PluginAdapter.LogDebug($"Fetched {goals.Count} community goals");
                },
                onPluginApiError => PluginAdapter.DefaultPluginError(onPluginApiError),
                onServerError => PluginAdapter.DefaultServerError(onServerError));
        }

        /// <summary>
        /// Records an event when a user joins the server by adding it to the queue of join events.
        /// </summary>
        /// <param name="userId">The unique identifier of the user joining the server.</param>
        /// <param name="ipAddress">The IP address of the user joining the server.</param>
        public void OnPlayerJoin(string userId, string ipAddress)
        {
            var joinEvent = new JoinEvent(userId, "server.join", ipAddress);
            JoinEvents.Add(joinEvent);
            PluginAdapter.LogDebug($"Added join event for {userId}. Join event queue size: {JoinEvents.Count}");
        }

        /// <summary>
        /// Initiates the checkout process for the specified package and user by generating a checkout URL through the plugin API.
        /// The user will receive a link to complete their purchase through a message provided by the plugin adapter.
        /// </summary>
        /// <param name="package">The package to be purchased, identified by its ID.</param>
        /// <param name="username">The username of the player initiating the checkout process.</param>
        public void Checkout(Package package, string username)
        {
            PluginAdapter.LogDebug("Generating checkout URL for " + username + " for package " + package.Id + "...");
            PluginApi.CreateCheckoutUrl(package.Id, username,
                checkout =>
                {
                    PluginAdapter.TellPlayer(username,
                        $"Please visit the following link to complete your purchase: {checkout.Url}");
                }, onPluginApiError => PluginAdapter.DefaultPluginError(onPluginApiError),
                onServerError => PluginAdapter.DefaultServerError(onServerError));
        }

        /// <summary>
        /// Handles the processing of commands from the provided command queue response.
        /// It determines if offline commands need to be executed and also processes the online commands accordingly.
        /// </summary>
        /// <param name="queue">The response from the command queue containing commands to process and metadata.</param>
        private void _handleCommandQueue(CommandQueueResponse queue)
        {
            if (queue.Meta.ExecuteOffline) // true if offline commands are available
            {
                PluginAdapter.LogDebug("Checking for offline commands...");
                _handleOfflineCommands();
            }
            else
            {
                PluginAdapter.LogDebug("No offline commands are queued.");
            }

            if (queue.Players.Count == 0)
            {
                PluginAdapter.LogDebug("No online commands are queued.");
                return;
            }
            
            PluginAdapter.LogDebug($"{queue.Players.Count} players have commands queued.");
            _handleOnlineCommands(queue);
        }

        /// <summary>
        /// Retrieves and handles the offline commands from Tebex.
        /// </summary>
        private void _handleOfflineCommands()
        {
            PluginApi.GetOfflineCommands(response =>
            {
                foreach (var command in response.Commands)
                {
                    PluginAdapter.LogDebug($"Executing offline command {command.Id}: '{command.CommandToRun}' | delay: {command.Conditions.Delay}");
                    
                    // Delay of 0 is executed immediately
                    Scheduler.ExecuteAfter(command.Conditions.Delay, () =>
                    {
                        var success = PluginAdapter.ExecuteCommand(command);
                        if (success)
                        {
                            CompletedCommands.Add(command);
                            PluginAdapter.LogDebug($"Offline command {command.Id} executed successfully. Completed commands: {CompletedCommands.Count}");
                        }
                        else
                        {
                            PluginAdapter.LogError($"Offline command {command.Id} failed to execute successfully.");
                        }
                    }).Wait();
                }
            }, onPluginApiError => PluginAdapter.DefaultPluginError(onPluginApiError), onServerError => PluginAdapter.DefaultServerError(onServerError)).Wait();
        }

        /// <summary>
        /// Handles the processing of commands for online players in the command queue. Players who are offline are skipped.
        /// </summary>
        /// <param name="queue">The <see cref="CommandQueueResponse"/> instance containing the list of players with pending commands and related metadata.</param>
        private void _handleOnlineCommands(CommandQueueResponse queue)
        {
            foreach (var duePlayer in queue.Players)
            {
                if (!PluginAdapter.IsPlayerOnline(duePlayer))
                {
                    PluginAdapter.LogDebug($"Player {duePlayer.Name} has commands due, but is not online. Skipping.");
                    continue;
                }

                PluginAdapter.LogDebug($"Retrieving commands for player {duePlayer.Name}...");
                PluginApi.GetOnlineCommands(duePlayer.Id, onlineCommandsForThisPlayer =>
                    {
                        PluginAdapter.LogDebug($"Player {duePlayer.Name} has {onlineCommandsForThisPlayer.Commands.Count} commands due.");
                        foreach (var command in onlineCommandsForThisPlayer.Commands)
                        {
                            PluginAdapter.LogDebug($"Executing command {command.Id} for player {duePlayer.Name}: '{command.CommandToRun}' | delay: {command.Conditions.Delay} | slots: {command.Conditions.Slots}...");
                            
                            // Schedule the command based on its delay. A delay of 0 will be executed immediately.
                            Scheduler.ExecuteAfter(command.Conditions.Delay, () =>
                            {
                                // If we have a slots requirement, verify those inventory slots are available
                                if (command.Conditions.Slots > 0 &&
                                    !PluginAdapter.PlayerHasInventorySlotsAvailable(duePlayer, command.Conditions.Slots))
                                {
                                    PluginAdapter.LogWarning(
                                        $"Player {command.Player.Username} does not have enough slots to execute command {command.Id}: '{command.CommandToRun}'",
                                        "We will try again at the next commands check.");
                                    return;
                                }

                                var success = PluginAdapter.ExecuteCommand(command);
                                if (success)
                                {
                                    CompletedCommands.Add(command);
                                    PluginAdapter.LogDebug($"Command {command.Id} executed successfully. Completed commands: {CompletedCommands.Count}");
                                }
                                else
                                {
                                    PluginAdapter.LogError($"Command {command.Id} for player {duePlayer.Name} failed to execute successfully.");
                                }
                            }).Wait();
                        }
                    }, onPluginApiError => PluginAdapter.DefaultPluginError(onPluginApiError),
                    onServerError => PluginAdapter.DefaultServerError(onServerError)).Wait();
            }
        }
    }
}