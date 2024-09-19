using Coravel;
using Cs2Bot.Data.Repositories;
using Cs2Bot.Data.Repositories.Interfaces;
using Cs2Bot.Invocables;
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
using Microsoft.Extensions.Hosting;


namespace Cs2Bot
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            
            // Build host
            var host = Host.CreateDefaultBuilder().ConfigureServices((hostContext, services) =>
            {
                IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

                var discordConfig = new DiscordSocketConfig()
                {
                    GatewayIntents = GatewayIntents.AllUnprivileged
                };
                
                // Add services to container
                services
                    .AddSingleton(config)
                    .AddSingleton(discordConfig)
                    .AddSingleton<DiscordSocketClient>()
                    .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                    .AddSingleton<InteractionHandler>()
                    .AddScoped<ISteamService, SteamService>()
                    .AddSingleton<IPatchNotesService, PatchNotesService>()
                    .AddScoped<IGuildRepository, GuildRepository>()
                    .AddScoped<IPatchNotesSettingRepository, PatchNotesSettingRepository>()
                    .AddSingleton<OnJoinService>()
                    .AddTransient<CheckForPatchInvocable>()
                    .AddHttpClient()
                    .AddDbContext<BotDbContext>(options =>
                    {
                        var connectionString = config["ConnectionStrings:Db"];
                        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                    })
                    .AddScheduler();
            }).Build();

            // Initialise client and interaction handler
            var client = host.Services.GetRequiredService<DiscordSocketClient>();
            await host.Services.GetRequiredService<InteractionHandler>()
               .InitializeAsync();

            // SERVICES THAT HOOK INTO EVENTS MUST BE INITIALISED HERE
            var join = host.Services.GetRequiredService<OnJoinService>();
            client.Log += LogAsync;

            var _config = host.Services.GetRequiredService<IConfiguration>();
            await client.LoginAsync(TokenType.Bot, _config["token"]);
            await client.StartAsync();

            // Schedule jobs
            host.Services.UseScheduler(scheduler =>
            {
                scheduler.Schedule<CheckForPatchInvocable>().EveryMinute();
            });

            await host.StartAsync();
            await Task.Delay(Timeout.Infinite);
        }

        private static Task LogAsync(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }
    }
}