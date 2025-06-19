using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<WebSocketHandler>();
builder.WebHost.UseUrls("http://0.0.0.0:5500"); // Chấp nhận kết nối LAN trên cổng 5500

var app = builder.Build();

app.UseWebSockets();
app.UseStaticFiles();

app.MapGet("/", async context => 
    await context.Response.WriteAsync(File.ReadAllText(Path.Combine(builder.Environment.WebRootPath, "index.html"))));

app.MapGet("/verify", async context => 
    await context.Response.WriteAsync(File.ReadAllText(Path.Combine(builder.Environment.WebRootPath, "verify.html"))));

app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var handler = context.RequestServices.GetRequiredService<WebSocketHandler>();
        await handler.HandleWebSocketAsync(webSocket);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.Run();