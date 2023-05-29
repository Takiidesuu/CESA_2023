using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmMove : MonoBehaviour
{
    public Transform[] checkpoints;  // ArmCheckPointの文字列を含むオブジェクトのTransform配列
    public Transform[] endpoints;    // ArmEndPointの文字列を含むオブジェクトのTransform配列

    public float movementSpeed = 5f; // 移動速度

    private List<Transform> waypoints = new List<Transform>(); // 経由するチェックポイントとエンドポイントのリスト
    private Transform currentTarget;  // 現在の目標位置
    private int currentIndex = 0;     // 現在のウェイポイントのインデックス
    private bool isMoving = false;    // 移動中かどうかのフラグ

    private void Start()
    {
        // シーン内からArmCheckPointの文字列を含むオブジェクトを探してcheckpointsに格納する
        GameObject[] checkpointObjects = GameObject.FindGameObjectsWithTag("ArmCheckPoint");
        checkpoints = new Transform[checkpointObjects.Length];
        for (int i = 0; i < checkpointObjects.Length; i++)
        {
            checkpoints[i] = checkpointObjects[i].transform;
        }

        // シーン内からArmEndPointの文字列を含むオブジェクトを探してendpointsに格納する
        GameObject[] endpointObjects = GameObject.FindGameObjectsWithTag("ArmEndPoint");
        endpoints = new Transform[endpointObjects.Length];
        for (int i = 0; i < endpointObjects.Length; i++)
        {
            endpoints[i] = endpointObjects[i].transform;
        }

        // ウェイポイントを設定する
        SetWaypoints();

        // 移動を開始する
        StartMoving();
    }

    private void Update()
    {
        // 移動中でなければ何もしない
        if (!isMoving)
            return;

        // 目標位置に向かって移動する
        Vector3 direction = (currentTarget.position - transform.position).normalized;
        transform.Translate(direction * movementSpeed * Time.deltaTime);

        // 目標位置に十分に近づいたら次の目標位置を設定する
        float distanceThreshold = 0.1f;
        if (Vector3.Distance(transform.position, currentTarget.position) < distanceThreshold)
        {
            currentIndex++;
            // ウェイポイントの最後に到達した場合、2秒停止して新しいウェイポイントを設定する
            if (currentIndex >= waypoints.Count)
            {
                isMoving = false;
                StartCoroutine(StopForSeconds(2f));
                SetWaypoints();
            }
            else
            {
                // 次の目標位置を設定する
                currentTarget = waypoints[currentIndex];
            }
        }
    }

    private void SetWaypoints()
    {
        waypoints.Clear();
        waypoints.AddRange(checkpoints);
        waypoints.AddRange(endpoints);

        // リストをランダムに並び替える
        ShuffleList(waypoints);

        currentIndex = 0;
        currentTarget = waypoints[currentIndex];
    }

    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private void StartMoving()
    {
        isMoving = true;
    }

    private IEnumerator StopForSeconds(float seconds)
    {
        isMoving = false;
        yield return new WaitForSeconds(seconds);
        StartMoving();
    }
}
