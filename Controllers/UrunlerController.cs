using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DeFactoRetailAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrunlerController : ControllerBase
    {
        private readonly string _connectionString;

        public UrunlerController(IConfiguration configuration)
        {
            // appsettings.json dosyasındaki bağlantı cümlemizi çekiyoruz
            _connectionString = configuration.GetConnectionString("SqlBaglantim");
        }

        // 1. BARKOD SORGULAMA FONKSİYONU
        [HttpGet("Sorgula/{barkod}")]
        public IActionResult UrunSorgula(string barkod)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // T-SQL JOIN sorgumuzla ürünün yerini buluyoruz
                string query = @"
                    SELECT 
                        u.UrunAdi,
                        g.GrupAdi,
                        s.StandKodu,
                        s.BolgeAciklamasi
                    FROM Urunler u
                    INNER JOIN UrunGruplari g ON u.GrupID = g.GrupID
                    INNER JOIN Standlar s ON g.AktifStandID = s.StandID
                    WHERE u.Barkod = @Barkod";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Barkod", barkod);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Ürün bulunduysa bilgileri paketleyip dönüyoruz
                            var sonuc = new
                            {
                                UrunAdi = reader["UrunAdi"].ToString(),
                                GrupAdi = reader["GrupAdi"].ToString(),
                                StandKodu = reader["StandKodu"].ToString(),
                                BolgeAciklamasi = reader["BolgeAciklamasi"].ToString()
                            };
                            return Ok(sonuc);
                        }
                    }
                }
            }

            // Ürün veritabanında yoksa kullanıcıya bilgi veriyoruz
            return NotFound(new { Mesaj = "Ürün veya güncel konumu sistemde bulunamadı!" });
        }

        // 2. ÜRÜN GRUBUNUN STANDINI GÜNCELLEME FONKSİYONU (Görsel Düzenleme İçin)
        [HttpPut("KonumGuncelle")]
        public IActionResult KonumGuncelle(int grupId, int yeniStandId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // İlgili ürün grubunun AktifStandID bilgisini güncelleyen T-SQL sorgusu
                string query = @"
                    UPDATE UrunGruplari 
                    SET AktifStandID = @YeniStandID 
                    WHERE GrupID = @GrupID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@YeniStandID", yeniStandId);
                    command.Parameters.AddWithValue("@GrupID", grupId);

                    connection.Open();
                    int etkilenenSatir = command.ExecuteNonQuery();

                    if (etkilenenSatir > 0)
                    {
                        return Ok(new { Mesaj = "Ürün grubunun mağaza içi konumu başarıyla güncellendi!" });
                    }
                }
            }

            return BadRequest(new { Mesaj = "Güncelleme başarısız! Grup bulunamadı." });
        }
    }
}