using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace Combat.DmgNumber
{
    public class DamageNumberManager : MonoBehaviour
    {
        public static DamageNumberManager Current { get; private set; }

        [SerializeField] private DamageNumber prefab;

        private IObjectPool<DamageNumber> _numbers;

        private void Awake()
        {
            Current = this;
        }

        private void Start()
        {
            _numbers = new ObjectPool<DamageNumber>(
                () => Instantiate(prefab, transform),
                x => x.gameObject.SetActive(true),
                x => x.gameObject.SetActive(false),
                Destroy
            );
        }

        public void CreateNumber(float number, Vector3 position, bool heal, bool crit)
        {
            var obj = _numbers.Get();
            var rand = Random.insideUnitSphere * 1.5f;
            rand.z = 0;
            obj.Init(heal ? $"+{number:F1}" : number.ToString("F1"), position + rand, () => _numbers.Release(obj), heal, crit);
        }
    }
}