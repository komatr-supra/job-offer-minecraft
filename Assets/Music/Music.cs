using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private AudioSource audioSource;
    private void Update() {
        if(!audioSource.isPlaying)
        {
            AudioClip randomClip = clips[Random.Range(0, clips.Length)];
            audioSource.PlayOneShot(randomClip);
        }
    }
}
