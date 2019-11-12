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
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}, they will be player 1", SceneManagerHelper.ActiveSceneName);
                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    GameObject player1GO = PhotonNetwork.Instantiate(this.PlayerPrefab.name, Player1TempPos.position, Player1TempPos.rotation);
                    player1GO.name = "Player1";
                    PlayerTileEntity pte = player1GO.GetComponent<PlayerTileEntity>();
                    pte.SetBoard(MyGameBoard);
                    localPlayer = pte;
                    pte.setUid(Guid.NewGuid().ToString()); //random int guid
                    if (pte != null)
                    {
                        this.MyGameBoard.AddEntityToTile(1, 1, pte);
                    }

                }
                else
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}, they will be player 2", SceneManagerHelper.ActiveSceneName);
                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    GameObject player2GO = PhotonNetwork.Instantiate(this.PlayerPrefab.name, Player2TempPos.position, Player2TempPos.rotation);
                    player2GO.name = "Player2";
                    PlayerTileEntity pte = player2GO.GetComponent<PlayerTileEntity>();
                    pte.SetBoard(MyGameBoard);
                    localPlayer = pte;
                    pte.setUid(Guid.NewGuid().ToString()); //random int guid
                    if (pte != null)
                    {
                        this.MyGameBoard.AddEntityToTile(1, 4, pte);
                    }
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