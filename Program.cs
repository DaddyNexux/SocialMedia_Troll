using SocialMedia.Extensions;
using SocialMedia.Helpers;
using SocialMedia.Extentions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigProvider.config = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSizeLimit();
builder.Services.AddDbConnection();
builder.Services.AddCorss();
builder.Services.AddIdentityConfig();
builder.Services.AddAuthConfig();
builder.Services.AddSwaggerConfig();
builder.Services.AddServices();
/*builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(7177);
});*/

var app = builder.Build();

app.UseHsts();
//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAll");

//app.UseIdentitySeedRoles(Roles.SuperAdmin, Roles.Admin);
app.UseAuth();
await app.UseSeeder();


app.UseCustomSwaggerWithAuth("Thawani");
app.UseContentSecurityPolicy();


app.Run();