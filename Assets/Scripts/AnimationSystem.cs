using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yarn.Unity.Example
{

    public class AnimationSystem : MonoBehaviour
    {
        //This struct is how we will organize our animation clips 
        [System.Serializable]
        public struct AnimationInfo
        {
            public string name;
            public Animator yarnAnim; //I dont know if we are going to use animation or animation clip here
              
        }

        //this is how we will organize our audio clips 
        [System.Serializable]
        public struct AudioInfo
        {
            public string name;
            public AudioClip yarnAudio; //need to check on use of audio clips vs interacting with the emitter
        }

        public AnimationInfo[] animations;
        public AudioInfo[] audioclips;
        public AudioSource audioSource;

        // Create a command to play and change animations 
        [YarnCommand("cueanim")] 
        public void PlayYarnAnim(string animName)
        {
            Debug.Log("YARN COMMAND ACTIVATED!");
            Animator a = null;
            foreach(var info in animations) 
            {
                if (info.name == animName)
                {
                    a = info.yarnAnim;
                    a.SetTrigger(animName);
                    Debug.Log("Set Trigger debug");

                    break;
                }
            }
        
            if (a == null)
            {
                Debug.LogErrorFormat("Can't find animator named {0}!", animName);
                return;
            }

            a = GetComponent<Animator>();
        }

        // Create a command to play and change audio clips 
        [YarnCommand("cueaudio")]
        public void PlayYarnAudio(string audioName)
        {
            Debug.Log("YARN AUDIO COMMAND ACTIVATED!");
            AudioClip musicClip = null;
            foreach (var info in audioclips)
            {
                if (info.name == audioName)
                {
                    musicClip = info.yarnAudio;
                    audioSource.clip = musicClip;
                    audioSource.Play();
                    Debug.Log("Set audio Trigger debug");

                    break;
                }
            }

            if (musicClip == null)
            {
                Debug.LogErrorFormat("Can't find audio named {0}!", audioName);
                return;
            }

            //musicClip = GetComponent<AudioClip>();
        }

    }
}