// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNetCore.Http
{
    public static class HttpRequestJsonExtensions
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

            if (mt.MediaType.Equals(JsonConstants.JsonContentType, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (mt.Suffix.Equals("json", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}
