using System.Collections;
using AssetLayer.SDK;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new BasicExceptionFilter());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

AssetLayerSDK.Initialize(new AssetLayerConfig {
    appSecret = builder.Configuration["ASSETLAYER_APP_SECRET"],
    // didToken = ""
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

if (builder.Configuration["ASSETLAYER_APP_SECRET"] == null) logger.LogInformation("Warning: ASSETLAYER_APP_SECRET Not Set");

app.Run();
