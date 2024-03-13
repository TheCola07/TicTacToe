using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyAudioManager : MonoBehaviour
{
    public AudioClip[] ses;
    AudioSource audioSource;

    private static MyAudioManager instance;
    public static MyAudioManager Instance { get { return instance; } }

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySE(int index) {
        audioSource.clip = ses[index];
        audioSource.Play();
    }
}
