using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaveLoadTutorial {

    public class Main : MonoBehaviour {

        [SerializeField] Panel_Login p_login;

        GameContext ctx;

        void Awake() {

            p_login.Ctor();

            p_login.OnNewGameHandle = () => {
                GameBusiness.NewGame();
                p_login.Hide();
            };

            p_login.OnLoadHandle = () => {
                GameBusiness.LoadGame();
                p_login.Hide();
            };

            ctx = new GameContext();

        }

        void Update() {
            if (Input.GetKeyUp(KeyCode.O)) {
                GameBusiness.SaveGame();
            }
        }

    }
}
