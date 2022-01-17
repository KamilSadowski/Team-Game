using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soundtrack : MonoBehaviour
{
    [SerializeField] AudioClip[] storedSoundtracks;
    [SerializeField] float soundtrackVolume = 1.0f;
    private AudioSource activeAudio;
    // Start is called before the first frame update
    void Start()
    {
        activeAudio = gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!activeAudio)
        {
            activeAudio = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            if(storedSoundtracks.Length > 0 && !activeAudio.isPlaying)
            activeAudio.PlayOneShot(storedSoundtracks[Random.Range(0, storedSoundtracks.Length)], soundtrackVolume);
        }
    }
}
