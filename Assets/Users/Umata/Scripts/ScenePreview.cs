using UnityEngine;
using UnityEditor.SceneManagement;

public class ScenePreview : MonoBehaviour
{
    public Camera previewCamera;
    public int previewWidth = 256;
    public int previewHeight = 256;
    public string previewFilename = "Preview.png";

    private void Start()
    {
        // ステージ1-1のシーンをロードする
       EditorSceneManager.OpenScene("Assets/Users/Pak/world1/Scenes/STAGE1-1Scene.unity", OpenSceneMode.Single);

        // カメラが設定されていない場合、デフォルトのメインカメラを使用する
        if (previewCamera == null)
        {
            previewCamera = Camera.main;
        }

        // プレビュー用のRenderTextureを作成する
        RenderTexture renderTexture = new RenderTexture(previewWidth, previewHeight, 16, RenderTextureFormat.ARGB32);

        // カメラにRenderTextureを設定し、シーンをレンダリングする
        previewCamera.targetTexture = renderTexture;
        previewCamera.Render();

        // RenderTextureからTexture2Dを作成する
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;

        // プレビュー画像を保存する
        string savePath = Application.dataPath + "/" + previewFilename;
        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(savePath, bytes);
        Debug.Log("Preview saved at: " + savePath);

        // カメラのRenderTextureをクリアする
        previewCamera.targetTexture = null;

        // ステージ1-1のシーンをアンロードする
    }
}
