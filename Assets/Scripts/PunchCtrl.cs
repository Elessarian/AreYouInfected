using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchCtrl : MonoBehaviour {
	public PlayerCtrl playerCtrl;
	// Use this for initialization

	void Start () {
        playerCtrl = this.GetComponentInParent<PlayerCtrl>();
    }
	
	// Update is called once per frame
	void Update () {
        
        Debug.DrawRay (transform.position, transform.forward * 5.0f, Color.green);
		if (Input.GetMouseButtonDown(0) && playerCtrl.isGetVaccine)
		{
			RaycastHit hit;

            playerCtrl.isGetVaccine = false;

			if (Physics.Raycast (transform.position, transform.forward, out hit, 3.0f)) {
				if (hit.collider.tag == "Player") {
					hit.collider.gameObject.SendMessage ("OnDamage", null, SendMessageOptions.DontRequireReceiver);
                }
            }
		}
	}
}
