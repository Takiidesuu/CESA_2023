using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBulbChangeMaterial : MonoBehaviour
{
    public Material OnPowerMaterial;
    public Material OffPowerMaterial;
    public bool OnPower;

    private GameObject ElectricEffect;
    private GameObject BulbCircle_0;
    private GameObject BulbCircle_1;
    Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        ElectricEffect = this.transform.Find("ElectlicEffect").gameObject;
        BulbCircle_0 = this.transform.Find("BulbCircle_0").gameObject;
        BulbCircle_1 = this.transform.Find("BulbCircle_1").gameObject;
        renderer = this.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(OnPower)
        {
            ElectricEffect.SetActive(true);
            BulbCircle_0.GetComponent<Renderer>().material = OnPowerMaterial;
            BulbCircle_1.GetComponent<Renderer>().material = OnPowerMaterial;
            BulbCircle_0.GetComponent<BulbCircleRotation>().OnPower = true;
            BulbCircle_1.GetComponent<BulbCircleRotation>().OnPower = true;

        }
        else
        {
            ElectricEffect.SetActive(false);
            BulbCircle_0.GetComponent<Renderer>().material = OffPowerMaterial; 
            BulbCircle_1.GetComponent<Renderer>().material = OffPowerMaterial;
            BulbCircle_0.GetComponent<BulbCircleRotation>().OnPower = false;
            BulbCircle_1.GetComponent<BulbCircleRotation>().OnPower = false;

        }
    }
}
