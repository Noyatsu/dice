using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace SSTraveler.Game
{
    public class InitController : MonoBehaviour
    {
        [FormerlySerializedAs("gobjTextbox")] [SerializeField]
        private GameObject _gobjTextbox;

        public void SetUsername()
        {
            if (_gobjTextbox.GetComponent<InputField>().text != "")
            {
                PlayerPrefs.SetString("userName", _gobjTextbox.GetComponent<InputField>().text);
                Debug.Log("Player情報を格納しました！");
                SceneManager.LoadScene("Tutorial");
            }
        }
    }
}