using System.Text;

namespace DeFactoSmartShop
{
    public partial class GuncellePage : ContentPage
    {
        // API Güncelleme adresi (Yine localhost ve senin portunla)
        private readonly string _apiUpdateUrl = "https://localhost:7159/api/Urunler/KonumGuncelle";

        public GuncellePage()
        {
            InitializeComponent();
            DatalariDoldur();
        }

        private void DatalariDoldur()
        {
            // Test amaçlı şimdilik listeleri elimizle dolduruyoruz
            // Gerçek ID'leri arkada tutmak için Picker nesnesini dolduruyoruz
            PckUrunGrubu.Items.Add("Erkek Oversize Tişörtler"); // Bunun ID'si veritabanında 1

            PckStand.Items.Add("Erkek Young - Giriş Sol Orta Katlı Masa");  // StandID: 1 (E-YNG-M1)
            PckStand.Items.Add("Erkek Casual - Arka Sağ Duvar Reyonu");     // StandID: 2 (E-CAS-D2)
        }

        private async void BtnGuncelle_Clicked(object sender, EventArgs e)
        {
            if (PckUrunGrubu.SelectedIndex == -1 || PckStand.SelectedIndex == -1)
            {
                await DisplayAlert("Hata", "Lütfen hem ürün grubunu hem de yeni standı seçiniz!", "Tamam");
                return;
            }

            BtnGuncelle.Text = "Güncelleniyor...";
            BtnGuncelle.IsEnabled = false;

            try
            {
                // Seçilen elemanların veritabanındaki karşılık gelen ID'lerini simüle ediyoruz
                int grupId = 1; // Şimdilik tek grubumuz var
                int yeniStandId = PckStand.SelectedIndex == 0 ? 1 : 2; // İlk seçilirse ID 1, ikinci seçilirse ID 2

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                using (HttpClient client = new HttpClient(handler))
                {
                    // PUT isteği için URL parametrelerini hazırlıyoruz
                    string requestUrl = $"{_apiUpdateUrl}?grupId={grupId}&yeniStandId={yeniStandId}";

                    // Boş bir içerikle PUT isteği atıyoruz (Çünkü parametreler URL içinde gidiyor)
                    var response = await client.PutAsync(requestUrl, new StringContent("", Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Başarılı", "Ürün grubunun stand konumu mağaza genelinde başarıyla güncellendi!", "Harika");
                    }
                    else
                    {
                        await DisplayAlert("Hata", "Güncelleme sırasında bir sorun oluştu.", "Tamam");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Bağlantı Hatası", "API Sunucusuna bağlanılamadı!", "Tamam");
            }
            finally
            {
                BtnGuncelle.Text = "STAND KONUMUNU GÜNCELLE";
                BtnGuncelle.IsEnabled = true;
            }
        }
    }
}