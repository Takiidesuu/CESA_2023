using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultNPCScript : MonoBehaviour
{
    private Animator animator;
    private ScoreManager scoreManager;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        
        scoreManager = GameObject.FindObjectOfType<ScoreManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (scoreManager.Score >= scoreManager.border_s)
        {
            animator.SetBool("ClearPerfect", true);
        }
        else if (scoreManager.Score >= scoreManager.border_b)
        {
            animator.SetBool("ClearMid", true);
        }
        else
        {
            animator.SetBool("ClearLow", true);
        }
    }
}