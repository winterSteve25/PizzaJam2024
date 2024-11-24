using DG.Tweening;
using UnityEngine;
using Worlds;

namespace Combat.Units.Shaman
{
    public class GoblinShaman : EnemyUnit
    {
        [SerializeField] private Totem totemPrefab;

        private Totem _redTotem;
        private Totem _blueTotem;

        private bool _started;
        private bool _counterAttacking;
        private Unit _counterAttackingTarget;

        public bool BlueMode { get; private set; } = false;

        public override void TurnStarted()
        {
            base.TurnStarted();

            if (_counterAttacking)
            {
                CounterAtk();
                return;
            }

            if (!_started)
            {
                _started = true;

                _redTotem = Instantiate(totemPrefab);
                _blueTotem = Instantiate(totemPrefab);

                var world = World.Current;

                _redTotem.transform.position =
                    world.CellToWorld(world.PlayableArea.GetRandomPoint(_redTotem.Size), _redTotem.Size);
                _redTotem.AddToWorld(this);

                _blueTotem.transform.position =
                    world.CellToWorld(world.PlayableArea.GetRandomPoint(_blueTotem.Size), _blueTotem.Size);
                _blueTotem.AddToWorld(this);

                _redTotem.SetMode(false);
                _blueTotem.SetMode(true);
            }

            CombatManager.Current.NextTurn();
        }

        private void CounterAtk()
        {
            _counterAttacking = false;

            var targetLocation = _counterAttackingTarget.gridPosition;
            transform.DOBlendableMoveBy(
                    new Vector3(World.Current.CellToWorld(targetLocation, Size).x - transform.position.x, 0, 0), 1)
                .OnComplete(Callback);

            transform.DOBlendableMoveBy(new Vector3(0, 5, 0), 0.5f)
                .SetEase(Ease.OutCubic)
                .SetLoops(2, LoopType.Yoyo);

            void Callback()
            {
                for (int i = 0; i < Size.x; i++)
                {
                    for (int j = 0; j < Size.y; j++)
                    {
                        var loc = new Vector2Int(i, j) + targetLocation;
                        if (!World.Current.GetUnitAt(loc, out var unit)) continue;
                        var dir = loc - targetLocation;

                        if (dir.x == 0 && dir.y == 0)
                        {
                            dir = new Vector2Int(0, -1);
                            // todo what if bottom is not emtpy
                        }

                        World.Current.MoveUnit(loc, loc + dir);
                        unit.transform.DOMove(World.Current.CellToWorld(loc + dir, unit.Size), 0.1f)
                            .SetEase(Ease.OutCubic);
                    }
                }

                World.Current.MoveUnit(gridPosition, targetLocation);
                CombatManager.Current.NextTurn();
            }
        }

        public void CounterAttack(Unit attacker)
        {
            CombatManager.Current.TakeOver(this);
            _counterAttacking = true;
            _counterAttackingTarget = attacker;
        }
    }
}