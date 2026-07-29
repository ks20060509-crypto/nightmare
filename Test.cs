using UnityEngine;
using System.Collections.Generic;

public class Test : MonoBehaviour
{
    public Transform startMaker;
    public Transform endMaker; //スタートと終わりの目印

    //スピード
    public float speed = 1.0f;

    //二点間の距離を入れる
    private float distance_two;
    
    void Start()
    {
        distance_two = Vector3.Distance(startMaker.position, endMaker.position);
    }

    
    void Update()
    {
        //ゴールに対しての現在の進捗度合いを求める
        float present_location = (Time.time * speed) / distance_two;
        //オブジェクトの移動
        transform.position = Vector3.Lerp(startMaker.position, endMaker.position, present_location);
    }
}
