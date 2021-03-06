﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerControl : NetworkBehaviour
{
	public float runPower;
	public float maxRunSpeed;
	public float jumpPower;

	private Rigidbody2D _rigid;
	private Animator _anim;
	private SpriteRenderer _sprite;

	[SyncVar]
	public bool isGrounded;
	[SyncVar]
	public bool isRunning;
	[SyncVar]
	public bool isJumping;
	[SyncVar]
	public bool inWater;
	[SyncVar]
	public bool isWaving;

	void Awake()
	{
		_anim = GetComponent<Animator>();
		_rigid = GetComponent<Rigidbody2D>();
		_sprite = GetComponent<SpriteRenderer>();
	}

	void Start()
	{
		transform.position = new Vector3(transform.position.x, transform.position.y, -5);
	}

	public override void OnStartLocalPlayer()
	{
		GetComponent<SpriteRenderer>().color = ColorPalette.cp.green4;
		var ps = GetComponentInChildren<ParticleSystem>().main;
		ps.startColor = ColorPalette.cp.green3;
	}

	// the Update loop contains a very simple example of moving the character around and controlling the animation
	void Update()
	{
		//Take Player movement
		if (isLocalPlayer)
		{
			//prevent action
			if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Crew Dead"))
			{
				return;
			}
			//Ground anim
			//_anim.SetBool("On Ground", isGrounded);

			float direction = 0;

			//Movement
			if ((direction = Input.GetAxis("Horizontal")) != 0)
			{
				//Facing
				_sprite.flipX = direction > 0;

				//Moving
				isRunning = true;//_anim.SetBool("Running", true);
			}
			else
			{
				//Not moving
				isRunning = false; // _anim.SetBool("Running", false);
			}

			//Dampen air movement
			if (!isGrounded)
				direction /= 2;

			//Movement
			_rigid.AddForce(new Vector2(runPower * direction, 0));

			//Clamp horizontal velocity
			_rigid.velocity = _rigid.velocity.x > maxRunSpeed ? new Vector2(maxRunSpeed, _rigid.velocity.y) : _rigid.velocity;
			_rigid.velocity = _rigid.velocity.x < -maxRunSpeed ? new Vector2(-maxRunSpeed, _rigid.velocity.y) : _rigid.velocity;

			//Vertical
			float vertical = Input.GetAxis("Vertical");

			if (Input.GetAxis("Jump") > 0)
				vertical += 1;

			//Waving
			isWaving = vertical < 0;

			//Jumping
			if (isGrounded && vertical > 0)
			{
				_rigid.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);

				isJumping = true;// _anim.SetTrigger("Jump");

				isGrounded = false;
			}
			else
			{
				isJumping = false;
			}

			CmdSyncState(isGrounded, isRunning, isJumping, inWater, isWaving);
		}
		//Everybody else now
		RunState(); //Run state regardless

	}

	[Command]
	private void CmdSyncState(bool grounded, bool running, bool jumping, bool water, bool wave)
	{
		if (isServer)
		{
			isGrounded = grounded;
			isRunning = running;
			isJumping = jumping;
			inWater = water;
			isWaving = wave;
		}
	}


	private void RunState()
	{
		_anim.SetBool("On Ground", isGrounded);
		_anim.SetBool("Running", isRunning);
		_anim.SetBool("In Water", inWater);
		_anim.SetBool("Waving", isWaving);
		if (isJumping)
			_anim.SetTrigger("Jump");

	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (isLocalPlayer)
		{
			if (other.tag == "Water")
			{
				_rigid.gravityScale = 0.1f;
				if (Mathf.Abs(_rigid.velocity.y) > 1)
					_rigid.velocity /= 10;

				inWater = true;//_anim.SetBool("In Water", true);
			}
			CmdSyncState(isGrounded, isRunning, isJumping, inWater, isWaving);
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (isLocalPlayer)
		{
			if (other.tag == "Water")
			{
				_rigid.gravityScale = 2f;

				inWater = false;//_anim.SetBool("In Water", false);
			}
			CmdSyncState(isGrounded, isRunning, isJumping, inWater, isWaving);
		}
	}


}
