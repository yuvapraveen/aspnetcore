// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace Microsoft.AspNetCore.Http.Json
{
    public static class HttpRequestJsonExtensions
    {
        [return: MaybeNull]
        public static ValueTask<TValue> ReadFromJsonAsync<TValue>(
            this HttpRequest request,
            CancellationToken cancellationToken = default)
        {
            return request.ReadFromJsonAsync<TValue>(options: null, cancellationToken);
        }

        [return: MaybeNull]
        public static ValueTask<TValue> ReadFromJsonAsync<TValue>(
            this HttpRequest request,
            JsonSerializerOptions? options,
            CancellationToken cancellationToken = default)
        {
            if (!request.HasJsonContentType())
            {
                return new ValueTask<TValue>(Task.FromException<TValue>(CreateContentTypeError(request)));
            }

            return JsonSerializer.DeserializeAsync<TValue>(request.Body, options, cancellationToken);
        }

        public static ValueTask<object?> ReadFromJsonAsync(
            this HttpRequest request,
            Type type,
            CancellationToken cancellationToken = default)
        {
            return request.ReadFromJsonAsync(type, options: null, cancellationToken);
        }

        public static ValueTask<object?> ReadFromJsonAsync(
            this HttpRequest request,
            Type type,
            JsonSerializerOptions? options,
            CancellationToken cancellationToken = default)
        {
            if (!request.HasJsonContentType())
            {
                return new ValueTask<object?>(Task.FromException<object?>(CreateContentTypeError(request)));
            }

            return JsonSerializer.DeserializeAsync(request.Body, type, options, cancellationToken);
        }

        private static InvalidOperationException CreateContentTypeError(HttpRequest request)
        {
            return new InvalidOperationException($"Unable to read the request as JSON because the request content type '{request.ContentType}' is not a known JSON content type.");
        }
    }
}
