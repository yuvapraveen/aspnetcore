// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Net.Http.Headers;

#nullable enable

namespace Microsoft.AspNetCore.Http
{
    public static class HttpRequestExtensions
    {
        public static bool HasJsonContentType(this HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (!MediaTypeHeaderValue.TryParse(request.ContentType, out var mt))
            {
                return false;
            }

            // Matches application/json
            if (mt.MediaType.Equals(JsonConstants.JsonContentType, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Matches +json, e.g. application/ld+json
            if (mt.Suffix.Equals("json", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}
