using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Shooter.Menu
{
    public class MenuManager : MonoBehaviour
    {
        private const string gameSceneName = "Game";
        CustomNetworkManager manager;

#pragma warning disable CS0649
        [SerializeField] Button createBtn;
        [SerializeField] Button joinBtn;
        [SerializeField] Button quitBtn;
        [SerializeField] Button popUpBtn;
        [SerializeField] GameObject joinPopUp;
        [SerializeField] Button connectBtn;
        [SerializeField] InputField adressInputField;
#pragma warning restore CS0649

        private InputField.SubmitEvent submit;        

        private void Start()
        {
            createBtn.onClick.AddListener(()=> StartGameAs(ConnectionStatus.Host));
            joinBtn.onClick.AddListener(() => StartGameAs(ConnectionStatus.Client));
            quitBtn.onClick.AddListener(Quit);
            popUpBtn.onClick.AddListener(() => ShowJoinPopUp(true));
            ShowJoinPopUp(false);
            adressInputField.onEndEdit.AddListener(ConnectionSettings.SetMatchIPAdress);
            
        }
        private void OnEnable()
        {
            manager = FindObjectOfType<CustomNetworkManager>();
        }
        public void Quit()
        {
            Application.Quit();
        }

   

        public void ShowJoinPopUp(bool active)
        {
            joinPopUp.SetActive(active);
        }

        AsyncOperation async;
        public void StartGameAs(ConnectionStatus status)
        {
            if (async!=null && !async.isDone) return;
            ConnectionSettings.SetConnectionStatus(status);
            async = SceneManager.LoadSceneAsync(gameSceneName);
            
        }

    }

}
