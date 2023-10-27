using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineResultController : MonoBehaviour {

	// Use this for initialization
	private void Start () {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
    }

    // Update is called once per frame
    private void Update () {
		
	}
}
