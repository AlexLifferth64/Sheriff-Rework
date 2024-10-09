using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    private GameObject player;
    private Transform mousePos;
    public float yPos;
    public bool mouseControl = false;

    [SerializeField] private float mouseCamInitTime = 0.5f;
    [SerializeField] private float mouseCamTime = 0.5f;


    [SerializeField] private float lerpNum = 0.15f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        mousePos = GameObject.Find("Player Relative Mouse").transform;

        // https://discussions.unity.com/t/pixel-perfect-plus-perspective-camera/521570/6
        //var pixelMult = 8;

        /*var camera = GetComponent<Camera>();
        var camFrustWidthShouldBe = Screen.height / 100f;
        var frustrumInnerAngles = (180f - camera.fieldOfView) / 2f * Mathf.PI / 180f;
        var newCamDist = Mathf.Tan(frustrumInnerAngles) * (camFrustWidthShouldBe / 2);*/
        //transform.position = new Vector3(0, 0, -10);//-newCamDist / pixelMult);

        Application.targetFrameRate = 60;
    }
    // Camera is 40 units wide and 22.5 units tall

    void FixedUpdate()
    {
        //transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);

        transform.position = Vector3.Lerp(transform.position, new Vector3(mousePos.position.x/*player.transform.position.x*/, yPos, -15), lerpNum);
        transform.position = new Vector3(player.transform.position.x + mousePos.localPosition.x / 3, transform.position.y, -15);

        if (mousePos.position.y - (yPos - 11.25f) < 0)
        {
            if (mouseCamTime < 0)
            {
                yPos = mousePos.position.y + 11.25f;
                mouseControl = true;
            }
            else
                mouseCamTime -= Time.deltaTime;
            //Debug.Log(mousePos.position.y - yPos - 11.25f + "");
        }
        else if (mousePos.position.y - (yPos + 11.25f) > 0)
        {
            if (mouseCamTime < 0)
            {
                yPos = mousePos.position.y - 11.25f;
                mouseControl = true;
            }
            else
                mouseCamTime -= Time.deltaTime;

            //Debug.Log(mousePos.position.y - (yPos + 11.25f) + "");
        }
        else if (mouseCamTime < mouseCamInitTime)
            mouseCamTime += Time.deltaTime;
        else
            mouseCamTime = mouseCamInitTime;


        if (Input.GetKey(KeyCode.L))
        {
            mouseCamTime = mouseCamInitTime;
            mouseControl = false;
        }
        /*f(player.GetComponent<SpriteRenderer>().flipX)
            transform.position = Vector3.Lerp(transform.position, new Vector3(player.transform.position.x - 7, transform.position.y, -15), lerpNum);
        else
            transform.position = Vector3.Lerp(transform.position, new Vector3(player.transform.position.x + 7, transform.position.y, -15), lerpNum);
*/



    }
}
