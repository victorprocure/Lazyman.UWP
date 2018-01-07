// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="AdditionalKinds.cs" company="Procure Software Development">
// //   Copyright (c) Procure Software Development
// // </copyright>
// // <author>Victor Procure</author>
// // <summary>
// // 
// //   </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace UI.Framework
{
    /// <summary>
    /// Represents additional start kinds
    /// </summary>
    public enum AdditionalKinds
    {
        /// <summary>
        /// Started as a primary tile
        /// </summary>
        Primary,

        /// <summary>
        /// Started as a toast
        /// </summary>
        Toast,

        /// <summary>
        /// Started as a secondary tile
        /// </summary>
        SecondaryTile,

        /// <summary>
        /// Another start kind
        /// </summary>
        Other,

        /// <summary>
        /// Started as a jump list item
        /// </summary>
        JumpListItem
    }
}