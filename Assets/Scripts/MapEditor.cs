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

        //�Ķ��� ť�� Prefab Field
        map.blueCube = (GameObject)EditorGUILayout.ObjectField("�Ķ��� ť��", map.blueCube, typeof(GameObject), false);

        //������ �߰�����
        EditorGUILayout.Space();

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
        //Map�� ���õǾ��� ��, Scene���� �ٸ� ������Ʈ�� Ŭ���ص� ������ ���� �ʰ� �ϱ�
        int id = GUIUtility.GetControlID(FocusType.Passive);
        HandleUtility.AddDefaultControl(id);

        DrawGrid();

        CreateObject();
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

    void CreateObject()
    {
        //���� Input �̺�Ʈ �����ϴ� ģ��...?
        Event e = Event.current;
        //���콺 �����ٸ�
        if(e.type == EventType.MouseDown)
        {
            //���࿡ ���� ���콺 ��ư�� ������ �ʾҴٸ�
            if (e.button != 0) return;

            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject floor = GameObject.Find("Floor");
                GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(map.blueCube);

                int x = (int)hit.point.x;
                int z = (int)hit.point.z;

                obj.transform.position = new Vector3(x, hit.point.y, z);
                obj.transform.parent = floor.transform;
            }
        }        
    }
}
