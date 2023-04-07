using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    [System.Serializable]
    public struct BGMData
    {
        public string sceneName;
        public AudioClip bgm;
        public float volume;
    }

    public BGMData[] bgmDataList;

    private AudioSource bgmAudioSource;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        // シーンが切り替わったらBGMを再生する
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // イベントリスナーを解除する
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // AudioSourceがない場合は生成する
        if (bgmAudioSource == null)
        {
            bgmAudioSource = GetComponent<AudioSource>();
            if (bgmAudioSource == null)
            {
                bgmAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // 現在のシーンに応じてBGMを再生する
        foreach (BGMData bgmData in bgmDataList)
        {
            if (bgmData.sceneName == scene.name)
            {
                // AudioClipをセットする
                bgmAudioSource.clip = bgmData.bgm;

                // ボリュームをセットする
                bgmAudioSource.volume = bgmData.volume;

                // ループ再生する
                bgmAudioSource.loop = true;

                // 再生する
                bgmAudioSource.Play();
                return;
            }
        }

        // 対応するBGMがない場合は停止する
        bgmAudioSource.Stop();
    }
}
