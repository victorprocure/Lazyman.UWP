// -----------------------------------------------------------------------
// <copyright file="LifecycleLogic.cs" company="Procure Software Development">
// Copyright (c) 2018 Procure Software Development
// </copyright>

namespace UI.Framework
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;

    using UI.Framework.Caching;
    using UI.Framework.Logging;

    using Windows.ApplicationModel;
    using Windows.ApplicationModel.Activation;
    using Windows.ApplicationModel.ExtendedExecution;

    /// <summary>
    /// Represents the lifecycle of an application
    /// </summary>
    public class LifecycleLogic
    {
        /// <summary>
        /// The logging service.
        /// </summary>
        private readonly ILoggingService loggingService;

        /// <summary>
        /// The caching service.
        /// </summary>
        private readonly ICachingService cachingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LifecycleLogic"/> class.
        /// </summary>
        /// <param name="loggingService">
        /// The logging service.
        /// </param>
        /// <param name="cachingService">The caching service</param>
        public LifecycleLogic(ILoggingService loggingService, ICachingService cachingService)
        {
            this.loggingService = loggingService;
            this.cachingService = cachingService;
        }

        /// <summary>
        /// Attempt to restore a lifecycle session
        /// </summary>
        /// <param name="args">
        /// The launch arguments.
        /// </param>
        /// <param name="nav">
        /// The navigation service.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> of <see cref="bool" />. This represents whether the session was restored
        /// </returns>
        public async Task<bool> AutoRestoreAsync(ILaunchActivatedEventArgs args, INavigationService nav)
        { 
            if (Bootstrapper.DetermineStartCause(args) != AdditionalKinds.Primary
                && args?.TileId != string.Empty)
                return false;

            var restored = await nav.RestoreSavedNavigationAsync();
            this.loggingService.WriteLine(
                $"{nameof(restored)}:{restored}",
                caller: nameof(nav.RestoreSavedNavigationAsync));

            return restored;
        }

        /// <summary>
        /// The automatically suspend all frames async.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <param name="autoExtendExecutionSession">
        /// The auto extend execution session.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task AutoSuspendAllFramesAsync(
            object sender,
            SuspendingEventArgs args,
            bool autoExtendExecutionSession)
        {
            this.loggingService.WriteLine($"{nameof(autoExtendExecutionSession)}:{autoExtendExecutionSession}");

            if (autoExtendExecutionSession)
            {
                using (var session =
                    new Windows.ApplicationModel.ExtendedExecution.ExtendedExecutionSession
                        {
                            Description =
                                this.GetType()
                                    .ToString(),
                            Reason =
                                ExtendedExecutionReason
                                    .SavingData
                        })
                {
                    await this.SuspendAllFramesAsync();
                }
            }
            else
            {
                await this.SuspendAllFramesAsync();
            }
        }

        /// <summary>
        /// Suspend all frames async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task SuspendAllFramesAsync()
        {
            this.loggingService.WriteLine();

            var services = WindowWrapper.ActiveWrappers.SelectMany(x => x.NavigationServices)
                .Where(x => x.IsInMainView);
            foreach (INavigationService nav in services)
            {
                try
                {
                    nav.FrameFacade.SetFrameState(this.cachingService.CacheDateKey, DateTime.Now.ToString(CultureInfo.InvariantCulture));
                    this.loggingService.WriteLine($"Nav.FrameId:{nav.FrameFacade.FrameId}");
                    await (nav as INavigationService).GetDispatcherWrapper()
                        .DispatchAsync(async () => await nav.SuspendingAsync());
                }
                catch (Exception ex)
                {
                    this.loggingService.WriteLine(
                        $"FrameId: [{nav.FrameFacade.FrameId}] {ex} {ex.Message}",
                        caller: nameof(this.AutoSuspendAllFramesAsync));
                }
            }
        }
    }
}