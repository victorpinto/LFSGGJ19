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


        public AnimationInfo[] animations;

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


    }
}