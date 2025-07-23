using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] 
    private Slider _healthSlider;

    [SerializeField] 
    private Image _fillImage;

    [SerializeField] 
    private Image _backgroundImage;

    [SerializeField] 
    private bool _enableAnimation = true;

    [SerializeField] 
    private float _animationSpeed = 2f;

    private float _targetHealth = 1f;
    private float _maxHealth = 100f;
    private bool _isInitialized = false;

    private void Awake()
    {
        SetupHealthBar();
    }

    private void Update()
    {
        if (_enableAnimation && _isInitialized)
        {
            AnimateHealthBar();
        }
    }

    private void SetupHealthBar()
    {
        if (_healthSlider == null)
            _healthSlider = GetComponent<Slider>();

        if (_fillImage == null && _healthSlider != null)
            _fillImage = _healthSlider.fillRect.GetComponent<Image>();

        if (_healthSlider != null)
        {
            _healthSlider.minValue = 0f;
            _healthSlider.maxValue = 1f;
            _healthSlider.value = 1f;
        }
    }

    public void Initialize(float maxHealth)
    {
        _maxHealth = maxHealth;
        _targetHealth = 1f;

        if (_healthSlider != null)
        {
            _healthSlider.value = 1f;
        }

        //UpdateHealthBarColor(1f);
        _isInitialized = true;
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        _maxHealth = maxHealth;
        _targetHealth = Mathf.Clamp01(currentHealth / maxHealth);

        if (!_enableAnimation && _healthSlider != null)
        {
            _healthSlider.value = _targetHealth;
            //UpdateHealthBarColor(_targetHealth);
        }
    }

    private void AnimateHealthBar()
    {
        if (_healthSlider == null)
        {
            return;
        }

        float currentValue = _healthSlider.value;
        float newValue = Mathf.MoveTowards(currentValue, _targetHealth, _animationSpeed * Time.deltaTime);

        _healthSlider.value = newValue;
        //UpdateHealthBarColor(newValue);
    }

    public void SetVisibility(bool visible)
    {
        gameObject.SetActive(visible);
    }

    [ContextMenu("Test Damage")]
    public void TestDamage()
    {
        UpdateHealth(_maxHealth * 0.5f, _maxHealth);
    }

    [ContextMenu("Test Critical Health")]
    public void TestCriticalHealth()
    {
        UpdateHealth(_maxHealth * 0.2f, _maxHealth);
    }

    [ContextMenu("Test Full Health")]
    public void TestFullHealth()
    {
        UpdateHealth(_maxHealth, _maxHealth);
    }
}