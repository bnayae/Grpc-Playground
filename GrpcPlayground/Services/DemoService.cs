using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Weknow;

namespace GrpcPlayground
{
    public class DemoService : Service.ServiceBase
    {
        private readonly ILogger<DemoService> _logger;
        public DemoService(ILogger<DemoService> logger)
        {
            _logger = logger;
        }

        public override Task<DemoResponse> Call(
            DemoRequest request, 
            ServerCallContext context)
        {
            return Task.FromResult(new DemoResponse
            {
                Message = "Hello " + request.Name
            });
        }

        public override async Task<DemoResponse> Upload(
            IAsyncStreamReader<DemoRequest> requestStream, 
            ServerCallContext context)
        {
            var builder = new StringBuilder();
            while (await requestStream.MoveNext(context.CancellationToken))
            {
                DemoRequest item = requestStream.Current;
                _logger.LogInformation("Uploading {request}", item);
                builder.Append($"{item.Name}, ");
            }
            return new DemoResponse { Message = $"Hello {builder}" };
        }

        public override async Task Download(
            DemoRequest request,
            IServerStreamWriter<DemoResponse> responseStream,
            ServerCallContext context)
        {
            for (int i = 0; i < 5; i++)
            {
                await responseStream.WriteAsync(new DemoResponse { Message = $"{request.Name} -> processed" });
                await Task.Delay(800);
            }
        }

        public override async Task Transform(
                        IAsyncStreamReader<DemoRequest> requestStream, 
                        IServerStreamWriter<DemoResponse> responseStream, 
                        ServerCallContext context)
        {
            while (await requestStream.MoveNext(context.CancellationToken))
            {
                DemoRequest item = requestStream.Current;
                _logger.LogInformation("Uploading {request}", item);
                await responseStream.WriteAsync(new DemoResponse { Message = $"{item.Name} -> processed" });
            }
        }
    }
}
