using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectManager : SingletonMonobehaviour<SoundEffectManager> {
    private List<AudioSourceInfo> sources;
    public AudioClip[] hitsounds;
    public AudioClip blockSound;
    public AudioClip[] koSounds;
    public AudioClip uiClickSound;
    public AudioClip[] attackMissSounds;
    public AudioClip roundBegin;
    public AudioClip matchEnd;
    public AudioClip countdown;

    private void Start()
    {
        sources = new List<AudioSourceInfo>();
        sources.Add(new AudioSourceInfo());
    }

    private class AudioSourceInfo
    {
        public float time, timeStarted;
    }

    public void PlayBlockSound()
    {
        PlayOneShot(blockSound, 1, 1);
    }

    public void PlayRoundBegin()
    {
        PlayOneShot(roundBegin, 1, 1);
    }

    public void PlayMatchEnd()
    {
        PlayOneShot(matchEnd, 1, 1);
    }

    public void PlayHitSound()
    {
        PlayOneShot(hitsounds[Random.Range(0, hitsounds.Length)], 1, 1);
    }

    public void PlayKOSounds()
    {
        PlayOneShot(koSounds[Random.Range(0, koSounds.Length)], 1, 1);
    }

    public void PlayCountdown()
    {
        PlayOneShot(countdown, 1, 1);
    }

    public void PlayAttackMissSounds()
    {
        PlayOneShot(attackMissSounds[Random.Range(0, attackMissSounds.Length)], 1, 1);
    }

    public void PlayUIClickSound()
    {
        PlayOneShot(uiClickSound, 1, 1);
    }

    public void PlayOneShot(AudioClip clip, float volume, float pitch, Vector3 loc = new Vector3())
    {
        for (int i = 0; i < sources.Count; i++)
        {
            if (Time.time > sources[i].timeStarted + sources[i].time)
            {
                sources[i].time = clip.length;
                sources[i].timeStarted = Time.time;
                PlaySound(clip, i, volume, pitch, loc);
                return;
            }
        }
        Instantiate(transform.GetChild(0).gameObject, transform);
        sources.Add(new AudioSourceInfo());
        sources[sources.Count - 1].time = clip.length;
        sources[sources.Count - 1].timeStarted = Time.time;
        PlaySound(clip, sources.Count - 1, volume, pitch, loc);
    }

    private void PlaySound(AudioClip clip, int source, float volume, float pitch, Vector3 loc = new Vector3())
    {
        if (loc != Vector3.zero)
        {
            transform.GetChild(source).position = loc;
            transform.GetChild(source).GetComponent<AudioSource>().spatialBlend = 1.0f;
        }
        else
        {
            transform.GetChild(source).GetComponent<AudioSource>().spatialBlend = 0.0f;
        }
        transform.GetChild(source).GetComponent<AudioSource>().loop = false;
        transform.GetChild(source).GetComponent<AudioSource>().pitch = pitch;
        transform.GetChild(source).GetComponent<AudioSource>().clip = clip;
        transform.GetChild(source).GetComponent<AudioSource>().volume = volume;
        transform.GetChild(source).GetComponent<AudioSource>().Play();
    }
}
