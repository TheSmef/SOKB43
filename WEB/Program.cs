using BlazorDownloadFile;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Models.Mapper;
using MudBlazor.Services;
using Radzen;
using System.Globalization;
using WEB.Data.Services;
using WEB.Data.Services.Base;
using WEB.Data.UtilityServices;
using WEB.Data.UtilityServices.Base;
using WEB.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazorDownloadFile();
builder.Services.AddMudServices();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthenticationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthProvider>();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration.GetSection("AppSettings:API").Value!) });

builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthInterceptor, AuthInterceptor>();
builder.Services.AddScoped<IEquipmentTypeService, EquipmentTypeService>();
builder.Services.AddScoped<IUserPostService, UserPostService>();
builder.Services.AddScoped<IContractorService, ContractorService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IServicesService, ServicesService>();
builder.Services.AddScoped<ITechnicalTestsService, TechnicalTestsService>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<ITechnicalTaskService, TechnicalTaskService>();
builder.Services.AddScoped<IBackUpService, BackUpService>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthProvider>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.Use(async (context, next) =>
{
    CultureInfo? culture = CultureInfo.CurrentCulture.Clone() as CultureInfo;
    if (culture != null)
    {
        culture.NumberFormat.NumberDecimalSeparator = ".";
        culture.NumberFormat.NumberGroupSeparator = ".";
        culture.NumberFormat.CurrencyDecimalSeparator = ".";
        culture.NumberFormat.PercentDecimalSeparator = ".";
        culture.NumberFormat.PercentGroupSeparator = ".";
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }

    await next();
});


app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
