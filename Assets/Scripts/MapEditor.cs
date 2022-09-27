using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

//누구를 커스텀 할것이냐?
[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    Map map;

    //map.objs 들의 이름을 담을 변수
    string[] objsName;

    //Hierarchy에서 클릭이 되었을 때 호출 되는 함수
    private void OnEnable()
    {
        map = (Map)target;

        // 오브젝트들 이름 셋팅
        objsName = new string[map.objs.Length];
        for(int i = 0; i < map.objs.Length; i++)
        {
            objsName[i] = map.objs[i].name;
        }
    }

    //Inspector를 그리는 함수
    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();

        map.tileX = EditorGUILayout.IntField("타일 가로", map.tileX);
        map.tileZ = EditorGUILayout.IntField("타일 세로", map.tileZ);

        //최소최대값을 정하자
        map.tileX = Mathf.Clamp(map.tileX, 1, 500);
        map.tileZ = Mathf.Clamp(map.tileZ, 1, 500);

        //바닥 Prefab Field
        map.floor = (GameObject)EditorGUILayout.ObjectField("바닥", map.floor, typeof(GameObject), false);

        //파란색 큐브 Prefab Field
        map.blueCube = (GameObject)EditorGUILayout.ObjectField("파란색 큐브", map.blueCube, typeof(GameObject), false);

        //map.objs Field
        EditorGUI.ChangeCheckScope check = new EditorGUI.ChangeCheckScope();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("objs"));
        if (check.changed)
        {
            serializedObject.ApplyModifiedProperties();
        }

        //선택한 오브젝트 Idx Field
        map.selectObjIdx = EditorGUILayout.Popup("선택 오브젝트", map.selectObjIdx, objsName);

        //공간을 추가하자
        EditorGUILayout.Space();

        //바닥 생성 버튼
        if(GUILayout.Button("바닥 생성"))
        {
            CreateFloor();
        }

        //만약 Inspector의 값이 변경되었다면
        if(GUI.changed)
        {
            //별표 모양 표시(씬 이동시, 유니티를 끌 때 저장 팝업이 뜨게)
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }

    //Scene 을 그리는 함수
    private void OnSceneGUI()
    {
        //Map이 선택되었을 때, Scene에서 다른 오브젝트를 클릭해도 선택이 되지 않게 하기
        int id = GUIUtility.GetControlID(FocusType.Passive);
        HandleUtility.AddDefaultControl(id);

        DrawGrid();

        CreateObject();

        DeleteObject();
    }

    void DeleteObject()
    {
        Event e = Event.current;

        //마우스 왼쪽 버튼을 누르면 & 컨트롤 키를 누르고 있으면
        if(e.type == EventType.MouseDown && e.button == 0 && e.control)
        {
            //마우스 포인터에서 Ray를 만들고
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;
            //만든 Ray를 발사해서 부딪힌 놈이 있다면
            if(Physics.Raycast(ray, out hit))
            {
                //부딪힌 놈의 Layer가 Object라면
                if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Object"))
                {
                    //해당 오브젝트를 파괴하겠다
                    DestroyImmediate(hit.transform.gameObject);
                }
            }   
        }
    }

    void DrawGrid()
    {
        Vector3 start;
        Vector3 end;
        //세로 줄 그리자
        Handles.color = Color.red;
        for (int i = 0; i <= map.tileX; i++)
        {
            start = new Vector3(i, 0, 0);
            end = new Vector3(i, 0, map.tileZ);
            Handles.DrawLine(start, end);
        }
        //가로 줄 그리자
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
        //만약에 기존 바닥이 있었다면 지우자
        if (floor != null)
        {
            DestroyImmediate(floor);
        }

        //기본 바닥 생성
        floor = (GameObject)PrefabUtility.InstantiatePrefab(map.floor);
        //tileX, tileY만큼 크기를 키운다
        floor.transform.localScale = new Vector3(map.tileX, 1, map.tileZ);
    }

    void CreateObject()
    {
        //현재 Input 이벤트 관리하는 친구...?
        Event e = Event.current;
        //마우스 눌렀다면
        if(e.type == EventType.MouseDown && e.button == 0 && !e.control)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject floor = GameObject.Find("Floor");
                GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(
                    map.objs[map.selectObjIdx]);

                int x = (int)hit.point.x;
                int z = (int)hit.point.z;

                obj.transform.position = new Vector3(x, hit.point.y, z);
                obj.transform.parent = floor.transform;
            }
        }        
    }
}
