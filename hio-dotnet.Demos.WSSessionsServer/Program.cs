using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Net.WebSockets;
using hio_dotnet.Demos.WSSessionsServer;
using hio_dotnet.HWDrivers.Server;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var loginSettings = builder.Configuration.GetSection("loginSettings");

MainDataContext.Login = loginSettings["login"] ?? "admin";
MainDataContext.Password = loginSettings["password"] ?? "12345678";

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear(); // Povol� proxy i mimo zn�m� s�t�
    options.KnownProxies.Clear();  // Povol� proxy i mimo zn�m� IP
});

var app = builder.Build();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.UseForwardedHeaders();

var webSocketOptions = new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};
app.UseWebSockets(webSocketOptions);

app.MapPost("/login", (LoginRequest request) =>
{
    if (request.Username == MainDataContext.Login && request.Password == MainDataContext.Password)
    {
        var token = JWTHelpers.GenerateJwtToken(jwtSettings);
        var expiresAt = DateTime.UtcNow.AddHours(1);
        MainDataContext.AddToken(request.Username, token, expiresAt);
        return Results.Ok(new { Token = token });
    }
    return Results.Unauthorized();
});

app.MapGet("/addsession", (HttpContext context) =>
{
    if (JWTHelpers.CheckToken(context) == false)
    {
        return Results.Unauthorized();
    }

    var session = new WSSession();
    session.ExpiresAt = DateTime.UtcNow.AddDays(1);
    MainDataContext.AddSession(session);
    return Results.Ok(session.Id.ToString());
});

app.Map("/ws", async (HttpContext context) =>
{
    var upgradeHeader = context.Request.Headers["Upgrade"].ToString().ToLower();
    var connectionHeader = context.Request.Headers["Connection"].ToString().ToLower();

    if (upgradeHeader == "websocket" && connectionHeader.Contains("upgrade"))
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await WebSocketProcessor.HandleWebSocketAsync(webSocket, context);
    }
    else
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Invalid WebSocket request");
    }
});

app.Run();

public record LoginRequest(string Username, string Password);

