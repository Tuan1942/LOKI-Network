using LOKI_Network.DbContexts;
using LOKI_Network.Interface;
using LOKI_Network.Middleware;
using LOKI_Network.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// Register services
builder.Services.AddSingleton<WebSocketService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFriendshipService, FriendshipService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IFileService, FileService>(provider =>
    new FileService(provider.GetRequiredService<LokiContext>(), configuration["FileStoragePath"]));

builder.Services.AddControllers(
    options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

// DbContext Connection
builder.Services.AddDbContext<LokiContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSignalR();

var key = Encoding.ASCII.GetBytes(configuration["Jwt:SecretKey"]);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Websocket
app.UseWebSockets();
app.UseMiddleware<WebSocketMiddleware>();
//app.Use(async (context, next) =>
//{
//    // Handle WebSocket connections
//    if (context.WebSockets.IsWebSocketRequest)
//    {
//        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
//        // Handle the WebSocket connection
//        await HandleWebSocketConnection(webSocket);
//    }
//    else
//    {
//        // Handle non-WebSocket requests
//        await next();
//    }
//});



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

async Task HandleWebSocketConnection(WebSocket webSocket)
{
    var buffer = new byte[1024 * 4];

    while (webSocket.State == WebSocketState.Open)
    {
        try
        {
            // Receive messages from the WebSocket
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                // Close the WebSocket connection
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
            else if (result.MessageType == WebSocketMessageType.Text)
            {
                // Process the received message
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Received message: {message}");

                // Optionally, send a response back
                var responseMessage = Encoding.UTF8.GetBytes("Message received");
                await webSocket.SendAsync(new ArraySegment<byte>(responseMessage), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket error: {ex.Message}");
            break;
        }
    }

    webSocket.Dispose();
}