using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipGate : MonoBehaviour
{
    private BoxCollider boxCollider;
    public GameObject Barrier;
    private GameObject FlipObj;
    private SoundManager soundManager;

    public Vector3 targetpos = new Vector3(0, -2.25f, 0);   //ˆÚ“®—Ê
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
        soundManager = GetComponent<SoundManager>();
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BackBuilding") && !IsFlip)
        {
            soundManager.PlaySoundEffect("Flip");
            Flip(other.gameObject);
        }
    }

    public void Flip(GameObject obj)
    {
        FlipObj = obj;
        IsFlip = true;
        StartTime = Time.time;
        boxCollider.enabled = false;
        Barrier.SetActive(true);
        if (FlipObj.CompareTag("Player") || FlipObj.CompareTag("ElectricalBall")) 
            StartCoroutine(Flip(FlipTime));
        else if (FlipObj.CompareTag("BackBuilding"))
        {
            FlipObj.transform.parent = this.transform;
            FlipObj.transform.localPosition = Vector3.zero;
        }
    }

    void FlipReturn()
    {
        IsReturnFlip = true;
        StartTime = Time.time;
    }

    IEnumerator MoveWait(float value) {
        Barrier.SetActive(false);
        if (FlipObj != null)
        {
            if (FlipObj.GetComponent<Rigidbody>())
                FlipObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
            if (FlipObj.CompareTag("BackBuilding"))
                FlipObj.transform.parent = null;
        }

        yield return new WaitForSeconds(value);

        FlipReturn();
    }

    IEnumerator Flip(float value)
    {
        yield return new WaitForSeconds(value);
        if (FlipObj.CompareTag("Player"))
            FlipObj.GetComponent<PlayerMove>().FlipCharacter();
        else if (FlipObj.CompareTag("ElectricalBall"))
            FlipObj.GetComponent<ElectricBallMove>().FlipUpsideDown();
    }
}
