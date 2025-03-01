using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieStoreMvc.Models.Domain;
using MovieStoreMvc.Repositories.Abstract;
using MovieStoreMvc.Repositories.Implementation;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// �������� �� �������������
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IMovieService, MovieServices>();

// ������ �� �������� ��� ������ �� ���������� �� �������
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("DATABASE_URL environment variable is not set.");
}

// ������������ �� PostgreSQL URI ��� �������� ������ �� Npgsql
connectionString = connectionString.Replace("postgres://", "Host=")
                                   .Replace("@", ";Username=")
                                   .Replace(":", ";Password=")
                                   .Replace("/", ";Database=");

// �������� �� ��������� �� Entity Framework
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(connectionString));

// ������������� �� Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<DatabaseContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// �������� �� �������� �� ����������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // �������� �� ���� (Owner, Admin, User)
    string[] roles = { "Owner", "Admin", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // ��������� �� �������� ���������� (Owner)
    string ownerEmail = "owner@example.com";
    string ownerPassword = "Owner@123";
    var ownerUser = await userManager.FindByEmailAsync(ownerEmail);
    if (ownerUser == null)
    {
        var newOwner = new ApplicationUser
        {
            UserName = "ownerUsername",
            Email = ownerEmail,
            Name = "Default Name"
        };

        var createOwnerResult = await userManager.CreateAsync(newOwner, ownerPassword);
        if (createOwnerResult.Succeeded)
        {
            await userManager.AddToRoleAsync(newOwner, "Owner");
        }
    }
}

app.Run();
