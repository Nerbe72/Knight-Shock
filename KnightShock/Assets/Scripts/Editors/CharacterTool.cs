using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Collections.Generic;
using System;
using UnityEditorInternal;
using System.IO;
using static UnityEditor.Progress;
using UnityEditor.Experimental.GraphView;

public class CharacterTool : EditorWindow
{
    private List<Character> characterList = new List<Character>();
    private Vector2 scrollPos;
    private string dataPath = Path.Combine(Application.dataPath, "Datas");

    [MenuItem("Tools/Character Creator")]
    public static void ShowWindow()
    {
        GetWindow<CharacterTool>("Character Tool");
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(1000)); // (1) �ֿܰ� Horizontal
        EditorGUILayout.Space(10);

        // ���� ���� (ĳ����)
        CharacterSide();

        EditorGUILayout.Space(10);

        EditorGUILayout.EndHorizontal(); // (1)
    }

    private void CharacterSide()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(1000));   // (2) ����

        GUILayout.Label("ĳ����");

        // �ҷ����� / �������� ��ư ���� ��ġ
        EditorGUILayout.BeginHorizontal(); // (3)
        {
            if (GUILayout.Button("�ҷ�����"))
            {
                ImportJson();
            }
            if (GUILayout.Button("��������"))
            {
                ExportJson();
            }
        }
        EditorGUILayout.EndHorizontal(); // (3)

        // �� ĳ���� ��ư
        if (GUILayout.Button("�� ĳ����"))
        {
            var newChar = new Character();
            characterList.Add(newChar);
        }

        GUILayout.Space(10);

        // ĳ���� ����Ʈ ��ũ�� ����
        CharacterScroll();

        EditorGUILayout.EndVertical();   // (2)
    }

    private void CharacterScroll()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        bool deletionOccurred = false;

        // ����Ʈ�� ĳ����ID �������� �����մϴ�.
        characterList.Sort((a, b) => a.Id.CompareTo(b.Id));

        int count = characterList.Count;
        // 3���� ��ġ�մϴ�.
        for (int i = 0; i < count; i += 3)
        {
            EditorGUILayout.BeginHorizontal();

            for (int j = i; j < i + 3 && j < count; j++)
            {
                EditorGUILayout.BeginVertical("box", GUILayout.Width(320));

                Rect lineRect = GUILayoutUtility.GetRect(30 * 9, 5);
                EditorGUI.DrawRect(new Rect(lineRect.x, lineRect.y + lineRect.height - 2, lineRect.width, 3), Color.green);

                var ch = characterList[j];

                ch.Id = EditorGUILayout.IntField("ĳ����ID", ch.Id);
                ch.Name = EditorGUILayout.TextField("�̸�", ch.Name);
                ch.SpritePath = EditorGUILayout.TextField("�̹��� ���", ch.SpritePath);

                if ((int)(ch.Id / 10000) == 1)
                {
                    ch.BaseRare = Rare.SSR;
                }
                else if ((int)(ch.Id / 10000) == 2)
                {
                    ch.BaseRare = Rare.SR;
                }
                else if ((int)(ch.Id / 10000) == 3)
                {
                    ch.BaseRare = Rare.R;
                }
                else
                {
                    ch.BaseRare = Rare.R;
                }

                ch.BaseRare = (Rare)EditorGUILayout.EnumFlagsField("���", ch.BaseRare);
                ch.BaseHp = EditorGUILayout.IntField("ü��", ch.BaseHp);
                ch.BaseMelee = EditorGUILayout.IntField("���� ���ݷ�", ch.BaseMelee);
                ch.BaseMagic = EditorGUILayout.IntField("���� ���ݷ�", ch.BaseMagic);
                ch.BaseMeleeDefense = EditorGUILayout.IntField("���� ����", ch.BaseMeleeDefense);
                ch.BaseMagicDefense = EditorGUILayout.IntField("���� ����", ch.BaseMagicDefense);

                // �нú� ��ų ����Ʈ
                ReorderableList reorderPassive = new ReorderableList(ch.PassiveId, typeof(int), true, true, true, true);
                reorderPassive.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "�нú� ��ų");
                };
                reorderPassive.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    ch.PassiveId[index] = EditorGUI.IntField(rect, ch.PassiveId[index]);
                };
                reorderPassive.DoLayoutList();

                // ��Ƽ�� ��ų ����Ʈ
                ReorderableList reorderActive = new ReorderableList(ch.SkillId, typeof(int), true, true, true, true);
                reorderActive.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "��Ƽ�� ��ų");
                };
                reorderActive.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    ch.SkillId[index] = EditorGUI.IntField(rect, ch.SkillId[index]);
                };
                reorderActive.DoLayoutList();

                // ���ĵ� Sprite ó��
                if (!string.IsNullOrEmpty(ch.SpritePath))
                {
                    ch.CharacterSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ch.SpritePath);
                }
                ch.CharacterSprite = (Sprite)EditorGUILayout.ObjectField("���ĵ�", ch.CharacterSprite, typeof(Sprite), false);
                if (ch.CharacterSprite != null)
                {
                    int size = 30;
                    Rect previewRect = GUILayoutUtility.GetRect(size * 9, size * 18);
                    float maxWidth = previewRect.width;

                    Rect spriteRect = ch.CharacterSprite.textureRect;
                    float aspectRatio = spriteRect.height / spriteRect.width;

                    // �ִ� ���� ���̿� ���� ��������Ʈ�� ���� ���̸� ����մϴ�.
                    float spriteHeight = maxWidth * aspectRatio;

                    // previewRect�� �ϴܿ� ���ĵǵ��� drawingRect�� �����մϴ�.
                    Rect drawingRect = new Rect(
                        previewRect.x,
                        previewRect.y + previewRect.height - spriteHeight,
                        maxWidth,
                        spriteHeight
                    );

                    Rect normalizedRect = new Rect(
                        spriteRect.x / ch.CharacterSprite.texture.width,
                        spriteRect.y / ch.CharacterSprite.texture.height,
                        spriteRect.width / ch.CharacterSprite.texture.width,
                        spriteRect.height / ch.CharacterSprite.texture.height
                    );

                    GUI.DrawTextureWithTexCoords(drawingRect, ch.CharacterSprite.texture, normalizedRect, true);
                    ch.SpritePath = AssetDatabase.GetAssetPath(ch.CharacterSprite);
                }

                // ���÷��� Sprite ó��
                if (!string.IsNullOrEmpty(ch.SplashPath))
                {
                    ch.SplashSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ch.SplashPath);
                }
                ch.SplashSprite = (Sprite)EditorGUILayout.ObjectField("���÷���", ch.SplashSprite, typeof(Sprite), false);
                if (ch.SplashSprite != null)
                {
                    int splashSize = 30;
                    Rect splashRect = GUILayoutUtility.GetRect(splashSize * 9, splashSize * 8);
                    float splashMaxWidth = splashRect.width;

                    Rect splashSpriteRect = ch.SplashSprite.textureRect;
                    float splashAspectRatio = splashSpriteRect.height / splashSpriteRect.width;
                    float splashSpriteHeight = splashMaxWidth * splashAspectRatio;

                    Rect splashDrawingRect = new Rect(
                        splashRect.x,
                        splashRect.y + splashRect.height - splashSpriteHeight,
                        splashMaxWidth,
                        splashSpriteHeight
                    );

                    Rect splashNormalizedRect = new Rect(
                        splashSpriteRect.x / ch.SplashSprite.texture.width,
                        splashSpriteRect.y / ch.SplashSprite.texture.height,
                        splashSpriteRect.width / ch.SplashSprite.texture.width,
                        splashSpriteRect.height / ch.SplashSprite.texture.height
                    );

                    GUI.DrawTextureWithTexCoords(splashDrawingRect, ch.SplashSprite.texture, splashNormalizedRect, true);
                    ch.SplashPath = AssetDatabase.GetAssetPath(ch.SplashSprite);
                }

                // ���� ��ư
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space(10);
                if (GUILayout.Button("����"))
                {
                    characterList.RemoveAt(j);
                    deletionOccurred = true;
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    break;
                }
                EditorGUILayout.EndHorizontal();

                Rect lineRect2 = GUILayoutUtility.GetRect(30 * 9, 3);
                EditorGUI.DrawRect(new Rect(lineRect2.x, lineRect2.y + lineRect2.height, lineRect2.width, 3), Color.red);

                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();

            if (deletionOccurred)
            {
                // ������ �߻��ϸ� ���� ���� �� �緻�����մϴ�.
                break;
            }
            GUILayout.Space(5);
        }
        EditorGUILayout.EndScrollView();
    }


    private void ImportJson()
    {
        string path = EditorUtility.OpenFilePanel("ĳ���� ���� ����", dataPath, "json");

        if (string.IsNullOrEmpty(path)) return;

        string json = File.ReadAllText(path);
        CharacterWrapper characterWrapper = JsonUtility.FromJson<CharacterWrapper>(json);

        if (characterWrapper != null && characterWrapper.characters != null)
        {
            characterList = characterWrapper.characters;
        }
    }

    private void ExportJson()
    {
        string path = EditorUtility.SaveFilePanel("ĳ���� ���� ����", dataPath, "CHARACTER", "json");

        if (string.IsNullOrEmpty(path)) return;

        CharacterWrapper characterWrapper = new CharacterWrapper();
        characterWrapper.characters = characterList;
        string json = JsonUtility.ToJson(characterWrapper, true);
        Debug.Log(json);

        File.WriteAllText(path, json);
    }
}
