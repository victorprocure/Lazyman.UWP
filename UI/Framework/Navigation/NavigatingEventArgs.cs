// -----------------------------------------------------------------------
// <copyright file="NavigatingEventArgs.cs" company="Procure Software Development">
// Copyright (c) 2018 Procure Software Development
// </copyright>
// <summary>
// Event arguments when a navigable is navigating
// </summary>

namespace UI.Framework.Navigation
{
    using System;
    
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Event arguments when a navigable is navigating
    /// </summary>
    public class NavigatingEventArgs : NavigatedEventArgs
    {
        /// <summary>
        /// The manager.
        /// </summary>
        private readonly DeferralManager manager;

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:UI.Framework.Navigation.NavigatingEventArgs" /> class.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        public NavigatingEventArgs(DeferralManager manager)
        {
            this.manager = manager;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigatingEventArgs"/> class.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="targetPageType">
        /// The target page type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="targetPageParameter">
        /// The target page parameter.
        /// </param>
        public NavigatingEventArgs(
            DeferralManager manager,
            NavigatingCancelEventArgs args,
            Page page,
            Type targetPageType,
            object parameter,
            object targetPageParameter)
            : this(manager)
        {
            this.NavigationMode = args.NavigationMode;
            this.PageType = args.SourcePageType;
            this.Page = page;
            this.Parameter = parameter;
            this.TargetPageType = targetPageType;
            this.TargetPageParameter = targetPageParameter;
        }

        /// <summary>
        /// Gets or sets a value indicating whether cancel.
        /// </summary>
        public bool Cancel { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether suspending.
        /// </summary>
        public bool Suspending { get; set; } = false;

        /// <summary>
        /// Gets or sets the target page type.
        /// </summary>
        public Type TargetPageType { get; set; }

        /// <summary>
        /// Gets or sets the target page parameter.
        /// </summary>
        public object TargetPageParameter { get; set; }

        /// <summary>
        /// The get deferral.
        /// </summary>
        /// <returns>
        /// The <see cref="Deferral"/>.
        /// </returns>
        public Deferral GetDeferral() => this.manager.GetDeferral();
    }
}