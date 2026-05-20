using UnityEngine;

public class KarakterKontrol : MonoBehaviour
{
    [SerializeField] float SicramaHizi = 7.0f;
    [SerializeField] GameObject OlumAnimasyonSablonu;
    Rigidbody2D _rb;
    Animator _animator;
    SpriteRenderer _renderer;
    float HizCarpani = 10.0f;
    bool Zemindemi = false;
    BoxCollider2D _boxCollider;


    [SerializeField] LayerMask ZeminLayer;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();

    }

    void Update()

    {
        bool Yuruyormu = false;

        float x = Input.GetAxis("Horizontal");
        int onuDolumu = OnuDolumu();
        if (x != 0.0f)
        {

            if ((x < 0 && onuDolumu != -1) || (x > 0 && onuDolumu != 1))
            {

                _rb.linearVelocityX = (Vector2.right * x * HizCarpani).x;
                Yuruyormu = true;
            }

            _renderer.flipX = false;
            if (x < 0.0f)
            {
                _renderer.flipX = true;
            }
        }
        else
        {
            Yuruyormu = false;
        }
        _animator.SetBool("YuruyorMu", Yuruyormu);
        ZiplamaKontrol();


    }

    int OnuDolumu()
    {

        var origin = _boxCollider.bounds.center;
        var halfsize = _boxCollider.bounds.size * 0.5f;
        var size = new Vector3(_boxCollider.bounds.size.x * 0.4f, _boxCollider.bounds.size.y * 0.5f, 0.02f);

        var carpisma = Physics2D.BoxCast(origin, size, 0.0f, Vector2.right, halfsize.x + 0.01f, ZeminLayer);
        if (carpisma)
        {
            return 1;
        }

        carpisma = Physics2D.BoxCast(origin, size, 0.0f, Vector2.left, halfsize.x + 0.01f, ZeminLayer);

        if (carpisma)
        {
            return -1;
        }

        return 0;
    }
    void OnDrawGizmos()
    {
        if (_boxCollider == null) return;
        var origin = _boxCollider.bounds.center;
        var halfsize = _boxCollider.bounds.size * 0.5f;
        var gizoSize = new Vector3(_boxCollider.bounds.size.x * 0.4f, _boxCollider.bounds.size.y * 0.5f, 0.02f);
        if (Zemindemi)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(origin + new Vector3(0.0f, -halfsize.y * 0.5f, 0.0f),
                            new Vector3(halfsize.x * 2, halfsize.y, 0.02f));
            //Gizmos.DrawCube(origin + new Vector3(0.0f, -halfsize.y * 0.5f, 0.0f),new Vector3(halfsize.x * 2, halfsize.y, 0.02f));
            //Gizmos.DrawLine(origin,origin + new Vector3(0.0f,-halfsize.y - 0.05f, 0.0f));
            Zemindemi = true;
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(origin + Vector3.right * (halfsize.x + 0.01f), gizoSize);
        }
        if (OnuDolumu() == 1)
        {

            Gizmos.color = Color.red; // Engel varsa kırmızı yanar
            Gizmos.DrawWireCube(origin + Vector3.right * (halfsize.x + 0.05f), _boxCollider.bounds.size * 0.6f);
        }
        else if (OnuDolumu() == -1)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(origin + Vector3.left * (halfsize.x + 0.01f), gizoSize);
        }
    }
    void ZiplamaKontrol()
    {
        bool Zipladimi = false;
        bool Dusuyormu = false;
        Zemindemi = ZemindemiKontrol();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Zemindemi)
            {
                _rb.AddForce(Vector2.up * SicramaHizi, ForceMode2D.Impulse);
                Zipladimi = true;
                Zemindemi = false;
            }



        }


        if (!Zemindemi)
        {

            if (_rb.linearVelocityY < 0)
            {
                Zipladimi = false;
                Dusuyormu = true;

            }
            else
            {
                Zipladimi = true;
                Dusuyormu = false;
            }
        }

        _animator.SetBool("ZipladiMi", Zipladimi);
        _animator.SetBool("DusuyorMu", Dusuyormu);
        _animator.SetBool("ZemindeMi", Zemindemi);

    }




    public bool ZemindemiKontrol()
    {
        bool zemindemi = false;
        var origin = _boxCollider.bounds.center;
        var halfsize = _boxCollider.bounds.size * 0.5f;
        var size = _boxCollider.bounds.size;
        var carpismaSize = halfsize;
        carpismaSize.y *= 0.25f;

        var carpismalar = Physics2D.BoxCastAll(origin + new Vector3(0.0f, -halfsize.y * 0.075f, 0.0f),
                                                new Vector3(halfsize.x * 2 * 0.8f, halfsize.y * 0.5f, 0.02f),
                                                0.0f, Vector2.down, halfsize.y + 0.05f);
        if (carpismalar.Length > 0)
        {
            foreach (var siradakiCarpisma in carpismalar)
            {
                if (siradakiCarpisma.collider.name == "Zemin")
                {
                    zemindemi = true;
                }
                if (siradakiCarpisma.collider.name == "Tepe")
                {
                    Destroy(siradakiCarpisma.collider.transform.parent.gameObject);
                    _rb.AddForce(Vector2.up * SicramaHizi, ForceMode2D.Impulse);
                    _animator.SetBool("ZipladiMi", true);
                    Zemindemi = false;
                    Instantiate(OlumAnimasyonSablonu).transform.position = siradakiCarpisma.collider.transform.position;
                }

            }


        }

        return zemindemi;
    }
}
