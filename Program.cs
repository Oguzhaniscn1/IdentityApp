using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();




builder.Services.AddDbContext<IdentityContext>(
    options=>options.UseSqlite(builder.Configuration["ConnectionStrings:Sqlite_Connection"]));

builder.Services.AddIdentity<AppUser,AppRole>().AddEntityFrameworkStores<IdentityContext>();

builder.Services.Configure<IdentityOptions>(options=>{

options.Password.RequiredLength=6;//min 6 karakterli parola
options.Password.RequireNonAlphanumeric=false;//numarik karakter zorunluluğu
options.Password.RequireLowercase=false;//küçük harf zorunluluğu
options.Password.RequireUppercase=false;//büyük harf duyarlılığı
options.Password.RequireDigit=false;//sayıslal değer

options.User.RequireUniqueEmail=false;//aynı email kullanma durumu




    });








var app = builder.Build();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


IdentitySeedData.IdentityTestUser(app);


app.Run();
