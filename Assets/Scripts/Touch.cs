using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Touch : MonoBehaviour {
    public GameObject player;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {

        GetMouseInput();
    }

    public void GetMouseInput()
    {
        while (Input.GetMouseButtonDown(0))
        {
            int speed = 5;
            float step = speed * Time.deltaTime;
            float distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            Vector3 pos_move = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));
            player.transform.position = Vector3.MoveTowards(player.transform.position, new Vector3(Mathf.Clamp(pos_move.x, -4f, 4f), player.transform.position.y, player.transform.position.z), step);
        }
    }

    void OnMouseDrag()
    {

    }
}
