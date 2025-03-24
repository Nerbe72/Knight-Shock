using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BootstrapManager : MonoBehaviour
{
    private void Start()
    {
        IInitializable[] initializables = Resources.FindObjectsOfTypeAll<MonoBehaviour>().OfType<IInitializable>().ToArray();

        List<IInitializable> sortedInitializables = initializables.OrderBy(init => init.InitializationPriority).ToList();

        foreach (var init in sortedInitializables)
        {
            init.Initialize();
        }
    }
}
