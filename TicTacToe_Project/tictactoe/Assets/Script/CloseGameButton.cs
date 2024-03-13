using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseGameButton : MonoBehaviour
{
    public void OnCloseGmeButtonClick() {
        //关闭游戏程序
        Application.Quit();
    }
}
