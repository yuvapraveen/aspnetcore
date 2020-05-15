// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// This factory copies settings from Microsoft.AspNetCore.Http.Json.JsonOptions,
    /// and they are used as the base for any additional MVC serialization configuration.
    /// Changes applies to Microsoft.AspNetCore.Mvc.JsonOptions are isolated to MVC.
    /// </summary>
    internal class JsonOptionsFactory : OptionsFactory<JsonOptions>
    {
        private JsonSerializerOptions _serializerOptions;

        public JsonOptionsFactory(
            IOptions<Http.Json.JsonOptions> options,
            IEnumerable<IConfigureOptions<JsonOptions>> setups,
            IEnumerable<IPostConfigureOptions<JsonOptions>> postConfigures,
            IEnumerable<IValidateOptions<JsonOptions>> validations) : base(setups, postConfigures, validations)
        {
            _serializerOptions = options.Value.SerializerOptions;
        }

        protected override JsonOptions CreateInstance(string name)
        {
            // Copy values from Microsoft.AspNetCore.Http.Json.JsonOptions serializer options.
            return new JsonOptions(new JsonSerializerOptions(_serializerOptions));
        }
    }
}
