﻿using UnityEngine;
using System.Collections;
using System.IO;
using Tango;

public class TranslateByTouch : MonoBehaviour
{
    public float speed = 0.01F;
    private Touch oldTouch1;  //上次触摸点1(手指1)  
    private Touch oldTouch2;  //上次触摸点2(手指2)  
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount <= 0)
        {
            return;
        }
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            // Get movement of the finger since last frame
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

            // Move object across XY plane
            transform.Translate(touchDeltaPosition.x * speed, 0, touchDeltaPosition.y * speed, Space.World);
            return;
        }
        //多点触摸, 旋转  
        Touch newTouch1 = Input.GetTouch(0);
        Touch newTouch2 = Input.GetTouch(1);
        //第2点刚开始接触屏幕, 只记录，不做处理  
        if (newTouch2.phase == TouchPhase.Began)
        {
            oldTouch2 = newTouch2;
            oldTouch1 = newTouch1;
            return;
        }
        //计算老的两点距离和新的两点间距离，变大要放大模型，变小要缩放模型  
        float oldDistance = Vector2.Distance(oldTouch1.position, oldTouch2.position);
        float newDistance = Vector2.Distance(newTouch1.position, newTouch2.position);


        //两个距离之差，为正表示放大手势， 为负表示缩小手势  
        float offset = newDistance - oldDistance;
        transform.Rotate(Vector3.up * offset * 0.5f, Space.World);

        //记住最新的触摸点，下次使用  
        oldTouch1 = newTouch1;
        oldTouch2 = newTouch2;

    }
}
