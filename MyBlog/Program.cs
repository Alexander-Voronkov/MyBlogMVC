using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyBlog.Authorization;
using MyBlog.Data;
using MyBlog.Data.Entities;
using MyBlog.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IEmailService, EmailService>();

// Add services to the container.
builder.Services.AddDbContext<ApplicationContext>(o =>
{
    string cs = builder.Configuration.GetConnectionString("DefaultConnection");

    o.UseSqlServer(cs);
});

builder.Services.AddIdentity<User, IdentityRole>(setupAction =>
{
    setupAction.User.RequireUniqueEmail = true;
    setupAction.Password.RequireDigit = false;
    setupAction.Password.RequireUppercase = false;
    setupAction.Password.RequireLowercase = false;
    setupAction.Password.RequireNonAlphanumeric = false;
    setupAction.Password.RequiredLength = 1;
})
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders();


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        MyPolicies.PostsWriterAndAboveAccess,
        policy => policy.RequireAssertion(context =>
        {
            return context.User.HasClaim(
                claim => claim.Type == MyClaims.SuperAdmin ||
                         claim.Type == MyClaims.Admin ||
                         claim.Type == MyClaims.PostsWriter);
        }));

    options.AddPolicy(
        MyPolicies.AdminAndAboveAccess,
        policy => policy.RequireAssertion(context =>
        {
            return context.User.HasClaim(
                claim => claim.Type == MyClaims.SuperAdmin ||
                         claim.Type == MyClaims.Admin);
        }));

    options.AddPolicy(
    MyPolicies.SuperAdminAccessOnly,
    policy => policy.RequireAssertion(context =>
    {
        return context.User.HasClaim(
            claim => claim.Type == MyClaims.SuperAdmin);
    }));
});


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(configure =>
{
    configure.Cookie.IsEssential = true;
    configure.IdleTimeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddControllersWithViews();
    //.AddJsonOptions(configure =>
    //{
    //    configure.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    //    configure.JsonSerializerOptions.WriteIndented = true;
    //});

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider sp = scope.ServiceProvider;

    await SeedData.Initialize(
        sp,
        app.Environment,
        app.Configuration);
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
