using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class AutomaticShooting : Shooting
{
    [SerializeField] private InputActionReference _shootAction;
    [SerializeField] private float _cooldown;

    private float _lastShotTime;

    public UnityEvent OnShoot;

    private void Update()
    {
        if (_shootAction.action.IsPressed() && FinishCooldown())
        {
            Shoot();
            _lastShotTime = Time.time;
        }
    }

    private bool FinishCooldown() => Time.time - _lastShotTime >= _cooldown;

    private void Shoot() => OnShoot.Invoke();
}
