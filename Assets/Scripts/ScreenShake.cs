using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MVMXIV;

public class ScreenShake : SingletonPattern<ScreenShake>
{
	Vector3 initPos;

	void Start()
	{
		initPos = this.transform.position;
	}

	public void Shake(float intensity, float duration)
	{
		StartCoroutine(shakeScreen(intensity, duration));
	}

	IEnumerator shakeScreen(float intensity, float duration)
	{
		while (duration > 0)
		{
			this.transform.position = initPos + Random.insideUnitSphere * intensity;
			duration -= Time.deltaTime;
			yield return null;
		}
		this.transform.position = initPos;
	}
}
