using BikeStoreVendor.API.Middelware;
using BikeStoreVendor.Data.Access;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<BikeStoreVendor.Data.Access.AppContext>(options =>
options.UseSqlServer(
                             builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IDapper, Dapperr>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<BasicAuth>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
