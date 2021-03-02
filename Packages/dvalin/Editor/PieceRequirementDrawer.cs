using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dvalin.Editor
{
    // [CustomPropertyDrawer(typeof(Piece.Requirement))]
    // public class PieceRequirementDrawer : PropertyDrawer
    // {
    //     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //     {
    //         EditorGUI.BeginProperty(position, label, property);

    //         // position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

    //         var resItem = property.FindPropertyRelative("m_resItem");
    //         var amount = property.FindPropertyRelative("m_amount");
    //         var amountPerLevel = property.FindPropertyRelative("m_amountPerLevel");
    //         var recover = property.FindPropertyRelative("m_recover");

    //         var width = (position.width - 50) / 3;
    //         var resItemRect = new Rect(position.x, position.y, width, position.height);
    //         var amountRect = new Rect(position.x + width + 5, position.y, width, position.height);
    //         var amountPerLevelRect = new Rect(position.x + width * 2 + 5, position.y, width, position.height);
    //         var recoverRect = new Rect(position.x + width * 3 + 5, position.y, 50, position.height);

    //         EditorGUI.ObjectField(resItemRect, resItem, GUIContent.none);
    //         amount.intValue = EditorGUI.IntField(amountRect, amount.intValue);
    //         amountPerLevel.intValue = EditorGUI.IntField(amountPerLevelRect, amountPerLevel.intValue);
    //         recover.boolValue = EditorGUI.Toggle(recoverRect, recover.boolValue);

    //         EditorGUI.EndProperty();
    //     }
    // }
}