// -----------------------------------------------------------------------
// <copyright file="INavigationService.cs" company="Procure Software Development">
// Copyright (c) 2018 Procure Software Development
// </copyright>
// <summary>
// Represents a navigation service
// </summary>

namespace UI.Framework.Navigation
{
    using System;
    using System.Threading.Tasks;
    
    using Windows.UI.ViewManagement;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media.Animation;

    using UI.Framework.Serialization;

    /// <summary>
    ///     Represents a navigation service
    /// </summary>
    public interface INavigationService
    {
        event TypedEventHandler<Type> AfterRestoreSavedNavigation;

        bool CanGoBack { get; }

        bool CanGoForward { get; }

        object Content { get; }

        object CurrentPageParam { get; }

        Type CurrentPageType { get; }

        DispatcherWrapper Dispatcher { get; }

        Frame Frame { get; }

        FrameFacade FrameFacade { get; }

        /// <summary>
        ///     Gets a value indicating whether this instance of INavigationService associated with
        ///     <see cref="Windows.ApplicationModel.Core.CoreApplication.MainView" /> or any other secondary view.
        /// </summary>
        bool IsInMainView { get; }

        string NavigationState { get; set; }

        ISerializationService SerializationService { get; }

        void ClearCache(bool removeCachedPagesInBackStack = false);

        void ClearHistory();

        void GoBack(NavigationTransitionInfo infoOverride = null);

        void GoForward();

        Task<bool> LoadAsync();

        void Navigate(Type page, object parameter = null, NavigationTransitionInfo infoOverride = null);

        void Navigate<T>(T key, object parameter = null, NavigationTransitionInfo infoOverride = null)
            where T : struct, IConvertible;

        Task<bool> NavigateAsync(Type page, object parameter = null, NavigationTransitionInfo infoOverride = null);

        Task<bool> NavigateAsync<T>(T key, object parameter = null, NavigationTransitionInfo infoOverride = null)
            where T : struct, IConvertible;

        Task<ViewLifetimeControl> OpenAsync(
            Type page,
            object parameter = null,
            string title = null,
            ViewSizePreference size = ViewSizePreference.UseHalf);

        void Refresh();

        void Refresh(object param);

        [Obsolete]
        Task<bool> RestoreSavedNavigationAsync();

        void Resuming();

        Task SaveAsync();

        [Obsolete]
        Task SaveNavigationAsync();

        Task SuspendingAsync();
    }
}