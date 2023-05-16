using UnityEngine;
using UnityEngine.SceneManagement;

public class PreviewSS : MonoBehaviour
{
    public Camera previewCamera;
    public int previewWidth = 1920;
    public int previewHeight = 1080;
    public string previewFilename = "Stage.png";

    private void Start()
    {
        // ステージ1-1の
        //EditorSceneManager.OpenScene("Assets/users/Pak/world1/Stage1-1.unity", OpenSceneMode.Single);

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

        // プレビュー画像を保存
        
        string savePath = Application.dataPath + "/" + SceneManager.GetActiveScene().name + ".png";
        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(savePath, bytes);
        Debug.Log("Preview saved at: " + savePath);

        // カメラのRenderTextureをクリアする
        previewCamera.targetTexture = null;

        // ステージ1-1のシーンをアンロードする
        //EditorSceneManager.CloseScene(stageScene, true);
    }
}