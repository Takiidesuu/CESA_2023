using UnityEngine;
using System.Collections;

public class ItemMover : MonoBehaviour
{
    public void MoveItem(GameObject item, float pushtime, Vector3 position)
    {
        StartCoroutine(MoveCoroutine(item, pushtime, position));
    }

    private IEnumerator MoveCoroutine(GameObject item, float pushtime, Vector3 position)
    {
        Vector3 startPos = item.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < pushtime)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / pushtime);
            item.transform.position = Vector3.Lerp(startPos, position, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        item.transform.position = position;
    }
}
