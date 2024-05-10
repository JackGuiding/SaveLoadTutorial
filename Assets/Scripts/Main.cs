using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaveLoadTutorial {

    public class Main : MonoBehaviour {

        [SerializeField] Panel_Login p_login;
        [SerializeField] RoleEntity role;

        GameContext ctx;

        void Awake() {

            ctx = new GameContext();
            ctx.role = role;

            p_login.Ctor();

            p_login.OnNewGameHandle = () => {
                GameBusiness.NewGame(ctx);
                p_login.Hide();
            };

            p_login.OnLoadHandle = () => {
                GameBusiness.LoadGame(ctx);
                p_login.Hide();
            };


        }

        void Update() {
            if (Input.GetKeyUp(KeyCode.O)) {
                GameBusiness.SaveGame(ctx);
            }
        }

    }
}
