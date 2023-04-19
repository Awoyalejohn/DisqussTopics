using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DisqussTopics.Data;
using DisqussTopics.Models;
using DisqussTopics.Repository;

var builder = WebApplication.CreateBuilder(args);

// Database context
builder.Services.AddDbContext<DTContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Set up Indentity service
builder.Services.AddDefaultIdentity<DTUser>(options => 
options.SignIn.RequireConfirmedAccount = true)
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DTContext>();

// Add Services to the container
builder.Services.AddControllersWithViews();

// Repository
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ITopicRepository, TopicRepository>();

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    await Seed.SeedRolesAsync(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute("slugRoute", "{controller=Home}/{action=Detail}/{Topic}/{Slug}/{Id}");

app.MapDefaultControllerRoute();

app.MapRazorPages();

app.Run();