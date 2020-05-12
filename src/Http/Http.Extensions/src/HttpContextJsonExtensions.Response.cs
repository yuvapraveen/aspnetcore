// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Http.Json
{
    public static partial class HttpContextJsonExtensions
    {
        public static ValueTask WriteAsJsonAsync<TValue>(
            this HttpResponse response,
            TValue value,
            CancellationToken cancellationToken = default)
        {
            return response.WriteAsJsonAsync<TValue>(value, options: null, contentType: JsonConstants.JsonContentType, cancellationToken);
        }

        public static ValueTask WriteAsJsonAsync<TValue>(
            this HttpResponse response,
            TValue value,
            JsonSerializerOptions options,
            CancellationToken cancellationToken = default)
        {
            return response.WriteAsJsonAsync<TValue>(value, options, contentType: JsonConstants.JsonContentType, cancellationToken);
        }

        public static ValueTask WriteAsJsonAsync<TValue>(
            this HttpResponse response,
            TValue value,
            JsonSerializerOptions options,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            if (contentType != null)
            {
                response.ContentType = contentType;
            }
            response.StatusCode = StatusCodes.Status200OK;
            return new ValueTask(JsonSerializer.SerializeAsync<TValue>(response.Body, value, options, cancellationToken));
        }

        public static ValueTask WriteAsJsonAsync(
            this HttpResponse response,
            Type type,
            object value,
            CancellationToken cancellationToken = default)
        {
            return response.WriteAsJsonAsync(type, value, options: null, contentType: JsonConstants.JsonContentType, cancellationToken);
        }

        public static ValueTask WriteAsJsonAsync(
            this HttpResponse response,
            Type type,
            object value,
            JsonSerializerOptions options,
            CancellationToken cancellationToken = default)
        {
            return response.WriteAsJsonAsync(type, value, options, contentType: JsonConstants.JsonContentType, cancellationToken);
        }

        public static ValueTask WriteAsJsonAsync(
            this HttpResponse response,
            Type type,
            object value,
            JsonSerializerOptions options,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            if (contentType != null)
            {
                response.ContentType = contentType;
            }
            response.StatusCode = StatusCodes.Status200OK;
            return new ValueTask(JsonSerializer.SerializeAsync(response.Body, value, type, options, cancellationToken));
        }
    }
}
