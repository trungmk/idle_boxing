using UnityEngine;

public class CharacterInputController : MonoBehaviour
{
    private Character _character;
    private bool _isInitialized;

    public void Initialize(Character character)
    {
        _character = character;
        _isInitialized = true;
    }

    public void UpdateLogic(float deltaTime)
    {
        if (!_isInitialized) return;

        HandleInput();
    }

    private void HandleInput()
    {
        // Handle touch/mouse input for player character
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            HandleAttackInput();
        }

        // Handle movement input if needed
        HandleMovementInput();
    }

    private void HandleAttackInput()
    {
        if (_character.CanAttack)
        {
            Character nearestEnemy = FindNearestEnemy();
            if (nearestEnemy != null)
            {
                _character.AttackTarget(nearestEnemy);
            }
        }
    }

    private void HandleMovementInput()
    {
        // Implement movement input if needed for player control
    }

    private Character FindNearestEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(_character.Transform.position, _character.Stats.RangeAttackPower);

        Character nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            Character other = collider.GetComponent<Character>();
            if (other != null && _character.IsEnemyOf(other) && !other.IsDead)
            {
                float distance = Vector3.Distance(_character.Transform.position, other.Transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = other;
                }
            }
        }

        return nearestEnemy;
    }
}