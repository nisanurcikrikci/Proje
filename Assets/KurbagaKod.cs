using Unity.VisualScripting;
using UnityEngine;

public class KurbagaKod : MonoBehaviour
{
    [SerializeField] float BeklemeSuresi = 2.0f;
    [SerializeField] float SicramaHizi = 10.0f;
    [SerializeField] LayerMask ZeminLayer;
    [SerializeField] int BirTurdakiSicramaSayisi = 2;
    float _gecenBeklemeSuresi = 0.0f;
    float _aktifTurSayisi;
    bool Zemindemi = false;
    Vector2 _yatayHareketYonu;
    Animator _animator;
    BoxCollider2D _boxCollider;
    Rigidbody2D _rb;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _yatayHareketYonu = -transform.right.normalized;
    }

    void Update()
    {
        ZiplamaKontrol();
    }

    void ZiplamaKontrol()
    {
        bool Zipliyormu = _animator.GetBool("ZipliyorMu");
        bool Dusuyormu = _animator.GetBool("DusuyorMu");
        Zemindemi = ZemindemiKontrol();
        if (Zemindemi)
        {
            if (Dusuyormu == true)
            {
                Zipliyormu = false;
                Dusuyormu = false;
                _aktifTurSayisi++;
                if (_aktifTurSayisi == BirTurdakiSicramaSayisi)
                {
                    _yatayHareketYonu = -_yatayHareketYonu;
                    transform.Rotate(0.0f, 180f, 0.0f);
                    _aktifTurSayisi = 0;
                }
            }
            if (_gecenBeklemeSuresi >= BeklemeSuresi)
            {
                _rb.AddForce((Vector2.up + _yatayHareketYonu) * SicramaHizi, ForceMode2D.Impulse);
                Zipliyormu = true;
                _gecenBeklemeSuresi = 0.0f;
            }
            _gecenBeklemeSuresi += Time.deltaTime;
        }
        else
        {
            if (_rb.linearVelocityY < 0)
            {
                Zipliyormu = false;
                Dusuyormu = true;

            }

        }
        _animator.SetBool("ZipliyorMu", Zipliyormu);
        _animator.SetBool("DusuyorMu", Dusuyormu);
        _animator.SetBool("ZemindeMi", Zemindemi);
    }

    public bool ZemindemiKontrol()
    {
        bool zemindemi = false;
        var origin = _boxCollider.bounds.center;
        var halfsize = _boxCollider.bounds.size * 0.5f;
        var size = _boxCollider.bounds.size;


        var carpismalar = Physics2D.BoxCastAll(origin, size, 0.0f, Vector2.down, halfsize.y + 0.05f);
        if (carpismalar.Length > 0)
        {
            foreach (var siradakiCarpisma in carpismalar)
            {

                if (siradakiCarpisma.collider.name == "Zemin")
                {
                    zemindemi = true;

                }
                if (siradakiCarpisma.collider.name == "Player")
                {
                    Destroy(siradakiCarpisma.collider.gameObject);
                }

            }
        }


        return zemindemi;
    }
}
