using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
	[SerializeField] private bool invertDirection = true;

	private Camera _camera;

	private void Start()
	{
		_camera = Camera.main;
	}
	private void LateUpdate()
	{
		if (!_camera)
			return;
		
		transform.forward = invertDirection ? -_camera.transform.forward : _camera.transform.forward;
	}
}