using System;
using System.Collections;
using _Scripts.GameEngine;
using _Scripts.Managers;
using Michsky.UI.Shift;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.UI
{
    public class ModalController : MonoBehaviour
    {
        [SerializeField] private ModalWindowManager newWeaponModalManager;
        [SerializeField] private WeaponUnlockedModalWindow weaponUnlockedModalWindow;
        [SerializeField] private ModalWindowManager playTutorialModalManager;
        [SerializeField] private float delayTime = 1f;
        private BlurManager _blurManager;
        private void Awake()
        {
            _blurManager = GetComponent<BlurManager>();
        }

        private void Start()
        {
            EventBus.AddListener<int>(EventTypes.WeaponUnlocked, ShowNewWeaponWindow);
            StartCoroutine(DelayedTutorialCheck());
        }

        private void OnDestroy()
        {
            EventBus.RemoveListener<int>(EventTypes.WeaponUnlocked, ShowNewWeaponWindow);
        }

        private void ShowNewWeaponWindow(int weaponId)
        {
            weaponUnlockedModalWindow.Display(weaponId);
            newWeaponModalManager.ModalWindowIn();
            _blurManager.BlurInAnim();
        }

        private void ShowPlayTutorialWindow()
        {
            playTutorialModalManager.ModalWindowIn();
            _blurManager.BlurInAnim();
        }

        private IEnumerator DelayedTutorialCheck()
        {
            yield return new WaitForSeconds(delayTime);
            if (!PlayerData.Instance.isTutorialCompleted)
            {
                // Didn't pass tutorial, pop up play tutorial modal window
                ShowPlayTutorialWindow();
            }
        }

        public void SkipTutorial()
        {
            PlayerData.Instance.CompleteTutorial();
        }
    }
}