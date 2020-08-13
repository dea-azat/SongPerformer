using BS_Utils.Utilities;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace SongPerformer
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
    /// 


    public class AudioClipController{
        AudioClip cutEffect;

        public AudioClip GetAudioClip()
        {
            return cutEffect;
        }

        public void LoadAudioClip(string filename, bool stream=true, AudioType audioType=AudioType.WAV)
        {
            Logger.log.Error("LoadAudioSource Start");

            using (WWW www = new WWW("file://" + filename))
            {
                cutEffect = www.GetAudioClip(threeD:false, stream:stream, audioType:audioType);

                Logger.log.Error("loadState = " + cutEffect.loadState);
                Logger.log.Error("loadType = " + cutEffect.loadType);
                bool flag = cutEffect.loadState != AudioDataLoadState.Loaded;

                if (flag)
                {
                    Logger.log.Error("Failed to load AudioClip.");
                    return;
                }

                Logger.log.Error("Clip Load Success");
            }
        }

        public IEnumerator LoadAudioClipWithWebRequest(string filename, UnityAction<AudioClip> callback)
        {
            AudioClip clip;

            Logger.log.Error("LoadAudioClipWithWebRequest Start");

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filename, AudioType.OGGVORBIS))
            {
                yield return www.SendWebRequest(); // must wait
                clip = DownloadHandlerAudioClip.GetContent(www);
            }

            cutEffect = clip;

            Logger.log.Error("loadState = " + clip.loadState);
            Logger.log.Error("loadType = " + clip.loadType);

            Logger.log.Error("Clip Load Success");

            callback(clip);
        }
    }

    public class SoundPlayer
    {
        protected AudioSource audioSource;
        static public float SEMITONE = Mathf.Pow(2f, 1f / 12f);

        public virtual void Init( GameObject gameObject )
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        public virtual void PlaySound()
        {

        }

        public virtual void SetEffect<Type>(Type T) //To make data container
        {

        }

        public virtual void SetPitch( float _pitch )
        {
            audioSource.pitch = _pitch;
        }
    }

    public class StarLightStagePlayer : SoundPlayer
    {
        
        private const string audioClipPath = "D:/BeatSaberMod/EffectSamples/deresuteMixed.wav";
        private AudioClipController clipController;

        public override void Init(GameObject gameObject)
        {
            base.Init(gameObject);

            audioSource.loop = false;
            audioSource.volume = 0.2f;

            clipController = new AudioClipController();
            clipController.LoadAudioClip(audioClipPath);            
        }

        public override void PlaySound()
        {
            audioSource.PlayOneShot(clipController.GetAudioClip());
        }
    }

    public class FutureBassPlayer : SoundPlayer
    {

        public enum PlayPosition
        {
            PLAY_LEFT,
            PLAY_RIGHT,
            PLAY_CENTER
        }

        private const string audioClipPath_Left = "D:/BeatSaberMod/EffectSamples/Left_Effect.wav";
        private const string audioClipPath_Right = "D:/BeatSaberMod/EffectSamples/Right_Effect.wav";
        private const string audioClipPath_Center = "D:/BeatSaberMod/EffectSamples/Center_Effect.wav";
        private AudioClipController clipController_Left, clipController_Right, clipController_Center;

        private AudioClip audioClip;

        public override void Init(GameObject gameObject)
        {
            base.Init(gameObject);

            audioSource.loop = false;
            audioSource.volume = 0.4f;

            clipController_Left = new AudioClipController();
            clipController_Left.LoadAudioClip(audioClipPath_Left);
            clipController_Right = new AudioClipController();
            clipController_Right.LoadAudioClip(audioClipPath_Right);
            clipController_Center = new AudioClipController();
            clipController_Center.LoadAudioClip(audioClipPath_Center);
        }

        public override void PlaySound()
        {
            audioSource.PlayOneShot(audioClip);
        }

        public override void SetEffect<Type>(Type T)
        {
            if (typeof(Type) != typeof(PlayPosition)) return;

            PlayPosition pos = (PlayPosition)(object)T;

            switch (pos)
            {
                case PlayPosition.PLAY_LEFT:
                    audioClip = clipController_Left.GetAudioClip();
                    break;

                case PlayPosition.PLAY_RIGHT:
                    audioClip = clipController_Right.GetAudioClip();
                    break;

                default:
                    audioClip = clipController_Center.GetAudioClip();
                    break;

            }
        }
    }

    public class SongPerformerController : MonoBehaviour
    {
        public static SongPerformerController instance { get; private set; }

        public static SoundPlayer player;

        #region Monobehaviour Messages
        /// <summary>
        /// Only ever called once, mainly used to initialize variables.
        /// </summary>
        private void Awake()
        {
            // For this particular MonoBehaviour, we only want one instance to exist at any time, so store a reference to it in a static property
            //   and destroy any that are created while one already exists.
            if (instance != null)
            {
                Logger.log?.Warn($"Instance of {this.GetType().Name} already exists, destroying.");
                GameObject.DestroyImmediate(this);
                return;
            }
            GameObject.DontDestroyOnLoad(this); // Don't destroy this object on scene changes
            instance = this;
            Logger.log?.Debug($"{name}: Awake()");

            //player = new StarLightStagePlayer();
            player = new FutureBassPlayer();

            player.Init(gameObject);

            BSEvents.noteWasCut += HandleControlelrNoteWasCut;
            BSEvents.gameSceneLoaded += AdjustPitchForCutEffect;
        }

        public static void HandleControlelrNoteWasCut(NoteData noteData, NoteCutInfo noteCutInfo, int multiplayer)
        {

            FutureBassPlayer.PlayPosition pos;

            switch (noteData.cutDirection)
            {
                case NoteCutDirection.Left:
                case NoteCutDirection.DownLeft:
                case NoteCutDirection.UpLeft:
                    pos = FutureBassPlayer.PlayPosition.PLAY_LEFT;
                    break;

                case NoteCutDirection.Right:
                case NoteCutDirection.DownRight:
                case NoteCutDirection.UpRight:
                    pos = FutureBassPlayer.PlayPosition.PLAY_RIGHT;
                    break;

                default:
                    pos = FutureBassPlayer.PlayPosition.PLAY_CENTER;
                    break;
            }

            player.SetEffect(pos);
            player.PlaySound();
        }

        public void AdjustPitchForCutEffect()
        {
            Logger.log.Error("AdjustPitchForCutEffect");

            player.SetPitch(1f); //must initialize 

            IDifficultyBeatmap diffBeatmap = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.difficultyBeatmap;
            CustomPreviewBeatmapLevel customPreviewBeatmapLevel = diffBeatmap.level as CustomPreviewBeatmapLevel;

            if (customPreviewBeatmapLevel != null)
            {
                string customLevelPath = customPreviewBeatmapLevel.customLevelPath;
                string songFileName = customPreviewBeatmapLevel.standardLevelInfoSaveData.songFilename;
                string filepath = customLevelPath + "\\" + songFileName;
                Logger.log.Error("custom level path = " + filepath);

                AudioClipController customSong = new AudioClipController();
                StartCoroutine(customSong.LoadAudioClipWithWebRequest(filepath, OnFinishLoadAudioClipWebRequest));
            }
        }

        public void OnFinishLoadAudioClipWebRequest( AudioClip audioClip )
        {
            AdjustPitch(audioClip);
        }

        void AdjustPitch(AudioClip clip)
        {
            int key = KeyFinder.KeyFind(clip);
            Logger.log.Error("the key of song is " + key);

            int sampleKey = 21; //GMinor

            int minMajDiff = (sampleKey % 2 == 0) ? -3 : 3;
            minMajDiff = ((key % 2) == (sampleKey % 2)) ? 0 : minMajDiff;

            int diff = ((int)(key - sampleKey) / 2 + minMajDiff + 24) % 12;

            float pitch = 1f;

            if (diff <= 7)
            {
                for (int i = 0; i < diff; i++)
                {
                    pitch *= SoundPlayer.SEMITONE;
                }
            }
            else
            {
                for (int i = 0; i < 12 - diff; i++)
                {
                    pitch /= SoundPlayer.SEMITONE;
                }
            }

            player.SetPitch(pitch);

            Logger.log.Error("the key diff is " + diff); 
        }

        /// <summary>
        /// Only ever called once on the first frame the script is Enabled. Start is called after any other script's Awake() and before Update().
        /// </summary>
        private void Start()
        {

        }

        /// <summary>
        /// Called every frame if the script is enabled.
        /// </summary>
        private void Update()
        {

        }

        /// <summary>
        /// Called every frame after every other enabled script's Update().
        /// </summary>
        private void LateUpdate()
        {

        }

        /// <summary>
        /// Called when the script becomes enabled and active
        /// </summary>
        private void OnEnable()
        {

        }

        /// <summary>
        /// Called when the script becomes disabled or when it is being destroyed.
        /// </summary>
        private void OnDisable()
        {

        }

        /// <summary>
        /// Called when the script is being destroyed.
        /// </summary>
        private void OnDestroy()
        {
            Logger.log?.Debug($"{name}: OnDestroy()");
            instance = null; // This MonoBehaviour is being destroyed, so set the static instance property to null.

        }
        #endregion
    }
}
