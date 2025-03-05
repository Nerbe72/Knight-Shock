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
        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(900)); // (1) 최외곽 Horizontal
        EditorGUILayout.Space(10);

        // 오른쪽 영역 (스킬)
        SkillSide();

        EditorGUILayout.Space(5);

        // 힌트 표시
        HintSide();

        EditorGUILayout.EndHorizontal(); // (1)
    }

    private void SkillSide()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(450)); // (6) 오른쪽

        GUILayout.Label("스킬");

        EditorGUILayout.BeginHorizontal(); // (7)
        {
            if (GUILayout.Button("불러오기"))
            {
                ImportJson();
            }
            if (GUILayout.Button("내보내기"))
            {
                ExportJson();
            }
        }
        EditorGUILayout.EndHorizontal(); // (7)

        if (GUILayout.Button("새 스킬"))
        {
            var newSkill = new Skill();
            skillList.Add(newSkill);
        }

        GUILayout.Space(10);

        // 스킬 리스트 스크롤 영역
        SkillScroll();

        EditorGUILayout.EndVertical(); // (6)
    }

    private void HintSide()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(100)); // (x)

        GUIStyle titleStyle = new GUIStyle(EditorStyles.label);
        titleStyle.fontSize = 14;
        titleStyle.fontStyle = FontStyle.Bold;

        EditorGUILayout.LabelField("감정", titleStyle);
        for (int i = (int)EmotionType.None + 1; i < (int)EmotionType.Count; i++)
        {
            EditorGUILayout.LabelField($"{((EmotionType)i).ToString()}");
        }
        EditorGUILayout.Space(15);

        EditorGUILayout.LabelField("스탯", titleStyle);
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

                sk.Id = EditorGUILayout.IntField("스킬 ID", sk.Id);
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

                sk.Type = (SkillType)EditorGUILayout.EnumFlagsField("타입", sk.Type);

                EditorGUILayout.LabelField("설명");
                sk.Description = EditorGUILayout.TextArea(sk.Description, GUILayout.Height(150));

                //감정 변경
                ReorderableList reorderEmotion = new ReorderableList(sk.ChangeEmotions, typeof(int), true, true, true, true);
                reorderEmotion.elementHeight = EditorGUIUtility.singleLineHeight * 4 + 16f;
                reorderEmotion.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "감정 변경치");
                };
                reorderEmotion.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var item = sk.ChangeEmotions[index];

                    var lineRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                    item.EmotionType = (EmotionType)EditorGUI.EnumPopup(lineRect, "감정", item.EmotionType);

                    // 2) IsPercent (Toggle)
                    var lineRect2 = new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight * 2f),
                                                rect.width, EditorGUIUtility.singleLineHeight);
                    item.IsPercent = EditorGUI.Toggle(lineRect2, "퍼센트 여부", item.IsPercent);

                    // 3) Amount (int)
                    var lineRect3 = new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight * 3f),
                                                rect.width, EditorGUIUtility.singleLineHeight);
                    item.Amount = EditorGUI.FloatField(lineRect3, "증감", item.Amount);

                    EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height - 4, rect.width, 1), Color.gray);
                };
                reorderEmotion.DoLayoutList();

                //스탯 변경
                ReorderableList reorderStatus = new ReorderableList(sk.ChangeStats, typeof(int), true, true, true, true);
                reorderStatus.elementHeight = EditorGUIUtility.singleLineHeight * 4 + 16f;
                reorderStatus.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "스탯 변경치");
                };
                reorderStatus.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var item = sk.ChangeStats[index];

                    var lineRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                    item.StatType = (StatType)EditorGUI.EnumPopup(lineRect, "스탯", item.StatType);

                    // 2) IsPercent (Toggle)
                    var lineRect2 = new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight * 2f),
                                                rect.width, EditorGUIUtility.singleLineHeight);
                    item.IsPercent = EditorGUI.Toggle(lineRect2, "퍼센트 여부", item.IsPercent);

                    // 3) Amount (int)
                    var lineRect3 = new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight * 3f),
                                                rect.width, EditorGUIUtility.singleLineHeight);
                    item.Amount = EditorGUI.FloatField(lineRect3, "증감", item.Amount);

                    EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height - 4, rect.width, 1), Color.gray);
                };
                reorderStatus.DoLayoutList();

                //공격 스킬
                ReorderableList reorderAttack = new ReorderableList(sk.Attacks, typeof(int), true, true, true, true);
                reorderAttack.elementHeight = EditorGUIUtility.singleLineHeight * 6 + 16f;
                reorderAttack.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "공격");
                };
                reorderAttack.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var item = sk.Attacks[index];

                    //공격 보정타입
                    var lineRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                    item.AttackType = (AttackType)EditorGUI.EnumPopup(lineRect, "공격 보정", item.AttackType);

                    //공격 범위
                    var lineRect2 = new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight * 2f),
                                                rect.width, EditorGUIUtility.singleLineHeight);
                    item.AttackArea = (AttackArea)EditorGUI.EnumPopup(lineRect2, "공격 범위", item.AttackArea);

                    //퍼센트 여부
                    var lineRect3 = new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight * 3f),
                                                rect.width, EditorGUIUtility.singleLineHeight);
                    item.IsPercent = EditorGUI.Toggle(lineRect3, "퍼센트 여부", item.IsPercent);

                    //고정 데미지 여부
                    var lineRect4 = new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight * 4f),
                                                rect.width, EditorGUIUtility.singleLineHeight);
                    item.IsFixedDamage = EditorGUI.Toggle(lineRect4, "고정 데미지 여부", item.IsFixedDamage);

                    //증감
                    var lineRect5 = new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight * 5f),
                                                rect.width, EditorGUIUtility.singleLineHeight);
                    item.Amount = EditorGUI.FloatField(lineRect5, "증감", item.Amount);

                    EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height - 4, rect.width, 1), Color.gray);
                };
                reorderAttack.DoLayoutList();

                // 삭제 버튼
                EditorGUILayout.BeginHorizontal(); // (9)

                EditorGUILayout.Space(10);
                if (GUILayout.Button("삭제"))
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
        string path = EditorUtility.OpenFilePanel("스킬 정보 열기", dataPath, "json");

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
        string path = EditorUtility.SaveFilePanel("스킬 정보 저장", dataPath, "SKILL", "json");

        if (string.IsNullOrEmpty(path)) return;

        SkillWrapper skillWrapper = new SkillWrapper();
        skillWrapper.skills = skillList;
        string json = JsonUtility.ToJson(skillWrapper, true);
        Debug.Log(json);

        File.WriteAllText(path, json);
    }
}
