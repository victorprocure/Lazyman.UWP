// -----------------------------------------------------------------------
// <copyright file="SerializationService.cs" company="Procure Software Development">
// Copyright (c) 2018 Procure Software Development
// </copyright>
// <summary>
// Default serialization service
// </summary>

namespace UI.Framework.Serialization
{
    /// <summary>
    /// Default serialization service
    /// </summary>
    public static class SerializationService
    {
        /// <summary>
        /// The JSON serialization service.
        /// </summary>
        private static ISerializationService json;

        /// <summary>
        /// The JSON serialization service.
        /// </summary>
        public static ISerializationService Json => json ?? (json = new JsonSerializationService());
    }
}