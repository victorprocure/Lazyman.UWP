// -----------------------------------------------------------------------
// <copyright file="Bootstrapper.cs" company="Procure Software Development">
// Copyright (c) 2018 Procure Software Development
// </copyright>


namespace UI.Framework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using Windows.ApplicationModel;
    using Windows.ApplicationModel.Activation;
    using Windows.Foundation.Metadata;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    using UI.Framework.Caching;
    using UI.Framework.Caching.Strategies;
    using UI.Framework.Display;
    using UI.Framework.Logging;
    using UI.Framework.Logging.Strategies;

    /// <summary>
    /// Represents the bootstrapping needed for an Application
    /// </summary>
    public abstract class Bootstrapper: Application, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value)) return;

            storage = value;
            this.RaisePropertyChanged(propertyName);
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null) =>
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public virtual INavigable ResolveForPage(Page page, NavigationService navigationService) => null;

        public static new Bootstrapper Current { get; private set; }

        public StateItems SessionState { get; set; } = new StateItems();

        private static ILoggingService loggingService { get; set; } = new DefaultLoggingService();

        private static ICachingService cachingService { get; set; } = new DefaultCachingService();

        private static void DebugWrite(
            string text = null,
            Severities severity = Severities.Debug,
            [CallerMemberName] string caller = null)
        {

            Bootstrapper.loggingService.WriteLine(text, severity, caller: $"Bootstrapper.{caller}");
        }

        public Bootstrapper()
        {
            Bootstrapper.DebugWrite("base.Constructor");

            Bootstrapper.Current = this;
            this.Resuming += this.CallResuming;
            this.Suspending += this.CallHandleSuspendingAsync;
        }

        public Bootstrapper(ICachingService cachingService) : this(null, cachingService) { }

        public Bootstrapper(ILoggingService loggingService = null, ICachingService cachingService = null)
        {
            if(loggingService != null)
                Bootstrapper.loggingService = loggingService;

            if (cachingService != null)
                Bootstrapper.cachingService = cachingService;
        }
        
        private void Loaded()
        {
            Bootstrapper.DebugWrite();

            var keyboardService = KeyboardService.Instance;
            keyboardService.AfterBackGesture = () =>
                {
                    Bootstrapper.DebugWrite(caller: nameof(keyboardService.AfterBackGesture));

                    var handled = false;
                    this.RaiseBackRequested(ref handled);
                };

            keyboardService.AfterForwardGesture = () =>
                {
                    Bootstrapper.DebugWrite(caller: nameof(keyboardService.AfterForwardGesture));

                    this.RaiseForwardRequested();
                };

            SystemNavigationManager.GetForCurrentView().BackRequested += this.BackHandler;
        }

        public event EventHandler<WindowCreatedEventArgs> WindowCreated;

        protected override sealed void OnWindowCreated(WindowCreatedEventArgs args)
        {
            Bootstrapper.DebugWrite();

            if(!WindowWrapper.ActiveWrapper.Any())
                this.Loaded();

            var window = new WindowWrapper(args.Window);
            ViewService.OnWindowCreated();
            this.WindowCreated?.Invoke(this, args);

            base.OnWindowCreated(args);
        }

        public INavigationService NavigationService => WindowWrapper.Current().NavigationServices.FirstOrDefault();

        public Func<SplashScreen, UserControl> SplashFactory { get; protected set; }
        
        public bool ShowShellBackButton { get; set; } = true;

        public bool ForceShowShellBackButton { get; set; } = false;

        protected override sealed void OnActivated(IActivatedEventArgs args) => CallInternalActivatedAsync(args);

        protected override sealed void OnCachedFileUpdaterActivated(CachedFileUpdaterActivatedEventArgs args) =>
            CallInternalActivatedAsync(args);

        protected override sealed void OnFileActivated(FileActivatedEventArgs args) => CallInternalActivatedAsync(args);

        protected override sealed void OnFileOpenPickerActivated(FileOpenPickerActivatedEventArgs args) => CallInternalActivatedAsync(args);

        protected override sealed void OnFileSavePickerActivated(FileSavePickerActivatedEventArgs args) => CallInternalActivatedAsync(args);

        protected override sealed void OnSearchActivated(SearchActivatedEventArgs args) => CallInternalActivatedAsync(args);

        protected override sealed void OnShareTargetActivated(ShareTargetActivatedEventArgs args) => CallInternalActivatedAsync(args);

        private async void CallInternalActivatedAsync(IActivatedEventArgs args,[CallerMemberName] string caller = null)
        {
            Bootstrapper.DebugWrite(caller: caller);

            this.CurrentState = ApplicationState.BeforeActivate;

            await this.InternalActivatedAsync(args);

            this.CurrentState = ApplicationState.AfterActivate;
        }

        private async Task InternalActivatedAsync(IActivatedEventArgs args)
        {
            Bootstrapper.DebugWrite();
            this.OriginalActivatedArgs = args;

            if (Window.Current.Content == null)
            {
                Bootstrapper.DebugWrite("Calling", caller: nameof(this.InternalActivatedAsync));
                await this.InitializeFrameAsync(args);
            }

            await this.CallOnStartAsync(true, StartKind.Activate);

            this.CallActivateWindow(ActivateWindowSources.Activating);
        }

        protected override sealed void OnLaunched(LaunchActivatedEventArgs args)
        {
            Bootstrapper.DebugWrite();
            this.CallInternalLaunchAsync(args);
        }

        private async void CallInternalLaunchAsync(ILaunchActivatedEventArgs args)
        {
            this.CurrentState = ApplicationState.BeforeLaunch;

            await this.InternalLaunchAsync(args);

            this.CurrentState = ApplicationState.AfterLaunch;
        }

        private async Task InternalLaunchAsync(ILaunchActivatedEventArgs args)
        {
            Bootstrapper.DebugWrite($"Previous: { args.PreviousExecutionState.ToString() }");

            this.OriginalActivatedArgs = args;

            if (args.PreviousExecutionState != ApplicationExecutionState.Running)
            {
                try
                {
                    await this.InitializeFrameAsync(args);
                }
                catch (Exception ex)
                {
                    Bootstrapper.DebugWrite($"InternalLaunch Exception:{ex.Message}");
                }
            }

            var restored = false;
            switch (args.PreviousExecutionState)
            {
                case ApplicationExecutionState.Suspended:
                case ApplicationExecutionState.Terminated:
                    {
                        this.OnResuming(this, null, AppExecutionState.Terminated);

                        restored = await this.CallAutoRestoreAsync(args, restored);
                        break;
                    }
            }

            if (!restored)
            {
                var kind = args.PreviousExecutionState == ApplicationExecutionState.Running
                               ? StartKind.Activate
                               : StartKind.Launch;
                await this.CallOnStartAsync(true, kind);
            }

            this.CallActivateWindow(ActivateWindowSources.Launching);
        }

        private void BackHandler(object sender, BackRequestedEventArgs args)
        {
            Bootstrapper.DebugWrite();

            var handled = false;
            if (ApiInformation.IsApiContractPresent(nameof(Windows.Phone.PhoneContract), 1, 0))
            {
                if (this.NavigationService?.CanGoBack == true)
                {
                    handled = true;
                }
            }
            else
            {
                handled = this.NavigationService?.CanGoBack == false;
            }

            this.RaiseBackRequested(ref handled);
            args.Handled = handled;
        }

        private void RaiseBackRequested(ref bool handled)
        {
            Bootstrapper.DebugWrite();

            var args = new HandledEventArgs();
            Bootstrapper.BackRequested?.Invoke(null, args);

            if (handled = args.Handled)
                return;

            foreach (var frame in WindowWrapper.Current().NavigationServices.Select(x => x.FrameFacade).Reverse())
            {
                frame.RaiseBackRequested(args);
                if (handled = args.Handled)
                    return;
            }

            this.NavigationService.GoForward();
        }

        public static event EventHandler<HandledEventArgs> BackRequested;

        private void RaiseForwardRequested()
        {
            Bootstrapper.DebugWrite();

            var args = new HandledEventArgs();
            Bootstrapper.ForwardRequested?.Invoke(null, args);

            if (args.Handled)
                return;

            foreach (var frame in WindowWrapper.Current().NavigationServices.Select(x => x.FrameFacade))
            {
                frame.RaiseForwardRequested(args);
                if (args.Handled)
                    return;
            }

            this.NavigationService.GoForward();
        }

        public void UpdateShellBackButton()
        {
            Bootstrapper.DebugWrite();

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                (this.ShowShellBackButton && (this.NavigationService.CanGoBack || this.ForceShowShellBackButton))
                    ? AppViewBackButtonVisibility.Visible
                    : AppViewBackButtonVisibility.Collapsed;
            this.ShellBackButtonUpdated?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler ShellBackButtonUpdated;

        public static event EventHandler<HandledEventArgs> ForwardRequested;

        public virtual Task OnPrelaunchAsync(IActivatedEventArgs args, out bool runOnStartAsync)
        {
            Bootstrapper.DebugWrite("Virtual");

            runOnStartAsync = false;

            return Task.CompletedTask;
        }

        private bool hasOnPrelaunchBeenCalledAsync = false;

        public abstract Task OnStartAsync(StartKind startKind, IActivatedEventArgs args);

        public virtual Task OnInitializeAsync(IActivatedEventArgs args)
        {
            Bootstrapper.DebugWrite($"Virtual {nameof(IActivatedEventArgs)}:{args.Kind}");

            return Task.CompletedTask;
        }

        public virtual Task OnSuspendingAsync(object sender, SuspendingEventArgs args, bool prelaunchActivated)
        {
            Bootstrapper.DebugWrite(
                $"Virtual {nameof(SuspendingEventArgs)}:{args.SuspendingOperation} {nameof(prelaunchActivated)}:{prelaunchActivated}");

            return Task.CompletedTask;
        }

        public virtual void OnResuming(object sender, object args, AppExecutionState previousExecutionState)
        {
            Bootstrapper.DebugWrite($"Virtual, {nameof(previousExecutionState)}:{previousExecutionState}");
        }

        public INavigationService NavigationServiceFactory(BackButton backButton, ExistingContent existingContent)
        {
            Bootstrapper.DebugWrite($"{nameof(BackButton)}:{backButton} {nameof(ExistingContent)}:{existingContent}");

            return NavigationServiceFactory(backButton, existingContent, new Frame());
        }

        protected virtual INavigationService CreateNavigationService(Frame frame)
        {
            Bootstrapper.DebugWrite($"Frame:{frame}");

            return new NavigationService(frame);
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public INavigationService NavigationServiceFactory(
            BackButton backButton,
            ExistingContent existingContent,
            Frame frame)
        {
            Bootstrapper.DebugWrite(
                $"{nameof(BackButton)}:{backButton} {nameof(ExistingContent)}:{existingContent} {nameof(Frame)}:{frame}");

            frame.Content = (existingContent == ExistingContent.Include) ? Window.Current.Content : null;

            foreach (var nav in WindowWrapper.ActiveWrappers.SelectMany(x=>x.NavigationServices))
            {
                if (nav.FrameFacade.Frame.Equals(frame))
                    return nav as INavigationService;
            }

            var navigationService = CreateNavigationService(frame);
            navigationService.FrameFacade.BackButtonHandling = backButton;
            WindowWrapper.Current().NavigationServices.Add(navigationService);

            if (backButton == BackButton.Attach)
            {
                frame.RegisterPropertyChangedCallback(
                    Frame.BackStackDepthProperty,
                    (s, args) => this.UpdateShellBackButton());

                frame.Navigated += (s, args) => this.UpdateShellBackButton();
            }


            var otherwise = DateTime.MinValue.ToString(CultureInfo.InvariantCulture);

            if (!DateTime.TryParse(navigationService.FrameFacade.GetFrameState(Bootstrapper.cachingService.CacheDateKey, otherwise), out var cacheDate))
                return navigationService;

                var cacheAge = DateTime.Now.Subtract(cacheDate);

                if (cacheAge < Bootstrapper.cachingService.CacheMaxDuration)
                    return navigationService;

                foreach (var nav in WindowWrapper.ActiveWrappers.SelectMany(x => x.NavigationServices))
                {
                    nav.FrameFacade.ClearFrameState();
                }
            

            return navigationService;
        }

        private ApplicationState currentState = ApplicationState.None;

        public ApplicationState CurrentState
        {
            get => this.currentState;
            set
            {
                Bootstrapper.DebugWrite($"CurrentState changed to {value}");
                this.currentStateHistory.Add($"{DateTime.Now}-{Guid.NewGuid()}", value);
                this.currentState = value;
            }
        }

        private Dictionary<string, ApplicationState> currentStateHistory = new Dictionary<string, ApplicationState>();

        private async Task InitializeFrameAsync(IActivatedEventArgs args)
        {
            Bootstrapper.DebugWrite($"{nameof(IActivatedEventArgs)}:{args.Kind}");

            this.CallShowSplashScreen(args);

            await this.CallOnInitializeAsync(false, args);

            this.SetupCustomTitleBar();

            if (this.splashLogic.Splashing || Window.Current.Content == null)
            {
                Window.Current.Content = this.CreateRootElement(args);
            }
        }

        private WindowLogic windowLogic = new WindowLogic(Bootstrapper.loggingService);

        private void CallActivateWindow(ActivateWindowSources source)
        {
            this.windowLogic.ActivateWindow(source, this.splashLogic);

            this.currentState = ApplicationState.Running;
        }

        public virtual UIElement CreateRootElement(IActivatedEventArgs args)
        {
            var navigationService = Bootstrapper.Current.NavigationServiceFactory(
                BackButton.Attach,
                ExistingContent.Include,
                new Frame());

            return new Controls.ModalDialog { DisableBackButtonWhenModal = true, Content = navigationService.Frame };
        }

        private void SetupCustomTitleBar()
        {
            Bootstrapper.InitResourceDueToPlatformBug();

            var count = Application.Current.Resources.Count;
            foreach (var currentResource in Application.Current.Resources)
            {
                var key = currentResource.Key;
                if (object.Equals(key, typeof(Controls.CustomTitleBar)))
                {
                    var style = currentResource.Value as Style;
                    var title = new Controls.CustomTitleBar();
                    title.Style = style;
                }

                count--;
                if (count == 0) break;
            }
        }

        private static void InitResourceDueToPlatformBug()
        {
            try
            {
                if (Application.Current.Resources.ContainsKey("ExtendedSplashBackground"))
                {
                    var unused = Application.Current.Resources["ExtendedSplashBackground"];
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {

            }
        }

        private async Task CallOnInitializeAsync(bool canRepeat, IActivatedEventArgs args)
        {
            Bootstrapper.DebugWrite();

            if (!canRepeat && this.currentStateHistory.ContainsValue(ApplicationState.BeforeInit))
                return;

            this.CurrentState = ApplicationState.BeforeInit;

            await this.OnInitializeAsync(args);

            this.CurrentState = ApplicationState.AfterInit;
        }

        private async Task CallOnStartAsync(bool canRepeat, StartKind startKind)
        {
            Bootstrapper.DebugWrite();

            if (!canRepeat && this.currentStateHistory.ContainsValue(ApplicationState.BeforeStart))
                return;

            this.CurrentState = ApplicationState.BeforeStart;
            while (!this.currentStateHistory.ContainsValue(ApplicationState.AfterInit))
            {
                await Task.Delay(500);
            }

            await OnStartAsync(startKind, this.OriginalActivatedArgs);
            this.CurrentState = ApplicationState.AfterStart;
        }

        private SplashLogic splashLogic = new SplashLogic();

        private void CallShowSplashScreen(IActivatedEventArgs args)
        {
            Bootstrapper.DebugWrite();

            this.splashLogic.Show(args.SplashScreen, this.SplashFactory, this.windowLogic);
        }

        [Obsolete("Use RootElementFactory.", true)]
        protected virtual Frame CreateRootFrame(IActivatedEventArgs args)
        {
            Bootstrapper.DebugWrite($"{nameof(IActivatedEventArgs)}:{args}");

            return new Frame();
        }

        public bool AutoRestoreAfterTerminated { get; set; } = true;

        public bool AutoExtendExecutionSession { get; set; } = true;

        public bool AutoSuspendAllFrames { get; set; } = true;

        LifecycleLogic lifecycleLogic = new LifecycleLogic();

        private async void CallResuming(object sender, object e)
        {
            Bootstrapper.DebugWrite(caller: nameof(Resuming));

            var args = this.OriginalActivatedArgs as LaunchActivatedEventArgs;

            if (!(args?.PrelaunchActivated ?? true))
            {
                this.OnResuming(sender, e, AppExecutionState.Suspended);
                return;
            }

            this.OnResuming(sender, e, AppExecutionState.Prelaunch);

            var kind = args?.PreviousExecutionState == ApplicationExecutionState.Running
                           ? StartKind.Activate
                           : StartKind.Launch;

            await this.CallOnStartAsync(false, kind);

            this.CallActivateWindow(ActivateWindowSources.Resuming);
            
        }

        private async Task<bool> CallAutoRestoreAsync(ILaunchActivatedEventArgs args, bool restored)
        {
            if (!this.AutoRestoreAfterTerminated)
                return false;

            return await this.lifecycleLogic.AutoRestoreAsync(args, this.NavigationService);
        }

        private async void CallHandleSuspendingAsync(object sender, SuspendingEventArgs args)
        {
            var deferral = args.SuspendingOperation.GetDeferral();

            try
            {
                if (this.AutoSuspendAllFrames)
                {
                    await this.lifecycleLogic.AutoSuspendAllFramesAsync(sender, args, this.AutoExtendExecutionSession);
                }

                await this.OnSuspendingAsync(
                    sender,
                    args,
                    (this.OriginalActivatedArgs as LaunchActivatedEventArgs)?.PrelaunchActivated ?? false);
            }
            finally
            {
                deferral.Complete();
            }
        }

        public Controls.ModalDialog ModalDialog => Window.Current.Content as Controls.ModalDialog;

        public UIElement ModalContent
        {
            get => this.ModalDialog?.ModalContent;
            set => this.ModalDialog?.ModalContent = value;
        }

        public const string DefaultTileId = "App";

        public static AdditionalKinds DetermineStartCause(IActivatedEventArgs args)
        {
            Bootstrapper.DebugWrite($"{nameof(IActivatedEventArgs)}:{args.Kind}");

            if (args is ToastNotificationActivatedEventArgs)
                return AdditionalKinds.Toast;

            var e = args as ILaunchActivatedEventArgs;
            switch (e?.TileId)
            {
                case DefaultTileId when string.IsNullOrEmpty(e?.Arguments):
                    return AdditionalKinds.Primary;
                case DefaultTileId when !string.IsNullOrEmpty(e?.Arguments):
                    return AdditionalKinds.JumpListItem;
                default:
                    if (e?.TileId != DefaultTileId && !string.IsNullOrEmpty(e?.TileId))
                    {
                        return AdditionalKinds.SecondaryTile;
                    }
                    else
                    {
                        return AdditionalKinds.Other;
                    }
            }
        }

        private object pageKeys;

        public Dictionary<T, Type> PageKeys<T>()
            where T : struct, IConvertible
        {
            if (!typeof(T).GetTypeInfo().IsEnum)
                throw new ArgumentException($"{nameof(T)} must be an enumerated type");

            if (this.pageKeys is Dictionary<T, Type> pk)
                return pk;

            return (Dictionary<T, Type>)(this.pageKeys = new Dictionary<T, Type>());
        }
    }
}