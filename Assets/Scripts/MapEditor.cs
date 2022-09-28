using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

//������ Custom�ҰŴ�?
[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    //target�� ���� ����
    Map map;
    //map.objectList�� ����ִ� �ֵ��� �̸�
    string[] objectListName;

    //���� ���� �̸�
    string saveFileName;

    //�ش� ���� ������Ʈ�� Ŭ���Ǿ����� ȣ��Ǵ� �Լ�
    private void OnEnable()
    {
        Debug.Log("���õ�");
        map = (Map)target;

        //map.objectList�� ����ִ� �ֵ��� �̸� ����
        objectListName = new string[map.objectList.Length];
        for (int i = 0; i < objectListName.Length; i++)
            objectListName[i] = map.objectList[i].name;
    }

    //Inspector �� ��ȭ�� ������ ȣ��Ǵ� �Լ�
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        //tileX, tileZ�� ǥ��
        map.tileX = EditorGUILayout.IntField("Ÿ�� ����", map.tileX);
        EditorGUILayout.Space();
        map.tileZ = EditorGUILayout.IntField("Ÿ�� ����", map.tileZ);
        //tileX, tileZ (1~100)
        map.tileX = Mathf.Clamp(map.tileX, 1, 100);
        map.tileZ = Mathf.Clamp(map.tileZ, 1, 100);

        //�ٴ� Prefab ����
        map.floorFactory = (GameObject)EditorGUILayout.ObjectField("�ٴ�", map.floorFactory, typeof(GameObject), false);
        //BlueCube Prefab ����
        map.blueCubeFactory = (GameObject)EditorGUILayout.ObjectField("�Ķ� ť��", map.blueCubeFactory, typeof(GameObject), false);

        //objectList ����
        EditorGUI.ChangeCheckScope check = new EditorGUI.ChangeCheckScope();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("objectList"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("createdObjects"));
        if (check.changed)
        {
            serializedObject.ApplyModifiedProperties();
        }

        //���� ������Ʈ ����
        map.selectObjIdx = EditorGUILayout.Popup("���� ������Ʈ", map.selectObjIdx, objectListName);

        //�ٴڻ��� ��ư
        if (GUILayout.Button("�ٴ� ����"))
        {
            CreateFloor();
        }

        saveFileName = EditorGUILayout.TextField("���������̸�", saveFileName);
        //Json���� ��ư
        if(GUILayout.Button("Json ����"))
        {
            SaveJson();
        }
        //Json�ҷ����� ��ư
        if (GUILayout.Button("Json �ҷ�����"))
        {
            LoadJson();
        }

        //�ν����Ϳ� ��������� ����ٸ�
        if(GUI.changed)
        {
            //Hierarchy�� Scene�̸� ���� *ǥ�� ������ ����
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            //Sceneȭ���� �ٽ� �׸���
            SceneView.RepaintAll();
        }
    }

    //Scene ȭ���� �׸��� �Լ�
    private void OnSceneGUI()
    {
        //���� Map �Ǿ������� (Sceneȭ�鿡����)�ٸ� ���ӿ�����Ʈ�� �������� ���ϰ� ����
        int id = GUIUtility.GetControlID(FocusType.Passive);
        HandleUtility.AddDefaultControl(id);

        DrawGrid();

        CreateObject();

        DeleteObject();
    }

    void LoadJson()
    {
        //���࿡ saveFileName �� ���̰� 0�̶�� 
        if(saveFileName.Length <= 0)
        {
            //�����̸��� �Է��ϼ���.
            Debug.LogError("���� �̸��� �Է��ϼ���.");
            return;
        }

        //���� �� �����͸� ����
        CreateFloor();

        //mapData.txt�� �ҷ�����
        string jsonData = File.ReadAllText(Application.dataPath + "/" + saveFileName +".txt");
        //ArrayJson ���·� Json�� ��ȯ
        ArrayJson arrayJson = JsonUtility.FromJson<ArrayJson>(jsonData);
        //ArrayJson�� �����͸� ������ ������Ʈ ����
        for(int i = 0; i < arrayJson.datas.Count; i++)
        {
            SaveJsonInfo info = arrayJson.datas[i];
            LoadObject(info.idx, info.position, info.eulerAngle, info.localScale);
        }
    }

    void SaveJson()
    {
        //���࿡ saveFileName �� ���̰� 0�̶�� 
        if (saveFileName.Length <= 0)
        {
            //�����̸��� �Է��ϼ���.
            Debug.LogError("���� �̸��� �Է��ϼ���.");
            return;
        }

        //map.createdObjects �� �ִ� ������ json���� ��ȯ
        //idx, postion, eulerAngle, localScale
        //map.createdObjects �� ������ŭ SaveJsonInfo ���� ����
        //ArrayJson �����.
        ArrayJson arrayJson = new ArrayJson();
        arrayJson.datas = new List<SaveJsonInfo>();

        SaveJsonInfo info;
        for(int i = 0; i < map.createdObjects.Count; i++)
        {
            CreatedInfo createdInfo = map.createdObjects[i];

            info = new SaveJsonInfo();
            info.idx = createdInfo.idx;
            info.position = createdInfo.go.transform.position;
            info.eulerAngle = createdInfo.go.transform.eulerAngles;
            info.localScale = createdInfo.go.transform.localScale;
            //ArrayJson �� datas �� �ϳ��� �߰�
            arrayJson.datas.Add(info);
        }
        //arrayJson�� Json���� ��ȯ
        string jsonData = JsonUtility.ToJson(arrayJson, true);
        Debug.Log("jsonData : " + jsonData);
        //jsonData�� ���Ϸ� ����
        File.WriteAllText(Application.dataPath + "/" + saveFileName + ".txt", jsonData);
    }

    void DeleteObject()
    {
        Event e = Event.current;
        //���࿡ ���� ���콺�� Ŭ���ߴٸ�
        //���࿡ ctrlŰ�� �����ִٸ�        
        if(e.type == EventType.MouseDown && e.button == 0 && e.control)
        {
            //Ray�� ���콺 ��ġ���� ����
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //���� ���ӿ�����Ʈ�� Layer�� Object��� 
                if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Object"))
                {
                    //map.createdObjects ���� hit.transform.gameObject �� ã�Ƽ� ������
                    for(int i = 0; i < map.createdObjects.Count; i++)
                    {
                        if(map.createdObjects[i].go == hit.transform.gameObject)
                        {
                            map.createdObjects.RemoveAt(i);
                            break;
                        }
                    }

                    //������
                    DestroyImmediate(hit.transform.gameObject);                   
                }
            }            
        }
    }

    void LoadObject(int idx, Vector3 position, Vector3 eulerAngle, Vector3 localScale)
    {
        //�ش� ��ġ�� BlueCube�� �����ؼ� ���´�.
        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(
            map.objectList[idx]);

        obj.transform.position = position;
        obj.transform.eulerAngles = eulerAngle;
        obj.transform.localScale = localScale;

        //�θ� obj_parent ���� ����
        obj.transform.parent = GameObject.Find("obj_parent").transform;


        //������� ������Ʈ�� ����Ʈ�� �߰�
        CreatedInfo info = new CreatedInfo();
        info.go = obj;
        info.idx = idx;
        map.createdObjects.Add(info);
    }

    void CreateObject()
    {
        Event e = Event.current;
        //���࿡ ���� ���콺�� Ŭ���ߴٸ�
        if(e.type == EventType.MouseDown && e.button == 0 && e.control == false)
        {
            //���콺 ��ġ�� Ray�� �����.
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;
            //���� Ray�� �߻��ؼ� �¾Ҵٸ�
            if (Physics.Raycast(ray, out hit))
            {
                //���࿡ �ε��� ���� Layer�� Floor�� ��
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Floor"))
                {
                    Vector3 p = new Vector3((int)hit.point.x, hit.point.y, (int)hit.point.z);
                    LoadObject(map.selectObjIdx, p, Vector3.zero, Vector3.one);
                }
            }
        }        
    }

    void CreateFloor()
    {
        //Floor���� ������Ʈ�� ã��
        GameObject floor = GameObject.Find("Floor");
        //���࿡ ������ �۾��ϴ� floor�� �����Ѵٸ�
        if(floor != null)
        {            
            //�ı�����
            DestroyImmediate(floor);
        }

        GameObject empty = GameObject.Find("obj_parent");
        if(empty != null)
        {
            DestroyImmediate(empty);
        }

        //�� ������Ʈ(��ġ�� ������Ʈ���� �θ�)
        empty = new GameObject();
        empty.name = "obj_parent";

        //prefab �� ����������� ���ӿ�����Ʈ
        //Instantiate(map.floorFactory);
        //prefab �� ����� ���ӿ�����Ʈ
        floor = (GameObject)PrefabUtility.InstantiatePrefab(map.floorFactory);
        //tileX, tileZ�� ������ floor�� ũ�� ���� ����
        floor.transform.localScale = new Vector3(map.tileX, 1, map.tileZ);
        //������� ������Ʈ ����Ʈ ������
        map.createdObjects.Clear();
    }

    void DrawGrid()
    {
        Vector3 start;
        Vector3 end;
        Handles.color = Color.red;
        //������(tileX)
        for (int i = 0; i <= map.tileX; i++)
        {
            start = new Vector3(i, 0, 0);
            end = new Vector3(i, 0, map.tileZ);
            Handles.DrawLine(start, end);
        }

        Handles.color = Color.blue;
        //������(tileZ)
        for (int i = 0; i <= map.tileZ; i++)
        {
            start = new Vector3(0, 0, i);
            end = new Vector3(map.tileX, 0, i);
            Handles.DrawLine(start, end);
        }
    }
}
