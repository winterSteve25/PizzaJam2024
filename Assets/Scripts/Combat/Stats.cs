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
        [field: SerializeField, Range(0, 1)] public float CritChance { get; private set; }
        [field: SerializeField, Range(0, 4)] public float CritMultiplier { get; private set; }
        [field: SerializeField] public int Taunt { get; private set; }

        public Stats(float attack, float defence, float maxHp, float speed, float critChance, float critMultiplier, int taunt)
        {
            Attack = attack;
            Defence = defence;
            MaxHp = maxHp;
            Speed = speed;
            CritChance = critChance;
            CritMultiplier = critMultiplier;
            Taunt = taunt;
        }

        public static Stats operator +(Stats a, Stats b)
        {
            return new Stats()
            {
                Attack = a.Attack + b.Attack,
                Defence = a.Defence + b.Defence,
                MaxHp = a.MaxHp + b.MaxHp,
                Speed = a.Speed + b.Speed,
                CritChance = a.CritChance + b.CritChance,
                CritMultiplier = a.CritMultiplier + b.CritMultiplier,
                Taunt = a.Taunt + b.Taunt
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
                CritChance = a.CritChance * b.CritChance,
                CritMultiplier = a.CritMultiplier * b.CritMultiplier,
                Taunt = a.Taunt * b.Taunt
            };
        }

        public float Dot(Stats other)
        {
            return Attack * other.Attack +
                   Defence * other.Defence +
                   MaxHp * other.MaxHp +
                   Speed * other.Speed +
                   CritChance * other.CritChance +
                   CritMultiplier * other.CritMultiplier +
                   Taunt * other.Taunt;
        }
    }
}