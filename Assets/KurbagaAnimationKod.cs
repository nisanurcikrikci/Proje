using UnityEngine;

public class KurbagaAnimationKod : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void ZiplamaBitti()
    {
        transform.parent.Rotate(0.0f, -180.0f, 0.0f);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
