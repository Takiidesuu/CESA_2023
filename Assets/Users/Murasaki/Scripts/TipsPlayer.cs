using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipsPlayer : MonoBehaviour
{
    [SerializeField] GameObject tips;
    public Vector3 targetScale = Vector3.one; // 表示するスケール
    private Vector3 initialScale; // 初期のスケール

    public string playerTag = "Player"; // プレイヤーのタグ
    public float duration = 1f; // 表示/非表示にかかる時間（秒）

    private bool isVisible = false; // 表示フラグ
    private float timer = 0f; // タイマー

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isVisible = true;
            timer = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isVisible = false;
            timer = 0f;
        }
    }

    private void Update()
    {
        if (isVisible)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            tips.transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
        }
        else
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            tips.transform.localScale = Vector3.Lerp(targetScale, initialScale, t);
        }
    }



    private void Start()
    {
        initialScale = tips.transform.localScale; // ゲームオブジェクトの初期のスケールを保存
        timer = 0f; // タイマーをリセット
    }
}