using Microsoft.EntityFrameworkCore;
using RESTAPI.Data;
using RESTAPI.Initializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=dpg-csmt149u0jms73fsvka0-a.oregon-postgres.render.com;Port=5432;Database=lab3_pehi;Username=postgress;Password=Ra5Sp7JJIWNsuRzpQ56HB7cAvB1H5IeJ"));

builder.Services.AddScoped<IDbInitializer, DbInitialize>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    dbInitializer.Initialize();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
