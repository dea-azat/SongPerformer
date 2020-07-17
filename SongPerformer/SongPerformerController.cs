using BS_Utils.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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

        public void LoadAudioClip(string filename)
        {
            Logger.log.Error("LoadAudioSource Start");

            using (WWW www = new WWW("file://" + filename))
            {
                cutEffect = www.GetAudioClip(false, true);
                bool flag = cutEffect.loadState != AudioDataLoadState.Loaded;
                if (flag)
                {
                    Logger.log.Error("Failed to load AudioClip.");
                    return;
                }
                Logger.log.Error("Clip Load Success");
            }
        }
    }

    public class SoundPlayer
    {
        protected AudioSource audioSource;

        public virtual void Init( GameObject gameObject )
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        public virtual void PlaySound()
        {

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
            audioSource.volume = 0.15f;

            clipController.LoadAudioClip(audioClipPath);            
        }

        public override void PlaySound()
        {
            audioSource.PlayOneShot(clipController.GetAudioClip());
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

            player = new StarLightStagePlayer();
            player.Init(gameObject);

            BSEvents.noteWasCut += HandleControlelrNoteWasCut;
        }

        public static void HandleControlelrNoteWasCut(NoteData noteData, NoteCutInfo noteCutInfo, int multiplayer)
        {
            player.PlaySound();
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
