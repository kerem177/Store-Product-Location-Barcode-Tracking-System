using Swashbuckle.AspNetCore.SwaggerGen;
var builder = WebApplication.CreateBuilder(args);

// Denetleyicileri (Controllers) ve Swagger servislerini ekliyoruz
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // .NET 10 için klasik Swagger üreteci

var app = builder.Build();

// Geliştirme ortamında Swagger'ı zorunlu olarak açıyoruz
if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Swagger'ın arayüzü kendi oluşturduğu json dosyasına bağlıyoruz
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DeFacto Retail API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();