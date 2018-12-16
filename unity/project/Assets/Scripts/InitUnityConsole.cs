using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitUnityConsole : MonoBehaviour {

	// Use this for initialization
	void Start () {
        UnityConsoleRedirect.Redirect();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
