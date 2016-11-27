using UnityEngine;
using System.Collections;

public class csSceneChange : MonoBehaviour
{
    public bool KeyBoard = true;
    public bool Mouse = false;
    public static string st = "KeyBoard Type";

    void Start()
    {
        if (KeyBoard == true)
            st = "Change Mouse Type";

        if (Mouse == true)
            st = "Change KeyBoard Type";
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 250, 200, 35), st))
        {
            if (KeyBoard == true)
            {
                Application.LoadLevel("MouseControl");
            }

            if (Mouse == true)
            {
                Application.LoadLevel("KeyBoardControl");
            }
        }
    }
}
