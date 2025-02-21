using System.Text.Json.Serialization;
using DotNetEnv;
using FPT.TeamMatching.API.Collections;
using FPT.TeamMatching.API.Hub;
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
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Npgsql;

Env.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging();

#region Hangfire Config

builder.Services.AddHangfire(config =>
{
    config.UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddHangfireServer();

builder.Services.AddScoped<IJobHangfireService, JobHangFireService>();

#endregion

#region Add SignalR

builder.Services.AddSignalR();

#endregion

#region Add Kafka Config

builder.Services.AddScoped<IKafkaProducerConfig, KafkaProducer>();

#endregion

#region Add-DbContext

var dbDataSource = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("DefaultConnection")).Build();
builder.Services.AddDbContext<FPTMatchingDbContext>(options =>
{
    options.UseNpgsql(dbDataSource,
        npgsqlOptions => npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});


// var mongoClient = new MongoClient(builder.Configuration["MONGODB_URI"]);
// var database = mongoClient.GetDatabase("fpt-matching-chatroom");
// builder.Services.AddDbContext<ChatRoomDbContext>(options =>
// {
//     options.UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName);
// });

builder.Services.AddSingleton<ChatRoomDbContext>();

#endregion

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    // options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddCollectionRepositories();
builder.Services.AddCollectionServices();

#region Cloudinary

builder.Services.AddScoped<CloudinaryConfig>();

#endregion

#region Redis

builder.Services.AddSingleton<RedisConfig>();
builder.Services.AddSingleton<RedisUtil>();
#endregion

#region Config-Authentication_Authorization

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

        // Lấy token từ cookie
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            { // httpContext
                var accessToken = context.Request.Cookies["accessToken"];
                if (!string.IsNullOrEmpty(accessToken)) context.Token = accessToken;

                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

#endregion

#region Add-Cors

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


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// -----------------app-------------------------

var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<AuthenticationMiddleware>()
    .UseHttpsRedirection()
    .UseRouting()
    .UseCors("AllowSpecificOrigins")
    .UseAuthentication()
    .UseAuthorization()
    .UseHangfireDashboard();

app.MapHub<ChatHub>("/chat");
app.MapControllers();
app.Run();