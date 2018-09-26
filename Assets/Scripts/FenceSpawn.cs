using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceSpawn : MonoBehaviour {

	public GameObject Model;
	// Use this for initialization
	void Start () {
	
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SpawnFence()
    {
        bool ran = false;
        if (!ran && Model != null)
        {
           
            ran = true;
        }
    }

}
