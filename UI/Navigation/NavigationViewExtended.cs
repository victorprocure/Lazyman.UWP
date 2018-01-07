// -----------------------------------------------------------------------
// <copyright file="NavigationViewExtended.cs" company="Procure Software Development">
// Copyright (c) 2018 Procure Software Development
// </copyright>

namespace UI.Navigation
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Windows.UI.Core;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <inheritdoc />
    /// <summary>
    ///     Represents a navigation view with built in navigation and back stack
    /// </summary>
    public class NavigationViewExtended : NavigationView
    {
        /// <summary>
        /// The navigation content frame.
        /// </summary>
        private readonly Frame frame;

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:UI.Navigation.NavigationViewExtended" /> class.
        /// </summary>
        public NavigationViewExtended()
            : this(new Frame())
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:UI.Navigation.NavigationViewExtended" /> class.
        /// </summary>
        /// <param name="frame">
        /// The navigation content frame
        /// </param>
        public NavigationViewExtended(Frame frame)
        {
            this.frame = frame;

            this.Content = this.frame;

            this.frame.Navigated += this.OnFrameNavigatedHandlerAsync;
            this.ItemInvoked += this.OnItemInvokedHandlerAsync;

            SystemNavigationManager.GetForCurrentView().BackRequested += this.OnBackRequestedHandlerAsync;
        }

        /// <summary>
        /// Gets or sets the settings page type.
        /// </summary>
        public Type SettingsPageType { get; set; }

        /// <summary>
        /// Find navigation view item async.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when no content given
        /// </exception>
        public async Task<NavigationViewItem> FindNavigationViewItemAsync(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException(nameof(content));

            var item = await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                               () => this.MenuItems.OfType<NavigationViewItem>()
                                   .SingleOrDefault(x => x.Content != null && x.Content.Equals(content))8);

            return item;
        }

        /// <summary>
        /// Find navigation view item async.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<NavigationViewItem> FindNavigationViewItemAsync(Type type)
        {
            var item = await Task.Run(
                               () => this.MenuItems.OfType<NavigationViewItem>().SingleOrDefault(
                                   x => type.Equals(x.GetValue(NavigationProperties.PageTypeProperty))));

            return item;
        }

        /// <summary>
        /// The navigate.
        /// </summary>
        /// <param name="internalFrame">
        /// The internal frame.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public virtual async Task NavigateAsync(Frame internalFrame, Type type)
        {
            var dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => internalFrame.Navigate(type));
        }

        /// <summary>
        /// The on back requested handler async.
        /// </summary>
        /// <param name="sender">
        /// The frame
        /// </param>
        /// <param name="e">
        /// The Navigation Manager back requested data.
        /// </param>
        private async void OnBackRequestedHandlerAsync(object sender, BackRequestedEventArgs e)
        {
            await Task.Run(() => this.frame.GoBack()).ConfigureAwait(false);
        }

        /// <summary>
        /// The on frame navigated handler async.
        /// </summary>
        /// <param name="sender">
        /// The Navigation View
        /// </param>
        /// <param name="e">
        /// The frame navigated event arguments
        /// </param>
        private async void OnFrameNavigatedHandlerAsync(object sender, NavigationEventArgs e)
        {
            await this.SelectItemAsync(
                    e.SourcePageType == this.SettingsPageType
                        ? this.SettingsItem
                        : await this.FindNavigationViewItemAsync(e.SourcePageType).ConfigureAwait(false) ?? this.SelectedItem)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// The on item invoked handler async.
        /// </summary>
        /// <param name="sender">
        /// The navigation view
        /// </param>
        /// <param name="args">
        /// The Navigation View Item Invoked event arguments
        /// </param>
        private async void OnItemInvokedHandlerAsync(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked) await this.SelectItemAsync(this.SettingsItem).ConfigureAwait(false);
            else await this.SelectItemAsync(this.FindNavigationViewItemAsync(args.InvokedItem.ToString())).ConfigureAwait(false);
        }

        /// <summary>
        /// The select item async.
        /// </summary>
        /// <param name="page">
        /// The page to select.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task SelectItemAsync(object page)
        {
            if (page == this.SettingsItem)
            {
                await this.NavigateAsync(this.frame, this.SettingsPageType);
                this.SelectedItem = page;
                this.frame.BackStack.Clear();
            }
            else if (page is NavigationViewItem i)
            {
                await this.NavigateAsync(this.frame, i.GetValue(NavigationProperties.PageTypeProperty) as Type);
                this.SelectedItem = page;
                this.frame.BackStack.Clear();
            }

            this.UpdateBackButton();
        }

        /// <summary>
        /// The update back button.
        /// </summary>
        private void UpdateBackButton()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                this.frame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }
    }
}