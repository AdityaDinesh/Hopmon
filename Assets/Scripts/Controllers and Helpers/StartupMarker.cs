using UnityEngine;

public static class StartupMarker
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Mark1() => Debug.Log("STARTUP_MARKER_1: SubsystemRegistration");

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void Mark2() => Debug.Log("STARTUP_MARKER_2: AfterAssembliesLoaded");

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    static void Mark3() => Debug.Log("STARTUP_MARKER_3: BeforeSplashScreen");

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Mark4() => Debug.Log("STARTUP_MARKER_4: BeforeSceneLoad");

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Mark5() => Debug.Log("STARTUP_MARKER_5: AfterSceneLoad");
}