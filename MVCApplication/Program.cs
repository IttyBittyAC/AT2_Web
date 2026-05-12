using MVCApplication.Data;
using MVCApplication.XMLServices;
using MVCApplication.Models.Seeding;

namespace MVCApplication
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Add XML configuration file for feature flags
            builder.Configuration.AddXmlFile("Config/features.xml", optional: false, reloadOnChange: true);

            //Register XML configuration service (loads Config/*.xml)
            builder.Services.AddSingleton<IXMLConfigService, XMLConfigService>();
            builder.Services.Configure<AdminSettings>(builder.Configuration.GetSection("AdminSettings"));


            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<AppDb>();
            builder.Services.AddScoped<Seeding>();
            builder.Services.AddSession(o => {
                o.IdleTimeout = TimeSpan.FromHours(8);
                o.Cookie.HttpOnly = true;
                o.Cookie.Name = "session";
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDb>();
                var seed = scope.ServiceProvider.GetRequiredService<Seeding>();
                await db.EnsureCreated();
                await seed.SeedAdminUser();
                await seed.SeedUser();
                await seed.SeedFeedback();
                await seed.SeedEvent();
                await seed.SeedBooking();
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.MapGet("/sitemap.xml", (HttpContext _) =>
            {
                var file = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "sitemap.xml");
                return File.Exists(file) ? Results.File(file, "application/xml") : Results.NotFound();
            });

            app.MapGet("/rss.xml", (HttpContext _) =>
            {
                var file = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "rss.xml");
                return File.Exists(file) ? Results.File(file, "application/xml") : Results.NotFound();
            });

            app.UseRouting();

            app.UseSession();
            app.UseAuthorization();
            
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );
            app.Run();
        }
    }
}
