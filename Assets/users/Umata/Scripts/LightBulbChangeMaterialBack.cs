using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBulbChangeMaterialBack : MonoBehaviour
{
    public Material OnPowerMaterial;
    public Material OffPowerMaterial;

    public Material OnPowerGlassMaterial;
    public Material OffPowerGlassMaterial;
    public bool OnPower;

    private GameObject ElectricEffect;
    private GameObject VoidElectricEffect;
    private GameObject BulbCircle_0;
    private GameObject BulbCircle_1;

    public float LifeTime = 10f;
    Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        VoidElectricEffect = this.transform.Find("VoidElectricBall").gameObject;
        ElectricEffect = this.transform.Find("ElectlicEffect").gameObject;
        BulbCircle_0 = this.transform.Find("BulbCircle_0").gameObject;
        BulbCircle_1 = this.transform.Find("BulbCircle_1").gameObject;
        renderer = this.GetComponent<Renderer>();

        Destroy(this.gameObject, LifeTime);
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
            this.gameObject.GetComponent<Renderer>().material = OnPowerGlassMaterial;

        }
        else
        {
            ElectricEffect.SetActive(false);
            BulbCircle_0.GetComponent<Renderer>().material = OffPowerMaterial; 
            BulbCircle_1.GetComponent<Renderer>().material = OffPowerMaterial;
            BulbCircle_0.GetComponent<BulbCircleRotation>().OnPower = false;
            BulbCircle_1.GetComponent<BulbCircleRotation>().OnPower = false;
            this.gameObject.GetComponent<Renderer>().material = OffPowerGlassMaterial;
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "FlipGate")
        {
            collision.gameObject.GetComponent<FlipGate>().Flip(this.gameObject);
        }
    }
}
