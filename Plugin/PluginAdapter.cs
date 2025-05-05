using System;
using System.Threading.Tasks;
using Tebex.Common;
using Tebex.PluginAPI;

namespace Tebex.Plugin
{
    /// <summary>
    /// The PluginAdapter defines operations needed to interface a Tebex plugin into a given platform/game.
    ///
    /// For example, some platforms may not allow us to use the standard library for sending HTTP requests.
    ///
    /// Each unique game may also have its own inventory system, chat system, etc., and a PluginAdapter will provide
    /// the interface for interacting with those functions.
    ///
    /// A PluginAdapter is meant to be a member of a TebexCorePlugin, through which the core plugin will facilitate Tebex
    /// functionality.
    /// </summary>
    public abstract class PluginAdapter
    {
        /// <summary>
        /// Sends an HTTP request.
        /// </summary>
        /// <param name="key">The plugin secret key.</param>
        /// <param name="url">The URL of the request</param>
        /// <param name="body">A JSON string representing the HTTP request body</param>
        /// <param name="verb">The type of HTTP request (GET/POST/PUT/DELETE)</param>
        /// <param name="onSuccess">Callback for successful responses</param>
        /// <param name="onPluginApiError">Callback for 400-level errors, provides a nicely formatted error</param>
        /// <param name="onServerError">Callback for unexpected or 500-level errors.</param>
        /// <typeparam name="TReturnType">The object type we expect to be returned on success.</typeparam>
        /// <returns>A Task representing the Send operation.</returns>
        public abstract Task Send<TReturnType>(string key, string url, string body, HttpVerb verb, ApiSuccessCallback onSuccess, Action<PluginApiError> onPluginApiError, Action<ServerError> onServerError);

        /// <summary>
        /// Checks if a player has the necessary number of inventory slots.
        /// </summary>
        /// <param name="player">The player whose inventory needs to be checked</param>
        /// <param name="slots">The number of slots required</param>
        /// <returns>True if we have enough available slots</returns>
        public abstract bool PlayerHasInventorySlotsAvailable(DuePlayer player, int slots);
        
        /// <summary>
        /// Runs a due command which was attached to a purchased package.
        /// </summary>
        /// <param name="command">The command to execute, see command.CommandToRun</param>
        /// <returns>True if the command runs successfully. Causes the command to be added to the list
        /// of commands to mark completed. If the command does not run successfully, it will not be removed
        /// from the queue until removed manually by the store operator or the command eventually runs successfully.</returns>
        public abstract bool ExecuteCommand(Command command);
        
        /// <summary>
        /// Checks if a given player is online.
        /// </summary>
        /// <param name="player">The player whose online status to check.</param>
        /// <returns>True if the player is logged in and in-game.</returns>
        public abstract bool IsPlayerOnline(DuePlayer player);

        /// <summary>
        /// Return true if the plugin is considered in debug mode, which will trigger all internal debug logging to appear.
        /// This includes the content of all requests and responses sent to and from the API.
        /// </summary>
        public abstract bool IsDebugModeEnabled();
        
        /// <summary>
        /// Sends a message to a particular player.
        /// </summary>
        /// <param name="userId">The user's ID or username, for looking up the player instance.</param>
        /// <param name="message">The message to send to the player.</param>
        public abstract void TellPlayer(string userId, string message);

        /// <summary>
        /// Logs details of a Plugin API error to the console. Replace with your own logger if you wish.
        /// </summary>
        /// <param name="error">The PluginApiError object representing the error details, including the error code and message.</param>
        public virtual void DefaultPluginError(PluginApiError error) => LogError(error.ToString());

        /// <summary>
        /// Handles server errors by logging the error details to console. Replace with your own logger if you wish.
        /// </summary>
        /// <param name="error">The server error containing the response code and body.</param>
        public virtual void DefaultServerError(ServerError error) => LogError(error.ToString());

        /// <summary>
        /// Tebex standard is that all Warning level events must be something resolvable either by
        /// the plugin operator or over an amount of time. Each warning is intended to have an explanation
        /// solution that informs the operator what they must do, if anything, to stop the warning.
        /// </summary>
        /// <param name="warning">The warning message.</param>
        /// <param name="solution">A potential solution for the problem, ex. ("We will try again in 5 minutes.")</param>
        public virtual void LogWarning(string warning, string solution)
        {
            Console.WriteLine(warning);
            Console.WriteLine("- " + solution);
        }
        
        /// <summary>
        /// Logs a standard information message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public virtual void LogInfo(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Logs a debug message, if debug mode is enabled.
        /// </summary>
        /// <param name="message"></param>
        public virtual void LogDebug(string message)
        {
            if (IsDebugModeEnabled())
            {
                Console.WriteLine("[DEBUG] " + message);    
            }
        }
        
        public virtual void LogError(string message)
        {
            Console.WriteLine("[ERROR] " + message);
        }
    }
}