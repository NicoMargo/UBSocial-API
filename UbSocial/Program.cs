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
        ValidAudience = "User",
        ValidIssuer = "api.UBSocial.com",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("9FKIW7hfFlkfdsfjglkjfasdf"))
    };
});

//Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("_corsPolicy",
                          policy =>
                          {
                              policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                          });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions()
{
    OnPrepareResponse = ctx => {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers",
          "Origin, X-Requested-With, Content-Type, Accept");
    },
});

app.UseCors("_corsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();