using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{

    private CharacterInfo characterType;
    Vector3 rot = Vector3.zero;
    float rotSpeed = 5f;
    Animator anim;
    public VirtualJoystick inputSource;
    private Rigidbody rigid;
    [SerializeField] private float movespeed = 20f, drag = 0.5f, terminalRotationSpeed = 25.0f;
    private Transform cameraMain;

    private void Start()
    {
        cameraMain = Camera.main.transform;
        characterType = transform.GetChild(0).GetComponent<Character>().characterType;
        rigid = gameObject.GetComponent<Rigidbody>();
        anim = transform.GetChild(0).gameObject.GetComponent<Animator>();
        gameObject.transform.eulerAngles = rot;
        rigid.maxAngularVelocity = terminalRotationSpeed;
        rigid.drag = drag;
    }


    private void FixedUpdate()
    {
        Check(characterType);
        Vector3 dir = Vector3.zero;

        dir.x = Input.GetAxis("Horizontal");
        dir.z = Input.GetAxis("Vertical");

        if (dir.magnitude > 1)
            dir.Normalize();

        if (inputSource.Direction != Vector3.zero)
            dir = new Vector3(inputSource.Direction.x, 0, inputSource.Direction.z);

        Vector3 rotatedDir = cameraMain.TransformDirection(dir);
        rotatedDir = new Vector3(rotatedDir.x, 0, rotatedDir.z);
        rotatedDir = rotatedDir.normalized * dir.magnitude;

        rigid.AddForce(rotatedDir * movespeed);
        if(rigid.constraints != RigidbodyConstraints.None)
            transform.GetChild(0).transform.LookAt(transform.position + new Vector3(rotatedDir.x, 0, rotatedDir.z));
    }

    private void Check(CharacterInfo characterType)
    {

        switch (characterType)
        {
            case CharacterInfo.top:
                rigid.constraints = RigidbodyConstraints.None;
                break;
            case CharacterInfo.kutuk:
                if (inputSource.Direction.x != 0 || inputSource.Direction.z != 0)
                {
                    anim.SetBool("Running", true);
                }
                else
                {
                    anim.SetBool("Running", false);
                }
                break;
            case CharacterInfo.yarasa:
                if (inputSource.Direction.x > 0)
                    rot[1] += rotSpeed * Time.fixedDeltaTime;
                else
                    rot[1] -= rotSpeed * Time.fixedDeltaTime;

                transform.eulerAngles = rot;
                break;
            case CharacterInfo.bomba:
                if (inputSource.Direction.x != 0 || inputSource.Direction.z != 0)
                {
                    if (inputSource.Direction.x > 0)
                        rot[1] += rotSpeed * Time.fixedDeltaTime;
                    else
                        rot[1] -= rotSpeed * Time.fixedDeltaTime;

                    anim.SetBool("walk", true);
                }
                else
                {
                    anim.SetBool("walk", false);
                }
                transform.eulerAngles = rot;

                break;
            case CharacterInfo.balkabagi:
                rigid.constraints = RigidbodyConstraints.None;
                break;
            case CharacterInfo.robot:
                if (inputSource.Direction.x != 0 || inputSource.Direction.z != 0)
                {
                    if (inputSource.Direction.x > 0)
                        rot[1] += rotSpeed * Time.fixedDeltaTime;
                    else
                        rot[1] -= rotSpeed * Time.fixedDeltaTime;

                    anim.SetBool("Roll_Anim", true);
                }
                else
                {
                    anim.SetBool("Roll_Anim", false);
                }
                transform.eulerAngles = rot;
                break;
            case CharacterInfo.slime:
                rigid.constraints = RigidbodyConstraints.None;
                break;
            case CharacterInfo.tascanavar:
                rigid.constraints = RigidbodyConstraints.None;
                break;
            case CharacterInfo.kaplumbaga:
                rigid.constraints = RigidbodyConstraints.None;
                break;
            default:
                break;
        }
    }
}
