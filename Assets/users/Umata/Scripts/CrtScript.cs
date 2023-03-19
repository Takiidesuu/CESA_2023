using UnityEngine;

[ExecuteAlways]
public class CrtScript : MonoBehaviour
{
    // ポストエフェクト用のシェーダを付けたマテリアルを入れる
    [SerializeField] private Material m_Material;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, m_Material);
    }
}