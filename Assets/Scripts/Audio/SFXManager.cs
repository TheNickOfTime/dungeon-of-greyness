using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
	public static AudioSource PlayClipAtPoint(AudioClip clip, Vector2 position)
	{
		AudioSource source = Instantiate(new GameObject().AddComponent<AudioSource>(), position, Quaternion.identity);
		source.spatialBlend = 0.0f;
		source.clip = clip;
		source.Play();
		Destroy(source, clip.length);

		return source;
	}
}
