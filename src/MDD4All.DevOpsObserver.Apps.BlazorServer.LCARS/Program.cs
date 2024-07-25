using MDD4All.DevOpsObserver.Apps.BlazorServer.LCARS.Data;
using MDD4All.DevOpsObserver.DataModels;
using MDD4All.DevOpsObserver.StatusLightControl.Contracts;
using MDD4All.DevOpsObserver.StatusLightControl.Hue;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace MDD4All.DevOpsObserver.Apps.BlazorServer.LCARS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddHttpClient();

            builder.Configuration.AddJsonFile("ProductionSecrets.json",
                                              optional: true,
                                              reloadOnChange: true);

            string ip = builder.Configuration["HueStatusLight:IP"];
            string key = builder.Configuration["HueStatusLight:ApiKey"];
            string bulbID = builder.Configuration["HueStatusLight:BulbID"];

            HueStatusLightController hueStatusLightController = new HueStatusLightController(ip, key, bulbID);
            builder.Services.AddSingleton<IStatusLightController>(hueStatusLightController);

            string configJSON = File.ReadAllText("DevOpsConfiguration.json");

            DevOpsConfiguration? devOpsConfiguration = JsonConvert.DeserializeObject<DevOpsConfiguration>(configJSON);

            if (devOpsConfiguration != null)
            {
                builder.Services.AddSingleton<DevOpsConfiguration>(devOpsConfiguration);
            }

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}