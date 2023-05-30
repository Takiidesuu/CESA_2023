using System.Collections;
using UnityEngine;

public class ElectricBallArm : MonoBehaviour
{
    public GameObject upEffect;
    public float moveTime = 2f;
    public float fallDelay = 4f;

    private Coroutine moveCoroutine;
    private Rigidbody rb;

    private void Start()
    {
        SetUpEffectActive(true);
    }

    public void UpObject(Vector3 endPos, GameObject parentArm)
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(MoveObject(endPos, parentArm));
    }

    public void DownObject()
    {
        rb = gameObject.AddComponent<Rigidbody>();
        StartCoroutine(DestroyAfterDelay());
    }

    private void SetUpEffectActive(bool isActive)
    {
        upEffect.SetActive(isActive);
    }

    private IEnumerator MoveObject(Vector3 endPos, GameObject parentArm)
    {
        float timer = 0f;
        Vector3 startPos = transform.position;

        while (timer <= moveTime)
        {
            timer += Time.deltaTime;
            float t = timer / moveTime;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        transform.position = endPos;
        transform.parent = parentArm.transform;
        SetUpEffectActive(false);
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(fallDelay);
        Destroy(rb);
        Destroy(gameObject);
    }
}
