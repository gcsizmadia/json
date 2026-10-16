// Copyright © 2023-2026 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System.Text.Json;

using EgonsoftHU.Extensions.Bcl;
using EgonsoftHU.Text.Json.Serialization;
using EgonsoftHU.Text.Json.Serialization.Converters;

namespace EgonsoftHU.Text.Json
{
    /// <summary>
    /// This class contains extension methods that are available for <see cref="JsonSerializerOptions"/> type.
    /// </summary>
    public static class JsonSerializerOptionsExtensions
    {
        /// <summary>
        /// Overrides <paramref name="target"/> settings with the <paramref name="source"/> settings.
        /// </summary>
        /// <param name="source">The <see cref="JsonSerializerOptions"/> instance the settings of which should be copied.</param>
        /// <param name="target">The <see cref="JsonSerializerOptions"/> instance the settings of which will be overridden.</param>
        /// <remarks>
        /// Useful when configuring <c>Microsoft.AspNetCore.Mvc.JsonOptions</c> instance
        /// with an existing instance of <see cref="JsonSerializerOptions"/><br/>
        /// using the <c>AddJsonOptions(this IMvcBuilder builder, Action&lt;JsonOptions&gt; configure)</c> extension method.
        /// <para>Example:</para>
        /// <c>
        /// JsonSerializerOptions options = /* custom settings */;<br/>
        /// <br/>
        /// var builder = WebApplicationBuilder.CreateBuilder(args);<br/>
        /// <br/>
        /// builder.AddControllers().AddJsonOptions(jsonOptions => options.CopyTo(jsonOptions.JsonSerializerOptions));
        /// </c>
        /// </remarks>
        public static void CopyTo(this JsonSerializerOptions source, JsonSerializerOptions target)
        {
            source.ThrowIfNull();
            target.ThrowIfNull();

            target.AllowDuplicateProperties = source.AllowDuplicateProperties;
            target.AllowOutOfOrderMetadataProperties = source.AllowOutOfOrderMetadataProperties;
            target.AllowTrailingCommas = source.AllowTrailingCommas;

            target.Converters.Clear();
            target.Converters.AddRange(source.Converters);

            target.DefaultBufferSize = source.DefaultBufferSize;
            target.DefaultIgnoreCondition = source.DefaultIgnoreCondition;
            target.DictionaryKeyPolicy = source.DictionaryKeyPolicy;
            target.Encoder = source.Encoder;
            target.IgnoreReadOnlyFields = source.IgnoreReadOnlyFields;
            target.IgnoreReadOnlyProperties = source.IgnoreReadOnlyProperties;
            target.IncludeFields = source.IncludeFields;
            target.IndentCharacter = source.IndentCharacter;
            target.IndentSize = source.IndentSize;
            target.MaxDepth = source.MaxDepth;
            target.NewLine = source.NewLine;
            target.NumberHandling = source.NumberHandling;
            target.PreferredObjectCreationHandling = source.PreferredObjectCreationHandling;
            target.PropertyNameCaseInsensitive = source.PropertyNameCaseInsensitive;
            target.PropertyNamingPolicy = source.PropertyNamingPolicy;
            target.ReadCommentHandling = source.ReadCommentHandling;
            target.ReferenceHandler = source.ReferenceHandler;
            target.RespectNullableAnnotations = source.RespectNullableAnnotations;
            target.RespectRequiredConstructorParameters = source.RespectRequiredConstructorParameters;
            target.TypeInfoResolver = source.TypeInfoResolver;
            target.UnknownTypeHandling = source.UnknownTypeHandling;
            target.UnmappedMemberHandling = source.UnmappedMemberHandling;
            target.WriteIndented = source.WriteIndented;
        }

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
