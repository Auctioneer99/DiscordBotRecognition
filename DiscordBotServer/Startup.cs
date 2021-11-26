using DiscordBotRecognition;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscordBotServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSingleton(BuildBot());
        }

        private DiscordBot BuildBot()
        {
            var buildTask = DiscordBot.DefaultBuild(Configuration["GOOGLE_API_KEY"], Configuration["KEYCLOAK_SECRET"]);
            var bot = buildTask.GetAwaiter().GetResult();

            var startTask = bot.Start(Configuration["DISCORD_TOKEN"]);
            startTask.GetAwaiter().GetResult();

            return bot;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            //app.UseMvcWithDefaultRoute();
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();     // нет определенных маршрутов
            //});
            //app.UseMvc();
        }
    }
}
