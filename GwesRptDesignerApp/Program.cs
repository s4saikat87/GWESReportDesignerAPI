using GwesRptDesignerApp.Data;
using GwesRptDesignerApp.Models;
using GwesRptDesignerApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text;


/*
 * Author       :       Sanjit Adhikary
 * Created On   :       08-SEPTEMBER-2022
 */

var builder = WebApplication.CreateBuilder(args);

var secretKey = builder.Configuration.GetValue<string>("AppSettings:Secret");
bool validateLifeTime = false;
if (builder.Configuration.GetValue<string>("AppSettings:ValidateLifetime") == "true") { validateLifeTime = true; }
bool validateIssuerSigningKey=false;
if (builder.Configuration.GetValue<string>("AppSettings:ValidateIssuerSigningKey") == "true") { validateIssuerSigningKey = true; }
bool validateAudience = false;
if (builder.Configuration.GetValue<string>("AppSettings:ValidateAudience") == "true") { validateAudience = true; }
bool validateIssuer = false;
if (builder.Configuration.GetValue<string>("AppSettings:ValidateIssuer") == "true") { validateIssuer = true; }
var validAudience = builder.Configuration.GetValue<string>("AppSettings:ValidAudience");
var validIssuer = builder.Configuration.GetValue<string>("AppSettings:ValidIssuer");


builder.Services.AddAuthentication(opt => {
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = validateIssuer,
            ValidateAudience = validateAudience,
            ValidateLifetime = validateLifeTime,
            ValidateIssuerSigningKey = validateIssuerSigningKey,
            ValidIssuer = validIssuer,
            ValidAudience = validAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("EnableCORS", builder =>
    {
        builder.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<UserContext>(opts =>
    opts.UseSqlServer(builder.Configuration["ConnectionString:DefaultConnection"]));

builder.Services.AddTransient<ITokenService, TokenService>();

builder.Services.AddControllers();


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseCors("EnableCORS");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.Run();