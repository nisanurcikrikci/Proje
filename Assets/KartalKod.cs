using UnityEngine;

public class KartalKod : MonoBehaviour
{
    // Unity editöründen Karakter objesini (Tilki) buraya sürükle
    [SerializeField] GameObject KarakterObjesi;
    KarakterKontrol karakterKodu;

    [Header("Menzil ve Hız Ayarları")]
    [SerializeField] float devriyeHizi = 2.0f;
    [SerializeField] float takipHizi = 5.0f;
    [SerializeField] float algilamaMenzili = 3.0f;
    [SerializeField] float birakmaMenzili = 6.0f;
    [SerializeField] float devriyeMesafesi = 3.0f;

    [Header("Vurma Ayarları")]
    [SerializeField] float puskurmeMesafesi = 1.5f;
    [SerializeField] float beklemeSuresi = 0.4f;

    Vector3 baslangicKonumu;
    Vector3 devriyeHedefi;
    bool kovaliyorMu = false;
    bool sagaMiGidiyor = true;

    bool darbeAldiMi = false;
    float beklemeZamanlayicisi = 0.0f;

    SpriteRenderer _renderer;
    Animator _animator;

    // Doğrudan BoxCollider2D olarak tanımladık
    BoxCollider2D _kartalBoxCollider;

    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        // Kartalın üzerindeki BoxCollider2D bileşenini alıyoruz
        _kartalBoxCollider = GetComponent<BoxCollider2D>();

        baslangicKonumu = transform.position;
        devriyeHedefi = baslangicKonumu + Vector3.right * devriyeMesafesi;

        if (KarakterObjesi != null)
        {
            karakterKodu = KarakterObjesi.GetComponent<KarakterKontrol>();
        }
    }

    void Update()
    {
        if (KarakterObjesi == null) return;

        // Vurma sonrası sersemleme kontrolü
        if (darbeAldiMi)
        {
            beklemeZamanlayicisi -= Time.deltaTime;
            if (beklemeZamanlayicisi <= 0)
            {
                darbeAldiMi = false;
            }
            return;
        }

        // === HATAYI ÇÖZEN KISIM ===
        // Sadece X eksenine bakıyorduk, şimdi kartalın gagası ile tilki arasındaki GERÇEK 2D mesafeyi ölçüyoruz.
        // Böylece kartal havada yüksekteyken tilkinin üstünden geçse bile erkenden vurma moduna girmeyecek.
        float gercekMesafe = Vector3.Distance(transform.position, KarakterObjesi.transform.position);

        // Algılama (Kovalama başlangıcı için X eksenindeki mesafe kontrolü kalabilir, kartal uzaktan tilkiyi fark etsin diye)
        float mesafeX = Mathf.Abs(transform.position.x - KarakterObjesi.transform.position.x);
        if (kovaliyorMu == false && mesafeX <= algilamaMenzili && karakterKodu.GizlendiMi == false)
        {
            kovaliyorMu = true;
            _animator.SetBool("saldiri", true);
        }

        // Bırakma
        if (kovaliyorMu == true && (mesafeX > birakmaMenzili || karakterKodu.GizlendiMi == true))
        {
            kovaliyorMu = false;
            _animator.SetBool("saldiri", false);

            if (transform.position.x > baslangicKonumu.x)
                devriyeHedefi = baslangicKonumu + Vector3.left * devriyeMesafesi;
            else
                devriyeHedefi = baslangicKonumu + Vector3.right * devriyeMesafesi;
        }

        // Takip ve Uçuş
        if (kovaliyorMu == true)
        {
            // Kartalın kendi merkezi ile BoxCollider'ının (yani gagasının) merkezi arasındaki fark
            Vector3 gagaOfset = _kartalBoxCollider.bounds.center - transform.position;

            // Tilkinin collider'ının en üst tavan noktası
            float karakterTepesiY = KarakterObjesi.GetComponent<Collider2D>().bounds.max.y;
            Vector3 hedefTepesi = new Vector3(KarakterObjesi.transform.position.x, karakterTepesiY, 0f);

            // Kartalı, gaga BoxCollider'ı tilkinin kafasına tam denk gelecek şekilde hizalama
            Vector3 gagaHedefi = hedefTepesi - gagaOfset;

            transform.position = Vector3.MoveTowards(transform.position, gagaHedefi, takipHizi * Time.deltaTime);

            _renderer.flipX = (KarakterObjesi.transform.position.x > transform.position.x);

            // === HAVADA ERKEN TETİKLENMEYİ ENGELLEYEN FİZİKSEL TEMAS KONTROLÜ ===
            // Eğer kartalın gaga collider'ı, tilkinin collider'ına gerçekten değdiyse (mesafe çok azaldıysa)
            if (gercekMesafe <= 0.8f && darbeAldiMi == false)
            {
                HasarVurmaModunaGec();
            }
        }
        else
        {
            // Devriye Gezme
            transform.position = Vector3.MoveTowards(transform.position, devriyeHedefi, devriyeHizi * Time.deltaTime);

            if (Vector3.Distance(transform.position, devriyeHedefi) < 0.1f)
            {
                sagaMiGidiyor = !sagaMiGidiyor;
                if (sagaMiGidiyor == true)
                    devriyeHedefi = baslangicKonumu + Vector3.right * devriyeMesafesi;
                else
                    devriyeHedefi = baslangicKonumu + Vector3.left * devriyeMesafesi;
            }

            _renderer.flipX = (devriyeHedefi.x > transform.position.x);
        }
    }

    // Gerçek hasar vurma anı fonksiyonu
    void HasarVurmaModunaGec()
    {
        darbeAldiMi = true;
        kovaliyorMu = false;
        beklemeZamanlayicisi = beklemeSuresi;

        // Geldiği yönün tam tersine sekme hesabı
        Vector3 kacisYonu = (transform.position - KarakterObjesi.transform.position).normalized;
        kacisYonu.z = 0;

        // Anında puskurmeMesafesi kadar geri fırlat (İç içe girmeyi keser)
        transform.position += kacisYonu * puskurmeMesafesi;

        // Animasyonlar
        _animator.SetTrigger("vurma");
        _animator.SetBool("saldiri", false);
    }

    // Güvenlik amacıyla tetikleyiciyi de açık tutuyoruz (Eğer tamamen dibine girerse diye)
    void OnTriggerEnter2D(Collider2D temasEdenNesne)
    {
        if (temasEdenNesne.gameObject == KarakterObjesi && darbeAldiMi == false)
        {
            HasarVurmaModunaGec();
        }
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