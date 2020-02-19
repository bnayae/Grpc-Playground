using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Threading;
using System.Threading.Tasks;
using Weknow;

namespace ConsoleClient
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            // The port number(5001) must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Service.ServiceClient(channel);
            var reply = await client.CallAsync(
                              new DemoRequest { Name = "Bnaya" });
            Console.WriteLine("Single call: " + reply.Message);

            Console.WriteLine("-------------------------------------------");

            AsyncClientStreamingCall<DemoRequest, DemoResponse> producer = 
                client.Upload(new Grpc.Core.CallOptions());
            for (int i = 0; i < 5; i++)
            {
                await producer.RequestStream.WriteAsync(new DemoRequest { Name = "User " + i });
                await Task.Delay(800);
            }
            await producer.RequestStream.CompleteAsync();
            reply = await producer;
            Console.WriteLine("Calls completes: " + reply.Message);

            Console.WriteLine("-------------------------------------------");

            AsyncDuplexStreamingCall<DemoRequest, DemoResponse> duplex = 
                client.Transform(new Grpc.Core.CallOptions());
            var _ = Task.Run(async () => 
            {
                await foreach (DemoResponse r in duplex.ResponseStream.ReadAllAsync(CancellationToken.None))
                {
                    Console.WriteLine(r.Message);
                }
            });
            for (int i = 0; i < 5; i++)
            {
                await duplex.RequestStream.WriteAsync(new DemoRequest { Name = "Actor " + i });
                await Task.Delay(800);
            }
            await duplex.RequestStream.CompleteAsync();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
