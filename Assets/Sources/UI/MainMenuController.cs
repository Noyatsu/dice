using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour {
    GameObject bgImage;

    private int frame = 0;
	// Use this for initialization
	void Start () {
        bgImage = GameObject.Find("bgImage");
        BgmManager.Instance.Play("opening");
    }
	
	// Update is called once per frame
	void Update () {
        bgImage.transform.Rotate(0f, 0f, 0.05f);
	}
}
