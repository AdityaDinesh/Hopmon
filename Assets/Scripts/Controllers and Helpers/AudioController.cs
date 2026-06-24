using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
    [SerializeField] private AssetReferenceGameObject sfxSourceRef; // <-- was: AudioSource sfxSourcePrefab
    [SerializeField] private SfxSoundEntry[] sfxSounds;
    [SerializeField] private int poolSize = 10;

    [Header("Volume Parameters")]
    [Range(0, 1)] [SerializeField] private float _musicVolume = 1f;
    [Range(0, 1)] [SerializeField] private float _sfxVolume = 0.7f;

    private Dictionary<BgmSoundType, AudioClip> bgmSoundDictionary;
    private Dictionary<SfxSoundType, SfxSoundEntry> sfxSoundDictionary;

    private List<AudioSource> sfxPool = new List<AudioSource>();
    private List<AsyncOperationHandle<GameObject>> sfxInstanceHandles = new List<AsyncOperationHandle<GameObject>>();

    private Transform _transform;
    private float _originalMusicVolume;

    public bool IsReady { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        bgmSoundDictionary = new Dictionary<BgmSoundType, AudioClip>();
        foreach (BgmSoundEntry sound in bgmSounds)
            bgmSoundDictionary[sound.soundType] = sound.clip;

        sfxSoundDictionary = new Dictionary<SfxSoundType, SfxSoundEntry>();
        foreach (SfxSoundEntry sound in sfxSounds)
            sfxSoundDictionary[sound.soundType] = sound;

        musicSource.volume = _musicVolume;
        _transform = transform;
        _originalMusicVolume = _musicVolume;

        StartCoroutine(CreatePool());
    }

    private IEnumerator CreatePool()
    {
        IsReady = false;

        if (sfxSourceRef == null || !sfxSourceRef.RuntimeKeyIsValid())
        {
            Debug.LogError("[Audio] sfxSourceRef is null or invalid. Check Inspector.");
            yield break;
        }

        for (int i = 0; i < poolSize; i++)
        {
            var handle = sfxSourceRef.InstantiateAsync(_transform);
            yield return handle;

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[Audio] Failed to instantiate SFX source at index {i}: {handle.OperationException}");
                continue;
            }

            if (handle.Result == null)
            {
                Debug.LogError($"[Audio] SFX handle result is null at index {i}");
                continue;
            }

            AudioSource source = handle.Result.GetComponent<AudioSource>();

            if (source == null)
            {
                Debug.LogError($"[Audio] No AudioSource on sfxSourcePrefab. Check the prefab.");
                Addressables.Release(handle);
                continue;
            }

            source.playOnAwake = false;
            sfxPool.Add(source);
            sfxInstanceHandles.Add(handle);

            if (i % 3 == 0)
                yield return null;
        }

        IsReady = true;
        Debug.Log("[Audio] SFX pool ready.");
    }

    private IEnumerator CreatePoolExpanded()
    {
        var handle = sfxSourceRef.InstantiateAsync(_transform);
        yield return handle;

        if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
        {
            Debug.LogError("[Audio] Failed to expand SFX pool.");
            yield break;
        }

        AudioSource source = handle.Result.GetComponent<AudioSource>();
        if (source == null) yield break;

        source.playOnAwake = false;
        sfxPool.Add(source);
        sfxInstanceHandles.Add(handle);
    }

    private AudioSource GetFreeSource()
    {
        for (int i = 0; i < sfxPool.Count; i++)
        {
            if (!sfxPool[i].isPlaying)
                return sfxPool[i];
        }

        // Expand pool dynamically — fire and forget
        StartCoroutine(CreatePoolExpanded());

        // Return last source as fallback while new one loads
        Debug.LogWarning("[Audio] SFX pool exhausted, expanding. Returning fallback source.");
        return sfxPool[sfxPool.Count - 1];
    }

    // =========================
    // 2D SOUND
    // =========================
    public void PlaySFX(SfxSoundType soundType)
    {
        if (!IsReady) return;
        if (!sfxSoundDictionary.TryGetValue(soundType, out SfxSoundEntry soundEntry)) return;

        AudioSource source = GetFreeSource();
        source.spatialBlend = 0f;

        float volume = soundEntry.volume > 0f ? soundEntry.volume : _sfxVolume;
        source.PlayOneShot(soundEntry.clip, volume);
    }

    // =========================
    // 3D SOUND
    // =========================
    public void PlaySFX(SfxSoundType soundType, Vector3 position)
    {
        if (!IsReady) return;
        if (!sfxSoundDictionary.TryGetValue(soundType, out SfxSoundEntry soundEntry)) return;

        AudioSource source = GetFreeSource();
        source.transform.position = position;
        source.spatialBlend = 1f;

        float volume = soundEntry.volume > 0f ? soundEntry.volume : _sfxVolume;
        source.PlayOneShot(soundEntry.clip, volume);
    }

    // =========================
    // MUSIC
    // =========================
    public void PlayMusic(BgmSoundType soundType)
    {
        if (!bgmSoundDictionary.TryGetValue(soundType, out AudioClip clip)) return;
        if (musicSource.clip == clip) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource == null) { Debug.LogError("Music Source is missing"); return; }
        musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource == null) { Debug.LogError("Music Source is missing"); return; }
        musicSource.volume = volume < 0 ? _originalMusicVolume : volume;
    }

    private void OnDestroy()
    {
        foreach (var handle in sfxInstanceHandles)
        {
            if (handle.IsValid() && handle.Status == AsyncOperationStatus.Succeeded)
                Addressables.Release(handle);
        }

        sfxInstanceHandles.Clear();
        sfxPool.Clear();
    }
}