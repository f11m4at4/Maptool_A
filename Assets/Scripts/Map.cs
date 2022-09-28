using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��ġ �������� ����
[Serializable]
public class CreatedInfo
{
    //������� ���ӿ�����Ʈ
    public GameObject go;
    //���õ� ������Ʈ�� idx
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

    //�ٴ� Prefab
    public GameObject floorFactory;
    //BlueCube Prefab
    public GameObject blueCubeFactory;

    //GameObject ���� �� �ִ� �迭
    public GameObject[] objectList;

    //���� ������ Object Index
    public int selectObjIdx;

    //������� ������Ʈ�� ��Ƴ��� ����Ʈ
    public List<CreatedInfo> createdObjects = new List<CreatedInfo>();

    private void Start()
    {
        
    }
}
