using ASSR.Server.Services;
using JWTAuthentication.Authentication;
using LinkedApiIntegration.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Quartz;
using ReactWithASP.Server;
using ReactWithASP.Server.Controllers;
using ReactWithASP.Server.InterfaceServices;
using ReactWithASP.Server.Models;
using ReactWithASP.Server.Services;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Hangfire;
using Hangfire.SqlServer;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
// Add services to the container.
/*builder.Services.AddTransient<IEmailSender, EmailSender>();*/
builder.Services.AddControllers();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<ILinkedInService, LinkedInService>();
//builder.Services.AddHostedService<BackgroundJob>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddHostedService<SubscriptionExpirationChecker>();
// Add Quartz services
builder.Services.AddQuartz(q =>
{
    q.AddJob<NotificationJob>(opts => opts.WithIdentity("NotificationJob").StoreDurably()); // Ensure the job is durable
    q.AddJob<ScheduledOnTimeJob>(opts => opts.WithIdentity("ScheduledOnTimeJob").StoreDurably()); // Ensure the job is durable
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
// Other service registrations
builder.Services.AddControllersWithViews();
builder.Services.AddHangfire(config =>
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
          .UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfireServer();



// For Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<ApplicationUser>>();
// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})

// Adding Jwt Bearer
            .AddJwtBearer(options =>
             {
                 options.SaveToken = true;
                 options.RequireHttpsMetadata = false;
                 options.TokenValidationParameters = new TokenValidationParameters()
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidAudience = builder.Configuration["JWT:ValidAudience"],
                     ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
                 };
             })
            .AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
                facebookOptions.SaveTokens = true;
                facebookOptions.Scope.Add("email"); // Add the email scope
            });
/*.AddFacebook(options =>
{
    options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
    options.Scope.Add("email");
    options.Scope.Add("public_profile");
    options.Fields.Add("email");
    options.Fields.Add("name");
    options.Fields.Add("picture");
    options.Fields.Add("phone");  // Note: Facebook may not provide phone number directly
    options.SaveTokens = true;
});*/


//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll",
//        builder => builder.AllowAnyOrigin()
//                          .AllowAnyMethod()
//                          .AllowAnyHeader());
//});
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowAll", policy =>
  {
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader();
  });
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();




builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JWT Authentication API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: \"Bearer 12345abcdef\""
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                                new string[] {}
                            }
                       });
});

/*builder.Services.AddAuthentication().AddFacebook(opt =>
{
    opt.AppId = "1033466688290874";
    opt.AppSecret = "ebabb1971e2e6ba0ac7bef0f5da65343";
});*/


//----------------------------------------------------------------
// Add services to the container.


/*builder.Services.AddHttpClient();
builder.Services.AddSingleton<IFacebookAuthService, FacebookAuthService>();*/

// Register IdentityService
/*builder.Services.AddScoped<IIdentityService, IdentityService>();*/
//----------------------------------------------------------------------
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();


/*// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    //app.UseSwaggerUI();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "JWT Authentication API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root (e.g. http://localhost:<port>/)
    });

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}*/
app.UseRouting();
app.UseHttpsRedirection();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();
//app.UseCors("AllowAll");
app.UseCors("AllowAll");
app.MapControllers();


app.MapFallbackToFile("/index.html");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "JWT Authentication API v1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root (e.g. http://localhost:<port>/)
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.Run();
