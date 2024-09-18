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
using System.Diagnostics;


namespace Cs2Bot
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder().ConfigureServices((hostContext, services) =>
            {
                IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

                var discordConfig = new DiscordSocketConfig()
                {
                    GatewayIntents = GatewayIntents.AllUnprivileged
                };

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

            var client= host.Services.GetRequiredService<DiscordSocketClient>();
            await host.Services.GetRequiredService<InteractionHandler>()
               .InitializeAsync();

            // SERVICES THAT HOOK INTO EVENTS MUST BE INITIALISED HERE
            // Initialise join services
            var join = host.Services.GetRequiredService<OnJoinService>();

            var _config = host.Services.GetRequiredService<IConfiguration>();

            client.Log += LogAsync;

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