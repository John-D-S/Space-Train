using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SpaceTrain.Music
{
    [RequireComponent(typeof(AudioSource))]
    public class Music : MonoBehaviour
    {
        [Header("-- Song Settings --")]
        [SerializeField, Tooltip("A list of songs that will play")]
        private List<AudioClip> songs;
        //the song that is currently playing
        private AudioClip currentSong;
        //the audioSource that plays the music.
        private AudioSource MusicPlayer;
        private Queue<AudioClip> lastFiveSongs = new Queue<AudioClip>();

        public static Music theBackgroundMusic;

        private void Awake()
        {
            //destroy this if theBackgroundMusic already exists in the scene
            if (theBackgroundMusic != null && theBackgroundMusic != this)
            {
                Destroy(this.gameObject);
                return;
            }
            //set the static theBackgroundMusic variable to this
            theBackgroundMusic = this;
            //set this to not be destroyed when the scene changes so that the music does not stop.
            DontDestroyOnLoad(gameObject);
            MusicPlayer = GetComponent<AudioSource>();
            StartCoroutine(StartMusic());
        }

        /// <summary>
        /// start playing songs
        /// </summary>
        private IEnumerator StartMusic()
        {
            //play a random song until it stops, then play another
            while (true)
            {
                MusicPlayer.Stop();
                //the music player will not choose a song in the lastFiveSongs queue to prevent repitition
                List <AudioClip> availableSongs = new List<AudioClip>();
                foreach (AudioClip song in songs)
                {
                    if (!lastFiveSongs.Contains(song))
                    {
                        availableSongs.Add(song);
                    }
                }
                currentSong = availableSongs[Random.Range(0, availableSongs.Count)];
                lastFiveSongs.Enqueue(currentSong);
                if (lastFiveSongs.Count > 5)
                {
                    lastFiveSongs.Dequeue();
                }
                MusicPlayer.clip = currentSong;
                MusicPlayer.Play();
                yield return new WaitForSeconds(currentSong.length + 5);
            }
        }
    }
}