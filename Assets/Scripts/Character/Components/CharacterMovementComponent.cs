using UnityEngine;

public class CharacterMovementComponent : CharacterComponentBase
{
    private Character _character;
    private bool _isInitialized;
    private bool _canMove = true;
    private Vector3 _targetPosition;
    private bool _isMoving;
    private float _currentSpeed;

    public bool CanMove => _canMove;
    public bool IsMoving => _isMoving;
    public float CurrentSpeed => _currentSpeed;
    public Vector3 TargetPosition => _targetPosition;

    public override void Initialize(Character character)
    {
        _character = character;
        _isInitialized = true;
    }

    public override void UpdateLogic(float deltaTime)
    {
        if (!_isInitialized || !_canMove) return;

        UpdateMovement(deltaTime);
    }

    private void UpdateMovement(float deltaTime)
    {
        if (!_isMoving) return;

        Vector3 direction = (_targetPosition - _character.Transform.position).normalized;
        float distance = Vector3.Distance(_character.Transform.position, _targetPosition);

        if (distance <= 0.1f)
        {
            Stop();
            return;
        }

        float moveSpeed = _character.Stats.MoveSpeed;
        _currentSpeed = moveSpeed;

        Vector3 velocity = direction * moveSpeed;

        if (_character.Rigidbody != null)
        {
            _character.Rigidbody.linearVelocity = new Vector3(velocity.x, _character.Rigidbody.linearVelocity.y, velocity.z);
        }
        else
        {
            _character.Transform.position += velocity * deltaTime;
        }

        // Rotate towards movement direction
        if (direction != Vector3.zero)
        {
            _character.Transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void MoveTo(Vector3 targetPosition)
    {
        if (!_canMove) return;

        _targetPosition = targetPosition;
        _isMoving = true;
    }

    public void LookAt(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - _character.Transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            _character.Transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void Stop()
    {
        _isMoving = false;
        _currentSpeed = 0f;

        if (_character.Rigidbody != null)
        {
            _character.Rigidbody.linearVelocity = new Vector3(0f, _character.Rigidbody.linearVelocity.y, 0f);
        }
    }

    public void SetCanMove(bool canMove)
    {
        _canMove = canMove;
        if (!canMove)
        {
            Stop();
        }
    }
}