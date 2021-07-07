using System;
using UnityEngine;
using System.Reflection;

public interface IDataResolver
{
    bool Resolve(string data, out string resolvedData);
}

public interface IPlayerPrefsData
{
    void Save();
    void Dispose();
}

[Serializable]
public class PlayerPrefsData<T> : IPlayerPrefsData where T : new()
{
    [SerializeField]
    private string m_Version;

    [SerializeField]
    private T m_Value;

    private string m_Key;
    public event Action<T> OnSaved;
    public event Action<T> OnChanged;
    private IDataResolver m_DataResolver;

    public PlayerPrefsData(string key, IDataResolver dataResolver)
    {
        m_Key = key;
        m_DataResolver = dataResolver;

        var s = PlayerPrefs.GetString(m_Key, "");
        if (m_DataResolver != null &&
            m_DataResolver.Resolve(s, out string rS))
        {
            Debug.Log($"Resolved from: {s} to: {rS}");
            s = rS;
        }
        if (string.IsNullOrEmpty(s)) m_Value = new T();
        else m_Value = JsonUtility.FromJson<PlayerPrefsData<T>>(s).m_Value;
    }

    public PlayerPrefsData(string key, T defaultValue = default)
    {
        m_Key = key;
        var s = PlayerPrefs.GetString(m_Key, "");
        if (string.IsNullOrEmpty(s)) m_Value = defaultValue;
        else m_Value = JsonUtility.FromJson<PlayerPrefsData<T>>(s).m_Value;
    }

    public T Value
    {
        get { return m_Value; }
        set
        {

            m_Value = value;
            OnChanged?.Invoke(m_Value);
        }
    }

    public static implicit operator T(PlayerPrefsData<T> d) => d.Value;

    public void Save()
    {
        m_Version = Application.version;
        PlayerPrefs.SetString(m_Key, JsonUtility.ToJson(this));
        OnSaved?.Invoke(m_Value);
    }

    public void Dispose()
    {
        m_Value = default(T);
        PlayerPrefs.SetString(m_Key, "");
    }
}


public class PlayerData : MonoBehaviour
{
    private static PlayerData _instance;
    public static PlayerData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerData>();
                if (_instance != null)
                {
                    _instance.Setup();
                }
                else
                {
                    var go = new GameObject($"[{typeof(PlayerData)}]");
                    _instance = go.AddComponent<PlayerData>();
                }
            }
            return _instance;
        }
    }

    protected void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            Setup();
            SaveAll();
        }
    }

    public PlayerPrefsData<int> score;
    public PlayerPrefsData<float> distance;
    
    public void SaveAll()
    {
        var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in fields)
        {
            var value = field.GetValue(this) as IPlayerPrefsData;
            if (value != null)
            {
                value.Save();
            }
        }
    }

    public void ClearAll()
    {
        var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in fields)
        {
            var value = field.GetValue(this) as IPlayerPrefsData;
            if (value != null) value.Dispose();
        }
    }

    public static void ResetData()
    {
        _instance.ClearAll();
        _instance.Setup();
        _instance.SaveAll();
    }

    private void Setup()
    {
        score = new PlayerPrefsData<int>("Score", 0);
        distance = new PlayerPrefsData<float>("Distance", 0);
    }
}

#if UNITY_EDITOR

namespace UnityEditor
{
    [CustomEditor(typeof(PlayerData))]
    public class PlayerDataButtons : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PlayerData myScript = (PlayerData)target;
            if (GUILayout.Button("Save all"))
            {
                myScript.SaveAll();
            }
            else if (GUILayout.Button("Clear all"))
            {
                myScript.ClearAll();
            }
        }
    }
}

#endif