using System;
using UnityEngine;

namespace Combat
{
    [Serializable]
    public struct Stats
    {
        [field: SerializeField] public float Attack { get; private set; }
        [field: SerializeField] public float Defence { get; private set; }
        [field: SerializeField] public float MaxHp { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField, Range(0, 1)] public float CritRate { get; private set; }
        [field: SerializeField, Range(0, 4)] public float CritMult { get; private set; }

        public Stats(float attack, float defence, float maxHp, float speed, float critRate, float critMult)
        {
            Attack = attack;
            Defence = defence;
            MaxHp = maxHp;
            Speed = speed;
            CritRate = critRate;
            CritMult = critMult;
        }

        public static Stats operator +(Stats a, Stats b)
        {
            return new Stats()
            {
                Attack = a.Attack + b.Attack,
                Defence = a.Defence + b.Defence,
                MaxHp = a.MaxHp + b.MaxHp,
                Speed = a.Speed + b.Speed,
                CritRate = a.CritRate + b.CritRate,
                CritMult = a.CritMult + b.CritMult
            };
        }

        public static Stats operator *(Stats a, Stats b)
        {
            return new Stats()
            {
                Attack = a.Attack * b.Attack,
                Defence = a.Defence * b.Defence,
                MaxHp = a.MaxHp * b.MaxHp,
                Speed = a.Speed * b.Speed,
                CritRate = a.CritRate * b.CritRate,
                CritMult = a.CritMult * b.CritMult
            };
        }
    }
}