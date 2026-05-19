using UnityEngine;

public class KarakterKontrol : MonoBehaviour
{
    [SerializeField] float SicramaHizi = 7.0f;
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
            Debug.Log("dolumu" + onuDolumu);
            Debug.Log("x:" + x);
            if ((x > 0 && onuDolumu != 1) || (x < 0 && onuDolumu != -1))
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
        var size = _boxCollider.bounds.size;

        var carpisma = Physics2D.BoxCast(origin, size, 0.0f, Vector2.right, halfsize.x + 0.1f, ZeminLayer);

        if (carpisma)
        {
            return 1;
        }

        carpisma = Physics2D.BoxCast(origin, size, 0.0f, Vector2.left, halfsize.x + 0.1f, ZeminLayer);

        if (carpisma)
        {
            return -1;
        }

        return 0;
    }
    void ZiplamaKontrol()
    {
        bool Zipladimi = false;
        bool Dusuyormu = false;
        Zemindemi = ZemindemiKontrol();

        if (Input.GetKeyDown(KeyCode.Space) && Zemindemi)
        {
            Zipladimi = true;
            Zemindemi = false;

            _rb.AddForce(Vector2.up * SicramaHizi, ForceMode2D.Impulse);
        }

        Zemindemi = ZemindemiKontrol();
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
    void OnDrawGizmos()
    {
        var origin = _boxCollider.bounds.center;
        var halfsize = _boxCollider.bounds.size * 0.5f;
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
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(origin + new Vector3(0.0f, -halfsize.y * 0.5f, 0.0f),
                             new Vector3(halfsize.x * 2, halfsize.y, 0.02f));
        }
        if (OnuDolumu() == 1)
        {

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(origin + new Vector3(halfsize.x * 0.5f, 0.0f, 0.0f),
                            new Vector3(halfsize.x, halfsize.y * 2, 0.02f));

        }
        else if (OnuDolumu() == -1)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(origin + new Vector3(-halfsize.x * 0.5f, 0.0f, 0.0f),
                            new Vector3(halfsize.x, halfsize.y * 2, 0.02f));
        }


    }
    public bool ZemindemiKontrol()
    {
        Zemindemi = false;
        var origin = _boxCollider.bounds.center;
        var halfsize = _boxCollider.bounds.size * 0.5f;
        var size = _boxCollider.bounds.size;
        //var carpisma = Physics2D.Raycast(origin, Vector2.down, halfsize.y + 0.05f, ZeminLayer);
        var carpisma = Physics2D.BoxCast(origin, size, 0.0f, Vector2.down, halfsize.y + 0.05f, ZeminLayer);
        if (carpisma)
        {
            Zemindemi = true;
        }

        return Zemindemi;
    }
}
