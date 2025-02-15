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
        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(900)); // (1) 최외곽 Horizontal
        EditorGUILayout.Space(10);

        // 왼쪽 영역 (캐릭터)
        CharacterSide();

        EditorGUILayout.Space(10);

        // 오른쪽 영역 (스킬)
        SkillSide();

        EditorGUILayout.Space(5);

        // 힌트 표시
        HintSide();

        EditorGUILayout.EndHorizontal(); // (1)
    }

    private void CharacterSide()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(400));   // (2) 왼쪽

        GUILayout.Label("캐릭터");

        // 불러오기 / 내보내기 버튼 가로 배치
        EditorGUILayout.BeginHorizontal(); // (3)
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
        EditorGUILayout.EndHorizontal(); // (3)

        // 새 캐릭터 버튼
        if (GUILayout.Button("새 캐릭터"))
        {
            var newChar = new Character();
            characterList.Add(newChar);
        }

        GUILayout.Space(10);

        // 캐릭터 리스트 스크롤 영역
        CharacterScroll();

        EditorGUILayout.EndVertical();   // (2)
    }

    private void CharacterScroll()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos); // (4)

        int count = characterList.Count;
        for (int i = 0; i < count; i++)
        {
            EditorGUILayout.BeginVertical("box");  // 박스

            var ch = characterList[i];

            ch.Id = EditorGUILayout.IntField("캐릭터ID", ch.Id);
            ch.Name = EditorGUILayout.TextField("이름", ch.Name);
            ch.BaseRare = (Rare)EditorGUILayout.EnumFlagsField("레어도", ch.BaseRare);
            ch.BaseHp = EditorGUILayout.IntField("체력", ch.BaseHp);
            ch.BaseMelee = EditorGUILayout.IntField("물리 공격력", ch.BaseMelee);
            ch.BaseMagic = EditorGUILayout.IntField("마법 공격력", ch.BaseMagic);
            ch.BaseMeleeDefense = EditorGUILayout.IntField("물리 방어력", ch.BaseMeleeDefense);
            ch.BaseMagicDefense = EditorGUILayout.IntField("마법 방어력", ch.BaseMagicDefense);

            //패시브
            ReorderableList reorderPassive = new ReorderableList(ch.PassiveId, typeof(int), true, true, true, true);
            reorderPassive.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "패시브 스킬");
            };
            reorderPassive.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                ch.PassiveId[index] = EditorGUI.IntField(rect, ch.PassiveId[index]);
            };
            reorderPassive.DoLayoutList();

            //액티브
            ReorderableList reorderActive = new ReorderableList(ch.SkillId, typeof(int), true, true, true, true);
            reorderActive.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "액티브 스킬");
            };
            reorderActive.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                ch.SkillId[index] = EditorGUI.IntField(rect, ch.SkillId[index]);
            };
            reorderActive.DoLayoutList();

            // 삭제 버튼
            EditorGUILayout.BeginHorizontal(); // (5)
            {
                EditorGUILayout.Space(10);
                if (GUILayout.Button("삭제"))
                {
                    characterList.RemoveAt(i);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    break;
                }
                EditorGUILayout.Space(10);
            }
            EditorGUILayout.EndHorizontal();   // (5)

            EditorGUILayout.EndVertical(); // 박스 종료
            GUILayout.Space(5);
        }

        EditorGUILayout.EndScrollView(); // (4)
    }

    private void SkillSide()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(450)); // (6) 오른쪽

        GUILayout.Label("스킬");

        EditorGUILayout.BeginHorizontal(); // (7)
        {
            if (GUILayout.Button("불러오기"))
            {
                ImportJsonSkill();
            }
            if (GUILayout.Button("내보내기"))
            {
                ExportJsonSkill();
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
        scrollPosSkill = EditorGUILayout.BeginScrollView(scrollPosSkill); // (8)

        int skillCount = skillList.Count;
        for (int i = 0; i < skillCount; i++)
        {
            EditorGUILayout.BeginVertical("box");
            {
                var sk = skillList[i];

                sk.Id = EditorGUILayout.IntField("스킬 ID", sk.Id);
                sk.Type = (SkillType)EditorGUILayout.EnumFlagsField("타입", sk.Type);
                EditorGUILayout.LabelField("설명");
                sk.Description = EditorGUILayout.TextArea(sk.Description, GUILayout.Height(150));

                //감정
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
        string path = EditorUtility.OpenFilePanel("캐릭터 정보 열기", dataPath, "json");

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
        string path = EditorUtility.SaveFilePanel("캐릭터 정보 저장", dataPath, "CHARACTER", "json");

        if (string.IsNullOrEmpty(path)) return;

        CharacterWrapper characterWrapper = new CharacterWrapper();
        characterWrapper.characters = characterList;
        string json = JsonUtility.ToJson(characterWrapper, true);
        Debug.Log(json);

        File.WriteAllText(path, json);
    }

    private void ImportJsonSkill()
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

    private void ExportJsonSkill()
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
