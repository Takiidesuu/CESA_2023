using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectManager : MonoBehaviour
{
    public GameObject[] HorogramMask;
    public GameObject[] LightBulb;
    public float MoveYPos;
    public float time;
    public bool IsWorldSelect = false;

    private float intensityReferenceValue;
    private float[] initialYPos;
    private Vector3[] targetPositions;
    private bool[] isMoving;
    private bool isFading = false;

    private void Start()
    {
        intensityReferenceValue = LightBulb[0].GetComponent<Light>().intensity;

        initialYPos = new float[HorogramMask.Length];
        targetPositions = new Vector3[HorogramMask.Length];
        isMoving = new bool[HorogramMask.Length];
        for (int i = 0; i < HorogramMask.Length; i++)
        {
            initialYPos[i] = HorogramMask[i].transform.localPosition.y;
            targetPositions[i] = new Vector3(HorogramMask[i].transform.localPosition.x, MoveYPos, HorogramMask[i].transform.localPosition.z);
            isMoving[i] = false;
        }
    }

    private void Update()
    {
        if(!IsWorldSelect)
        {
            for (int i = 0; i < HorogramMask.Length; i++)
            {
                if (isMoving[i])
                {
                    StartCoroutine(MoveHorogramMask(i, new Vector3(HorogramMask[i].transform.localPosition.x, initialYPos[i], HorogramMask[i].transform.localPosition.z)));
                }
            }
        }
        if (IsWorldSelect && !isFading)
        {
            for (int i = 0; i < HorogramMask.Length; i++)
            {
                if (!isMoving[i])
                {
                    StartCoroutine(MoveHorogramMask(i, targetPositions[i]));
                }
            }
            StartCoroutine(FadeOutLightIntensity());

        }
       else if (!IsWorldSelect && !isFading)
        {
            StartCoroutine(FadeInLightIntensity());
        }

    }

    private System.Collections.IEnumerator MoveHorogramMask(int index, Vector3 targetPosition)
    {
        isMoving[index] = true;
        float elapsedTime = 0f;
        Vector3 initialPosition = HorogramMask[index].transform.localPosition;
        while (elapsedTime < time)
        {
            HorogramMask[index].transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        HorogramMask[index].transform.localPosition = targetPosition;
        isMoving[index] = false;
    }
    IEnumerator FadeOutLightIntensity()
    {
        isFading = true;
        float elapsedTime = 0.0f;
        float startIntensity = LightBulb[0].GetComponent<Light>().intensity;

        while (elapsedTime < (time / 3))
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / (time / 3));
            foreach (GameObject light in LightBulb)
            {
                light.GetComponent<Light>().intensity = Mathf.Lerp(startIntensity, 0.0f, t);
            }
            yield return null;
        }

        isFading = false;
    }

    IEnumerator FadeInLightIntensity()
    {
        isFading = true;
        float elapsedTime = 0.0f;
        float startIntensity = LightBulb[0].GetComponent<Light>().intensity;

        while (elapsedTime < (time / 3))
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / (time/3));
            foreach (GameObject light in LightBulb)
            {
                light.GetComponent<Light>().intensity = Mathf.Lerp(startIntensity, intensityReferenceValue, t);
            }
            yield return null;
        }

        isFading = false;
    }
}
