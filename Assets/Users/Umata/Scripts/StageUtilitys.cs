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
                // ƒV[ƒ“–¼‚©‚ç world ‚Æ stage ‚ğæ“¾‚Å‚«‚½ê‡‚ÍA•Ï”‚ÉŠi”[‚·‚é
                world--;
                stage--;
            }
            else
            {
                Debug.LogWarning("Invalid scene name format: " + currentSceneName);
            }
        }
        else
        {
            Debug.LogWarning("This function can only be used in stage scenes");
        }
    }
}
