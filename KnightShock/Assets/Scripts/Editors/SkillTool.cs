using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class SkillTool : EditorWindow
{
    private List<Skill> skillList = new List<Skill>();
    private string dataPath = Path.Combine(Application.dataPath, "Datas");
    private Vector2 scrollPos;

    [MenuItem("Tools/Skill Creator")]
    public static void ShowWindow()
    {
        GetWindow<SkillTool>("Skill Tool");
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(900)); // (1) �ֿܰ� Horizontal
        EditorGUILayout.Space(10);

        // ������ ���� (��ų)
        SkillSide();

        EditorGUILayout.Space(5);

        // ��Ʈ ǥ��
        HintSide();

        EditorGUILayout.EndHorizontal(); // (1)
    }

    private void SkillSide()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(450)); // (6) ������

        GUILayout.Label("��ų");

        EditorGUILayout.BeginHorizontal(); // (7)
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
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos); // (8)

        int skillCount = skillList.Count;
        for (int i = 0; i < skillCount; i++)
        {
            EditorGUILayout.BeginVertical("box");
            {
                var sk = skillList[i];

                sk.Id = EditorGUILayout.IntField("��ų ID", sk.Id);
                if ((int)(sk.Id / 1000) == 1)
                {
                    sk.Type = SkillType.Passive;
                }
                else if ((int)(sk.Id / 1000) == 2)
                {
                    sk.Type = SkillType.Active;
                }
                else
                {
                    sk.Type = SkillType.None;
                }

                sk.Type = (SkillType)EditorGUILayout.EnumFlagsField("Ÿ��", sk.Type);

                EditorGUILayout.LabelField("����");
                sk.Description = EditorGUILayout.TextArea(sk.Description, GUILayout.Height(150));

                //���� ����
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

                //���� ����
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

                //���� ��ų
                ReorderableList reorderAttack = new ReorderableList(sk.Attacks, typeof(int), true, true, true, true);
                reorderAttack.elementHeight = EditorGUIUtility.singleLineHeight * 6 + 16f;
                reorderAttack.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "����");
                };
                reorderAttack.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var item = sk.Attacks[index];

                    //���� ����Ÿ��
                    var lineRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                    item.AttackType = (AttackType)EditorGUI.EnumPopup(lineRect, "���� ����", item.AttackType);

                    //���� ����
                    var lineRect2 = new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight * 2f),
                                                rect.width, EditorGUIUtility.singleLineHeight);
                    item.AttackArea = (AttackArea)EditorGUI.EnumPopup(lineRect2, "���� ����", item.AttackArea);

                    //�ۼ�Ʈ ����
                    var lineRect3 = new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight * 3f),
                                                rect.width, EditorGUIUtility.singleLineHeight);
                    item.IsPercent = EditorGUI.Toggle(lineRect3, "�ۼ�Ʈ ����", item.IsPercent);

                    //���� ������ ����
                    var lineRect4 = new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight * 4f),
                                                rect.width, EditorGUIUtility.singleLineHeight);
                    item.IsFixedDamage = EditorGUI.Toggle(lineRect4, "���� ������ ����", item.IsFixedDamage);

                    //����
                    var lineRect5 = new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight * 5f),
                                                rect.width, EditorGUIUtility.singleLineHeight);
                    item.Amount = EditorGUI.FloatField(lineRect5, "����", item.Amount);

                    EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height - 4, rect.width, 1), Color.gray);
                };
                reorderAttack.DoLayoutList();

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
        string path = EditorUtility.OpenFilePanel("��ų ���� ����", dataPath, "json");

        if (string.IsNullOrEmpty(path)) return;

        string json = File.ReadAllText(path);
        SkillWrapper skillWrapper = JsonUtility.FromJson<SkillWrapper>(json);

        if (skillWrapper != null && skillWrapper.skills != null)
        {
            skillList = skillWrapper.skills;
        }
    }

    private void ExportJson()
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
