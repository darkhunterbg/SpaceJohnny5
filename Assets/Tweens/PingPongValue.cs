using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongValue : MonoBehaviour
{
	public float StartValue;
	public float EndValue;
	public float Speed;
	public float CurrentValue;

	private float _currentValue;
	private short _direction;

	private void Awake()
	{
		ResetValue();
	}

	public void ResetValue()
	{
		_currentValue = StartValue;
	}

	void Update()
	{
		_currentValue += Time.deltaTime * Speed * _direction;

		if (_currentValue <= StartValue) {
			_direction = 1;
		} else if (_currentValue >= EndValue) {
			_direction = -1;
		}

		CurrentValue = _currentValue;
	}
}
