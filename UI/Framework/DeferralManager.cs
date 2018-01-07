// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeferralManager.cs" company="Procure Software Development">
//   Copyright (c) 2018 Procure Software Development
// </copyright>
// <summary>
//   Represents the manager for deferrals
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UI.Framework
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the manager for deferrals
    /// </summary>
    public sealed class DeferralManager
    {
        /// <summary>
        /// The completed task.
        /// </summary>
        private readonly TaskCompletionSource<object> completed = new TaskCompletionSource<object>();

        /// <summary>
        /// The count.
        /// </summary>
        private int count;

        /// <summary>
        /// The get deferral.
        /// </summary>
        /// <returns>
        /// The <see cref="Deferral"/>.
        /// </returns>
        public Deferral GetDeferral()
        {
            Interlocked.Increment(ref this.count);

            return new Deferral(
                () =>
                    {
                        var localCount = Interlocked.Decrement(ref this.count);
                        if (localCount == 0)
                            this.completed.SetResult(null);
                    });
        }

        /// <summary>
        /// Is deferred task complete
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsComplete()
        {
            return this.WaitForDeferralsAsync().IsCompleted;
        }

        /// <summary>
        /// Wait for deferrals async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task WaitForDeferralsAsync()
        {
            return this.count == 0 ? Task.CompletedTask : this.completed.Task;
        }
    }
}