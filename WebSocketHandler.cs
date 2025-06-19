using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

public class WebSocketHandler
{
    public async Task HandleWebSocketAsync(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Binary)
            {
                using var ms = new MemoryStream(buffer, 0, result.Count);
                var dataLength = result.Count - 256; // 256 bytes cho chữ ký
                var data = new byte[dataLength];
                var signature = new byte[256];
                await ms.ReadAsync(data, 0, dataLength);
                await ms.ReadAsync(signature, 0, 256);

                // Lưu dữ liệu và chữ ký
                File.WriteAllBytes("received_data", data);
                File.WriteAllBytes("received_signature", signature);
                await webSocket.SendAsync(System.Text.Encoding.UTF8.GetBytes("Data and signature received"), 
                    WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}