using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRelativeMouse : MonoBehaviour
{
    public float mouseSensitivity = 100;
    public bool lockMouse = false;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if(!lockMouse)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity / 100;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity / 100;

            transform.position += new Vector3(mouseX, mouseY);
        }
    }
    public void ResetMousePos()
    {
        transform.position = transform.parent.transform.position;
    }
}
