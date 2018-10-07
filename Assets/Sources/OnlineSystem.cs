using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineSystem : MonoBehaviour {

    int mode = 0; //1ならフリーマッチ
    public GameObject objLoading;

    void Start()
    {

    }

    public void startFreeMatch()
    {
        objLoading.SetActive(true);
        mode = 1;
        PhotonNetwork.ConnectUsingSettings("v0.5");
    }

    // ロビーに入ると呼ばれる
    void OnJoinedLobby()
    {
        Debug.Log("ロビーに入りました。");

        if (mode == 1)
        {
            // ランダムにルームに入室する
            PhotonNetwork.JoinRandomRoom();
        }
    }

    // ルームに入室すると呼ばれる
    void OnJoinedRoom()
    {
        Debug.Log("ルームへ入室しました。");
    }

    // ルームの入室に失敗すると呼ばれる
    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("ルームの入室に失敗しました。");

        if(mode == 1)
        {
            // ルームがないと入室に失敗するため、その時は自分で作る
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.maxPlayers = 2;
            PhotonNetwork.CreateRoom(null, roomOptions, null);
        }
    }
}
