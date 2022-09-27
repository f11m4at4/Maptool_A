using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    //맵의 가로 크기
    public int tileX;

    //맵의 세로 크기
    public int tileZ;

    //바닥 Prefab
    public GameObject floor;

    //파란색 큐브 Prefab
    public GameObject blueCube;

    //생성하고 싶은 게임오브젝트들 담을 변수
    public GameObject[] objs;
    //선택한 오브젝트 Index
    public int selectObjIdx;

    private void Start()
    {
        
    }
}
