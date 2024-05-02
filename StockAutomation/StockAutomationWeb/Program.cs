using BusinessLayer.Services;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using StockAutomationCore.Download;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddConfiguration(StockAutomationCore.Configuration.StockAutomationConfig.Configuration);
var configuration = builder.Configuration;

var postgresConnectionString = configuration.GetConnectionString("PostgresConnectionString") ??
                               throw new InvalidOperationException(
                                   "Connection string 'PostgresConnectionString' not found.");
builder.Services.AddDbContext<StockAutomationDbContext>(options =>
    options.UseNpgsql(postgresConnectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<ISnapshotService<Downloader>, SnapshotService<Downloader>>();
builder.Services.AddHttpClient<ISnapshotService<Downloader>, SnapshotService<Downloader>>(c =>
{
    c.DefaultRequestHeaders.Add("User-Agent", "StockAutomationCore/1.0");
    c.BaseAddress = new Uri(configuration.GetSection("download")["defaultUrl"] ??
                            "https://ark-funds.com/wp-content/uploads/funds-etf-csv/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv");
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
