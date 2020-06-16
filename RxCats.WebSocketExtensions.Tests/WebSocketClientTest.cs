using System;
using System.Net.WebSockets;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace RxCats.WebSocketExtensions.Tests
{
    public class WebSocketClientTest
    {
        private readonly ITestOutputHelper testOutputHelper;

        public WebSocketClientTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        //[Fact]
        public async void ConnectTest()
        {

            var client = new ClientWebSocket();
            
            await client.ConnectAsync(new Uri("wss://localhost:5001/ws"), CancellationToken.None);
            
            Assert.Equal(WebSocketState.Open, client.State);
        }
    }
}