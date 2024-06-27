using Microsoft.AspNetCore.Authentication.Cookies;
using TelebidTask.Data.Contracts;
using TelebidTask.Data.DatabaseRepository;
using TelebidTask.Services.Contracts;
using TelebidTask.Services.PasswordService;
using TelebidTask.Services.UserService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
    options.AddPolicy("AllowSpecificOrigin", builder => 
        builder.WithOrigins("http://localhost:3005")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    )
);

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDatabaseRepository, DatabaseRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
