using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {
    public GameObject Instance { get; private set; }
    [SerializeField]
    List<AudioClip> songs;
    int songIndex = 0;
    private AudioSource audioSource;

    private void Awake() {
        if (Instance == null) {
            Instance = this.gameObject;
        }
        if (this.gameObject != Instance) {
            Destroy(this.gameObject);
        }
    }

    // Use this for initialization
    void Start () {

        audioSource = this.gameObject.AddComponent<AudioSource>();

        audioSource.clip = songs[songIndex];
        audioSource.Play();
	}
	
	// Update is called once per frame
	void Update () {
        if (!audioSource.isPlaying) {
            songIndex = 1;
            audioSource.clip = songs[songIndex];
            audioSource.Play();
            audioSource.loop = true;
        }
	}
}
