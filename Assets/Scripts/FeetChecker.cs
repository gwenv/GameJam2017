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

	void Update()
	{
		_player.isGrounded = _coll.IsTouchingLayers();
	}
}
