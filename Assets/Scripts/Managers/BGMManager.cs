using UnityEngine;

public class BGMManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip titleBGM;       
    [SerializeField] private AudioClip battleBGM1;     
    [SerializeField] private AudioClip battleBGM2;     
    [SerializeField] private AudioClip bossBGM;        
    [SerializeField] private AudioClip victoryBGM;     
    [SerializeField] private AudioClip defeatBGM;      

    public void PlayTitle() 
    { 
        PlayMusic(titleBGM); 
    }
    
    public void PlayBattle1() { PlayMusic(battleBGM1); }
    public void PlayBattle2() { PlayMusic(battleBGM2); }
    public void PlayBoss() { PlayMusic(bossBGM); }
    public void PlayVictory() { PlayMusic(victoryBGM, false); } 
    public void PlayDefeat() { PlayMusic(defeatBGM, false); }   

    private void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null || audioSource == null) return;

        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();
    }

    public void StopMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}