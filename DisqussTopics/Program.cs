using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DisqussTopics.Data;
using DisqussTopics.Models;
using DisqussTopics.Repository;
using DisqussTopics.Service;
using DisqussTopics.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Database context
builder.Services.AddDbContext<DTContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Set up Indentity service
builder.Services.AddDefaultIdentity<DTUser>(options => 
options.SignIn.RequireConfirmedAccount = false)
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DTContext>();

// Add Services to the container
builder.Services.AddControllersWithViews();

// Repository
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ITopicRepository, TopicRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRuleRepository, RuleRepository>();

// Add sevice for Cloudinary
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IVideoService, VideoSevice>();
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection(nameof(CloudinarySettings))
    );

// service for sessions
builder.Services.AddSession();

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

app.UseSession();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute("postDetailRoute", "{controller=Post}/{action=Detail}/{Topic}/{Slug}/{Id}");

app.MapControllerRoute("editRuleRoute", "{controller=Topic}/{action=EditRule}/{slug}/{id}");

app.MapControllerRoute("topicDetailRoute", "{controller=Topic}/{action=Detail}/{Slug}");


app.MapDefaultControllerRoute();

app.MapRazorPages();

app.Run();
