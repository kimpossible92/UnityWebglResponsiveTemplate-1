using System.Collections;
using UnityEngine;

namespace Assets.script
{
    public class MenuGames : MonoBehaviour
    {
        public void setGame1()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("mainGame");
        }
        public void setGame2()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("GamePlayScene");
        }
        public void setGame3()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Demo");
        }
        public void setGame4()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("inGame");
        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}