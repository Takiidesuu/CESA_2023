using UnityEngine.SceneManagement;
using UnityEngine;

public class StageUtilitys : MonoBehaviour
{
    public static void GetCurrentWorldAndStage(out int world, out int stage)
    {
        world = 0;
        stage = 0;
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName.StartsWith("Stage"))
        {
            string[] splitSceneName = currentSceneName.Split('-');
            if (splitSceneName.Length == 2 && int.TryParse(splitSceneName[0].Substring(5), out world) && int.TryParse(splitSceneName[1], out stage))
            {
                // シーン名から world と stage を取得できた場合は、変数に格納する
                world--;
                stage--;
            }
            else
            {
            }
        }
        else
        {
        }
    }
}
