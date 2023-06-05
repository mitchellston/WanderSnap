var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddAuthentication("default").AddCookie("default", config =>
{
    config.Cookie.Name = "tokenCookie";
    config.LoginPath = "/Login";
    config.AccessDeniedPath = "/Login";
    config.ExpireTimeSpan = TimeSpan.FromHours(1);
    config.SlidingExpiration = true;
    config.Cookie.HttpOnly = true;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

