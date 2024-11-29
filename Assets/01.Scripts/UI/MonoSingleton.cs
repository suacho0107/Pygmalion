//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
//{
//    private static T _instance;

//    public static T Instance
//    {
//        get
//        {
//            if (_instance == null)
//            {
//                _instance = FindObjectOfType<T>();

//                if (_instance == null)
//                {
//                    Debug.LogError($"{typeof(T)} 싱글톤 인스턴스가 존재하지 않습니다.");
//                }
//            }

//            return _instance;
//        }
//    }

//    protected virtual void Awake()

//    {
//        if (_instance == null)
//        {
//            _instance = this as T;
//            DontDestroyOnLoad(gameObject);
//        }
//    }

//}
