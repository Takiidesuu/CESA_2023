using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion_Object : MonoBehaviour
{

    public GameObject originalObject;
    public GameObject fracturedObject;

    public float epxlisionMinForce = 50;
    public float epxlisionMaxForce = 100;
    public float epxlisionForceRadius = 100;
    public float fragScaleFactor = 1;

    bool IsEpxlision = false;

    private GameObject fractObj;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Explode();
        if (Input.GetKeyDown(KeyCode.R))
            Reset();
        if (fractObj != null && IsEpxlision)
            ResrtUpdeta();
    }

    public void Explode()
    {
        if (originalObject != null)
        {
             originalObject.SetActive(false);

            if (fracturedObject != null)
            {
                fractObj = Instantiate(fracturedObject);
                fractObj.transform.position = transform.GetChild(0).transform.position;

                foreach (Transform t in fractObj.transform)
                {
                    var rb = t.GetComponent<Rigidbody>();
                    var col = t.GetComponent<MeshCollider>();

                    if (rb != null)
                        rb.AddExplosionForce(Random.Range(epxlisionMinForce, epxlisionMaxForce), fractObj.transform.position, epxlisionForceRadius);

                    StartCoroutine(Shrink(rb, col, 0.1f, originalObject.transform));
                }
            }
        }
    }

    void ResrtUpdeta()
    {
        foreach (Transform t in fractObj.transform)
        {
            var rb = t.GetComponent<Rigidbody>();

            rb.AddForce(originalObject.transform.position - rb.position, ForceMode.Force);

            if (t.localScale.x >= 0)
            {
                Vector3 newScale = t.localScale;

                newScale -= new Vector3(fragScaleFactor, fragScaleFactor, fragScaleFactor);

                t.localScale = newScale;
            }
            else
            {
                Destroy(fractObj);
            }
        }
    }

    void Reset()
    {
        Destroy(fractObj);
        IsEpxlision = false;
        originalObject.SetActive(true);
    }

    IEnumerator Shrink(Rigidbody rb, MeshCollider col, float delay, Transform orign)
    {
        yield return new WaitForSeconds(delay);

        col.enabled = false;
        IsEpxlision = true;
    }
}