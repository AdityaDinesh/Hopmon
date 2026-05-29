using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BgmSoundType
{
    MainMenu,
    World1,
}
public enum SfxSoundType
{
    PlayerHop,
    PlayerShoot,
    Explosion,
    FireballHit,
    CanonFireball,
    PotCanonFireball,
    SpikeTrap,
    LevelEnd,
    UIChange,
    UISelect,
    CrystalCollect,
    CrystalFly,
    FireReady,
    GameOver,
}

[System.Serializable]
public class BgmSoundEntry
{
    public BgmSoundType soundType;
    public AudioClip clip;
}

[System.Serializable]
public class SfxSoundEntry
{
    public SfxSoundType soundType;
    public AudioClip clip;
    [Range(0, 1)]
    public float volume;
}

public class AudioController : MonoBehaviour
{
    public static AudioController Instance;

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private BgmSoundEntry[] bgmSounds;

    [Header("SFX Pool")]
    [SerializeField] private AudioSource sfxSourcePrefab;
    [SerializeField] private SfxSoundEntry[] sfxSounds;
    [SerializeField] private int poolSize = 10;

    [Header("Volume Parameters")]
    [Range(0,1)]
    [SerializeField] private float _musicVolume = 1f;
    [Range(0, 1)]
    [SerializeField] private float _sfxVolume = 0.7f;

    private Dictionary<BgmSoundType, AudioClip> bgmSoundDictionary;
    //private Dictionary<SfxSoundType, AudioClip> sfxSoundDictionary;
    private Dictionary<SfxSoundType, SfxSoundEntry> sfxSoundDictionary;

    private List<AudioSource> sfxPool = new List<AudioSource>();
    private Transform _transform;

    private float _originalMusicVolume;

    private void Awake()
    {
        // Singleton Setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        // Load Bgm and Sfx sounds in Dictionary
        bgmSoundDictionary = new Dictionary<BgmSoundType, AudioClip>();

        foreach (BgmSoundEntry sound in bgmSounds)
        {
            bgmSoundDictionary[sound.soundType] = sound.clip;
        }

        sfxSoundDictionary = new Dictionary<SfxSoundType, SfxSoundEntry>();

        foreach (SfxSoundEntry sound in sfxSounds)
        {
            //sfxSoundDictionary[sound.soundType] = sound.clip;
            sfxSoundDictionary[sound.soundType] = sound;
        }

        // Generate pool and Set basic params
        
        CreatePool();

        musicSource.volume = _musicVolume;
        _transform = transform;
        _originalMusicVolume = _musicVolume;
    }

    private void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource source = Instantiate(
                sfxSourcePrefab,
                _transform
            );

            source.playOnAwake = false;

            sfxPool.Add(source);
        }
    }

    private AudioSource GetFreeSource()
    {
        for (int i = 0; i < sfxPool.Count; i++)
        {
            if (!sfxPool[i].isPlaying)
            {
                return sfxPool[i];
            }
        }

        // expand pool dynamically

        AudioSource newSource = Instantiate(sfxSourcePrefab, _transform);
        sfxPool.Add(newSource);
        return newSource;
    }

    // =========================
    // 2D SOUND
    // =========================
    //public void PlaySFX(SfxSoundType soundType, float volume = -1f)
    public void PlaySFX(SfxSoundType soundType)
    {
        //if (!sfxSoundDictionary.TryGetValue(soundType, out AudioClip clip)) return;
        if (!sfxSoundDictionary.TryGetValue(soundType, out SfxSoundEntry soundEntry)) return;

        AudioSource source = GetFreeSource();

        source.spatialBlend = 0f; // 2D

        //if (volume <= 0f) volume = _sfxVolume;
        float volume = soundEntry.volume;

        if (volume <= 0f) volume = _sfxVolume;

        //source.PlayOneShot(clip, volume);
        source.PlayOneShot(soundEntry.clip, volume);
    }

    // =========================
    // 3D SOUND
    // =========================
    public void PlaySFX(SfxSoundType soundType, Vector3 position)
    {
        //if (!sfxSoundDictionary.TryGetValue(soundType, out AudioClip clip)) return;
        if (!sfxSoundDictionary.TryGetValue(soundType, out SfxSoundEntry soundEntry)) return;

        AudioSource source = GetFreeSource();

        source.transform.position = position;
        source.spatialBlend = 1f; // 3D

        //if (volume <= 0f) volume = _sfxVolume;
        float volume = soundEntry.volume;

        if (volume <= 0f) volume = _sfxVolume;

        //source.PlayOneShot(clip, volume);
        source.PlayOneShot(soundEntry.clip, volume);
    }

    // =========================
    // MUSIC
    // =========================
    public void PlayMusic(BgmSoundType soundType)
    {
        if (!bgmSoundDictionary.TryGetValue(soundType, out AudioClip clip)) return;

        if (musicSource.clip == clip)
            return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if(musicSource == null)
        {
            Debug.LogError("Music Source is missing");
            return;
        }

        musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource == null)
        {
            Debug.LogError("Music Source is missing");
            return;
        }

        if(volume < 0)
        {
            musicSource.volume = _originalMusicVolume;
            return;
        }

        musicSource.volume = volume;
    }
}
