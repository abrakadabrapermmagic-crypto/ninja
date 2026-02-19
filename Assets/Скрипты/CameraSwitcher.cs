using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera camera1;
    public Camera camera2;

    // Метод для переключения: выключает camera1, включает camera2
    public void SwitchToCamera2()
    {
        if (camera1 != null) camera1.enabled = false;
        if (camera2 != null) camera2.enabled = true;
    }

    // Обратное переключение (опционально)
    public void SwitchToCamera1()
    {
        if (camera2 != null) camera2.enabled = false;
        if (camera1 != null) camera1.enabled = true;
    }
}
