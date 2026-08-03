using System.Net.Http.Json;

namespace DeFactoSmartShop
{
    public partial class MainPage : ContentPage
    {
        // Bilgisayardaki API projemizin adresi (Android emülatör IP'si ile)
        // NOT: API projenizin port numarası 7159'dan farklı ise aşağıdaki numarayı değiştirin!

private readonly string _apiBaseUrl = "https://localhost:7159/api/Urunler/Sorgula/";
        public MainPage()
        {
            InitializeComponent();
        }

        private async void BtnSorgula_Clicked(object sender, EventArgs e)
        {
            string barkod = TxtBarkod.Text?.Trim();

            if (string.IsNullOrEmpty(barkod))
            {
                await DisplayAlert("Hata", "Lütfen geçerli bir barkod giriniz!", "Tamam");
                return;
            }

            // Butona basıldığında yükleniyor efekti vermek için yazıyı değiştirelim
            BtnSorgula.Text = "Aranıyor...";
            BtnSorgula.IsEnabled = false;

            try
            {
                // Güvenlik sertifikalarını yerel test ortamında yok saymak için HttpClient oluşturuyoruz
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                using (HttpClient client = new HttpClient(handler))
                {
                    // API'ye istek atıyoruz
                    var response = await client.GetAsync(_apiBaseUrl + barkod);

                    if (response.IsSuccessStatusCode)
                    {
                        // Gelen başarılı sonucu C# nesnesine çeviriyoruz
                        var urunBilgisi = await response.Content.ReadFromJsonAsync<UrunSonucModel>();

                        // Ekrandaki labelları dolduruyoruz
                        LblUrunAdi.Text = urunBilgisi.UrunAdi;
                        LblGrupAdi.Text = "Koleksiyon: " + urunBilgisi.GrupAdi;
                        LblStandKodu.Text = urunBilgisi.StandKodu;
                        LblAciklama.Text = urunBilgisi.BolgeAciklamasi;

                        // Sonuç panelini görünür yapıyoruz
                        ResultFrame.IsVisible = true;
                    }
                    else
                    {
                        ResultFrame.IsVisible = false;
                        await DisplayAlert("Bulunamadı", "Bu barkoda ait güncel konum bilgisi mağaza sisteminde yok!", "Tamam");
                    }
                }
            }
            catch (Exception ex)
            {
                ResultFrame.IsVisible = false;
                await DisplayAlert("Bağlantı Hatası", "API Sunucusuna bağlanılamadı! Lütfen API projesinin açık olduğundan emin olun.", "Tamam");
            }
            finally
            {
                // Butonu eski haline getiriyoruz
                BtnSorgula.Text = "ÜRÜNÜN YERİNİ BUL";
                BtnSorgula.IsEnabled = true;
            }
        }
    }

    // API'den gelen veriyi karşılayacak olan model sınıfı
    public class UrunSonucModel
    {
        public string UrunAdi { get; set; }
        public string GrupAdi { get; set; }
        public string StandKodu { get; set; }
        public string BolgeAciklamasi { get; set; }
    }
}