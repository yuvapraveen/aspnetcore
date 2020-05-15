// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

#nullable enable

namespace Microsoft.AspNetCore.Http.Json
{
    public static partial class HttpResponseJsonExtensions
    {
        public static Task WriteAsJsonAsync<TValue>(
            this HttpResponse response,
            TValue value,
            CancellationToken cancellationToken = default)
        {
            return response.WriteAsJsonAsync<TValue>(value, options: null, contentType: JsonConstants.JsonContentTypeWithCharset, cancellationToken);
        }

        public static Task WriteAsJsonAsync<TValue>(
            this HttpResponse response,
            TValue value,
            JsonSerializerOptions? options,
            CancellationToken cancellationToken = default)
        {
            return response.WriteAsJsonAsync<TValue>(value, options, contentType: JsonConstants.JsonContentTypeWithCharset, cancellationToken);
        }

        public static Task WriteAsJsonAsync<TValue>(
            this HttpResponse response,
            TValue value,
            JsonSerializerOptions? options,
            string? contentType,
            CancellationToken cancellationToken = default)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (options == null)
            {
                options = ResolveSerializerOptions(response.HttpContext);
            }

            if (contentType != null)
            {
                response.ContentType = contentType;
            }
            response.StatusCode = StatusCodes.Status200OK;
            return JsonSerializer.SerializeAsync<TValue>(response.Body, value, options, cancellationToken);
        }

        public static Task WriteAsJsonAsync(
            this HttpResponse response,
            object value,
            Type type,
            CancellationToken cancellationToken = default)
        {
            return response.WriteAsJsonAsync(value, type, options: null, contentType: JsonConstants.JsonContentTypeWithCharset, cancellationToken);
        }

        public static Task WriteAsJsonAsync(
            this HttpResponse response,
            object value,
            Type type,
            JsonSerializerOptions? options,
            CancellationToken cancellationToken = default)
        {
            return response.WriteAsJsonAsync(value, type, options, contentType: JsonConstants.JsonContentTypeWithCharset, cancellationToken);
        }

        public static Task WriteAsJsonAsync(
            this HttpResponse response,
            object value,
            Type type,
            JsonSerializerOptions? options,
            string? contentType,
            CancellationToken cancellationToken = default)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (options == null)
            {
                options = ResolveSerializerOptions(response.HttpContext);
            }

            if (contentType != null)
            {
                response.ContentType = contentType;
            }
            response.StatusCode = StatusCodes.Status200OK;
            return JsonSerializer.SerializeAsync(response.Body, value, type, options, cancellationToken);
        }

        private static JsonSerializerOptions ResolveSerializerOptions(HttpContext? httpContext)
        {
            // Attempt to resolve options from DI then fallback to default options
            return httpContext?.RequestServices?.GetService<IOptions<JsonOptions>>()?.Value?.SerializerOptions ?? JsonOptions.DefaultSerializerOptions;
        }
    }
}
