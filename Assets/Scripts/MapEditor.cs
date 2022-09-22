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
    //Hierarchy에서 클릭이 되었을 때 호출 되는 함수
    private void OnEnable()
    {
        map = (Map)target;
    }

    //Inspector를 그리는 함수
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        map.tileX = EditorGUILayout.IntField("타일 가로", map.tileX);
        map.tileZ = EditorGUILayout.IntField("타일 세로", map.tileZ);

        //최소최대값을 정하자
        map.tileX = Mathf.Clamp(map.tileX, 1, 500);
        map.tileZ = Mathf.Clamp(map.tileZ, 1, 500);

        //바닥 Prefab Field
        map.floor = (GameObject)EditorGUILayout.ObjectField("바닥", map.floor, typeof(GameObject), false);

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
        DrawGrid();
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
}
