using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipGate : MonoBehaviour
{
    private BoxCollider boxCollider;
    public GameObject Barrier;
    private GameObject FlipObj;

    private Vector3 targetpos = new Vector3(0, -2.25f, 0);
    private float StartTime;
    public float FlipSpeed = 1;
    public float StopTime = 1;
    public float FlipTime = 1;
    private  bool IsFlip;
    private bool IsReturnFlip;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsFlip)
        {
            transform.GetChild(0).localPosition = Vector3.Lerp(Vector3.zero, targetpos, (Time.time - StartTime) * FlipSpeed);
            transform.GetChild(1).localPosition = Vector3.Lerp(Vector3.zero, targetpos, (Time.time - StartTime) * FlipSpeed);
            if ((Time.time - StartTime) * FlipSpeed > 1)
            {
                IsFlip = false;
                StartCoroutine(MoveWait(StopTime));
            }
        }
        if (IsReturnFlip)
        {
            transform.GetChild(0).localPosition = Vector3.Lerp(targetpos, Vector3.zero, (Time.time - StartTime) * FlipSpeed);
            transform.GetChild(1).localPosition = Vector3.Lerp(targetpos, Vector3.zero, (Time.time - StartTime) * FlipSpeed);
            if ((Time.time - StartTime) * FlipSpeed > 1)
            {
                IsReturnFlip = false;
                boxCollider.enabled = true;
            }
        }
    }

    public void Flip(GameObject obj)
    {
        FlipObj = obj;
        IsFlip = true;
        StartTime = Time.time;
        boxCollider.enabled = false;
        Barrier.SetActive(true);
        StartCoroutine(PlayerFlip(FlipTime));
    }

    void FlipReturn()
    {
        IsReturnFlip = true;
        StartTime = Time.time;
    }

    IEnumerator MoveWait(float value) {
        Barrier.SetActive(false);
        FlipObj.GetComponent<Rigidbody>().velocity = Vector3.zero;

        yield return new WaitForSeconds(value);

        FlipReturn();
    }

    IEnumerator PlayerFlip(float value)
    {
        yield return new WaitForSeconds(value);
        if (FlipObj.CompareTag("Player"))
            FlipObj.GetComponent<PlayerMove>().FlipCharacter();
        else if (FlipObj.CompareTag("ElectricalBall"))
            FlipObj.GetComponent<ElectricBallMove>().FlipUpsideDown();
    }
}
