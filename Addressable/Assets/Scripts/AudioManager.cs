using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// 全局配置。不放场景中，通过脚本创建。
public class AudioManager : MonoBehaviour
{
    static AudioManager _instance;
    public static AudioManager Get()
    {
        if (_instance == null)
            _instance = FindObjectOfType<AudioManager>();
        if (_instance == null)
        {
            var obj = new GameObject("AudioManager");
            _instance = obj.AddComponent<AudioManager>();
        }
        return _instance;
    }
    static bool created = false;

    public Dictionary<string, Sound> dic_sounds = new Dictionary<string, Sound>();

    public const string Round_1 = "round1";
    public const string GetHeart = "GetHeart";
    public const string Kill = "Kill";
    public const string MediumItem = "MediumItem";
    public const string Paradise = "dolls in pseudo paradise";
    public const string FallGround = "fall_ground";
    public const string FistHit = "FistHit";
    public const string HeavyHit = "HeavyHit";

    public float musicVolume = 1;
    public float soundVolume = 1;
    // 应用到当播放中的音乐
    public void ApplyToCurrent()
    {
        //Debug.Log($"当前有{dic_sounds.Count}个音乐");
        foreach (var item in dic_sounds)
        {
            var sound = item.Value;
            float volume = sound.isMusic ? musicVolume : soundVolume;
            Debug.Log($"给《{sound.audioName}({sound.isMusic})》设置音量：{sound.source.volume} → {volume}");
            sound.source.volume = volume;
        }
    }

    void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(gameObject);
            created = true;
            musicVolume = 1;
            soundVolume = 1;
        }
        else
        {
            DestroyImmediate(gameObject, true); //多了一个
            Debug.LogError($"多了一个{this.GetType()}");
            return;
        }
    }

    void OnApplicationQuit()
    {
        created = false;
    }

    // 播放BGM
    public void PlayMusic(string soundName, bool loop = false)
    {
        Sound sound = null;
        if (dic_sounds.TryGetValue(soundName, out sound) == false)
        {
            sound = new Sound(soundName);
            dic_sounds.Add(soundName, sound);
        }
        if (sound.source == null)
        {
            // 没有source组件，创建
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.volume = musicVolume;
            sound.source = source;
        }
        else
        {
            // 复用原来的source组件
        }
        sound.loop = loop;
        sound.isMusic = true;
        sound.TweenPlay();
        Debug.Log($"播放《{sound.audioName}({sound.isMusic})》音量：{sound.source.volume}");
    }
    // 播放音效
    public void PlaySound(string soundName, bool loop = false)
    {
        Sound sound = new Sound(soundName);
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.volume = soundVolume;
        sound.source = source;
        sound.loop = loop;
        sound.isMusic = false;
        sound.TweenPlay();
    }
    // 停止所有声音
    public void StopAll()
    {
        var array = gameObject.GetComponents<AudioSource>();
        for (int i = 0; i < array.Length; i++)
        {
            var source = array[i];
            DestroyImmediate(source);
        }
    }
}
public class Sound
{
    public Sound(string newName)
    {
        audioName = newName;
        //clip = ABManager.LoadAudioClip($"audios/{audioName}");
        //Debug.Log("Load AB Clip");
        if (clip == null)
            throw new System.Exception("Couldn't find AudioClip with name '" + audioName + "'. Are you sure the file is in a folder named 'Resources'?");
    }

    public string audioName;
    public AudioClip clip;
    public AudioSource source;
    public bool loop;
    public bool isMusic;

    public async void TweenPlay()
    {
        source.playOnAwake = false;
        source.clip = clip;
        source.loop = loop;
        source.Play();

        int msec = Mathf.CeilToInt(clip.length * 1000);
        await Task.Delay(msec);
        if (!loop)
        {
            //Debug.Log($"<color=red>播放完毕，删除：{audioName} x{msec}ms</color>");
            MonoBehaviour.DestroyImmediate(source);
            source = null;
        }
        else
        {
            Debug.Log($"<color=green>播放完毕，保留：{audioName}</color>");
        }
    }
}