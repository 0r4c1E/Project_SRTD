using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BarParu
{
    public class ParuPool : MonoBehaviour
    {
        public static ParuPool inst;
        enum PoolOption
        {
            Prefab,
            Resources,
            All
        }
        // 플레이어가 세팅할 것
        [SerializeField]
        private PoolOption poolOption;  // 풀링 옵션을 지정합니다. items에 세팅해둔 오브젝트만 당겨올 것 인지, 리소스 폴더에서 가져올 것 인지를 체크합니다.

        public List<PoolObject> items = new List<PoolObject>();

        // 보유 데이터
        private readonly string resourcesPath = "ParuPoolObjects/";
        private Dictionary<string, Queue<GameObject>> poolItmes = new Dictionary<string, Queue<GameObject>>();

        private void Awake()
        {
            if (inst == null) inst = this;
        }

        public void ObjectPool()
        {
            StartCoroutine(PoolSequence());
            IEnumerator PoolSequence()
            {
                switch (poolOption)
                {
                    case PoolOption.Prefab:
                        for (int i = 0; i < items.Count; i++)
                        {
                            Queue<GameObject> _buff = new Queue<GameObject>();
                            for (int j = 0; j < items[i].poolCnt; j++)
                            {
                                _buff.Enqueue(CreateObject(i));
                            }
                            poolItmes.Add(items[i].key, _buff);
                        }
                        break;
                    case PoolOption.Resources:

                        yield return new WaitForSeconds(.5f);
                        break;
                    case PoolOption.All:

                        yield return new WaitForSeconds(.5f);
                        break;
                }
            }
        }
        private GameObject CreateObject(int id)
        {
            GameObject _obj = Instantiate(items[id].gameObject, Vector3.zero, Quaternion.identity);
            _obj.SetActive(false);
            _obj.transform.SetParent(transform);
            return _obj;
        }
        public T GetPoolItem<T>(string key) where T : Component
        {
            GameObject _obj;
            if (poolItmes[key].Count > 0)
            {
                _obj = poolItmes[key].Dequeue();
            }
            else
            {
                _obj = CreateObject(ReturnListCount(key));
            }
            if (_obj.TryGetComponent(out T component))
            {
                return component;
            }
            else
            {
                Palog.Log("요청한 컴포넌트를 오브젝트가 가지고있지 않습니다!");
                return null;
            }
        }
        public void ReturnPoolObject(string key, GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            poolItmes[key].Enqueue(obj);
        }


        private int ReturnListCount(string key)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].key == key) return i;
            }
            return -999;
        }
    }
}
