var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Add HttpClient for microservices
builder.Services.AddHttpClient("TimeService", client =>
{
    client.BaseAddress = new Uri("http://timeservice:8080/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient("DataService", client =>
{
    client.BaseAddress = new Uri("http://dataservice:8080/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowVueFrontend");
app.UseRouting();
app.MapControllers();

app.Run();