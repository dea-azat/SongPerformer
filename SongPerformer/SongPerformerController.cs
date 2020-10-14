using BS_Utils.Utilities;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SongPerformer
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
    /// 

    public class StarLightStagePlayer : SoundPlayer
    {
        
        private const string audioClipPath = "D:/BeatSaberMod/EffectSamples/deresuteMixed.wav";
        private AudioClipController clipController;

        public override async void Init(GameObject gameObject)
        {
            base.Init(gameObject);

            audioSource.loop = false;
            audioSource.volume = 0.2f;

            clipController = new AudioClipController();
            await clipController.LoadAudioClipWithWebRequest(audioClipPath, AudioType.WAV);            
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
            PLAY_UP,
            PLAY_DOWN,
            PLAY_NONE
        }

        private const string audioClipPath_Left = "D:/BeatSaberMod/EffectSamples/Left_Effect.wav";
        private const string audioClipPath_Right = "D:/BeatSaberMod/EffectSamples/Right_Effect.wav";
        private const string audioClipPath_Up = "D:/BeatSaberMod/EffectSamples/Up_Effect.wav"; 
        private const string audioClipPath_Down = "D:/BeatSaberMod/EffectSamples/Down_Effect.wav";
        private AudioClipController clipController_Left, clipController_Right, clipController_Down, clipController_Up;

        private AudioClip audioClip;

        public override void Init(GameObject gameObject)
        {
            base.Init(gameObject);

            audioSource.loop = false;
            audioSource.volume = 0.5f;

            /*
            clipController_Left = new AudioClipController();
            clipController_Left.LoadAudioClip(audioClipPath_Left);
            clipController_Right = new AudioClipController();
            clipController_Right.LoadAudioClip(audioClipPath_Right);
            clipController_Up = new AudioClipController();
            clipController_Up.LoadAudioClip(audioClipPath_Up);
            clipController_Down = new AudioClipController();
            clipController_Down.LoadAudioClip(audioClipPath_Down);
            */
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
                    SetAudioClip(clipController_Left);
                    break;

                case PlayPosition.PLAY_RIGHT:
                    SetAudioClip(clipController_Right);
                    break;

                case PlayPosition.PLAY_DOWN:
                    SetAudioClip(clipController_Down);
                    break;

                case PlayPosition.PLAY_UP:
                    SetAudioClip(clipController_Up);
                    break;

                default:
                    break;

            }
        }
    }

    public class WontSurvivePlayer : SoundPlayer
    {

        public enum PlayPosition
        {
            PLAY_LEFT_LEFT,
            PLAY_LEFT_RIGHT,
            PLAY_LEFT_DOWN,
            PLAY_LEFT_UP,
            PLAY_RIGHT_LEFT,
            PLAY_RIGHT_RIGHT,
            PLAY_RIGHT_DOWN,
            PLAY_RIGHT_UP,
            PLAY_NONE
        }

        private const string audioClipPath_Root = "D:/BeatSaberMod/EffectSamples/WontSurvive/WontSurvive_";

        private const string audioClipPath_LeftLeft = audioClipPath_Root + "LeftLeft.wav";
        private const string audioClipPath_LeftRight = audioClipPath_Root + "LeftRight.wav";
        private const string audioClipPath_RightRight = audioClipPath_Root + "RightRight.wav";
        private const string audioClipPath_RightLeft = audioClipPath_Root + "RightLeft.wav";
        private const string audioClipPath_RightDown = audioClipPath_Root + "RightDown.wav";
        private const string audioClipPath_RightUp = audioClipPath_Root + "RightUp.wav";
        private const string audioClipPath_LeftDown = audioClipPath_Root + "LeftDown.wav";
        private const string audioClipPath_LeftUp = audioClipPath_Root + "LeftUp.wav";

        private AudioClipController clipController_LeftLeft, clipController_LeftRight,
            clipController_RightRight, clipController_RightLeft,
            clipController_RightDown, clipController_RightUp,
            clipController_LeftDown, clipController_LeftUp;

        private AudioClip audioClip;

        public override void Init(GameObject gameObject)
        {
            base.Init(gameObject);

            audioSource.loop = false;
            audioSource.volume = 1f;

            /*
            clipController_LeftLeft = new AudioClipController();
            clipController_LeftLeft.LoadAudioClip(audioClipPath_LeftLeft);

            clipController_LeftRight = new AudioClipController();
            clipController_LeftRight.LoadAudioClip(audioClipPath_LeftRight);

            clipController_RightRight = new AudioClipController();
            clipController_RightRight.LoadAudioClip(audioClipPath_RightRight);

            clipController_RightLeft = new AudioClipController();
            clipController_RightLeft.LoadAudioClip(audioClipPath_RightLeft);

            clipController_RightDown = new AudioClipController();
            clipController_RightDown.LoadAudioClip(audioClipPath_RightDown);

            clipController_RightUp = new AudioClipController();
            clipController_RightUp.LoadAudioClip(audioClipPath_RightUp);

            clipController_LeftDown = new AudioClipController();
            clipController_LeftDown.LoadAudioClip(audioClipPath_LeftDown);

            clipController_LeftUp = new AudioClipController();
            clipController_LeftUp.LoadAudioClip(audioClipPath_LeftUp);
            */
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
                case PlayPosition.PLAY_LEFT_LEFT:
                    SetAudioClip(clipController_LeftLeft);
                    break;

                case PlayPosition.PLAY_LEFT_RIGHT:
                    SetAudioClip(clipController_LeftRight);
                    break;

                case PlayPosition.PLAY_LEFT_DOWN:
                    SetAudioClip(clipController_LeftDown);
                    break;

                case PlayPosition.PLAY_LEFT_UP:
                    SetAudioClip(clipController_LeftUp);
                    break;

                case PlayPosition.PLAY_RIGHT_RIGHT:
                    SetAudioClip(clipController_RightRight);
                    break;

                case PlayPosition.PLAY_RIGHT_LEFT:
                    SetAudioClip(clipController_RightLeft);
                    break;

                case PlayPosition.PLAY_RIGHT_DOWN:
                    SetAudioClip(clipController_RightDown);
                    break;

                case PlayPosition.PLAY_RIGHT_UP:
                    SetAudioClip(clipController_RightUp);
                    break;

                default:
                    SetAudioClip(clipController_RightDown);
                    break;

            }
        }
    }

    public class SongPerformerController : MonoBehaviour
    {
        public static SongPerformerController instance { get; private set; }

        public static CutEffectPlayer player;

        #region Monobehaviour Messages
        /// <summary>
        /// Only ever called once, mainly used to initialize variables.
        /// </summary>
        private async void Awake()
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


            Logger.Write("Success Load Effect Map");

            BSEvents.noteWasCut += HandleControlelrNoteWasCut;
            BSEvents.noteWasMissed += HandleControlelrNoteWasMissed;
            BSEvents.gameSceneLoaded += LoadScene;
        }

        public static void HandleControlelrNoteWasCut(NoteData noteData, NoteCutInfo noteCutInfo, int multiplayer)
        {

            if (player == null) return;

            NoteInfo noteInfo = new NoteInfo(noteData.time, (int)noteData.noteType, (int)noteData.cutDirection);
            noteInfo.ConvertTime2Measure(180f);
            

            AudioTimeSyncController audioTimeSync = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().FirstOrDefault();

            float diffTime = 0;

            if (audioTimeSync != null)
            {
                diffTime = noteData.time - audioTimeSync.songTime;
            }

            if (diffTime > 0)
            {
                player.PlayScheduled(noteInfo, diffTime);
            }
            else
            {
                player.Play(noteInfo);
            }

            Logger.Write("NoteCut");
            Logger.Write(diffTime.ToString());
            Logger.Write(noteData.time.ToString());
            Logger.Write(noteInfo.time.ToString());
        }

        public static void HandleControlelrNoteWasMissed(NoteData noteData, int multiplayer)
        {
            NoteInfo noteInfo = new NoteInfo(noteData.time, (int)noteData.noteType, (int)noteData.cutDirection);
            noteInfo.ConvertTime2Measure(180f);

            Logger.Write("NoteMissed");
            Logger.Write(noteData.time.ToString());
            Logger.Write(noteInfo.time.ToString());
        }

        public static void FutureBassPlayerNoteWasCut(NoteData noteData, SoundPlayer player)
        {
            FutureBassPlayer.PlayPosition pos = FutureBassPlayer.PlayPosition.PLAY_NONE;

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

                case NoteCutDirection.Down:
                    pos = FutureBassPlayer.PlayPosition.PLAY_DOWN;
                    break;

                case NoteCutDirection.Up:
                    pos = FutureBassPlayer.PlayPosition.PLAY_UP;
                    break;

                default:
                    break;
            }

            player.SetEffect(pos);
            player.PlaySound();
        }

        public static void WontSurvivePlayerNoteWasCut(NoteData noteData, SoundPlayer player)
        {
            WontSurvivePlayer.PlayPosition pos = WontSurvivePlayer.PlayPosition.PLAY_NONE;

            switch (noteData.noteType)
            {
                case NoteType.NoteA:
                    switch (noteData.cutDirection)
                    {
                        case NoteCutDirection.Left:
                        case NoteCutDirection.DownLeft:
                        case NoteCutDirection.UpLeft:
                            pos = WontSurvivePlayer.PlayPosition.PLAY_LEFT_LEFT;
                            break;

                        case NoteCutDirection.Right:
                        case NoteCutDirection.DownRight:
                        case NoteCutDirection.UpRight:
                            pos = WontSurvivePlayer.PlayPosition.PLAY_LEFT_RIGHT;
                            break;

                        case NoteCutDirection.Down:
                            pos = WontSurvivePlayer.PlayPosition.PLAY_LEFT_DOWN;
                            break;

                        case NoteCutDirection.Up:
                            pos = WontSurvivePlayer.PlayPosition.PLAY_LEFT_UP;
                            break;

                        default:
                            pos = WontSurvivePlayer.PlayPosition.PLAY_LEFT_DOWN;
                            break;
                    }
                    break;

                case NoteType.NoteB:
                    switch (noteData.cutDirection)
                    {
                        case NoteCutDirection.Left:
                        case NoteCutDirection.DownLeft:
                        case NoteCutDirection.UpLeft:
                            pos = WontSurvivePlayer.PlayPosition.PLAY_RIGHT_LEFT;
                            break;

                        case NoteCutDirection.Right:
                        case NoteCutDirection.DownRight:
                        case NoteCutDirection.UpRight:
                            pos = WontSurvivePlayer.PlayPosition.PLAY_RIGHT_RIGHT;
                            break;

                        case NoteCutDirection.Down:
                            pos = WontSurvivePlayer.PlayPosition.PLAY_RIGHT_DOWN;
                            break;

                        case NoteCutDirection.Up:
                            pos = WontSurvivePlayer.PlayPosition.PLAY_RIGHT_UP;
                            break;

                        default:
                            pos = WontSurvivePlayer.PlayPosition.PLAY_RIGHT_DOWN;
                            break;
                    }
                    break;
                default:
                    break;
            }

            player.SetEffect(pos);
            player.PlaySound();
        }

        public async void AdjustPitchForCutEffect()
        {

            return;

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
                await customSong.LoadAudioClipWithWebRequest(filepath, AudioType.OGGVORBIS);
            }
        }

        public async void LoadScene()
        {
            IDifficultyBeatmap diffBeatmap = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.difficultyBeatmap;
            CustomPreviewBeatmapLevel customPreviewBeatmapLevel = diffBeatmap.level as CustomPreviewBeatmapLevel;

            if (customPreviewBeatmapLevel != null)
            {
                string customLevelPath = customPreviewBeatmapLevel.customLevelPath;
                string mapFileName = diffBeatmap.difficulty.Name();
                string filepath = customLevelPath + "\\ExpertPlusEffect.dat";
                Logger.log.Error("custom level path = " + filepath);
                Logger.log.Error("custom level difficulty = " + mapFileName);
                

                player = new CutEffectPlayer();
                player.Init(gameObject);
                if(await player.TryLoadCutEffectMap(filepath))
                {
                    return;
                }
            }

            player = null;
            //string cutEffectMapPath = "D:\\ProgramFiles\\Steam\\steamapps\\common\\Beat Saber\\Beat Saber_Data\\CustomLevels\\2a7 (Night of Nights - squeaksies)\\ExpertPlusEffect.dat";
            
        }

        public void OnFinishLoadAudioClipWebRequest( AudioClip audioClip, SoundPlayer player)
        {
            KeyFinder.AdjustPitch(audioClip, player);
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
