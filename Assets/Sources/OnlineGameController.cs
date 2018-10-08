using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlineGameController : MonoBehaviour
{
    MainGameController script;
    private PhotonView objPhotonViewControl;
    private AudioSource sound_enemy;


    public int enemyScore;

    void Start()
    {
        script = GameObject.Find("Board").GetComponent<MainGameController>();
        script.gameType = 1; // ゲームタイプを1に設定
        sound_enemy = GetComponent<AudioSource>();
        objPhotonViewControl = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable changedProperties)
    {
        // ルームプロパティからsumScoreを取得
        object value = null;
        if (changedProperties.TryGetValue("sumScore", out value))
        {
            script.sumScore = (int)value;
            //レベルを計算
            script.ComputeLevel();
        }
    }

    public void sendScore(int score)
    {
        objPhotonViewControl.RPC("setEnemyScore", PhotonTargets.Others, score);
    }

    public void sendLose()
    {
        objPhotonViewControl.RPC("youWin", PhotonTargets.Others, 0);
        Debug.Log("RPCED");
    }

    [PunRPC]
    private void setEnemyScore(int eScore)
    {
        enemyScore = eScore;
        sound_enemy.PlayOneShot(sound_enemy.clip);
    }

    [PunRPC]
    private void youWin(int data)
    {
        SceneManager.LoadScene("YouWin");
    }
}