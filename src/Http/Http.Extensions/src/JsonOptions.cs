// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text.Encodings.Web;
using System.Text.Json;

#nullable enable

namespace Microsoft.AspNetCore.Http.Json
{
    public class JsonOptions
    {
        // TODO: Update to create options using https://github.com/dotnet/runtime/pull/36073
        internal static readonly JsonSerializerOptions DefaultSerializerOptions = new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public JsonSerializerOptions SerializerOptions { get; } = new JsonSerializerOptions(DefaultSerializerOptions);
    }
}
