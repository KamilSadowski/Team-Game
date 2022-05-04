using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{

    [SerializeField] AudioClip[] movementClip;
    [SerializeField] AudioClip[] onDamageClip;
    [SerializeField] float movementClipVolume = 1.0f;
    [SerializeField] AudioMixerGroup audioMixer;
    private List<AudioSource> activeAudioSources;

    GameObject tempObject;
    public void PlaySound(int index)
    {
       tempObject = new GameObject("SoundEffect_" + activeAudioSources.Count);
       // Instantiate(tempObject, transform.position, transform.rotation);

        activeAudioSources.Add(tempObject.AddComponent<AudioSource>());

        activeAudioSources[activeAudioSources.Count - 1].outputAudioMixerGroup = audioMixer;


        if (index <= 0)
            activeAudioSources[activeAudioSources.Count - 1].PlayOneShot(movementClip[Random.Range(0, movementClip.Length)], movementClipVolume);
        else if (index <= 1)
            activeAudioSources[activeAudioSources.Count - 1].PlayOneShot(onDamageClip[Random.Range(0, onDamageClip.Length)], 0.875f*0.33f);
    }
    // Start is called before the first frame update
    private void Awake()
    {
        activeAudioSources = new List<AudioSource>();
    }
    private void FixedUpdate()
    {
        if (activeAudioSources.Count > 0)
        {
            for (int i = 0; i < activeAudioSources.Count; ++i)
            {
                if (!activeAudioSources[i] || !activeAudioSources[i].isPlaying)
                {
                    Destroy(activeAudioSources[i].gameObject);
                    activeAudioSources.RemoveAt(i);
                }
            }
        }
    }

   ~SoundManager()
    {
        for (int i = 0; i < activeAudioSources.Count; ++i)
        {
            Destroy(activeAudioSources[i].gameObject);
            activeAudioSources.RemoveAt(i);
        }
    }
}
