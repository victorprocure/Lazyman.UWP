// -----------------------------------------------------------------------
// <copyright file="NavigationProperties.cs" company="Procure Software Development">
// Copyright (c) 2018 Procure Software Development
// </copyright>

namespace UI.Navigation
{
    using System;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <inheritdoc />
    /// <summary>
    ///     Represents navigation view properties
    /// </summary>
    public class NavigationProperties : DependencyObject
    {
        /// <summary>
        /// The page type property.
        /// </summary>
        public static readonly DependencyProperty PageTypeProperty = DependencyProperty.RegisterAttached(
            "PageType",
            typeof(Type),
            typeof(NavigationProperties),
            new PropertyMetadata(null));

        /// <summary>
        /// The get page type.
        /// </summary>
        /// <param name="obj">
        /// The navigation item object.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/>.
        /// </returns>
        public static Type GetPageType(NavigationViewItem obj)
        {
            return (Type)obj.GetValue(PageTypeProperty);
        }

        /// <summary>
        /// The set page type.
        /// </summary>
        /// <param name="obj">
        /// The navigation view item object.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetPageType(NavigationViewItem obj, Type value)
        {
            obj.SetValue(PageTypeProperty, value);
        }
    }
}