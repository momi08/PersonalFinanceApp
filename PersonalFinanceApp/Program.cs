using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Components;
using PersonalFinanceApp.Data;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;

namespace PersonalFinanceApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();

            builder.Services.AddScoped<Authentication>();
            builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<Authentication>());
            builder.Services.AddScoped<SavingsGoalService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<TransactionService>();

            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string DefaultConnString not found.")));

            builder.Services
                .AddBlazorise(options =>
                 {
                    options.Immediate = true;
                 })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            var app = builder.Build();
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}