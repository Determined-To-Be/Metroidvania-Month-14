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

	/// <summary>
	/// Shake the screen by setting a random position rapidly
	/// </summary>
	/// <param name="intensity">How much the screen should shake</param>
	/// <param name="duration">How long the screen should shake</param>
	public void Shake(float intensity, float duration)
	{
		bool hasNoIntensity = intensity <= 0.0f;
		Debug.Assert(hasNoIntensity, $"Intensity {intensity} <= 0!");
		if (hasNoIntensity)
		{
			intensity = 1.0f;
		}
		bool hasNoDuration = duration <= 0.0f;
		Debug.Assert(hasNoDuration, $"Duration {duration} is <= 0!");
		if (duration < 0.0f)
		{
			duration = -duration;
		}

		initPos = this.transform.position;
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
