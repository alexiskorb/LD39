﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Pickup : MonoBehaviour
{
	public static List<Pickup> Pickups = new List<Pickup>();

	[Tooltip("If set, only the specified player can pick this up.")]
	[SerializeField]
	private PlayerType m_onlyPlayer = PlayerType.None;

	[SerializeField]
	private bool m_resetRotationOnDrop = false;

	[SerializeField]
	protected AudioClip m_onPickUpAudio = null;

	[Tooltip("Optional substitute for the sprite on the HUD.")]
	public Sprite HudSprite = null;

	public PlayerInteraction HeldBy { get; private set; }

	#region Cached Components

	private Collider2D m_collider;
	private Rigidbody2D m_rigidbody;

	#endregion

	protected virtual void Awake()
	{
		m_collider = GetComponent<Collider2D>();
		m_rigidbody = GetComponent<Rigidbody2D>();
	}

	private void OnEnable()
	{
		Pickups.Add(this);
	}

	private void OnDisable()
	{
		Pickups.Remove(this);
	}

	private void OnDestroy()
	{
		if (HeldBy)
		{
			HeldBy.SetDownPickup();
		}
	}

	public virtual void NotifyPickUp(PlayerInteraction pickUpper)
	{
		if (m_collider != null)
		{
			m_collider.enabled = false;
		}
		if (m_rigidbody)
		{
			m_rigidbody.velocity = Vector3.zero;
			m_rigidbody.simulated = false;
		}
		if (m_onPickUpAudio)
		{
			AudioSource audioSource = OurUtility.GetOrAddComponent<AudioSource>(pickUpper.gameObject);
			audioSource.PlayOneShot(m_onPickUpAudio);
		}
		HeldBy = pickUpper;
	}

	public virtual void NotifyDrop(PlayerInteraction dropper)
	{
		if (m_collider)
		{
			m_collider.enabled = true;
		}
		if (m_rigidbody)
		{
			m_rigidbody.simulated = true;
		}
		if (m_resetRotationOnDrop)
		{
			transform.localRotation = Quaternion.identity;
		}
		HeldBy = null;
	}

	/// <summary>
	/// Returns true if the specified player can grab this.
	/// </summary>
	public virtual bool CanBeGrabbedBy(GameObject player)
	{
		if (m_onlyPlayer == PlayerType.Both)
		{
			return true;
		}
		else if (m_onlyPlayer == PlayerType.None)
		{
			return false;
		}
		else
		{
			if (player.GetComponent<PlayerInteraction>().m_isBeep)
			{
				return m_onlyPlayer == PlayerType.Beep;
			}
			else
			{
				return m_onlyPlayer == PlayerType.Boop;
			}
		}
	}
}
