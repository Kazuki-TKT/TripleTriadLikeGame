using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TripleTriad
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        [SerializeField] AudioSource[] seSource;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void PlayOneShotClip(AudioClip clip)
        {
            foreach (AudioSource source in seSource)
            {
                if (!source.isPlaying)
                {
                    source.PlayOneShot(clip);
                    break;
                }
            }
        }
    }
}
