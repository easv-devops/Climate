using Fleck;
using lib;
using System.Text.Json;

namespace api.helpers;

public static class ExtensionMethods
{
    public static void SendDto<T>(this IWebSocketConnection ws, T dto) where T : BaseDto
    {
        ws.Send(JsonSerializer.Serialize(dto) ?? throw new ArgumentException("Failed to serialize dto"));
    }
}