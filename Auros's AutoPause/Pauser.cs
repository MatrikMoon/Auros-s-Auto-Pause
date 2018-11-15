using IllusionPlugin;
using System.Collections;
using System.Linq;
using UnityEngine;
using System.Media;

namespace AurosAutoPause
{
    public class Pauser : MonoBehaviour
    {
        public float threshold = 40.0f;
        public bool fpsCheckerEnabled = false;
        public float period = 0.1f;
        public bool pluginEnabled = false;

        PlayerController _playerController;
        PlayerController PlayerController
        {
            get
            {
                if (_playerController == null)
                    _playerController = Resources.FindObjectsOfTypeAll<PlayerController>().FirstOrDefault();

                return _playerController;
            }
        }

        GamePauseManager _gamePauseManager;
        GamePauseManager GamePauseManager
        {
            get
            {
                if (_gamePauseManager == null)
                    _gamePauseManager = Resources.FindObjectsOfTypeAll<GamePauseManager>().FirstOrDefault();

                return _gamePauseManager;
            }
        }

        GameplayManager _gameplayManager;
        GameplayManager GameplayManager
        {
            get
            {
                if (_gameplayManager == null)
                    _gameplayManager = Resources.FindObjectsOfTypeAll<GameplayManager>().FirstOrDefault();

                return _gameplayManager;
            }
        }

        IEnumerator WaitForStart()
        {
            System.Console.WriteLine("[AutoPause] Waiting for Managers to be available");
            yield return new WaitUntil(() => GamePauseManager != null && PlayerController != null && GameplayManager != null);
            pluginEnabled = ModPrefs.GetBool(name, "Enabled", pluginEnabled, true);
        }

        public void Start()
        {
            threshold = ModPrefs.GetFloat(name, "FPSThreshold", threshold, true);
            fpsCheckerEnabled = ModPrefs.GetBool(name, "FPSCheckerOn", fpsCheckerEnabled, true);
            period = ModPrefs.GetFloat(name, "ResponseTime", period, true);
            StartCoroutine(WaitForStart());
        }

        private float nextActionTime = 0.0f;

        //Previous Saber and FPS Values
        private static Vector3 PreviousLeftSaberHandleLocation;
        private static Vector3 PreviousRightSaberHandleLocation;
        private static float despacito;

        public void Update()
        {
            //Slowing The Repeat Thing
            if (Time.time > nextActionTime && pluginEnabled)
            {
                nextActionTime += period;

                //Finding Saber Location
                Saber SaberThatIsLeft = PlayerController.leftSaber;
                Saber SaberThatIsRight = PlayerController.rightSaber;
                Vector3 LeftSaberHandleLocation = SaberThatIsLeft.handlePos;
                Vector3 RightSaberHandleLocation = SaberThatIsRight.handlePos;


                //When the game is paused, saber position freezes. This if statement is to make sure that when the game is unpaused, it doesn't take the value which set off the tracking issue in the first place (if that makes any sense)
                if (GamePauseManager != null && GamePauseManager.pause == true)
                {
                    PreviousLeftSaberHandleLocation = RightSaberHandleLocation;
                    PreviousRightSaberHandleLocation = LeftSaberHandleLocation;
                }
                else
                {
                    //FPS CHECKER
                    float fps = 1.0f / Time.deltaTime;

                    if (fps < threshold && fps < despacito && fpsCheckerEnabled == true)
                    {
                        if (GameplayManager != null)
                        {
                            GameplayManager.Pause();
                            System.Console.WriteLine("[AutoPause] FPS Checker Has Just Been Activated");
                        }
                    }

                    //TRACKING DETECTOR
                    if (PreviousLeftSaberHandleLocation == LeftSaberHandleLocation || PreviousRightSaberHandleLocation == RightSaberHandleLocation)
                    {
                        if (GameplayManager != null)
                        {
                            GameplayManager.Pause();
                            System.Console.WriteLine("[AutoPause] Tracking Detector Has Just Been Activated");
                        }
                    }

                    //SABER FLY AWAYYYYYYYYYYYYYY
                    if (LeftSaberHandleLocation.x > 1.4 || LeftSaberHandleLocation.x < -1.4 || RightSaberHandleLocation.x > 1.4 || RightSaberHandleLocation.x < -1.4 || LeftSaberHandleLocation.z > 1.3 || LeftSaberHandleLocation.z < -1.3 || RightSaberHandleLocation.z > 1.3 || RightSaberHandleLocation.z < -1.3 || LeftSaberHandleLocation.y < -0.1f || RightSaberHandleLocation.y < -0.1f)
                    {
                        if (GameplayManager != null)
                        {
                            GameplayManager.Pause();
                            System.Console.WriteLine("[AutoPause] Saber Fly Away Has Just Been Activated");
                        }
                    }

                    //Set Saber Locations To Previous Saber Location and do FPS value thing
                    PreviousLeftSaberHandleLocation = LeftSaberHandleLocation;
                    PreviousRightSaberHandleLocation = RightSaberHandleLocation;
                    despacito = fps;
                }
            }
        }
    }
}
