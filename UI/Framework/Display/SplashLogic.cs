// -----------------------------------------------------------------------
// <copyright file="SplashLogic.cs" company="Procure Software Development">
// Copyright (c) 2018 Procure Software Development
// </copyright>

namespace UI.Framework.Display
{
    using System;

    using Windows.ApplicationModel.Activation;

    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;

    /// <summary>
    /// Represents splash screen logic
    /// </summary>
    public class SplashLogic
    {
        /// <summary>
        /// The splash screen popup.
        /// </summary>
        private Popup popup;

        /// <summary>
        /// Is splash screen visible
        /// </summary>
        public bool Splashing => this.popup?.IsOpen ?? false;

        /// <summary>
        /// Show the splash screen
        /// </summary>
        /// <param name="splashScreen">
        /// The splash screen.
        /// </param>
        /// <param name="splashFactory">
        /// The splash screen factory.
        /// </param>
        /// <param name="windowLogic">
        /// The window logic.
        /// </param>
        public void Show(
            SplashScreen splashScreen,
            Func<SplashScreen, UserControl> splashFactory,
            WindowLogic windowLogic)
        {
            if (splashFactory == null)
                return;

            var splash = splashFactory(splashScreen);

            var service = new PopupService();
            this.popup = service.Show(PopupService.PopupSize.FullScreen, splash);
            windowLogic.ActivateWindow(ActivateWindowSources.SplashScreen, this);
        }

        /// <summary>
        /// Hide the splash screen
        /// </summary>
        public void Hide()
        {
            this.popup?.Hide();
        }
    }
}