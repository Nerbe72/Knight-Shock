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
    private List<Skill> skillList = new List<Skill>();
    private Vector2 scrollPos;
    private Vector2 scrollPosSkill;
    private string dataPath = Path.Combine(Application.dataPath, "Datas");

    [MenuItem("Tools/Character Creator")]
    public static void ShowWindow()
    {
        GetWindow<CharacterTool>("Character Tool");
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(900)); // (1) �ֿܰ� Horizontal
        EditorGUILayout.Space(10);

        // ���� ���� (ĳ����)
        CharacterSide();

        EditorGUILayout.Space(10);

        // ������ ���� (��ų)
        SkillSide();

        EditorGUILayout.Space(5);

        // ��Ʈ ǥ��
        HintSide();

        EditorGUILayout.EndHorizontal(); // (1)
    }

    private void CharacterSide()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(400));   // (2) ����

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
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos); // (4)

        int count = characterList.Count;
        for (int i = 0; i < count; i++)
        {
            EditorGUILayout.BeginVertical("box");  // �ڽ�

            var ch = characterList[i];

            ch.Id = EditorGUILayout.IntField("ĳ����ID", ch.Id);
            ch.Name = EditorGUILayout.TextField("�̸�", ch.Name);
            ch.BaseRare = (Rare)EditorGUILayout.EnumFlagsField("���", ch.BaseRare);
            ch.BaseHp = EditorGUILayout.IntField("ü��", ch.BaseHp);
            ch.BaseMelee = EditorGUILayout.IntField("���� ���ݷ�", ch.BaseMelee);
            ch.BaseMagic = EditorGUILayout.IntField("���� ���ݷ�", ch.BaseMagic);
            ch.BaseMeleeDefense = EditorGUILayout.IntField("���� ����", ch.BaseMeleeDefense);
            ch.BaseMagicDefense = EditorGUILayout.IntField("���� ����", ch.BaseMagicDefense);

            //�нú�
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

            //��Ƽ��
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

            // ���� ��ư
            EditorGUILayout.BeginHorizontal(); // (5)
            {
                EditorGUILayout.Space(10);
                if (GUILayout.Button("����"))
                {
                    characterList.RemoveAt(i);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    break;
                }
                EditorGUILayout.Space(10);
            }
            EditorGUILayout.EndHorizontal();   // (5)

            EditorGUILayout.EndVertical(); // �ڽ� ����
            GUILayout.Space(5);
        }

        EditorGUILayout.EndScrollView(); // (4)
    }

    private void SkillSide()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(450)); // (6) ������

        GUILayout.Label("��ų");

        EditorGUILayout.BeginHorizontal(); // (7)
        {
            if (GUILayout.Button("�ҷ�����"))
            {
                ImportJsonSkill();
            }
            if (GUILayout.Button("��������"))
            {
                ExportJsonSkill();
            }
        }
        EditorGUILayout.EndHorizontal(); // (7)

        if (GUILayout.Button("�� ��ų"))
        {
            var newSkill = new Skill();
            skillList.Add(newSkill);
        }

        GUILayout.Space(10);

        // ��ų ����Ʈ ��ũ�� ����
        SkillScroll();

        EditorGUILayout.EndVertical(); // (6)
    }

    private void HintSide()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(100)); // (x)

        GUIStyle titleStyle = new GUIStyle(EditorStyles.label);
        titleStyle.fontSize = 14;
        titleStyle.fontStyle = FontStyle.Bold;

        EditorGUILayout.LabelField("����", titleStyle);
        for (int i = (int)EmotionType.None + 1; i < (int)EmotionType.Count; i++)
        {
            EditorGUILayout.LabelField($"{((EmotionType)i).ToString()}");
        }
        EditorGUILayout.Space(15);

        EditorGUILayout.LabelField("����", titleStyle);
        for (int i = (int)StatType.None + 1; i < (int)StatType.Count; i++)
        {
            EditorGUILayout.LabelField($"{((StatType)i).ToString()}");
        }

        EditorGUILayout.EndVertical(); // (x)
    }

    private void SkillScroll()
    {
        scrollPosSkill = EditorGUILayout.BeginScrollView(scrollPosSkill); // (8)

        int skillCount = skillList.Count;
        for (int i = 0; i < skillCount; i++)
        {
            EditorGUILayout.BeginVertical("box");
            {
                var sk = skillList[i];

                sk.Id = EditorGUILayout.IntField("��ų ID", sk.Id);
                sk.Type = (SkillType)EditorGUILayout.EnumFlagsField("Ÿ��", sk.Type);
                EditorGUILayout.LabelField("����");
                sk.Description = EditorGUILayout.TextArea(sk.Description, GUILayout.Height(150));

                //����
                ReorderableList reorderEmotion = new ReorderableList(sk.ChangeEmotions, typeof(int), true, true, true, true);
                reorderEmotion.elementHeight = EditorGUIUtility.singleLineHeight * 4 + 16f;
                reorderEmotion.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "���� ����ġ");
                };
                reorderEmotion.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var item = sk.ChangeEmotions[index];

                    var lineRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                    item.EmotionType = (EmotionType)EditorGUI.EnumPopup(lineRect, "����", item.EmotionType);

                    // 2) IsPercent (Toggle)
                    var lineRect2 = new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight * 2f),
                                                rect.width, EditorGUIUtility.singleLineHeight);
                    item.IsPercent = EditorGUI.Toggle(lineRect2, "�ۼ�Ʈ ����", item.IsPercent);

                    // 3) Amount (int)
                    var lineRect3 = new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight * 3f),
                                                rect.width, EditorGUIUtility.singleLineHeight);
                    item.Amount = EditorGUI.FloatField(lineRect3, "����", item.Amount);

                    EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height - 4, rect.width, 1), Color.gray);
                };
                reorderEmotion.DoLayoutList();

                ReorderableList reorderStatus = new ReorderableList(sk.ChangeStats, typeof(int), true, true, true, true);
                reorderStatus.elementHeight = EditorGUIUtility.singleLineHeight * 4 + 16f;
                reorderStatus.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "���� ����ġ");
                };
                reorderStatus.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var item = sk.ChangeStats[index];

                    var lineRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                    item.StatType = (StatType)EditorGUI.EnumPopup(lineRect, "����", item.StatType);

                    // 2) IsPercent (Toggle)
                    var lineRect2 = new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight * 2f),
                                                rect.width, EditorGUIUtility.singleLineHeight);
                    item.IsPercent = EditorGUI.Toggle(lineRect2, "�ۼ�Ʈ ����", item.IsPercent);

                    // 3) Amount (int)
                    var lineRect3 = new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight * 3f),
                                                rect.width, EditorGUIUtility.singleLineHeight);
                    item.Amount = EditorGUI.FloatField(lineRect3, "����", item.Amount);

                    EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height - 4, rect.width, 1), Color.gray);
                };
                reorderStatus.DoLayoutList();

                // ���� ��ư
                EditorGUILayout.BeginHorizontal(); // (9)

                EditorGUILayout.Space(10);
                if (GUILayout.Button("����"))
                {
                    skillList.RemoveAt(i);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    break;
                }
                EditorGUILayout.Space(10);

                EditorGUILayout.EndHorizontal(); // (9)
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
        }
        EditorGUILayout.EndScrollView(); // (8)
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

    private void ImportJsonSkill()
    {
        string path = EditorUtility.OpenFilePanel("��ų ���� ����", dataPath, "json");

        if (string.IsNullOrEmpty(path)) return;

        string json = File.ReadAllText(path);
        SkillWrapper skillWrapper = JsonUtility.FromJson<SkillWrapper>(json);

        if (skillWrapper != null && skillWrapper.skills != null)
        {
            skillList = skillWrapper.skills;
        }
    }

    private void ExportJsonSkill()
    {
        string path = EditorUtility.SaveFilePanel("��ų ���� ����", dataPath, "SKILL", "json");

        if (string.IsNullOrEmpty(path)) return;

        SkillWrapper skillWrapper = new SkillWrapper();
        skillWrapper.skills = skillList;
        string json = JsonUtility.ToJson(skillWrapper, true);
        Debug.Log(json);

        File.WriteAllText(path, json);
    }
}
