
using UnityEngine;
using System.Collections;

public class mousePointer : MonoBehaviour
{
    public Texture2D cursorImage; //Текстура курсора

	
	//Размеры курсора
    private int cursorWidth = 32;
    private int cursorHeight = 32;

    void Start()
    {
        Screen.showCursor = false;
    }


    void OnGUI()
    {
        GUI.DrawTexture(new Rect(Input.mousePosition.x - cursorWidth/2, Screen.height - Input.mousePosition.y - cursorHeight/2, cursorWidth, cursorHeight), cursorImage);
    }
}