// -----------------------------------------------------------------------
// <copyright file="WindowLogic.cs" company="Procure Software Development">
// Copyright (c) 2018 Procure Software Development
// </copyright>

namespace UI.Framework.Display
{
    using Windows.UI.Xaml;

    using UI.Framework.Logging;

    /// <summary>
    /// Represents application window logic
    /// </summary>
    public class WindowLogic
    {
        /// <summary>
        /// The logging service.
        /// </summary>
        private readonly ILoggingService loggingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowLogic"/> class.
        /// </summary>
        /// <param name="loggingService">
        /// The logging service.
        /// </param>
        public WindowLogic(ILoggingService loggingService)
        {
            this.loggingService = loggingService;
        }

        /// <summary>
        /// Activate a window from source
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="splashLogic">
        /// The splash logic.
        /// </param>
        public void ActivateWindow(ActivateWindowSources source, SplashLogic splashLogic)
        {
            this.loggingService.WriteLine($"Source:{source}");

            if (source != ActivateWindowSources.SplashScreen)
            {
                splashLogic.Hide();
            }

            Window.Current.Activate();
        }
    }
}