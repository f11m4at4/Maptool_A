using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//설치 했을때의 정보
[Serializable]
public class CreatedInfo
{
    //만들어진 게임오브젝트
    public GameObject go;
    //선택된 오브젝트의 idx
    public int idx;
}

[Serializable]
public class SaveJsonInfo
{
    public int idx;
    public Vector3 position;
    public Vector3 eulerAngle;
    public Vector3 localScale;
}

[Serializable]
public class ArrayJson
{
    public List<SaveJsonInfo> datas;
}


public class Map : MonoBehaviour
{
    public int tileX;
    public int tileZ;

    //바닥 Prefab
    public GameObject floorFactory;
    //BlueCube Prefab
    public GameObject blueCubeFactory;

    //GameObject 담을 수 있는 배열
    public GameObject[] objectList;

    //현재 선택한 Object Index
    public int selectObjIdx;

    //만들어진 오브젝트들 담아놓을 리스트
    public List<CreatedInfo> createdObjects = new List<CreatedInfo>();

    private void Start()
    {
        
    }
}
