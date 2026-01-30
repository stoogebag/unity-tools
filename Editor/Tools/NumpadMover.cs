using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class NumpadMover
{
    [Shortcut("Move Object/Up", KeyCode.Keypad8)]
    static void MoveUp() { MoveSelected(Vector3.up, 1f); }
    
    [Shortcut("Move Object/Up Half", KeyCode.Keypad8, ShortcutModifiers.Control)]
    static void MoveUpHalf() { MoveSelected(Vector3.up, 0.5f); }
    
    [Shortcut("Move Object/Down", KeyCode.Keypad2)]
    static void MoveDown() { MoveSelected(Vector3.down, 1f); }
    
    [Shortcut("Move Object/Down Half", KeyCode.Keypad2, ShortcutModifiers.Control)]
    static void MoveDownHalf() { MoveSelected(Vector3.down, 0.5f); }
    
    [Shortcut("Move Object/Left", KeyCode.Keypad4)]
    static void MoveLeft() { MoveSelected(Vector3.left, 1f); }
    
    [Shortcut("Move Object/Left Half", KeyCode.Keypad4, ShortcutModifiers.Control)]
    static void MoveLeftHalf() { MoveSelected(Vector3.left, 0.5f); }
    
    [Shortcut("Move Object/Right", KeyCode.Keypad6)]
    static void MoveRight() { MoveSelected(Vector3.right, 1f); }
    
    [Shortcut("Move Object/Right Half", KeyCode.Keypad6, ShortcutModifiers.Control)]
    static void MoveRightHalf() { MoveSelected(Vector3.right, 0.5f); }

    [Shortcut("Rotate Object/Rotate 90", KeyCode.Keypad7)]
    static void Rotate90() { RotateSelected(90f); }
    
    [Shortcut("Rotate Object/Rotate 45", KeyCode.Keypad7, ShortcutModifiers.Control)]
    static void Rotate45() { RotateSelected(45f); }
    
    [Shortcut("Rotate Object/Rotate -90", KeyCode.Keypad9)]
    static void RotateMinus90() { RotateSelected(-90f); }
    
    [Shortcut("Rotate Object/Rotate -45", KeyCode.Keypad9, ShortcutModifiers.Control)]
    static void RotateMinus45() { RotateSelected(-45f); }

    static void MoveSelected(Vector3 direction, float distance)
    {
        if (Selection.transforms.Length > 0)
        {
            Undo.RecordObjects(Selection.transforms, "Move Objects");
            foreach (Transform t in Selection.transforms)
            {
                t.position += direction * distance;
            }
        }
    }

    static void RotateSelected(float degrees)
    {
        if (Selection.transforms.Length > 0)
        {
            Undo.RecordObjects(Selection.transforms, "Rotate Objects");
            foreach (Transform t in Selection.transforms)
            {
                t.Rotate(0, 0, degrees);
            }
        }
    }

}
