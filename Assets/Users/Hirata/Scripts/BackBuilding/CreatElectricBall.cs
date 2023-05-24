using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatElectricBall : MonoBehaviour
{
    private ElectricRoads electricRoads;
    private GameObject LastElectricBall;
    public GameObject ElectricBall;
    public float CreatePosition = 1;

    private float time;

    private void Start()
    {
        electricRoads = transform.parent.GetComponent<ElectricRoads>();

        LastElectricBall = Instantiate(ElectricBall, transform.position, Quaternion.identity, transform);
        electricRoads.SetElectricBall(LastElectricBall.GetComponent<ElectricBallMove>());
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(transform.position.x - LastElectricBall.transform.position.x) > CreatePosition) 
        {
            LastElectricBall = Instantiate(ElectricBall, transform.position, Quaternion.identity, transform);
            //LastElectricBall.GetComponent<ElectricBallMove>().ChangeSpeed(LastElectricBall.GetComponent<ElectricBallMove>().GetSpeed());
            electricRoads.SetElectricBall(LastElectricBall.GetComponent<ElectricBallMove>());
        }
    }
}
