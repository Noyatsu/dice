using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private GameObject _bgImage;
    [FormerlySerializedAs("StageDice")] [SerializeField]
    private GameObject _stageDice;
    [FormerlySerializedAs("Up")] [SerializeField]
    private GameObject _up;
    [FormerlySerializedAs("Down")] [SerializeField]
    private GameObject _down;
    [FormerlySerializedAs("Left")] [SerializeField]
    private GameObject _left;
    [FormerlySerializedAs("Right")] [SerializeField]
    private GameObject _right;
    [FormerlySerializedAs("rankImg")] [SerializeField]
    private GameObject _rankImg;
    
    // Use this for initialization
    private void Start()
    {
        _bgImage = GameObject.Find("bgImage");

        // 初回起動時はチュートリアルへ
        if (!PlayerPrefs.HasKey("userName"))
        {
            BgmManager.Instance.Play("online");
            SceneManager.LoadScene("Init");
        }
        else
        {
            BgmManager.Instance.Play("opening");
        }

        //ランクを表示
        Texture2D texture = Resources.Load("ranks/" + GetString()) as Texture2D;
        _rankImg.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

    }

    // Update is called once per frame
    private void Update()
    {
        _bgImage.transform.Rotate(0f, 0f, Time.deltaTime * 2.0f);
    }
    

    private string GetString()
    {
        int i = PlayerPrefs.GetInt("rank", 0) / 100;

        if (i < 1) return "C-";
        else if (i < 2) return "C+";
        else if (i < 3) return "B-";
        else if (i < 4) return "B+";
        else if (i < 5) return "A-";
        else if (i < 6) return "A+";
        else if (i < 7) return "S-";
        else if (i < 8) return "S+";
        else if (i < 9) return "Master";
        else return "Grand-Master";
    }
}


 