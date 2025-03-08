using Lopez_TicTacToe.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR(); // Add SignalR support

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapHub<GameHub>("/gameHub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
