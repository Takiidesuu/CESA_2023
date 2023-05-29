using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameScript : MonoBehaviour
{
    [Header("フレームのモデル")]
    [SerializeField] private GameObject world1_frame;
    [SerializeField] private GameObject world2_frame;
    [SerializeField] private GameObject world3_frame;
    [SerializeField] private GameObject world4_frame;
    
    [Header("電球UIモデル")]
    [SerializeField] private GameObject bulb_ui_model;
    
    int worldId;
    
    // Start is called before the first frame update
    void Start()
    {
        int tesStage;
        StageUtilitys.GetCurrentWorldAndStage(out worldId,out tesStage);
        
        GameObject frame_to_use;
        
        switch (worldId + 1)
        {
            case 1:
            frame_to_use = world1_frame;
            break;
            
            case 2:
            frame_to_use = world2_frame;
            break;
            
            case 3:
            frame_to_use = world3_frame;
            break;
            
            case 4:
            frame_to_use = world4_frame;
            break;
            
            default:
            frame_to_use = world1_frame;
            break;
        }
        
        frame_to_use = Instantiate(frame_to_use, Vector3.zero, Quaternion.identity);
        frame_to_use.transform.SetParent(transform.GetChild(0));
        
        frame_to_use.transform.localPosition = new Vector3(0.007f, 0.006f, 0);
        frame_to_use.transform.localScale = new Vector3(15.855f, 28.98f, 6142.26f);
        
        frame_to_use.layer = LayerMask.NameToLayer("Frame");
        
        for (int i = 0; i < frame_to_use.transform.childCount; i++)
        {
            frame_to_use.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Frame");
        }
    }
}
