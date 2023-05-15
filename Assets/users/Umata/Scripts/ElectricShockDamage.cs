using UnityEngine;

public class ElectricShockDamage : MonoBehaviour
{
    public GameObject[] m_models; // GameObject配列を設定可能にする
    public Material m_BaseMat; // 衝撃時に適用するマテリアル0
    public Material m_ShockMat; // 衝撃時に適用するマテリアル1
    public Material m_ShockBoneMat; // 衝撃時に適用するマテリアル2
    public bool is_damage = false; // ダメージフラグ

    // マテリアルを更新する関数
    private void UpdateMaterial()
    {
        foreach (GameObject model in m_models)
        {
            SkinnedMeshRenderer meshRenderer = model.GetComponent<SkinnedMeshRenderer>();
            if (meshRenderer != null)
            {
                if (is_damage)
                {
                    if (model.name != "RightLeg")
                    {
                        meshRenderer.materials = new Material[] { m_ShockMat, m_ShockBoneMat };
                    }
                    else
                    {
                        meshRenderer.materials = new Material[] { m_ShockMat};
                    }
                }
                else
                {
                    meshRenderer.materials = new Material[] { m_BaseMat };

                }
            }
        }
    }

    // マテリアルを除去する関数
    private Material[] RemoveMaterial(Material[] materials, Material material)
    {
        int index = System.Array.IndexOf(materials, material);
        if (index >= 0)
        {
            Material[] newMaterials = new Material[materials.Length - 1];
            for (int i = 0, j = 0; i < materials.Length; i++)
            {
                if (i != index)
                {
                    newMaterials[j++] = materials[i];
                }
            }
            materials = newMaterials;
        }
        return materials;
    }
}
