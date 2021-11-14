using System.Collections;
using UnityEngine.Audio;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]

public class AudioManager : SingletonPattern<AudioManager>
{
    public enum Channel { player, friendly, hostile, ambientActive, ambientPassive, background }; // corresponds to audio group of the same name in the mixer

#pragma warning disable 0649
    [SerializeField] float variety, ambientPassiveMaxWait, ambientPassiveMinWait;
#pragma warning restore 0649

    string[] channelNames;

    AudioSource[] channels;
    AudioMixer mixer;
    AudioClip[] samples; // naming: channelName_sample_name.wav

    void Start()
    {
        mixer = Resources.Load<AudioMixer>("Sounds/Master");
        samples = Resources.LoadAll<AudioClip>("Sounds");
        channels = new AudioSource[System.Enum.GetNames(typeof(Channel)).Length];
        channelNames = System.Enum.GetNames(typeof(Channel));
        for (int chan = 0; chan < channels.Length; chan++)
        {
            channels[chan] = gameObject.AddComponent<AudioSource>();
            channels[chan].outputAudioMixerGroup = mixer.FindMatchingGroups(channelNames[chan])[0];
        }

        // start bg track
        int chin = (int)Channel.background;
        channels[chin].loop = true;
        PlaySound(Channel.background, GetSample("background"));
        StartCoroutine(PlayAmbient()); // start random sfx
    }

    public void PlaySoundOnce(Channel chan, AudioClip smpl, float vol = 1f, float pitch = 1f, bool rand = false)
    {
        int chin = (int)chan;
        channels[chin].volume = vol;
        channels[chin].pitch = pitch + (rand ? Random.Range(-variety, variety) : 0f);
        channels[chin].PlayOneShot(smpl);
    }

    public void PlaySound(Channel chan, AudioClip smpl, float vol = 1f, float pitch = 1f, bool rand = false)
    {
        StartCoroutine(PlaySoundThrough(chan, smpl, vol, pitch, rand));
    }

    IEnumerator PlaySoundThrough(Channel chan, AudioClip smpl, float vol, float pitch, bool rand)
    {
        int chin = (int)chan;
        channels[chin].volume = vol;
        channels[chin].pitch = pitch + (rand ? Random.Range(-variety, variety) : 0f);
        channels[chin].clip = smpl;
        channels[chin].Play();
        yield return new WaitWhile(() => channels[chin].isPlaying);
    }

    public void StartSound(Channel chan, AudioClip smpl, float vol = 1f, float pitch = 1f)
    {
        int chin = (int)chan;
        ToggleLoop(chan);
        channels[chin].volume = vol;
        channels[chin].pitch = pitch;
        channels[chin].clip = smpl;
        channels[chin].Play();

    }

    public void ToggleLoop(Channel chan)
    {
        int chin = (int)chan;
        channels[chin].loop = !channels[chin].loop;
    }

    public void StopSound(Channel chan)
    {
        channels[(int)chan].Stop();
        ToggleLoop(chan);

    }

    public bool IsPlaying(Channel chan)
    {
        return channels[(int)chan].isPlaying;
    }

    public void ToggleBackgroundMusic()
    {
        int chin = (int)Channel.background;
        if (channels[chin].isPlaying)
            channels[chin].Pause();
        else
            channels[chin].UnPause();
    }

    public AudioClip GetSample(string name)
    {
        foreach (AudioClip smpl in samples)
        {
            if (name.Equals(smpl.name))
                return smpl;
        }
        return null;
    }

    public AudioSource GetChannel(Channel chan)
    {
        return channels[(int)chan];
    }
    public AudioClip GetRandomSample(string sub)
    {
        AudioClip[] possible = new AudioClip[samples.Length];
        int i = 0;
        foreach (AudioClip smpl in samples)
        {
            if (smpl.name.Contains(sub))
                possible.SetValue(smpl, i++);
        }
        if (i == 0)
            return null;
        return possible[Random.Range(0, i)];
    }

    IEnumerator PlayAmbient()
    {
        int chin = (int)Channel.ambientPassive;
        AudioSource chan = channels[chin];
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(ambientPassiveMaxWait, ambientPassiveMinWait));
            chan.clip = GetRandomSample(channelNames[chin]);
            chan.Play();
            yield return new WaitWhile(() => chan.isPlaying);
        }
    }
}
