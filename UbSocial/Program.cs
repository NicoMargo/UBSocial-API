using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UbSocial.Models.Helpers;

var builder = WebApplication.CreateBuilder(args);

DBHelper.ConnectionString = builder.Configuration["ConnectionString"];
// Add services to the container.

builder.Services.AddControllers();

//JWT config
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

//Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("_corsPolicy",
                          policy =>
                          {
                              policy.WithOrigins(
                                  "http://localhost:3000",
                                  "http://localhost:80",
                                  "http://frontadmin.ayukelen.com.ar", 
                                  "http://ktm.ddns.net:41678")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .AllowCredentials();
                          });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseStaticFiles();
app.Use((context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
        return Task.CompletedTask;
    }

    return next();
});
app.UseCors("_corsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();