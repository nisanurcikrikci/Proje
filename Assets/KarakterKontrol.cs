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

        if (x != 0.0f)
        {
            float yercekimiY = _rb.linearVelocityY;
            _rb.linearVelocity = Vector2.right * x * HizCarpani + new Vector2(0.0f, yercekimiY);
            Yuruyormu = true;
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
        ZeminKontrol();

    }


    void ZiplamaKontrol()
    {
        bool Zipladimi = false;
        bool Dusuyormu = false;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Zipladimi = true;
            Zemindemi = false;
            _rb.AddForce(Vector2.up * SicramaHizi, ForceMode2D.Impulse);
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
                Dusuyormu = false;
            }
        }

        _animator.SetBool("ZipladiMi", Zipladimi);
        _animator.SetBool("DusuyorMu", Dusuyormu);
        _animator.SetBool("ZemindeMi", Zemindemi);

    }
    public bool ZeminKontrol()
    {
        bool Zemindemi = false;
        var origin = _boxCollider.bounds.center;
        var halfsize = _boxCollider.bounds.size * 0.5f;
        var carpisma = Physics2D.Raycast(origin, Vector2.down, halfsize.y + 0.01f, ZeminLayer);
        if (carpisma)
        {
            Debug.DrawLine(origin, origin + new Vector3(0.0f, -halfsize.y - 0.005f, 0.0f), Color.red);
        }
        else
        {
            Debug.DrawLine(origin, origin + new Vector3(0.0f, -halfsize.y - 0.005f, 0.0f), Color.green);
        }
        return Zemindemi;
    }
}
