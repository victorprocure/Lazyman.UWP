// -----------------------------------------------------------------------
// <copyright file="ISerializationService.cs" company="Procure Software Development">
// Copyright (c) 2018 Procure Software Development
// </copyright>
// <summary>
// Represents a serialization service
// </summary>


namespace UI.Framework.Serialization
{
    using System;

    /// <summary>
    /// Represents a serialization service
    /// </summary>
    public interface ISerializationService
    {
        /// <summary>
        /// Serializes the parameter.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string Serialize(object parameter);



        /// <summary>
        /// Deserializes the parameter.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        object Deserialize(string parameter);



        /// <summary>
        /// Deserializes the parameter.
        /// </summary>
        /// <typeparam name="T">
        /// Type of deserialized object
        /// </typeparam>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T Deserialize<T>(string parameter);



        /// <summary>
        /// Attempts to deserialize the parameter.
        /// </summary>
        /// <typeparam name="T">
        /// Type of deserialized object
        /// </typeparam>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool TryDeserialize<T>(string parameter, out T result);
    }
}