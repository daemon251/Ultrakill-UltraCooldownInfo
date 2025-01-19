using System;
using System.IO;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using UKEnemyIdentifier;

namespace UltraCooldownInfo
{
    //ADAPTED FROM https://github.com/GAMINGNOOBdev/UltraRankSounds/tree/master
    //dont believe this needs to be an object, change later
    public class CustomSoundPlayer : MonoBehaviour
    {
        private static CustomSoundPlayer _instance = null;
        public static CustomSoundPlayer Instance
        {
            get { return _instance; }
            set { throw new NotImplementedException(); }
        }

        public AudioSource source;
        private string soundPath;

        private void Start()
        {
            _instance = this;
            source = gameObject.AddComponent<AudioSource>();
        }

        public void PlaySound(string file, int i)
        {
            if (string.IsNullOrEmpty(file))
                return;

            if (!File.Exists(file))
            {
                Debug.LogError($"Could not find audio file '{file}'");
                return;
            }
            soundPath = file;
            gameObject.SetActive(true);
            StartCoroutine(PlaySoundRoutine(i));
        }
        private IEnumerator PlaySoundRoutine(int i)
        {
            FileInfo fileInfo = new(soundPath);
            AudioType audioType = AudioType.WAV;

            using UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(new Uri(soundPath).AbsoluteUri, audioType);
            DownloadHandlerAudioClip handler = request.downloadHandler as DownloadHandlerAudioClip;
            handler.streamAudio = false;
            request.SendWebRequest();
            yield return request;

            source.Stop(); //clear sound
            source.clip = handler.audioClip;
            source.volume = 1.0f * Plugin.UltraCooldownInfoWeapons[i].volumeMult * MonoSingleton<PrefsManager>.Instance.GetFloat("sfxVolume"); 
            source.Play();
        }
        
    }

}