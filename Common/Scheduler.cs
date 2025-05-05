using System;
using System.Threading;
using System.Threading.Tasks;
using Tebex.Plugin;

namespace Tebex.Common
{
    /// <summary>
    /// The Scheduler provides methods for executing Tasks at certain intervals.
    /// </summary>
    public static class Scheduler
    {
        /// <summary>
        /// Executes an action on a regular schedule. Waits the associated interval first.
        /// </summary>
        /// <param name="adapter">The PluginAdapter running this task, used for passing back log messages</param>
        /// <param name="interval">Interval between executions.</param>
        /// <param name="action">Action to execute.</param>
        /// <param name="cancellationToken">Token to stop the loop.</param>
        /// <param name="immediate">True if the task should execute immediately instead of waiting the interval first.</param>
        /// <returns>A Task representing the running loop. Cancels on token cancellation.</returns>
        public static Task ExecuteEvery(PluginAdapter adapter, TimeSpan interval, Action action, CancellationToken cancellationToken = default, bool immediate = false)
        {
            if (interval <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(interval), "Interval must be positive.");
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return Task.Run(async () =>
            {
                if (immediate)
                {
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        adapter.LogError($"Error in scheduled action: {ex}");
                    }
                }

                // Next scheduled run time
                var nextRun = DateTime.UtcNow + interval;

                while (!cancellationToken.IsCancellationRequested)
                {
                    // Wait until the next scheduled time, honoring cancellation
                    var delay = nextRun - DateTime.UtcNow;

                    if (delay > TimeSpan.Zero)
                    {
                        await Task.Delay(delay, cancellationToken);
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        adapter.LogError($"Error in scheduled action: {ex}");
                    }

                    // Schedule next run
                    nextRun += interval;
                }
            }, cancellationToken);
        }
        
        /// <summary>
        /// Executes a specified action after a defined delay in seconds.
        /// </summary>
        /// <param name="delaySeconds">The number of seconds to wait before executing the action.</param>
        /// <param name="action">The action to execute after the delay.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public static async Task ExecuteAfter(int delaySeconds, Action action)
        {
            await Task.Delay(delaySeconds);
            action();
        }
    }
}