using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetChecker : MonoBehaviour
{

	Collider2D _coll;
	PlayerControl _player;

	void Start()
	{
		_coll = GetComponent<Collider2D>();
		_player = GetComponentInParent<PlayerControl>();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		_player.isGrounded = true;
	}

	void OnTriggerStay2D(Collider2D other)
	{
		_player.isGrounded = true;
	}

	void OnTriggerExit2D(Collider2D other)
	{
		_player.isGrounded = false;
	}
}
