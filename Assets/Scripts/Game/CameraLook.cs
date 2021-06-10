using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineFreeLook))]

public class CameraLook : MonoBehaviour
{

    public VirtualJoystick[] cameraJoystick;


    private GameManager gameManager;
    private CinemachineFreeLook cinemachine;
    public VirtualJoystick cJoystick;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        cinemachine = GetComponent<CinemachineFreeLook>();
    }

    private void FixedUpdate()
    {
        if (gameManager.analog)
        {
            if (cameraJoystick[1].gameObject.activeSelf == false)
            {
                cameraJoystick[1].gameObject.SetActive(true);
                cameraJoystick[0].gameObject.SetActive(false);
            }
            cJoystick = cameraJoystick[1];
        }
        else
        {
            if (cameraJoystick[0].gameObject.activeSelf == false)
            {
                cameraJoystick[0].gameObject.SetActive(true);
                cameraJoystick[1].gameObject.SetActive(false);
            }
            cJoystick = cameraJoystick[0];
        }

        cinemachine.m_XAxis.Value += cJoystick.Direction.x * 50 * 1 * Time.deltaTime;
        cinemachine.m_YAxis.Value += cJoystick.Direction.z / 10 * Time.deltaTime;

    }


}
