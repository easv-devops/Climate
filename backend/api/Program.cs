using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Authentication;
using api.helpers;
using api.mqttEventListeners;
using api.ServerEventHandlers;
using api.serverEventModels;
using api.WebSocket;
using Fleck;
using infrastructure;
using infrastructure.repositories;
using infrastructure.repositories.readingsRepositories;
using lib;
using service.services;
using service.services.notificationServices;

namespace api;

public static class Startup
{
    public static void Main(string[] args)
    {
        var app = Start(args, "");
        app.Run();
    }

    
    public static WebApplication Start(string[] args, string? connectionString)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddSingleton<SmtpRepository>();
        
        if (ReferenceEquals(connectionString, ""))
        {
            var dbConnTask = KeyVaultService.GetDbConn();
            dbConnTask.Wait();
            connectionString =  dbConnTask.Result;
            builder.Services.AddSingleton(provider => connectionString);
        }
        else
        {
            // Gets connection string for local testing
            builder.Services.AddSingleton(provider => connectionString);
        }
        
        builder.Services.AddSingleton(provider => new PasswordHashRepository(provider.GetRequiredService<string>()));
        builder.Services.AddSingleton(provider => new UserRepository(provider.GetRequiredService<string>()));
        builder.Services.AddSingleton(provider => new DeviceRepository(provider.GetRequiredService<string>()));
        builder.Services.AddSingleton(provider => new RoomsRepository(provider.GetRequiredService<string>()));
        builder.Services.AddSingleton(provider => new DeviceSettingsRepository(provider.GetRequiredService<string>()));
        builder.Services.AddSingleton(provider => new DeviceRangeRepository(provider.GetRequiredService<string>()));
        builder.Services.AddSingleton(provider => new AlertRepository(provider.GetRequiredService<string>()));

        builder.Services.AddSingleton<DeviceSettingsService>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<TokenService>();
        builder.Services.AddSingleton<NotificationService>();
        builder.Services.AddSingleton<DeviceService>();
        builder.Services.AddSingleton<DeviceReadingsService>();
        builder.Services.AddSingleton<RoomService>();
        builder.Services.AddSingleton<UserService>();
        builder.Services.AddSingleton<RoomReadingsService>();
        builder.Services.AddSingleton<AlertService>();

        builder.Services.AddSingleton(provider => new HumidityRepository(provider.GetRequiredService<string>()));
        builder.Services.AddSingleton(provider => new TemperatureRepository(provider.GetRequiredService<string>()));
        builder.Services.AddSingleton(provider => new ParticlesRepository(provider.GetRequiredService<string>()));
        builder.Services.AddSingleton<MqttClientSubscriber>();
        
        builder.Services.AddSingleton<ServerWantsToSendDevice>();
        builder.Services.AddSingleton<ServerWantsToSendRoom>();
        builder.Services.AddSingleton<ServerWantsToSendUser>();
        builder.Services.AddSingleton<ServerWantsToInitUser>();
        builder.Services.AddSingleton<ServerWantsToSendAlert>();

        // Add services to the container.
        var services = builder.FindAndInjectClientEventHandlers(Assembly.GetExecutingAssembly());
        
        var app = builder.Build();
        
        var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
        var server = new WebSocketServer("ws://0.0.0.0:"+port);
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
                    Console.WriteLine(e);
                    //error handler
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
        app.Services.GetService<MqttClientSubscriber>()?.CommunicateWithBroker();
        return app;
    }
}