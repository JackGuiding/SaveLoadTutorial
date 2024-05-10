using System;
using UnityEngine;
using UnityEngine.UI;

namespace SaveLoadTutorial {

    public class Panel_Login : MonoBehaviour {

        [SerializeField] Button newGameBtn;
        [SerializeField] Button loadBtn;

        public Action OnNewGameHandle;
        public Action OnLoadHandle;

        public void Ctor() {
            newGameBtn.onClick.AddListener(() => {
                OnNewGameHandle.Invoke();
            });
            loadBtn.onClick.AddListener(() => {
                OnLoadHandle.Invoke();
            });
        }

        public void Hide() {
            gameObject.SetActive(false);
        }
    }

}