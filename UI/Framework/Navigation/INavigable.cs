// -----------------------------------------------------------------------
// <copyright file="INavigable.cs" company="Procure Software Development">
// Copyright (c) 2018 Procure Software Development
// </copyright>
// <summary>
// Represents an element that can be navigated to
// </summary>

namespace UI.Framework.Navigation
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Represents an element that can be navigated to
    /// </summary>
    public interface INavigable
    {
        /// <summary>
        /// Called when navigable is navigated to
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="mode">
        /// The navigation mode
        /// </param>
        /// <param name="state">
        /// The session state
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state);

        /// <summary>
        /// Called when navigable navigated to has loaded
        /// </summary>
        /// <param name="suspensionState">
        /// The suspension state.
        /// </param>
        /// <param name="suspending">
        /// The is navigable suspending
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending);

        /// <summary>
        /// Called when navigable is navigating from
        /// </summary>
        /// <param name="args">
        /// The navigation event arguments
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task OnNavigatingFromAsync(NavigatingEventArgs args);

        /// <summary>
        /// Gets or sets the navigation service.
        /// </summary>
        INavigationService NavigationService { get; set; }

        /// <summary>
        /// Gets or sets the dispatcher.
        /// </summary>
        IDispatcherWrapper Dispatcher { get; set; }

        /// <summary>
        /// Gets or sets the session state.
        /// </summary>
        IStateItems SessionState { get; set; }
    }
}