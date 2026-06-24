using Microsoft.EntityFrameworkCore;
using NotesApp.Data;
using NotesApp.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Добавляем контроллеры с представлениями
builder.Services.AddControllersWithViews();

// Настройка PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Регистрация репозитория
builder.Services.AddScoped<INoteRepository, EFNoteRepository>();

var app = builder.Build();

// Настройка конвейера запросов
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Note}/{action=All}/{id?}");

// Автоматическое применение миграций при запуске (опционально)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.Run();