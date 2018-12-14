using UnityEngine;
using System.Collections;
 
public class PauseScript : MonoBehaviour {
 
    //　ポーズした時に表示するUI
    [SerializeField]
    private GameObject pauseUI;
    
    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown ("q")) {
            //　ポーズUIのアクティブ、非アクティブを切り替え
            pauseUI.SetActive (!pauseUI.activeSelf);
 
            //　ポーズUIが表示されてる時は停止
            if(pauseUI.activeSelf) {
                Time.timeScale = 0f;
            //　ポーズUIが表示されてなければ通常通り進行
            } else {
                Time.timeScale = 1f;
            }
        }
    }
}