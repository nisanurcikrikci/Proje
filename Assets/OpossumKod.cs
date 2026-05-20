using UnityEngine;

public class OpossumKod : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    [SerializeField] float hareketHizi = 2.0f;
    [SerializeField] float devriyeMesafesi = 3.0f; // Ne kadar uzağa gidip dönecek

    Vector3 baslangicKonumu;
    Vector3 hedefKonumu;
    bool sagaMiGidiyor = true;

    SpriteRenderer _renderer;
    Rigidbody2D _rb;

    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();

        // İlk başladığı yeri kaydet ve sağdaki ilk hedefi belirle
        baslangicKonumu = transform.position;
        hedefKonumu = baslangicKonumu + Vector3.right * devriyeMesafesi;
    }

    void Update()
    {
        // Yerde sadece X ekseninde (sağa-sola) hareket edecek, Y pozisyonunu kendi koruyacak
        Vector3 gidilecekNokta = new Vector3(hedefKonumu.x, transform.position.y, transform.position.z);

        // Hedefe doğru yürütme
        transform.position = Vector3.MoveTowards(transform.position, gidilecekNokta, hareketHizi * Time.deltaTime);

        // Hedefe ulaştı mı kontrolü (X ekseninde çok yaklaştıysa yön değiştir)
        if (Mathf.Abs(transform.position.x - hedefKonumu.x) < 0.05f)
        {
            sagaMiGidiyor = !sagaMiGidiyor;

            if (sagaMiGidiyor == true)
            {
                hedefKonumu = baslangicKonumu + Vector3.right * devriyeMesafesi;
            }
            else
            {
                hedefKonumu = baslangicKonumu + Vector3.left * devriyeMesafesi;
            }
        }

        // Karakterin baktığı yöne göre sprite'ı çevir (Görselin ham yönüne göre true/false ayarla)
        _renderer.flipX = sagaMiGidiyor;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Eğer çarpan nesne Oyuncu (Tilki) ise VE kaplumbağa şu an kabukta DEĞİLSE
        if (collision.gameObject.CompareTag("Player"))
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
}