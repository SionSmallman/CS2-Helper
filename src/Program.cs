using Cs2Bot.Data.Repositories;
using Cs2Bot.Data.Repositories.Interfaces;
using Cs2Bot.Models;
using Cs2Bot.Services;
using Cs2Bot.Services.Interfaces;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using InteractionFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Reflection;

public class Program
{
    private static IServiceProvider _serviceProvider;
    private DiscordSocketClient client;

    static IServiceProvider CreateProvider()
    {

        IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build(); 

        var discordConfig = new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.AllUnprivileged
        };

        var dbString = config["DbConnectionString"];

        var collection = new ServiceCollection()
            .AddSingleton(config)
            .AddSingleton(discordConfig)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<InteractionHandler>()
            .AddScoped<ISteamService, SteamService>()
            .AddScoped<IGuildRepository, GuildRepository>()
            .AddSingleton<OnJoin>()
            .AddHttpClient()
            .AddDbContext<BotDbContext>(options =>
            {
                var connectionString = config["DbConnectionString"];
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });



        return collection.BuildServiceProvider();
    }

    static async Task Main(string[] args)
    {
        var _services = CreateProvider();

        var client = _services.GetRequiredService<DiscordSocketClient>();
        await _services.GetRequiredService<InteractionHandler>()
           .InitializeAsync();


        
        var _config = _services.GetRequiredService<IConfiguration>();

        client.Log += LogAsync;
        client.JoinedGuild += JoinedGuild;

        await client.LoginAsync(TokenType.Bot, _config["token"]);
        await client.StartAsync();

        await Task.Delay(Timeout.Infinite);

    }
    private static Task LogAsync(LogMessage message)
    {
        Console.WriteLine(message.ToString());
        return Task.CompletedTask;
    }

    private static Task JoinedGuild(SocketGuild guild)
    {
        // Check DB to see if returning guild, if so, set IsActive to true;
        // If new, add to DB
        Console.WriteLine($"Joined {guild.Name}, GuildId: {guild.Id}");
        var dbGuild = new Guild()
        {
            GuildId = (long)guild.Id,
            IsActive = true
        };


        return Task.CompletedTask;
    }
}