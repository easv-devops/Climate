using System.Text.Json;
using lib;
using Websocket.Client;

namespace tests.WebSocket;

public class WebSocketTestClient
{
    public readonly WebsocketClient Client;
    public readonly List<BaseDto> ReceivedMessages = new List<BaseDto>();

    public WebSocketTestClient(string? url = null)
    {
        this.Client = url == null ? new WebsocketClient(new Uri("ws://localhost:" + (Environment.GetEnvironmentVariable("FULLSTACK_API_PORT") ?? "8181"))) : new WebsocketClient(new Uri(url));
        this.Client.MessageReceived.Subscribe<ResponseMessage>((Action<ResponseMessage>) (msg =>
        {
            BaseDto baseDto = JsonSerializer.Deserialize<BaseDto>(msg.Text);
            lock (this.ReceivedMessages)
                this.ReceivedMessages.Add(baseDto);
        }));
    }

    public async Task<WebSocketTestClient> ConnectAsync()
    {
        await this.Client.Start();
        if (!this.Client.IsRunning)
            throw new Exception("Could not start client!");
        return this;
    }

    public void Send<T>(T dto) where T : BaseDto
    {
        this.Client.Send(JsonSerializer.Serialize<T>(dto));
    }

    public async Task DoAndAssert<T>(T? action = null, Func<List<BaseDto>, bool>? condition = null) where T : BaseDto
    {
        if ((object) (T) action != null)
            this.Send<T>(action);
        if (condition != null)
        {
            DateTime startTime = DateTime.UtcNow;
            while (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(5.0))
            {
                lock (this.ReceivedMessages)
                {
                    if (condition(this.ReceivedMessages))
                        return;
                }
                await Task.Delay(100);
            }
            throw new TimeoutException("Condition not met: ");
        }
    }
}