// -----------------------------------------------------------------------
// <copyright file="FrameFacade.cs" company="Procure Software Development">
// Copyright (c) 2018 Procure Software Development
// </copyright>
// <summary>
// Represents a navigation frame
// </summary>

namespace UI.Framework.Navigation
{
    using System;
    using System.Collections.Generic;

    using UI.Framework.Logging;

    using Windows.ApplicationModel.Resources.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media.Animation;
    using Windows.UI.Xaml.Navigation;

    using UI.Framework.Serialization;

    /// <summary>
    ///     Represents a navigation frame
    /// </summary>
    public class FrameFacade
    {
        private readonly ILoggingService loggingService;

        private readonly IList<EventHandler<NavigatedEventArgs>> navigatedEventHandlers =
            new List<EventHandler<NavigatedEventArgs>>();

        private readonly IList<EventHandler<NavigatingEventArgs>> navigatingEventHandlers =
            new List<EventHandler<NavigatingEventArgs>>();

        internal FrameFacade(INavigationService navigationService, Frame frame, ILoggingService loggingService)
        {
            this.loggingService = loggingService;

            this.NavigationService = navigationService;
            this.Frame = frame;
            frame.Navigated += this.FacadeNavigatedEventHandler;
            frame.Navigating += this.FacadeNavigatingCancelEventHandler;

            this.InitializeAnimations();
        }

        public event EventHandler<HandledEventArgs> BackRequested;

        public event EventHandler<NavigatedEventArgs> Navigated
        {
            add
            {
                if (!this.navigatedEventHandlers.Contains(value)) this.navigatedEventHandlers.Add(value);
            }

            remove
            {
                if (this.navigatedEventHandlers.Contains(value)) this.navigatedEventHandlers.Add(value);
            }
        }

        public event EventHandler<NavigatingEventArgs> Navigating
        {
            add
            {
                if (!this.navigatingEventHandlers.Contains(value)) this.navigatingEventHandlers.Add(value);
            }

            remove
            {
                if (this.navigatingEventHandlers.Contains(value)) this.navigatingEventHandlers.Remove(value);
            }
        }

        public BackButton BackButtonHandling { get; internal set; }

        public int BackStackDepth => this.Frame.BackStackDepth;

        public bool CanGoBack => this.Frame.CanGoBack;

        public bool CanGoForward => this.Frame.CanGoForward;

        public object Content => this.Frame.Content;

        public object CurrentPageParam { get; internal set; }

        public Type CurrentPageType { get; internal set; }

        public Frame Frame { get; }

        public string FrameId { get; set; } = string.Empty;

        public NavigationMode NavigationModeHint { get; private set; } = NavigationMode.New;

        internal INavigationService NavigationService { get; set; }

        internal ISerializationService serializationService => this.NavigationService.SerializationService;

        public void ClearFrameState()
        {
            this.FrameStateSettingsService().Clear();
        }

        public void ClearPageState(Type type)
        {
            this.FrameStateSettingsService().Remove(this.GetPageStateKey(this.FrameId, type, this.BackStackDepth));
        }

        public void ClearValue(DependencyProperty dp)
        {
            this.Frame.ClearValue(dp);
        }

        public string GetFrameState(string key, string otherwise)
        {
            return this.FrameStateSettingsService().Read(key, otherwise);
        }

        public object GetValue(DependencyProperty dp)
        {
            return this.Frame.GetValue(dp);
        }

        public void GoBack(NavigationTransitionInfo infoOverride = null)
        {
            this.loggingService.WriteLine($"CanGoBack:{this.CanGoBack}");

            this.NavigationModeHint = NavigationMode.Back;

            if (!this.CanGoBack) return;

            if (infoOverride == null) this.Frame.GoBack();
            else this.Frame.GoBack(infoOverride);
        }

        public void GoForward()
        {
            this.loggingService.WriteLine($"CanGoForward {this.CanGoForward}");

            this.NavigationModeHint = NavigationMode.Forward;

            if (this.CanGoForward)
                this.Frame.GoForward();
        }

        public bool Navigate(Type page, object parameter, NavigationTransitionInfo infoOverride)
        {
            this.loggingService.WriteLine();

            if (this.Frame.Navigate(page, parameter, infoOverride)) return page == this.Frame.Content?.GetType();

            return false;
        }

        public ISettingsService PageStateSettingsService(Type type)
        {
            return this.FrameStateSettingsService().Open(
                this.GetPageStateKey(this.FrameId, type, this.BackStackDepth),
                true);
        }

        public ISettingsService PageStateSettingsService(string key)
        {
            return this.FrameStateSettingsService().Open(key, true);
        }

        public void RaiseBackRequested(HandledEventArgs args)
        {
            this.BackRequested?.Invoke(this, args);

            if (this.BackButtonHandling == BackButton.Attach && !args.Handled
                                                             && (args.Handled = this.Frame.BackStackDepth > 0))
                this.GoBack();
        }

        public void Refresh()
        {
            this.loggingService.WriteLine();

            this.NavigationModeHint = NavigationMode.Refresh;

            try
            {
                var context = this.Frame.DataContext;

                ResourceContext.GetForCurrentView().Reset();

                var state = this.Frame.GetNavigationState();

                this.Frame.SetNavigationState(state);

                this.Frame.DataContext = context;
            }
            catch (Exception)
            {
                if (this.Frame.CanGoBack)
                {
                    this.Frame.GoBack();
                    this.Frame.GoForward();
                }
                else if (this.Frame.CanGoForward)
                {
                    this.Frame.GoForward();
                    this.Frame.GoBack();
                }
                else
                {
                    (this.Frame.Content as Page)?.UpdateLayout();
                }
            }
        }

        public void Refresh(object param)
        {
            this.loggingService.WriteLine();

            try
            {
                var context = this.Frame.DataContext;
                ResourceContext.GetForCurrentView().Reset();

                this.Frame.Navigate(this.CurrentPageType, param, new SuppressNavigationTransitionInfo());
                this.Frame.DataContext = context;
            }
            catch (Exception)
            {
                if (this.Frame.CanGoBack) this.Frame.GoBack();
                else if (this.Frame.CanGoForward)
                    this.Frame.GoForward();
                else
                    (this.Frame.Content as Page)?.UpdateLayout();
            }
        }

        public void SetFrameState(string key, string value)
        {
            this.FrameStateSettingsService().Write(key, value);
        }

        public void SetValue(DependencyProperty dp, object value)
        {
            this.Frame.SetValue(dp, value);
        }

        private void FacadeNavigatedEventHandler(object sender, NavigationEventArgs args)
        {
            this.loggingService.WriteLine();

            this.CurrentPageType = args.SourcePageType;
            this.CurrentPageParam = this.serializationService.Deserialize(args.Parameter?.ToString());

            var navigatedArgs = new NavigatedEventArgs(args, this.Content as Page);

            if (this.NavigationModeHint != NavigationMode.New)
                navigatedArgs.NavigationMode = this.NavigationModeHint;

            this.NavigationModeHint = NavigationMode.New;

            foreach (var handler in this.navigatedEventHandlers) handler(this, navigatedArgs);
        }

        private async void FacadeNavigatingCancelEventHandler(object sender, NavigatingCancelEventArgs args)
        {
            this.loggingService.WriteLine();

            object parameter = null;

            try
            {
                parameter = this.serializationService.Deserialize(args.Parameter?.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("Parameter must be serializable", ex);
            }

            var deferral = new DeferralManager();
            var navigatingArgs = new NavigatingEventArgs(
                deferral,
                args,
                this.Content as Page,
                args.SourcePageType,
                parameter,
                args.Parameter);

            if (this.NavigationModeHint != NavigationMode.New)
                navigatingArgs.NavigationMode = this.NavigationModeHint;

            this.NavigationModeHint = NavigationMode.New;

            foreach (var handler in this.navigatingEventHandlers) handler(this, navigatingArgs);

            await deferral.WaitForDeferralsAsync().ConfigureAwait(false);

            args.Cancel = navigatingArgs.Cancel;
        }

        private ISettingsService FrameStateSettingsService()
        {
            return this.SettingsService.SettingsService.Create(SettingsStrategies.Local, this.GetFrameStateKey(), true);
        }

        private string GetFrameStateKey()
        {
            return $"{this.FrameId}-PageState";
        }

        private string GetPageStateKey(string frameId, Type type, int backStackDepth)
        {
            return $"{frameId}-{type}-{backStackDepth}";
        }

        private void InitializeAnimations()
        {
            var t = new NavigationThemeTransition
                        {
                            DefaultNavigationTransitionInfo =
                                new EntranceNavigationTransitionInfo()
                        };
            this.Frame.ContentTransitions = new TransitionCollection { t };
        }
    }
}