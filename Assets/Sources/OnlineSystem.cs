using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OnlineSystem : MonoBehaviour
{

    int _mode = 0; //1ならフリーマッチ
    string _version = "v0.5", _key = "";
    [FormerlySerializedAs("objLoading")] public GameObject ObjLoading;
    [FormerlySerializedAs("objkeyBox")] public GameObject ObjkeyBox;
    [FormerlySerializedAs("objKey")] public GameObject ObjKey;

    void Start()
    {

    }

    public void StartFreeMatch()
    {
        ObjLoading.SetActive(true);
        _mode = 1;
        PhotonNetwork.ConnectUsingSettings(_version);
    }

    public void DisplayKeyWindow()
    {
        ObjkeyBox.SetActive(true);
    }
    public void StartKeyMatch()
    {
        _key = ObjKey.GetComponent<InputField>().text;
        Debug.Log(_key);
        ObjkeyBox.SetActive(false);
        ObjLoading.SetActive(true);
        _mode = 3;
        PhotonNetwork.ConnectUsingSettings(_version);
    }

    // ロビーに入ると呼ばれる
    void OnJoinedLobby()
    {
        Debug.Log("ロビーに入りました。");

        if (_mode == 1)
        {
            // ランダムにルームに入室する
            ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "mode", 1 } };
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 2);
        }
        else if (_mode == 3)
        {
            // ルームに入室する
            ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "mode", 3 }, {"key", _key} };
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 2);
        }
    }

    // ルームに入室すると呼ばれる
    void OnJoinedRoom()
    {
        Debug.Log("ルームへ入室しました。");
        Debug.Log(PhotonNetwork.room.PlayerCount);
        if (PhotonNetwork.room.PlayerCount == 2)
        {
            GameStart();
        }
    }

    // ルームの入室に失敗すると呼ばれる
    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("ルームの入室に失敗しました。");

        RoomOptions roomOptions = new RoomOptions();

        if (_mode == 1)
        {
            // ルームがないと入室に失敗するため、その時は自分で作る
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "mode", 1 } };
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "mode" };
            roomOptions.MaxPlayers = 2;
        }
        else if (_mode == 3)
        {
            // ルームがないと入室に失敗するため、その時は自分で作る
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "mode", 3 }, { "key", _key } };
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "mode", "key" };
            roomOptions.MaxPlayers = 2;
        }
        PhotonNetwork.CreateRoom(null, roomOptions, null);
    }

    // ほかのプレイヤーが入室してきた際に呼ばれる
    void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        Debug.Log(player.name + " is joined.");
        GameStart();
    }


    public void CancelButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        ObjLoading.SetActive(false);
    }

    void GameStart()
    {
        //ゲームを開始
        PhotonNetwork.room.IsOpen = false;
        FadeManager.Instance.LoadScene("OnlineGame", 0.3f);
    }
}
