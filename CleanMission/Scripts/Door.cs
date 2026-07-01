using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    public bool useAnimation = false;   // ← 追加
    public Animator animator;           // ← アニメーション用
    public bool isOpen = false;
    public bool isLivingDoor = false; // ← 追加
    public bool isClosetDoor1 = false;
    public bool isClosetDoor2 = false;


    // スクリプト回転用
    public float openAngle = 90f;
    public float speed = 3f;
    Quaternion closedRot;
    Quaternion openRot;

    void Start()
    {
        closedRot = transform.rotation;
        openRot = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;

        // ★ ここを追加（最重要）
        if (!useAnimation || animator == null)
        {
            StopAllCoroutines();
            StartCoroutine(RotateDoor());
            return;
        }

        // ① Living 用アニメーション
        if (isLivingDoor)
        {
            animator.Play(isOpen ? "DoorLivingOpen" : "DoorLivingClose");
            return;
        }

        // ② Closet 用アニメーション
        if (isClosetDoor1)
        {
            animator.Play(isOpen ? "DoorCloset" : "DoorClosetClose");
            return;
        }

        // ③ Closet2 用アニメーション
        if (isClosetDoor2)
        {
            animator.Play(isOpen ? "DoorCloset2" : "DoorClosetClose2");
            return;
        }

        // ④ Bath 用アニメーション（デフォルト）
        animator.Play(isOpen ? "BathDoorOpen" : "BathDoorClose");
    }

    IEnumerator RotateDoor()
    {
        Quaternion target = isOpen ? openRot : closedRot;

        while (Quaternion.Angle(transform.rotation, target) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * speed);
            yield return null;
        }

        transform.rotation = target;
    }
}