using System;
using System.Collections;


using UnityEngine;
using UnityEngine.SceneManagement;


using Photon.Pun;
using Photon.Realtime;

namespace MegaGame
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public static GameManager Instance;

        [Tooltip("The prefab to use for representing the player")]
        public GameObject PlayerPrefab;
        public GameBoard MyGameBoard;

        public Transform Player1TempPos;
        public Transform Player2TempPos;

        private PlayerTileEntity localPlayer;

        ///Called by Unity when the application is closed.Tries to disconnect.
        protected void OnApplicationQuit() { PhotonNetwork.Disconnect(); }

        void Start()
        {
            Instance = this;

            if (PlayerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
                {
                    int initialRow = MyGameBoard.GameBoardHeight / 2 + (MyGameBoard.GameBoardHeight % 2 - 1);
                    int initialWidth = MyGameBoard.GameBoardWidth / 4 + ((MyGameBoard.GameBoardWidth / 2) % 2 - 1);
                    SetupLocalPlayer("Player1", initialRow, initialWidth);
                }
                else
                {
                    int initialRow = MyGameBoard.GameBoardHeight / 2 + (MyGameBoard.GameBoardHeight % 2 - 1);
                    int initialWidth = MyGameBoard.GameBoardWidth - 1 - (MyGameBoard.GameBoardWidth / 4 + ((MyGameBoard.GameBoardWidth / 2) % 2 - 1));
                    SetupLocalPlayer("Player2", initialRow, initialWidth);
                }
            }
        }

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting

            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom() {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                LoadArena();
            }
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("Player {0} left the room and ruined it for everyone else.", other.NickName); // seen when other disconnects

            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Destroy(localPlayer.gameObject);
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            base.OnMasterClientSwitched(newMasterClient);
            Debug.Log("Player left the room and ruined it for everyone else."); // seen when other disconnects

            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Destroy(localPlayer.gameObject);
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Destroy(localPlayer.gameObject);
        }

        private void SetupLocalPlayer(string name, int row, int col)
        {
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}, they will be " + name, SceneManagerHelper.ActiveSceneName);

            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            GameObject playerGO = PhotonNetwork.Instantiate(this.PlayerPrefab.name, Player1TempPos.position, Player1TempPos.rotation);
            playerGO.name = name;
            PlayerTileEntity pte = playerGO.GetComponent<PlayerTileEntity>();
            pte.SetBoard(MyGameBoard);
            localPlayer = pte;
            pte.SetUid(Guid.NewGuid().ToString()); //random int guid

            //init player tile
            if (pte != null)
            {
                this.MyGameBoard.AddEntityToTile(row, col, pte);
            }
        }

        private void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to load a level but we are not the master Client");
            }
            Debug.Log("PhotonNetwork : Loading level RoomFor2");

            //only ever call this if you are the master client
            PhotonNetwork.LoadLevel("RoomFor2");
        }
    }
}