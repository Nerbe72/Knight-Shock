using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public static class SceneStackTracer
{
    public static bool isMain;

    private static Stack<IWindowController> sceneStack = new Stack<IWindowController>();

    //홈으로 돌아가기
    public static void InitStack()
    {
        while(sceneStack.Count > 1)
        {
            PopScene();
        }

        PeekScene();
    }

    public static void AddScene(IWindowController _window)
    {
        sceneStack.Push(_window);
        PeekScene();
    }

    public static IWindowController PopScene()
    {
        int count = sceneStack.Count;
        if (count <= 1)
        {
            return null;
        }

        IWindowController closeTarget = sceneStack.Pop();
        closeTarget.Self.SetActive(false);

        PeekScene();

        return closeTarget;
    }

    public static IWindowController PeekScene()
    {
        IWindowController outWindow = null;
        if (sceneStack.TryPeek(out outWindow))
        {
            DebugStack();
            isMain = false;
            return outWindow;
        }
        DebugStack();
        isMain = true;
        return null;
    }

    private static void DebugStack()
    {
        foreach (var scene in sceneStack)
        {
            Debug.Log($"씬스택 <color=magenta>// {scene.Self.gameObject.name} //</color>");
        }
        Debug.Log($"씬스택 <color=orange>////////////////////////////////////////////</color>");
    }
}
