using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharterAni2 : MonoBehaviour {
    private Animator anim;
    private CharacterController controller;
    // Use this for initialization
    void Start () {
        anim = gameObject.GetComponentInChildren<Animator>();
        controller = gameObject.GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 localVelocity = this.transform.InverseTransformDirection(controller.velocity);
        Vector3 forwrad = new Vector3(0f, 0f, localVelocity.z);

        Vector3 rightDir = new Vector3(localVelocity.x, 0f, 0f);
        if (forwrad.z !=0&&rightDir.x !=0)
        {
            anim.SetBool("IsWalk", true);
            
        }
        else if (Input.GetMouseButtonDown(0))
        {
            
        }

        else
        {
            anim.SetBool("IsRun", false);
            anim.SetBool("IsWalk", false);
            anim.SetBool("IsJump", false);
        }
          if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("IsJump");
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if(!anim.GetBool("IsRun"))
           anim.SetBool("IsRun", true);

        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (anim.GetBool("IsRun"))
                anim.SetBool("IsRun", false);
        }
    }
}
