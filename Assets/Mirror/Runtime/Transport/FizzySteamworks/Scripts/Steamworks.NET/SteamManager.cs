using UnityEngine;
#if !DISABLESTEAMWORKS
using Steamworks;
#endif

[DisallowMultipleComponent]
public class SteamManager : MonoBehaviour
{
#if !DISABLESTEAMWORKS
    protected static bool s_EverInitialized = false;

    protected static SteamManager s_instance;
    protected static SteamManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                var manager = new GameObject("SteamManager");
                s_instance = manager.AddComponent<SteamManager>();
                DontDestroyOnLoad(manager);
            }
            return s_instance;
        }
    }

    protected bool m_bInitialized = false;
    public static bool Initialized => Instance.m_bInitialized;

    private SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;

    [AOT.MonoPInvokeCallback(typeof(SteamAPIWarningMessageHook_t))]
    protected static void SteamAPIDebugTextHook(int nSeverity, System.Text.StringBuilder pchDebugText)
    {
        Debug.LogWarning(pchDebugText);
    }

#if UNITY_2019_3_OR_NEWER
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void InitOnPlayMode()
    {
        s_EverInitialized = false;
        s_instance = null;
    }
#endif

    protected virtual void Awake()
    {
        if (s_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        s_instance = this;

        if (s_EverInitialized)
        {
            Debug.LogWarning("SteamAPI already initialized. Only one instance of SteamManager should exist.");
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        if (!Packsize.Test())
        {
            Debug.LogError("[Steamworks.NET] Packsize Test failed. Check that the correct Steamworks.NET version is being used.");
            return;
        }

        if (!DllCheck.Test())
        {
            Debug.LogError("[Steamworks.NET] DllCheck Test failed. Ensure all Steamworks binaries are correct.");
            return;
        }

        try
        {
            // Replace AppId_t.Invalid with a valid App ID for release builds.
            if (SteamAPI.RestartAppIfNecessary(AppId_t.Invalid))
            {
                Application.Quit();
                return;
            }
        }
        catch (System.DllNotFoundException e)
        {
            Debug.LogError("[Steamworks.NET] Steam API DLL not found. Ensure steam_api.dll is in the correct location.\n" + e);
            Application.Quit();
            return;
        }

        m_bInitialized = SteamAPI.Init();
        if (!m_bInitialized)
        {
            Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed. See Steamworks documentation for possible reasons.");
            return;
        }

        s_EverInitialized = true;
    }

    protected virtual void OnEnable()
    {
        if (!m_bInitialized)
            return;

        if (m_SteamAPIWarningMessageHook == null)
        {
            m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
            SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);
        }
    }

    protected virtual void OnDestroy()
    {
        if (s_instance != this)
            return;

        s_instance = null;

        if (m_bInitialized)
        {
            SteamAPI.Shutdown();
            m_bInitialized = false;
        }
    }

    protected virtual void Update()
    {
        if (m_bInitialized)
        {
            SteamAPI.RunCallbacks();
        }
    }
#else
    public static bool Initialized => false;
#endif
}
