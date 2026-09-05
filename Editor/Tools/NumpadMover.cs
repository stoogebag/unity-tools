using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class NumpadMover
{
    static NumpadMoverSettings S => NumpadMoverSettings.Instance;
    static float Full => S.moveFull;
    static float Small => S.moveSmall;

    static Vector3 NumpadUpDir => S.Axis == NumpadMoverSettings.AxisChoice.XY ? Vector3.up : Vector3.forward;
    static Vector3 NumpadDownDir => S.Axis == NumpadMoverSettings.AxisChoice.XY ? Vector3.down : Vector3.back;
    static Vector3 AltUpDir => S.Axis == NumpadMoverSettings.AxisChoice.XY ? Vector3.forward : Vector3.up;
    static Vector3 AltDownDir => S.Axis == NumpadMoverSettings.AxisChoice.XY ? Vector3.back : Vector3.down;

    [Shortcut("Move Object/Up", KeyCode.Keypad8)]
    static void MoveUp() { MoveSelected(NumpadUpDir, Full); }

    [Shortcut("Move Object/Up Small", KeyCode.Keypad8, ShortcutModifiers.Control)]
    static void MoveUpSmall() { MoveSelected(NumpadUpDir, Small); }

    [Shortcut("Move Object/Down", KeyCode.Keypad2)]
    static void MoveDown() { MoveSelected(NumpadDownDir, Full); }

    [Shortcut("Move Object/Down Small", KeyCode.Keypad2, ShortcutModifiers.Control)]
    static void MoveDownSmall() { MoveSelected(NumpadDownDir, Small); }

    [Shortcut("Move Object/Alt Up", KeyCode.PageUp)]
    static void MoveAltUp() { MoveSelected(AltUpDir, Full); }

    [Shortcut("Move Object/Alt Up Small", KeyCode.PageUp, ShortcutModifiers.Control)]
    static void MoveAltUpSmall() { MoveSelected(AltUpDir, Small); }

    [Shortcut("Move Object/Alt Down", KeyCode.PageDown)]
    static void MoveAltDown() { MoveSelected(AltDownDir, Full); }

    [Shortcut("Move Object/Alt Down Small", KeyCode.PageDown, ShortcutModifiers.Control)]
    static void MoveAltDownSmall() { MoveSelected(AltDownDir, Small); }

    [Shortcut("Move Object/Left", KeyCode.Keypad4)]
    static void MoveLeft() { MoveSelected(Vector3.left, Full); }

    [Shortcut("Move Object/Left Small", KeyCode.Keypad4, ShortcutModifiers.Control)]
    static void MoveLeftSmall() { MoveSelected(Vector3.left, Small); }

    [Shortcut("Move Object/Right", KeyCode.Keypad6)]
    static void MoveRight() { MoveSelected(Vector3.right, Full); }

    [Shortcut("Move Object/Right Small", KeyCode.Keypad6, ShortcutModifiers.Control)]
    static void MoveRightSmall() { MoveSelected(Vector3.right, Small); }

    [Shortcut("Rotate Object/Rotate 90", KeyCode.Keypad7)]
    static void Rotate90() { RotateSelected(S.rotate90); }

    [Shortcut("Rotate Object/Rotate Small", KeyCode.Keypad7, ShortcutModifiers.Control)]
    static void RotateSmall() { RotateSelected(S.rotateSmall); }

    [Shortcut("Rotate Object/Rotate -90", KeyCode.Keypad9)]
    static void RotateMinus90() { RotateSelected(-S.rotate90); }

    [Shortcut("Rotate Object/Rotate -Small", KeyCode.Keypad9, ShortcutModifiers.Control)]
    static void RotateMinusSmall() { RotateSelected(-S.rotateSmall); }

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
                t.Rotate(0, -degrees, 0);
            }
        }
    }
}
