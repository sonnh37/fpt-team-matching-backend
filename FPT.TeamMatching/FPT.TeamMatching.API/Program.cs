using System.Text.Json.Serialization;
using DotNetEnv;
using FPT.TeamMatching.API.Collections;
using FPT.TeamMatching.API.Hubs;
using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Domain.Configs;
using FPT.TeamMatching.Domain.Configs.Mapping;
using FPT.TeamMatching.Domain.Contracts.Hangfire;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Utilities.Redis;
using FPT.TeamMatching.Services;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

Env.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging();

#region Core services

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(MappingProfile));

#endregion

#region Database

var dbDataSource = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("DefaultConnection")).Build();
builder.Services.AddDbContext<FPTMatchingDbContext>(options =>
{
    options.UseNpgsql(dbDataSource,
        npgsqlOptions => npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});
builder.Services.AddSingleton<ChatRoomDbContext>();

#endregion

#region Authentication & Authorization

builder.Services.Configure<TokenSetting>(builder.Configuration.GetSection("TokenSetting"));
builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = "Role",

            IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
            {
                var serviceProvider = builder.Services.BuildServiceProvider();
                var authService = serviceProvider.GetRequiredService<IAuthService>();

                var rsa = authService.GetRSAKeyFromTokenAsync(token, kid).Result;
                return new List<SecurityKey> { new RsaSecurityKey(rsa) };
            }
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Cookies["accessToken"];
                if (!string.IsNullOrEmpty(accessToken)) context.Token = accessToken;

                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

#endregion

#region CORS

builder.Services.AddCors(options =>
{
    var frontendDomains = builder.Configuration.GetValue<string>("Frontend:Domain")?.Split(',');

    options.AddPolicy("AllowSpecificOrigins", builder =>
    {
        builder.WithOrigins(frontendDomains)
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

#endregion

#region SignalR

builder.Services.AddSignalR();

#endregion

#region Kafka

builder.Services.AddScoped<IKafkaProducerConfig, KafkaProducer>();

#endregion

#region Hangfire

builder.Services.AddHangfire(config =>
{
    config.UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddHangfireServer();
builder.Services.AddScoped<IJobHangfireService, JobHangFireService>();

#endregion

#region Cloudinary

builder.Services.AddScoped<CloudinaryConfig>();

#endregion

#region Redis

builder.Services.AddSingleton<RedisConfig>();
builder.Services.AddSingleton<RedisUtil>();

#endregion

#region Custom Repositories & Services

builder.Services.AddCollectionRepositories();
builder.Services.AddCollectionServices();

#endregion

// -----------------app-------------------------

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AuthenticationMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHangfireDashboard();
app.MapHub<ChatHub>("/chat");
app.MapHub<NotificationHub>("/notification");
app.MapControllers();
app.Run();