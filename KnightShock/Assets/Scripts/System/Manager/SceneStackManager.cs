using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public static class SceneStackManager
{
    public static bool isMain;

    private static Stack<IWindowController> sceneStack = new Stack<IWindowController>();

    //È¨À¸·Î µ¹¾Æ°¡±â
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
            isMain = (outWindow.Self.name == "Lobby_Main");
            return outWindow;
        }

        DebugStack();
        isMain = true;
        return null;
    }

    private static void DebugStack()
    {
        int i = sceneStack.Count - 1;
        foreach (var scene in sceneStack)
        {
            Debug.Log($"¾À½ºÅÃ <color=magenta>{i}Ãþ// {scene.Self.gameObject.name} //</color>");
            i--;
        }
        Debug.Log($"¾À½ºÅÃ <color=orange>////////////////////////////////////////////</color>");
    }
}
