using UnityEngine;

public class KaplumbagaKod : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    [SerializeField] float hareketHizi = 1.5f;
    [SerializeField] float devriyeMesafesi = 3.0f;

    [Header("Kabuk Ayarları")]
    [SerializeField] float kabuktaKalmaSuresi = 2.0f;

    Vector2 baslangicKonumu;
    Vector2 hedefKonumu;
    bool sagaMiGidiyor = true;

    // Durum Kontrolleri
    bool kabuktaMi = false;
    float kabukZamanlayici = 0.0f;

    SpriteRenderer _renderer;
    Animator _animator;

    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        // Pozisyonları doğrudan Vector2 olarak kaydediyoruz
        baslangicKonumu = transform.position;
        hedefKonumu = baslangicKonumu + Vector2.right * devriyeMesafesi;
    }

    void Update()
    {
        // === KABUK DURUMU KONTROLÜ ===
        if (kabuktaMi)
        {
            kabukZamanlayici -= Time.deltaTime;

            // 2 saniye dolduysa kabuktan çık ve tekrar yürümeye başla
            if (kabukZamanlayici <= 0)
            {
                kabuktaMi = false;
                _animator.SetBool("VurulduMu", false);
            }

            // Kabuktayken yürütme, burada kodu kes
            return;
        }

        // === NORMAL YÜRÜME DURUMU (Sadece Vector2) ===
        Vector2 mevcutPozisyon = transform.position;
        Vector2 gidilecekNokta = new Vector2(hedefKonumu.x, mevcutPozisyon.y);

        // MoveTowards artık Vector2 olarak çalışıyor
        transform.position = Vector2.MoveTowards(mevcutPozisyon, gidilecekNokta, hareketHizi * Time.deltaTime);

        // Hedefe ulaştı mı kontrolü
        if (Mathf.Abs(transform.position.x - hedefKonumu.x) < 0.05f)
        {
            sagaMiGidiyor = !sagaMiGidiyor;
            if (sagaMiGidiyor)
                hedefKonumu = baslangicKonumu + Vector2.right * devriyeMesafesi;
            else
                hedefKonumu = baslangicKonumu + Vector2.left * devriyeMesafesi;
        }

        // Sağa gidiyorsa flipX kapalı, sola gidiyorsa açık
        _renderer.flipX = !sagaMiGidiyor;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Eğer çarpan nesne Oyuncu (Tilki) ise VE kaplumbağa şu an kabukta DEĞİLSE
        if (collision.gameObject.CompareTag("Player") && !kabuktaMi)
        {
            // Oyuncunun üzerindeki KarakterKontrol koduna ulaşıyoruz
            KarakterKontrol oyuncu = collision.gameObject.GetComponent<KarakterKontrol>();

            if (oyuncu != null)
            {
                // Oyuncunun hasar alma fonksiyonunu çağır!
                oyuncu.HasarAl();
            }
        }
    }

    // Tilki tepesine bastığında dışarıdan çağrılacak fonksiyon
    public void KabugaCekil()
    {
        if (kabuktaMi == false)
        {
            kabuktaMi = true;
            kabukZamanlayici = kabuktaKalmaSuresi;
            _animator.SetBool("VurulduMu", true);
        }
    }
}