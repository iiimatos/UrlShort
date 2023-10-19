using Microsoft.EntityFrameworkCore;
using UrlShort;
using UrlShort.Models;
using UrlShort.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<UrlShorteningService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/shorturl", async (UrlDto request, UrlShorteningService urlShorteningService , ApplicationDbContext db, HttpContext ctx) =>
{
    if (!Uri.TryCreate(request.Url, UriKind.Absolute, out var inputUrl))
    {
        return Results.BadRequest("The specified URL is invalid.");
    }

    var code = await urlShorteningService.GenerateUniqueCode();

    var sUrl = new UrlManagement()
    {
        Url = request.Url,
        ShortUrl = $"{ctx.Request.Scheme}://{ctx.Request.Host}/{code}",
        Code = code,
        CreatedOnUtc = DateTime.Now
    };

    await db.Urls.AddAsync(sUrl);
    await db.SaveChangesAsync();

    return Results.Ok(new UrlShortResponseDto()
    {
        Url = sUrl.ShortUrl
    });
});

app.MapFallback(async (ApplicationDbContext db, HttpContext ctx) =>
{
    var path = ctx.Request.Path.ToUriComponent().Trim('/');
    var urlMatch = await db.Urls.FirstOrDefaultAsync(x =>
        x.Code.Trim() == path.Trim()
    );

    if(urlMatch is null)
        return Results.BadRequest("Invalid request");
    return Results.Redirect(urlMatch.Url);
});

app.Run();