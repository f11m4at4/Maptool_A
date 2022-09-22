using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

//������ Ŀ���� �Ұ��̳�?
[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    Map map;
    //Hierarchy���� Ŭ���� �Ǿ��� �� ȣ�� �Ǵ� �Լ�
    private void OnEnable()
    {
        map = (Map)target;
    }

    //Inspector�� �׸��� �Լ�
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        map.tileX = EditorGUILayout.IntField("Ÿ�� ����", map.tileX);
        map.tileZ = EditorGUILayout.IntField("Ÿ�� ����", map.tileZ);

        //�ּ��ִ밪�� ������
        map.tileX = Mathf.Clamp(map.tileX, 1, 500);
        map.tileZ = Mathf.Clamp(map.tileZ, 1, 500);

        //�ٴ� Prefab Field
        map.floor = (GameObject)EditorGUILayout.ObjectField("�ٴ�", map.floor, typeof(GameObject), false);

        //�ٴ� ���� ��ư
        if(GUILayout.Button("�ٴ� ����"))
        {
            CreateFloor();
        }

        //���� Inspector�� ���� ����Ǿ��ٸ�
        if(GUI.changed)
        {
            //��ǥ ��� ǥ��(�� �̵���, ����Ƽ�� �� �� ���� �˾��� �߰�)
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }

    //Scene �� �׸��� �Լ�
    private void OnSceneGUI()
    {
        DrawGrid();
    }

    void DrawGrid()
    {
        Vector3 start;
        Vector3 end;
        //���� �� �׸���
        Handles.color = Color.red;
        for (int i = 0; i <= map.tileX; i++)
        {
            start = new Vector3(i, 0, 0);
            end = new Vector3(i, 0, map.tileZ);
            Handles.DrawLine(start, end);
        }
        //���� �� �׸���
        Handles.color = Color.blue;
        for (int i = 0; i <= map.tileZ; i++)
        {
            start = new Vector3(0, 0, i);
            end = new Vector3(map.tileX, 0, i);
            Handles.DrawLine(start, end);
        }
    }

    
    void CreateFloor()
    {
        GameObject floor = GameObject.Find("Floor");
        //���࿡ ���� �ٴ��� �־��ٸ� ������
        if (floor != null)
        {
            DestroyImmediate(floor);
        }

        //�⺻ �ٴ� ����
        floor = (GameObject)PrefabUtility.InstantiatePrefab(map.floor);
        //tileX, tileY��ŭ ũ�⸦ Ű���
        floor.transform.localScale = new Vector3(map.tileX, 1, map.tileZ);
    }
}
