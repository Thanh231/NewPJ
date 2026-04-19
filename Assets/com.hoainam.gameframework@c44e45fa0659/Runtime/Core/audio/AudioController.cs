
using UnityEngine;

public class AudioController : SingletonMonoBehaviour<AudioController>
{
	public AudioSource audioSource_music;
	public AudioSource audioSource_sound;

	public void StopMusic()
	{
		audioSource_music.Stop();
	}

	public void PlayMusic(string key)
	{
		audioSource_music.clip = GetAudioClip(key);
		audioSource_music.Play();
	}

	public void PlaySound(string key)
	{
		audioSource_sound.PlayOneShot(GetAudioClip(key));
	}

	public void SetVolumeMusic(float volume)
	{
		audioSource_music.volume = volume;
	}

	public void SetVolumeSound(float volume)
	{
		audioSource_sound.volume = volume;
	}

	private AudioClip GetAudioClip(string key)
	{
		return GameFrameworkConfig.instance.audioClips.Find(x => x.audioClipName.Equals(key)).audioClip;
	}
}