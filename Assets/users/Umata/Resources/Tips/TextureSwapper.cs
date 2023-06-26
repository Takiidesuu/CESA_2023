using UnityEngine;

public class TextureSwapper : MonoBehaviour
{
    public Material material1;
    public Material material2;
    public float swapInterval = 1f;

    private MeshRenderer rend;
    private float timer;
    bool isSelectMat = true;
    private void Start()
    {
        rend = GetComponent<MeshRenderer>();
        rend.material = material2;
        timer = swapInterval;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            if(isSelectMat)
            {
                isSelectMat = false;
            }
            else
            {
                isSelectMat = true;
            }
            SwapMaterial();
            timer = swapInterval;
        }
    }

    private void SwapMaterial()
    {
        if (isSelectMat)
        {
            rend.material = material2;
        }
        else
        {
            rend.material = material1;
        } 
        }
}
