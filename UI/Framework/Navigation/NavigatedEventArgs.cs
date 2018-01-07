// -----------------------------------------------------------------------
// <copyright file="NavigatedEventArgs.cs" company="Procure Software Development">
// Copyright (c) 2018 Procure Software Development
// </copyright>

namespace UI.Framework.Navigation
{
    using System;

    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <inheritdoc />
    /// <summary>
    /// Represents the event arguments for navigated events
    /// </summary>
    public class NavigatedEventArgs : EventArgs
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:UI.Framework.Navigation.NavigatedEventArgs" /> class.
        /// </summary>
        public NavigatedEventArgs()
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:UI.Framework.Navigation.NavigatedEventArgs" /> class.
        /// </summary>
        /// <param name="args">
        /// The navigation event arguments
        /// </param>
        /// <param name="page">
        /// The page navigated to
        /// </param>
        // ReSharper disable once SuggestBaseTypeForParameter
        public NavigatedEventArgs(NavigationEventArgs args, Page page)
        {
            this.Page = page;
            this.PageType = args.SourcePageType;
            this.Parameter = args.Parameter;
            this.NavigationMode = args.NavigationMode;
        }

        /// <summary>
        /// Gets or sets the navigation mode.
        /// </summary>
        public NavigationMode NavigationMode { get; set; }

        /// <summary>
        /// Gets or sets the page type.
        /// </summary>
        public Type PageType { get; set; }

        /// <summary>
        /// Gets or sets the parameter.
        /// </summary>
        public object Parameter { get; set; }

        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        public Page Page { get; set; }
    }
}