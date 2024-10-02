using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.TextCore.Text;
using TMPro;

[CustomEditor(typeof(Recipe2x2))]
public class RecipeScriptableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Recipe2x2 recipeScriptableObject = (Recipe2x2)target;

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("OUTPUT COUNT", new GUIStyle { fontStyle = FontStyle.Bold });
        GUILayout.FlexibleSpace();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("outputCount"), GUIContent.none, true, GUILayout.Width(150));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginVertical();

        Sprite sprite = null;
        if (recipeScriptableObject.output != null)
        {
            sprite = recipeScriptableObject.output.Image;
        }
        DrawSprite(sprite, 150, 150);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("output"), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        GUILayout.Label("RECIPE", new GUIStyle { fontStyle = FontStyle.Bold });
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        sprite = null;
        if (recipeScriptableObject.item_01 != null)
        {
            sprite = recipeScriptableObject.item_01.Image;
        }
        DrawSprite(sprite, 150, 150);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("item_01"), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        sprite = null;
        if (recipeScriptableObject.item_11 != null)
        {
            sprite = recipeScriptableObject.item_11.Image;
        }
        DrawSprite(sprite, 150, 150);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("item_11"), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        sprite = null;
        if (recipeScriptableObject.item_00 != null)
        {
            sprite = recipeScriptableObject.item_00.Image;
        }
        DrawSprite(sprite, 150, 150);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("item_00"), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        sprite = null;
        if (recipeScriptableObject.item_10 != null)
        {
            sprite = recipeScriptableObject.item_10.Image;
        }
        DrawSprite(sprite, 150, 150);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("item_10"), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSprite(Sprite sprite, float width, float height)
    {
        if (sprite != null)
        {
            Rect rect = GUILayoutUtility.GetRect(width, height);
            Rect spriteRect = new Rect(sprite.rect.x / sprite.texture.width,
                                        sprite.rect.y / sprite.texture.height,
                                        sprite.rect.width / sprite.texture.width,
                                        sprite.rect.height / sprite.texture.height);
            GUI.DrawTextureWithTexCoords(rect, sprite.texture, spriteRect);
        }
        else
        {
            GUILayout.Box("", GUILayout.Width(width), GUILayout.Height(height));
        }
    }
}
