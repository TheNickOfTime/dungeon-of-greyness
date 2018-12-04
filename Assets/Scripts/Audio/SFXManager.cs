using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
	public static AudioSource PlayClipAtPoint(AudioClip clip, Vector2 position)
	{
		AudioSource source = new GameObject("[OneShotAudio]").AddComponent<AudioSource>();
		source.spatialBlend = 0.0f;
		source.clip = clip;
		source.Play();

		source.gameObject.AddComponent<TimedDestroy>().timer = clip.length;

		return source;
	}
}
