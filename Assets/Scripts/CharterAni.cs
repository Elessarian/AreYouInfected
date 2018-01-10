using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharterAni : MonoBehaviour {

    public enum Animstate
    {
        idle = 0,
        run,
        isattack
    }

    public Animstate animst = Animstate.idle;
    private Animator anim;
    private CharacterController controller;
    private NetworkView networkView;
    // Use this for initialization
    
    void Start () {
        anim = gameObject.GetComponentInChildren<Animator>();
        controller = gameObject.GetComponent<CharacterController>();
        networkView = GetComponent<NetworkView>();
    }

	// Update is called once per frame
	void Update () {
        if (networkView.isMine)
        {
            Vector3 localVelocity = this.transform.InverseTransformDirection(controller.velocity);
            Vector3 forwrad = new Vector3(0f, 0f, localVelocity.z);

            Vector3 rightDir = new Vector3(localVelocity.x, 0f, 0f);
            if (forwrad.z != 0 && rightDir.x != 0)
            {
                animst = Animstate.run;


            }
            else if (Input.GetMouseButtonDown(0))
            {
                animst = Animstate.isattack;

            }
            else
            {
                animst = Animstate.idle;

            }
            AnimateControll();
        }
        else
        {
           
            AnimateControll();
        }
	}
    private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        if (stream.isWriting)
        {
            int _animStae = (int)animst;
            stream.Serialize(ref _animStae);

        }
        else
        {
            int _animState = 0;
            stream.Serialize(ref _animState);
            animst = (Animstate)_animState;
        }
    }
    void AnimateControll()
    {
        switch (animst)
        {
            case Animstate.idle:
                anim.SetBool("IsRun", false);
                anim.SetBool("IsAttack", false);
                break;
            case Animstate.isattack:
                anim.SetBool("IsAttack", true);
                anim.SetBool("IsRun", false);
                break;
            case Animstate.run:
                anim.SetBool("IsRun", true);
                break;
        }
    }
}
