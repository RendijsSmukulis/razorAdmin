using RazorAdmin.Services;
using RazorAdmin.Repositories;
using RazorAdmin.Configuration;
using RazorAdmin.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure strongly-typed settings
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// Add repositories
builder.Services.AddScoped<IFeatureRepository, FeatureRepository>();

// Add services
builder.Services.AddScoped<IFeatureService, FeatureService>();
builder.Services.AddScoped<IDatabaseInitializationService, DatabaseInitializationService>();

// Add Markdown service
builder.Services.AddSingleton<IMarkdownService, MarkdownService>();

// Add Swagger/OpenAPI services
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "RazorAdmin Features API",
        Version = "v1",
        Description = "A RESTful API for managing features in the RazorAdmin system",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "RazorAdmin Team",
            Email = "admin@razoradmin.com"
        }
    });
    
    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RazorAdmin Features API V1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "RazorAdmin API Documentation";
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Add global exception handler
app.UseMiddleware<GlobalExceptionHandler>();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var databaseInitializationService = scope.ServiceProvider.GetRequiredService<IDatabaseInitializationService>();
    await databaseInitializationService.InitializeDatabaseAsync();
}

app.Run();
