
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Authentication;
using api.helpers;
using api.security;
using api.WebSocket;
using Fleck;
using infrastructure;
using infrastructure.Models.serverEvents;
using lib;
using MySqlConnector;
using service.services;

var builder = WebApplication.CreateBuilder(args);

//saves connection string
//gets connection string to db
builder.Services.AddSingleton(provider => Utilities.MySqlConnectionString);

builder.Services.AddSingleton(provider => new PasswordHashRepository(provider.GetRequiredService<string>()));
builder.Services.AddSingleton(provider => new UserRepository(provider.GetRequiredService<string>()));
builder.Services.AddSingleton(provider => new (provider.GetRequiredService<string>()));
builder.Services.AddSingleton<UserRepository>();


builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<AuthService>();


// Add services to the container.

var services = builder.FindAndInjectClientEventHandlers(Assembly.GetExecutingAssembly());


builder.WebHost.UseUrls("http://*:9999");

var app = builder.Build();

var server = new WebSocketServer("ws://0.0.0.0:8181");
server.RestartAfterListenError = true;

server.Start(socket =>
{
    socket.OnOpen = () => StateService.AddClient(socket.ConnectionInfo.Id, socket);
    socket.OnClose = () => StateService.RemoveClient(socket.ConnectionInfo.Id);
    socket.OnMessage = async message =>
    {
        try
        {
            await app.InvokeClientEventHandler(services, socket, message);
        }
        catch (Exception e)
        {
            //error handler
            //todo should have a logger that logs the error so we can se it when deployed 
            if (app.Environment.IsProduction() && (e is ValidationException || e is AuthenticationException))
            {
                socket.SendDto(new ServerSendsErrorMessageToClient()
                {
                    errorMessage = "Something went wrong",
                    receivedMessage = message
                });
            }
            else
            {
                socket.SendDto(new ServerSendsErrorMessageToClient
                    { errorMessage = e.Message, receivedMessage = message });
            }
        }
    };
});

app.Run();

