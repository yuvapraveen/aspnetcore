// Copyright (c) .NET Foundation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Json;
using Xunit;

namespace Microsoft.AspNetCore.Http.Extensions.Tests
{
    public class HttpContextJsonExtensionsTests
    {
        [Fact]
        public async Task ReadFromJsonAsyncGeneric_NonJsonContentType_ThrowError()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.ContentType = "text/json";

            // Act
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await context.Request.ReadFromJsonAsync<int>());

            // Assert
            var exceptedMessage = $"Unable to read the request as JSON because the request content type 'text/json' is not a known JSON content type.";
            Assert.Equal(exceptedMessage, ex.Message);
        }

        [Fact]
        public async Task ReadFromJsonAsyncGeneric_NoBodyContent_ThrowError()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.ContentType = "application/json";

            // Act
            var ex = await Assert.ThrowsAsync<JsonException>(async () => await context.Request.ReadFromJsonAsync<int>());

            // Assert
            var exceptedMessage = $"The input does not contain any JSON tokens. Expected the input to start with a valid JSON token, when isFinalBlock is true. Path: $ | LineNumber: 0 | BytePositionInLine: 0.";
            Assert.Equal(exceptedMessage, ex.Message);
        }

        [Fact]
        public async Task ReadFromJsonAsyncGeneric_ValidBodyContent_ReturnValue()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.ContentType = "application/json";
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("1"));

            // Act
            var result = await context.Request.ReadFromJsonAsync<int>();

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task ReadFromJsonAsyncGeneric_WithOptions_ReturnValue()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.ContentType = "application/json";
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("[1,2,]"));

            var options = new JsonSerializerOptions();
            options.AllowTrailingCommas = true;

            // Act
            var result = await context.Request.ReadFromJsonAsync<List<int>>(options);

            // Assert
            Assert.Collection(result,
                i => Assert.Equal(1, i),
                i => Assert.Equal(2, i));
        }

        [Fact]
        public async Task ReadFromJsonAsyncGeneric_WithCancellationToken_CancellationRaised()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.ContentType = "application/json";
            context.Request.Body = new TestStream();

            var cts = new CancellationTokenSource();

            // Act
            var readTask = context.Request.ReadFromJsonAsync<List<int>>(cts.Token);
            Assert.False(readTask.IsCompleted);

            cts.Cancel();

            // Assert
            await Assert.ThrowsAsync<TaskCanceledException>(async () => await readTask);
        }

        [Fact]
        public async Task ReadFromJsonAsync_ValidBodyContent_ReturnValue()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.ContentType = "application/json";
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("1"));

            // Act
            var result = (int)await context.Request.ReadFromJsonAsync(typeof(int));

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task ReadFromJsonAsync_WithOptions_ReturnValue()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.ContentType = "application/json";
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("[1,2,]"));

            var options = new JsonSerializerOptions();
            options.AllowTrailingCommas = true;

            // Act
            var result = (List<int>)await context.Request.ReadFromJsonAsync(typeof(List<int>), options);

            // Assert
            Assert.Collection(result,
                i => Assert.Equal(1, i),
                i => Assert.Equal(2, i));
        }

        [Fact]
        public async Task WriteAsJsonAsyncGeneric_SimpleValue_JsonResponse()
        {
            // Arrange
            var body = new MemoryStream();
            var context = new DefaultHttpContext();
            context.Response.Body = body;

            // Act
            await context.Response.WriteAsJsonAsync(1);

            // Assert
            Assert.Equal(JsonConstants.JsonContentType, context.Response.ContentType);

            var data = body.ToArray();
            Assert.Collection(data, b => Assert.Equal((byte)'1', b));
        }

        [Fact]
        public async Task WriteAsJsonAsyncGeneric_WithOptions_JsonResponse()
        {
            // Arrange
            var body = new MemoryStream();
            var context = new DefaultHttpContext();
            context.Response.Body = body;

            // Act
            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            await context.Response.WriteAsJsonAsync(new int[] { 1 }, options);

            // Assert
            Assert.Equal(JsonConstants.JsonContentType, context.Response.ContentType);

            var data = body.ToArray();
            Assert.Collection(data,
                b => Assert.Equal((byte)'[', b),
                b => Assert.Equal((byte)'\r', b),
                b => Assert.Equal((byte)'\n', b),
                b => Assert.Equal((byte)' ', b),
                b => Assert.Equal((byte)' ', b),
                b => Assert.Equal((byte)'1', b),
                b => Assert.Equal((byte)'\r', b),
                b => Assert.Equal((byte)'\n', b),
                b => Assert.Equal((byte)']', b));
        }

        [Fact]
        public async Task WriteAsJsonAsyncGeneric_WithContentType_JsonResponseWithCustomContentType()
        {
            // Arrange
            var body = new MemoryStream();
            var context = new DefaultHttpContext();
            context.Response.Body = body;

            // Act
            await context.Response.WriteAsJsonAsync(1, options: null, contentType: "application/custom-type");

            // Assert
            Assert.Equal("application/custom-type", context.Response.ContentType);
        }

        [Fact]
        public async Task WriteAsJsonAsyncGeneric_WithCancellationToken_CancellationRaised()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new TestStream();

            var cts = new CancellationTokenSource();

            // Act
            var writeTask = context.Response.WriteAsJsonAsync(1, cts.Token);
            Assert.False(writeTask.IsCompleted);

            cts.Cancel();

            // Assert
            await Assert.ThrowsAsync<TaskCanceledException>(async () => await writeTask);
        }

        [Fact]
        public async Task WriteAsJsonAsync_SimpleValue_JsonResponse()
        {
            // Arrange
            var body = new MemoryStream();
            var context = new DefaultHttpContext();
            context.Response.Body = body;

            // Act
            await context.Response.WriteAsJsonAsync(typeof(int), 1);

            // Assert
            Assert.Equal(JsonConstants.JsonContentType, context.Response.ContentType);

            var data = body.ToArray();
            Assert.Collection(data, b => Assert.Equal((byte)'1', b));
        }

        private class TestStream : Stream
        {
            public override bool CanRead { get; }
            public override bool CanSeek { get; }
            public override bool CanWrite { get; }
            public override long Length { get; }
            public override long Position { get; set; }

            public override void Flush()
            {
                throw new NotImplementedException();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotImplementedException();
            }

            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }

            public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
            {
                var tcs = new TaskCompletionSource<int>();
                cancellationToken.Register(s => ((TaskCompletionSource<int>)s).SetCanceled(), tcs);
                return new ValueTask<int>(tcs.Task);
            }

            public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
            {
                var tcs = new TaskCompletionSource<int>();
                cancellationToken.Register(s => ((TaskCompletionSource<int>)s).SetCanceled(), tcs);
                return new ValueTask(tcs.Task);
            }
        }
    }
}
