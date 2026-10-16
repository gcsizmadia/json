// Copyright © 2023-2026 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EgonsoftHU.Text.Json.Serialization.Converters
{
    internal sealed class EnumConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsEnum;
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return Create(typeToConvert, options, EnumConverterOptions.Default);
        }

        internal static JsonConverter? Create(
            Type enumType,
            JsonSerializerOptions jsonSerializerOptions,
            EnumConverterOptions jsonEnumConverterOptions
        )
        {
            return
                (JsonConverter?)Activator.CreateInstance(
                    GetEnumConverterType(enumType),
                    jsonSerializerOptions,
                    jsonEnumConverterOptions
                );
        }

        private static Type GetEnumConverterType(Type enumType)
        {
            return typeof(EnumConverter<>).MakeGenericType(enumType);
        }
    }
}
