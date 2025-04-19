using back_end.Utilities.Extensions;
using FastEndpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddFastEndpoints();

builder.Services.WithConfiguration(builder.Configuration);
builder.Services.ConfigureOptions();
builder.Services.ConfigureLogging();
builder.Services.ConfigureDependencies();
builder.Services.ConfigureDatabase();
builder.Services.ConfigureHttpClients();
builder.Services.ConfigureAuth();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowOrigins",
                      policy =>
                      {
                          policy.WithOrigins(
                              "https://localhost:7052",
                              "http://192.168.1.3",
                              "https://192.168.1.3",
                              "http://localhost:7051",
                              "https://qa-web-mu.vercel.app",
                              "https://qa-web-dthvinhs-projects.vercel.app",
                              "https://qa-2vvl99o65-dthvinhs-projects.vercel.app");
                          policy.AllowAnyHeader();
                          policy.SetIsOriginAllowed(origin => true);
                          policy.AllowAnyMethod();
                          policy.AllowCredentials();
                      });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseCors("AllowOrigins");

app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api";
});
app.UseHttpsRedirection();

app.Run();
