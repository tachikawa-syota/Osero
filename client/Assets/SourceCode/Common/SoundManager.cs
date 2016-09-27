using UnityEngine;
using System.Collections.Generic;

// サウンド管理クラス
public class SoundManager : MonoBehaviour
{
    public enum SoundData
    {
        MainBGM = 0
    }

    // メイン音楽
	public AudioClip m_mainBGM;

    private List<AudioSource> audioSources = new List<AudioSource>();

    void Start()
    {
        audioSources.Add(gameObject.GetComponent<AudioSource>());
        //	for(int i=0;i<10;i++){
        //		audioSources[i] = gameObject.GetComponent<AudioSource>();
        //	}
        audioSources[0].clip = m_mainBGM;
        //audioSources = gameObject.GetComponents<AudioSource>();
        //audioSources[mainBGM.ToString()].clip = mainBGM;
    }

    public void Play(SoundData data)
    {
        audioSources[(int)data].Play();
    }

};
