// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Json;
using Xunit;

namespace Microsoft.AspNetCore.Http.Extensions.Tests
{
    public class HttpResponseJsonExtensionsTests
    {
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
            Assert.Equal(JsonConstants.JsonContentTypeWithCharset, context.Response.ContentType);
            Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);

            var data = body.ToArray();
            Assert.Collection(data, b => Assert.Equal((byte)'1', b));
        }

        [Fact]
        public async Task WriteAsJsonAsyncGeneric_NullValue_JsonResponse()
        {
            // Arrange
            var body = new MemoryStream();
            var context = new DefaultHttpContext();
            context.Response.Body = body;

            // Act
            await context.Response.WriteAsJsonAsync<Uri>(value: null);

            // Assert
            Assert.Equal(JsonConstants.JsonContentTypeWithCharset, context.Response.ContentType);

            var data = Encoding.UTF8.GetString(body.ToArray());
            Assert.Equal("null", data);
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
            Assert.Equal(JsonConstants.JsonContentTypeWithCharset, context.Response.ContentType);

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
            await context.Response.WriteAsJsonAsync(1, typeof(int));

            // Assert
            Assert.Equal(JsonConstants.JsonContentTypeWithCharset, context.Response.ContentType);
            Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);

            var data = body.ToArray();
            Assert.Collection(data, b => Assert.Equal((byte)'1', b));
        }

        [Fact]
        public async Task WriteAsJsonAsync_NullValue_JsonResponse()
        {
            // Arrange
            var body = new MemoryStream();
            var context = new DefaultHttpContext();
            context.Response.Body = body;

            // Act
            await context.Response.WriteAsJsonAsync(value: null, typeof(int?));

            // Assert
            Assert.Equal(JsonConstants.JsonContentTypeWithCharset, context.Response.ContentType);

            var data = Encoding.UTF8.GetString(body.ToArray());
            Assert.Equal("null", data);
        }

        [Fact]
        public async Task WriteAsJsonAsync_NullType_ThrowsArgumentNullException()
        {
            // Arrange
            var body = new MemoryStream();
            var context = new DefaultHttpContext();
            context.Response.Body = body;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await context.Response.WriteAsJsonAsync(value: null, type: null));
        }

        [Fact]
        public async Task WriteAsJsonAsync_NullResponse_ThrowsArgumentNullException()
        {
            // Arrange
            var body = new MemoryStream();
            var context = new DefaultHttpContext();
            context.Response.Body = body;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await HttpResponseJsonExtensions.WriteAsJsonAsync(response: null, value: null, typeof(int?)));
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
