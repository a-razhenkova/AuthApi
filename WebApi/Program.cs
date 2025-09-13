using WebApi;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.BindConfigurationSource();

builder.AddLogger();
builder.AddCache();
builder.AddDatabase();

builder.AddMapper();
builder.AddServices();
builder.AddHealthChecks();

builder.AddControllers();
builder.AddAuthentication();
builder.AddAuthorization();
builder.AddRateLimiter();

builder.AddSwagger();

WebApplication app = builder.Build();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();
app.UseSwagger();

app.UseMiddleware<HttpMessageLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<HttpHeaderHandlingMiddleware>();

await app.ApplyDbPendingMigrationsAndScriptsAsync();

await app.RunAsync();