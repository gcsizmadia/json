# Egonsoft.HU JSON Library

[![GitHub](https://img.shields.io/github/license/gcsizmadia/EgonsoftHU.Text.Json?label=License)](https://opensource.org/licenses/MIT)
[![Nuget](https://img.shields.io/nuget/v/EgonsoftHU.Text.Json?label=NuGet)](https://www.nuget.org/packages/EgonsoftHU.Text.Json)
[![Nuget](https://img.shields.io/nuget/dt/EgonsoftHU.Text.Json?label=Downloads)](https://www.nuget.org/packages/EgonsoftHU.Text.Json)

`System.Text.Json` extensions.

- Supports advanced enum member serialization in the following order:
  - Any attribute applied to an enum member, built-in support for 5 attributes.
  - `JsonNamingPolicy`
  - The enum member name as it is written.
- Supports custom serializer implementations for enum members.
- Pre-configured `JsonSerializerOptions` instances.
- Extension methods for `JsonSerializerOptions`.
- Additional `JsonNamingPolicy` implementations.

## Table of Contents

- [Introduction](#introduction)
- [Releases](#releases)
- [Features](#features)
  * [EnumValueSerializer](#enumvalueserializer)
    + [Default implementation](#default-implementation)
  * [JsonEnumValueSerializer](#jsonenumvalueserializer)
    + [Value precedence](#value-precedence)
  * [Custom implementation of EnumValueSerializer](#custom-implementation-of-enumvalueserializer)
  * [EgonsoftJsonStringEnumConverterFactory](#egonsoftjsonstringenumconverterfactory)
  * [JsonSerializerOptionsProvider](#jsonserializeroptionsprovider)
  * [JsonSerializerOptions extension methods](#jsonserializeroptions-extension-methods)
    + [AddEgonsoftJsonStringEnumConverter()](#addegonsoftjsonstringenumconverter--)
    + [CopyTo()](#copyto--)
  * [Naming policies](#naming-policies)
- [Example configuration](#example-configuration)
- [JsonStringEnumMemberNameAttribute support](#jsonstringenummembernameattribute-support)

## Introduction

The motivation behind this project is to extend `System.Text.Json` features.

## Releases

You can download the package from **nuget.org**: [EgonsoftHU.Text.Json](https://www.nuget.org/packages/EgonsoftHU.Text.Json)

You can find the release notes [here](https://github.com/gcsizmadia/json/releases).

[Releases](RELEASES.md) under active support or development:

|Version|Release Date|Latest Patch Version|End of Support|Targets|
|-|-|-|-|-|
|10.0|2026-10-17|10.0.0|2028-11-14|`net8.0`, `net9.0`, `net10.0`<br/>`net472`<br/>`netstandard2.0`, `netstandard2.1`|

## Features

This library comes with the following features.

### EnumValueSerializer

This library makes use of an implementation of this base class.

**Please note:**  
This base class has been moved from `EgonsoftHU.Extensions.Bcl` nuget package to this nuget package.

#### Default implementation

The default implementation supports the following attributes:
- `System.Text.Json.Serialization.JsonStringEnumMemberNameAttribute`
- `System.Runtime.Serialization.EnumMemberAttribute`

Converting / retrieving an enum member value to / from a string uses these values in the following order, from highest to lowest priority:

1. The value of the `JsonStringEnumMemberNameAttribute.Name` property, if that attribute is applied to the current enum member.
   - Both reading and writing are case sensitive.
2. The value of the `EnumMemberAttribute.Value` property, if that attribute is applied to the current enum member.
   - Both reading and writing are case sensitive.
3. The name of the enum member.
   - Both reading and writing are case sensitive.

**Please note:**  
This implementation makes use of the `EgonsoftHU.Extensions.Bcl.Enumerations.EnumInfo<TEnum>` type.

### JsonEnumValueSerializer

This is an implementation of the above mentioned `EgonsoftHU.Text.Json.Serialization.Converters.EnumValueSerializer` type and
it supports attributes and `System.Text.Json.JsonNamingPolicy` to convert/retrieve enum member values to/from a string.

The following attributes are supported:

- `System.Text.Json.Serialization.JsonStringEnumMemberNameAttribute` (available in `System.Text.Json` (`>= 9.0.0`))  
- `System.Runtime.Serialization.EnumMemberAttribute`
- `System.ComponentModel.DescriptionAttribute`
- `System.ComponentModel.DisplayNameAttribute`
- `System.ComponentModel.DataAnnotations.DisplayAttribute` (`Description`, `Name` and `ShortName` properties are supported.)

To create an instance and select an attribute using the `EgonsoftHU.Text.Json.Serialization.EnumMemberNameSelectorOption` enum type:

```csharp
using System.Text.Json;
using EgonsoftHU.Text.Json.Serialization;
using EgonsoftHU.Text.Json.Serialization.Converters;

var serializer =
    new JsonEnumValueSerializer(
        JsonNamingPolicy.CamelCase,
        // The second parameter is optional. This is the default value:
        EnumMemberNameSelectorOption.UseJsonStringEnumMemberNameAttribute
    );
```

#### Value precedence

Converting / retrieving an enum member value to / from a string uses these values in the following order, from highest to lowest priority:

1. The value of the selected attribute, if that attribute is applied to the current enum member.
   - Both reading and writing are case sensitive.
2. The value produced by the specified `JsonNamingPolicy`.
   - Both reading and writing are case sensitive.
3. The name of the enum member.
   - Both reading and writing are case sensitive.

### Custom implementation of EnumValueSerializer

You can create and use your own `EnumValueSerializer` implementation if the above mentioned
`JsonEnumValueSerializer` implementation does not fit your needs.

Additionally, you can use the `EnumInfo<TEnum>.Attributes` instance property to access all the
attributes that are applied to an enum member.

### EgonsoftJsonStringEnumConverterFactory

This is an implementation of the `System.Text.Json.Serialization.JsonConverterFactory` that you can add to the
`System.Text.Json.JsonSerializerOptions.Converters` collection.

You can specify
- a `JsonNamingPolicy`
- if numbers are allowed during both serialization and deserialization
- an instance of an `EnumValueSerializer` implementation

### JsonSerializerOptionsProvider

This is a static class that provides
- read-only default (`Default`, `DefaultWriteIndented`) properties and
- a writable current (`Current`) property

to hold instances of the `System.Text.Json.JsonSerializerOptions` type.

The default instance is created this way:

```csharp
JsonSerializerOptions options =
    new(JsonSerializerDefaults.Web)
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        WriteIndented = false
    };

options.AddEgonsoftJsonStringEnumConverter();
```

### JsonSerializerOptions extension methods

The following extensions methods are defined:
- AddEgonsoftJsonStringEnumConverter()
- CopyTo()

#### AddEgonsoftJsonStringEnumConverter()

You can easily add this converter to your `JsonSerializerOptions` instance.

```csharp
using System.Text.Json;
using EgonsoftHU.Text.Json.Serialization;

JsonSerializerOptions options = /* omitted for clarity */

options.AddEgonsoftJsonStringEnumConverter();
```

By default, this will add a converter that:
- checks if `JsonStringEnumMemberNameAttribute` attribute is applied to an enum member,
- uses `JsonNamingPolicy.CamelCase` and
- allows numbers.

You can customize the settings by passing an instance of `EgonsoftHU.Text.Json.Serialization.Converters.EnumConverterOptions` as a parameter.

#### CopyTo()

Suppose you already have an existing instance of the `JsonSerializerOptions` class that you would like to use when configuring ASP.NET Core.

This `CopyTo()` extension method comes in handy in this case:

```csharp
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

JsonSerializerOptions options = /* your already existing instance */;

builder
    .Services
    .AddControllers()
    // source.CopyTo(target) will override target's settings with the source's settings.
    .AddJsonOptions(jsonOptions => options.CopyTo(jsonOptions.JsonSerializerOptions));
```

### Naming policies

Although these are not the most wanted naming policies, they are available in case you need them.

- `EgonsoftHU.Text.Json.JsonUpperCaseNamingPolicy`
- `EgonsoftHU.Text.Json.JsonLowerCaseNamingPolicy`

## Example configuration

In this example configuration converting / retrieving an enum member value to / from a string
will use these values in the following order, from highest to lowest priority:

1. `JsonStringEnumMemberNameAttribute.Name`, if that attribute is applied to the current enum member.
   - Both reading and writing are case sensitive.
2. The value produced by `JsonNamingPolicy.CamelCase`.
   - Both reading and writing are case sensitive.
3. The name of the enum member.
   - Both reading and writing are case sensitive.

```csharp
using System.Text.Json;
using EgonsoftHU.Text.Json.Serialization;
using EgonsoftHU.Text.Json.Serialization.Converters;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

JsonSerializerOptionsProvider.Current =
    new(JsonSerializerDefaults.Web)
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        WriteIndented = false
    }
    .AddEgonsoftJsonStringEnumConverter();

builder
    .Services
    .AddControllers()
    .AddJsonOptions(jsonOptions => JsonSerializerOptionsProvider.Current.CopyTo(jsonOptions.JsonSerializerOptions));
```

## JsonStringEnumMemberNameAttribute support

This nuget package references the `10.0.x` version of the `System.Text.Json` nuget package,
so this attribute is available for all targets of this nuget package.
