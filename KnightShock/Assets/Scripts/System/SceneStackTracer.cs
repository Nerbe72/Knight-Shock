using System.Collections.Generic;
using UnityEngine;

public static class SceneStackTracer
{
    private static Stack<string> sceneStack = new Stack<string>();

    public static void InitStack()
    {
        if (sceneStack.Count == 0)
        {
            sceneStack.Push("/");
        }

        if (sceneStack.Peek() == "/")
        {
            sceneStack.Push("Lobby");
        }
    }

    public static void ResetStack()
    {
        sceneStack.Clear();
        InitStack();
    }

    public static void AddScene(string _name)
    {
        sceneStack.Push(_name);
    }

    public static string PopScene()
    {
        return sceneStack.Pop();
    }

    public static string PeekScene()
    {
        string outScene = "";
        if (sceneStack.TryPeek(out outScene))
        {
            return outScene;
        }

        return null;
    }
}
