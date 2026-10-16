// Copyright © 2023-2026 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System.Text.Json;

using EgonsoftHU.Text.Json.Serialization.Converters;

namespace EgonsoftHU.Text.Json.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public static class JsonSerializerOptionsExtensions
    {
        /// <summary>
        /// Registers <see cref="EgonsoftJsonStringEnumConverterFactory"/> in the list of user-defined converters of a specified <paramref name="options"/>.
        /// </summary>
        /// <param name="options">The <see cref="JsonSerializerOptions"/> instance, the converters of which will be extended with <see cref="EgonsoftJsonStringEnumConverterFactory"/>.</param>
        /// <param name="converterOptions">The configurable options to be used by the <see cref="EnumConverter{TEnum}"/> instances.</param>
        /// <returns></returns>
        public static JsonSerializerOptions AddEgonsoftJsonStringEnumConverter(
            this JsonSerializerOptions options,
            EnumConverterOptions? converterOptions = null
        )
        {
            converterOptions ??= EnumConverterOptions.Default with
            {
                EnumValueSerializer =
                    new JsonEnumValueSerializer(
                        JsonNamingPolicy.CamelCase,
                        EnumMemberAttributeSelectorOption.UseJsonStringEnumMemberNameAttribute
                    )
            };

            options.Converters.Add(new EgonsoftJsonStringEnumConverterFactory(converterOptions));

            return options;
        }

        /// <summary>
        /// Registers <see cref="EgonsoftJsonStringEnumConverterFactory"/> in the list of user-defined converters of a specified <paramref name="options"/>.
        /// </summary>
        /// <param name="options">The <see cref="JsonSerializerOptions"/> instance, the converters of which will be extended with <see cref="EgonsoftJsonStringEnumConverterFactory"/>.</param>
        /// <param name="jsonNamingPolicy">The naming policy to be used by the <see cref="JsonEnumValueSerializer"/>.</param>
        /// <returns></returns>
        public static JsonSerializerOptions AddEgonsoftJsonStringEnumConverter(
            this JsonSerializerOptions options,
            JsonNamingPolicy jsonNamingPolicy
        )
        {
            return options.AddEgonsoftJsonStringEnumConverter(
                EnumConverterOptions.Default with
                {
                    EnumValueSerializer =
                        new JsonEnumValueSerializer(
                            jsonNamingPolicy,
                            EnumMemberAttributeSelectorOption.UseJsonStringEnumMemberNameAttribute
                        )
                }
            );
        }
    }
}
