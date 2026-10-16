// Copyright © 2023-2026 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

using EgonsoftHU.Extensions.Bcl;
using EgonsoftHU.Extensions.Bcl.Enumerations;
using EgonsoftHU.Text.Json.Exceptions;

namespace EgonsoftHU.Text.Json.Serialization.Converters
{
    internal sealed class EnumConverter<TEnum> : JsonConverter<TEnum>
        where TEnum : struct, Enum
    {
        private readonly bool allowNumbers;
        private readonly bool allowStrings;
        private readonly EnumValueSerializer enumValueSerializer;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public EnumConverter(JsonSerializerOptions jsonSerializerOptions, EnumConverterOptions jsonEnumConverterOptions)
        {
            this.jsonSerializerOptions = jsonSerializerOptions;

            allowNumbers = jsonEnumConverterOptions.AllowNumbers;
            allowStrings = jsonEnumConverterOptions.AllowStrings;
            enumValueSerializer = jsonEnumConverterOptions.EnumValueSerializer;
        }

        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                {
                    if (allowStrings)
                    {
                        return ReadJsonString(ref reader);
                    }

                    break;
                }
                case JsonTokenType.Number:
                {
                    if (allowNumbers)
                    {
                        return ReadJsonNumber(ref reader);
                    }

                    break;
                }
            }

            throw JsonExceptions.InvalidToken();
        }

        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        {
            if (allowStrings && EnumInfo.TryFromValue(value, out EnumInfo<TEnum>? enumeration))
            {
                if (!NameCache.ForWriting.TryGetValue(enumeration.UInt64Value, out JsonEncodedText text))
                {
                    string serializedValue = enumValueSerializer.Serialize(enumeration);

                    text = JsonEncodedText.Encode(serializedValue, jsonSerializerOptions.Encoder);

                    NameCache.ForWriting.TryAdd(enumeration.UInt64Value, text);
                }

                writer.WriteStringValue(text);
                return;
            }

            if (allowNumbers)
            {
                switch (EnumInfo<TEnum>.EnumTypeCode)
                {
                    case TypeCode.SByte:
                        writer.WriteNumberValue(Unsafe.As<TEnum, sbyte>(ref value));
                        return;

                    case TypeCode.Int16:
                        writer.WriteNumberValue(Unsafe.As<TEnum, short>(ref value));
                        return;

                    case TypeCode.Int32:
                        writer.WriteNumberValue(Unsafe.As<TEnum, int>(ref value));
                        return;

                    case TypeCode.Int64:
                        writer.WriteNumberValue(Unsafe.As<TEnum, long>(ref value));
                        return;

                    case TypeCode.Byte:
                        writer.WriteNumberValue(Unsafe.As<TEnum, byte>(ref value));
                        return;

                    case TypeCode.UInt16:
                        writer.WriteNumberValue(Unsafe.As<TEnum, ushort>(ref value));
                        return;

                    case TypeCode.UInt32:
                        writer.WriteNumberValue(Unsafe.As<TEnum, uint>(ref value));
                        return;

                    case TypeCode.UInt64:
                        writer.WriteNumberValue(Unsafe.As<TEnum, ulong>(ref value));
                        return;

                    default:
                        break;
                }
            }

            throw JsonExceptions.InvalidEnumValue();
        }

        private static TEnum GetValue<TUnderlying>(bool readSuccess, ref TUnderlying underlyingValue)
            where TUnderlying : struct
        {
            return
                readSuccess
                    ? Unsafe.As<TUnderlying, TEnum>(ref underlyingValue)
                    : throw JsonExceptions.InvalidToken();
        }

        private TEnum ReadJsonString(ref Utf8JsonReader reader)
        {
            if (!allowStrings)
            {
                throw JsonExceptions.InvalidToken();
            }

            string? serializedValue = reader.GetString();

            if (serializedValue.IsNullOrWhiteSpace())
            {
                throw JsonExceptions.InvalidToken();
            }

            bool isInCache = NameCache.ForReading.TryGetValue(serializedValue, out EnumInfo<TEnum>? enumeration);

            EnumInfo<TEnum>? result =
                isInCache
                    ? enumeration
                    : enumValueSerializer.Deserialize<TEnum>(serializedValue);

            if (result is not null)
            {
                if (!isInCache)
                {
                    NameCache.ForReading.TryAdd(serializedValue, result);
                }

                return result.Value;
            }

            throw JsonExceptions.InvalidToken();
        }

        private TEnum ReadJsonNumber(ref Utf8JsonReader reader)
        {
            return
                allowNumbers
                    ? EnumInfo<TEnum>.EnumTypeCode switch
                    {
                        TypeCode.SByte => GetValue(reader.TryGetSByte(out sbyte value), ref value),
                        TypeCode.Int16 => GetValue(reader.TryGetInt16(out short value), ref value),
                        TypeCode.Int32 => GetValue(reader.TryGetInt32(out int value), ref value),
                        TypeCode.Int64 => GetValue(reader.TryGetInt64(out long value), ref value),
                        TypeCode.Byte => GetValue(reader.TryGetByte(out byte value), ref value),
                        TypeCode.UInt16 => GetValue(reader.TryGetUInt16(out ushort value), ref value),
                        TypeCode.UInt32 => GetValue(reader.TryGetUInt32(out uint value), ref value),
                        TypeCode.UInt64 => GetValue(reader.TryGetUInt64(out ulong value), ref value),
                        _ => throw JsonExceptions.InvalidToken()
                    }
                    : throw JsonExceptions.InvalidToken();
        }

        private static class NameCache
        {
            [SuppressMessage(SonarQube.Category, SonarQube.S2743)]
            internal static ConcurrentDictionary<ulong, JsonEncodedText> ForWriting { get; } = new();

            internal static ConcurrentDictionary<string, EnumInfo<TEnum>> ForReading { get; } = new();
        }
    }
}
