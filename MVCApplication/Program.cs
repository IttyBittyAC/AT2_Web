using MVCApplication.Data;

namespace MVCApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddScoped<AppDb>();
            builder.Services.AddControllersWithViews();

            builder.Services.AddSession(o => {
                o.IdleTimeout = TimeSpan.FromHours(8);
                o.Cookie.HttpOnly = true;
                o.Cookie.Name = "session";
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseSession();


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=OneRingToRuleThemAll}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "better",

                pattern: "{table}/{action}/{id?}",
                defaults: new { controller = "OneRingToRuleThemAll" });

            app.Run();
        }
    }
}
