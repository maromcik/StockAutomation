using BusinessLayer.Facades;
using BusinessLayer.Services;
using DataAccessLayer;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using StockAutomationCore.Files;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var postgresConnectionString = configuration.GetConnectionString("PostgresConnectionString") ??
                               throw new InvalidOperationException(
                                   "Connection string 'PostgresConnectionString' not found.");

builder.Services.AddDbContext<StockAutomationDbContext>(options =>
    options.UseNpgsql(postgresConnectionString));

builder.Services.AddLogging();
builder.Services.AddRazorPages();
builder.Services.AddTransient<ISnapshotService, SnapshotService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddHttpClient<ISnapshotService, SnapshotService>(c =>
{
    c.DefaultRequestHeaders.Add("User-Agent", "StockAutomationCore/1.0");
    c.BaseAddress = new Uri(configuration.GetSection("download")["defaultUrl"] ??
                                   "https://ark-funds.com/wp-content/uploads/funds-etf-csv/ARK_INNOVATION_ETF_ARKK_HOLDINGS.csv");
});

builder.Services.AddTransient<ISendDifferencesFacade, SendDifferencesFacade>();

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    );

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();
app.UsePathBase(new PathString("/api"));
app.UseRouting();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


// WE WANT SWAGGER IN PRODUCTION AS WELL
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.UseCors();
app.Run();
