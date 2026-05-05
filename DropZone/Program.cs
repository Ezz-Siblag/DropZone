var builder = WebApplication.CreateBuilder(args);

// =========================
// SERVICES
// =========================
builder.Services.AddRazorPages();
builder.Services.AddSession();

var app = builder.Build();

// =========================
// PIPELINE
// =========================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// MUST be before auth middleware
app.UseSession();

// =========================
// LOGIN PROTECTION MIDDLEWARE
// =========================
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";

    // Allow public pages
    if (path.StartsWith("/Login") ||
        path.StartsWith("/SignUp") ||   // ✅ FIX ADDED
        path.StartsWith("/css") ||
        path.StartsWith("/js") ||
        path.StartsWith("/lib") ||
        path.StartsWith("/Error"))
    {
        await next();
        return;
    }

    var user = context.Session.GetString("User");

    if (string.IsNullOrEmpty(user))
    {
        context.Response.Redirect("/Login");
        return;
    }

    await next();
});

app.UseAuthorization();

app.MapRazorPages();

app.Run();