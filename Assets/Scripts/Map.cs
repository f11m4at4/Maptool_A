using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    //���� ���� ũ��
    public int tileX;

    //���� ���� ũ��
    public int tileZ;

    //�ٴ� Prefab
    public GameObject floor;

    //�Ķ��� ť�� Prefab
    public GameObject blueCube;

    //�����ϰ� ���� ���ӿ�����Ʈ�� ���� ����
    public GameObject[] objs;
    //������ ������Ʈ Index
    public int selectObjIdx;

    private void Start()
    {
        
    }
}
