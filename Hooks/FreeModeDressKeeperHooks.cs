using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine.Events;
using HarmonyLib;


namespace COM3D2.UndressUtil.Plugin.Hooks
{
    public class FreeModeDressKeeperHooks
    {
        public static readonly UnityEvent OnFreeModeDressKeeperRestore = new UnityEvent();

        static Harmony harmony;
        public static bool Available { get; private set; } = false;

        public static void Init()
        {
            if (harmony == null)
            {
                harmony = new Harmony("COM3D2.UndressUtil.Plugin.Hooks.FreeModeDressKeeperHooks");
                Log.LogVerbose("FreeModeDressKeeperHooks loaded");
                HookUnityInjectorLoader();
            }
        }


        static Assembly AssemblyByName(string name)
        {
            return AccessTools.AllAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == name);
        }

        static Type TypeFromAssembly(string assemblyName, string typeName)
        {
            var assembly = AssemblyByName(assemblyName);
            if (assembly is null)
            {
                Log.LogVerbose("Assembly [{0}] not found.", assemblyName);
                return null;
            }
            Type type;
            type = AccessTools.GetTypesFromAssembly(assembly).FirstOrDefault(t => t.FullName == typeName);
            
            if(type is null)
            {
                Log.LogVerbose("Type [{0}] not found in assembly [{1}]", typeName, assemblyName);
            }

            return type;
        }

        static Type GetUnityInjectorLoaderType()
        {
            return TypeFromAssembly("BepInEx.UnityInjectorLoader", "BepInEx.UnityInjectorLoader.UnityInjectorLoader");
        }

        public static void HookUnityInjectorLoader()
        {
            var type = AccessTools.TypeByName("BepInEx.UnityInjectorLoader.UnityInjectorLoader");
            if (type == null)
            {
                Log.LogVerbose("UnityInjectorLoader is not available");
                return;
            }

            var finalizer = new HarmonyMethod(typeof(FreeModeDressKeeperHooks), nameof(PatchFreeModeDressKeeper));

            var method = AccessTools.Method(type, "Init");
            harmony.Patch(method,
                finalizer: finalizer);

            Log.LogVerbose("UnityInjector hook created, waiting for unityinjector load");
        }

        public static void PatchFreeModeDressKeeper()
        {
            try
            {
                Log.LogVerbose("Now hooking into FreeModeDressKeeper...");
                var type = AccessTools.TypeByName("COM3D2.FreeModeDressKeeper.Plugin.FreeModeDressKeeper");
                if (type == null)
                {
                    Log.LogVerbose("FreeModeDressKeeperHooks is not available");
                    return;
                }

                var postfix = new HarmonyMethod(typeof(FreeModeDressKeeperHooks), nameof(FreeModeDressKeeper_RestoreUndressingStatusCo));

                var method = AccessTools.Method(type, "RestoreUndressingStatusCo");
                harmony.Patch(method,
                    postfix: postfix);

                Available = true;

            }
            catch (Exception e)
            {
                Log.LogError(e);
            }
        }

        public class EnumeratorWrapper : IEnumerable
        {
            public IEnumerator enumerator;
            public Action prefix;
            public Action finalizer;

            public IEnumerator GetEnumerator()
            {
                prefix();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        yield return enumerator.Current;
                    }
                }
                finally
                {
                    finalizer();

                }
            }
        }

        public static void FreeModeDressKeeper_RestoreUndressingStatusCo(ref IEnumerator __result)
        {
            if(UndressUtilPlugin.Instance.IsKeepYotogiDressState)
            {
                Log.LogInfo("Supressing FreeModeDressKeeper");
                __result = Noop();
            }
        }

        static IEnumerator Noop()
        {
            yield return null;
        }
    }
}
