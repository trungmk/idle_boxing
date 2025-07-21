using UnityEngine;

public class CharacterAIController : MonoBehaviour
{
    private Character _character;
    private bool _isInitialized;
    private float _decisionTimer;
    private const float DECISION_INTERVAL = 0.5f;

    public void Initialize(Character character)
    {
        _character = character;
        _isInitialized = true;
    }

    public void UpdateLogic(float deltaTime)
    {
        if (!_isInitialized) return;

        _decisionTimer += deltaTime;

        if (_decisionTimer >= DECISION_INTERVAL)
        {
            MakeDecision();
            _decisionTimer = 0f;
        }
    }

    private void MakeDecision()
    {
        if (_character.IsDead || _character.IsStunned) return;

        Character nearestEnemy = FindNearestEnemy();

        if (nearestEnemy != null)
        {
            float distance = _character.GetDistanceTo(nearestEnemy);

            //if (distance <= _character.Stats.RangeAttackPower && _character.CanAttack)
            //{
            //    _character.AttackTarget(nearestEnemy);
            //}
            //else if (distance <= _character.Stats.DetectionRange)
            //{
            //    _character.MoveTo(nearestEnemy.Transform.position);
            //    _character.ChangeState(CharacterStateType.Chase);
            //}
        }
        else if (_character.CurrentState != CharacterStateType.Idle)
        {
            _character.ChangeState(CharacterStateType.Idle);
        }
    }

    private Character FindNearestEnemy()
    {
        //Collider[] colliders = Physics.OverlapSphere(_character.Transform.position, _character.Stats.DetectionRange);

        Character nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        //foreach (var collider in colliders)
        //{
        //    Character other = collider.GetComponent<Character>();
        //    if (other != null && _character.IsEnemyOf(other) && !other.IsDead)
        //    {
        //        float distance = Vector3.Distance(_character.Transform.position, other.Transform.position);
        //        if (distance < nearestDistance)
        //        {
        //            nearestDistance = distance;
        //            nearestEnemy = other;
        //        }
        //    }
        //}

        return nearestEnemy;
    }
}