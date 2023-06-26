using UnityEngine;

public class CellEffect : MonoBehaviour
{
    public GameObject[] Models;
    public Material InvisibleMat;
    public GameObject CellEffectParticle;

    public void ChangeMaterialsToInvisible()
    {
        foreach (GameObject model in Models)
        {
            Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.material = InvisibleMat;
            }
        }
        CellEffectParticle.active = true;
    }
}
