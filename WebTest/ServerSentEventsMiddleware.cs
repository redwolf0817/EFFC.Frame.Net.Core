using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebTest
{
    public class ServerSentEventsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ServerSentEventsService _serverSentEventsService;

        public ServerSentEventsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers["Accept"] == "text/event-stream")
            {
                var t = Task.Delay(-1);
                
                IHttpBufferingFeature bufferingFeature = context.Features.Get<IHttpBufferingFeature>();
                if (bufferingFeature != null)
                {
                    bufferingFeature.DisableResponseBuffering();
                }
                context.Response.ContentType = "text/event-stream";
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Body.Flush();

                while (!context.RequestAborted.WaitHandle.GetSafeWaitHandle().IsClosed)
                {
                    string data = "id: 123456\nevent: message\ndata:now time: "+DateTime.Now+"\n\n";

                    await context.Response.WriteAsync(data);
                    Thread.Sleep(2000);
                }
                
                
                

                
                

                
            }
            else
            {
               
            }
        }
    }

    public class ServerSentEventsClient
    {
        private readonly HttpResponse _response;

        internal ServerSentEventsClient(HttpResponse response)
        {
            _response = response;
        }
    }

    public class ServerSentEventsService
    {
        private readonly ConcurrentDictionary<Guid, ServerSentEventsClient> _clients = new ConcurrentDictionary<Guid, ServerSentEventsClient>();

        internal Guid AddClient(ServerSentEventsClient client)
        {
            Guid clientId = Guid.NewGuid();

            _clients.TryAdd(clientId, client);

            return clientId;
        }

        internal void RemoveClient(Guid clientId)
        {
            ServerSentEventsClient client;

            _clients.TryRemove(clientId, out client);
        }
    }

    public class ServerSentEvent
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public IList<string> Data { get; set; }
    }

    internal static class ServerSentEventsHelper
    {
        internal static async Task WriteSseEventAsync(this HttpResponse response, ServerSentEvent serverSentEvent)
        {
            if (!String.IsNullOrWhiteSpace(serverSentEvent.Id))
                await response.WriteSseEventFieldAsync("id", serverSentEvent.Id);

            if (!String.IsNullOrWhiteSpace(serverSentEvent.Type))
                await response.WriteSseEventFieldAsync("event", serverSentEvent.Type);

            if (serverSentEvent.Data != null)
            {
                foreach (string data in serverSentEvent.Data)
                    await response.WriteSseEventFieldAsync("data", data);
            }

            await response.WriteSseEventBoundaryAsync();
            response.Body.Flush();
        }

        private static Task WriteSseEventFieldAsync(this HttpResponse response, string field, string data)
        {
            return response.WriteAsync($"{field}: {data}\n");
        }

        private static Task WriteSseEventBoundaryAsync(this HttpResponse response)
        {
            return response.WriteAsync("\n");
        }
    }
}
