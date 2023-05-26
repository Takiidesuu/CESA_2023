using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricRoads : MonoBehaviour
{
    public List<ElectricBallMove> electricBall = new List<ElectricBallMove>();
    public float AddSpeed = 1.0f;

    public void SpeedUp()
    {
        foreach(ElectricBallMove electricBall in electricBall)
        {
            if (electricBall.GetSpeed() > 0)
                electricBall.ChangeSpeed(AddSpeed);
            else
                electricBall.ChangeSpeed(-AddSpeed);
        }
    }

    public void SetElectricBall(ElectricBallMove electricball)
    {
        electricBall.Add(electricball);
    }
}
