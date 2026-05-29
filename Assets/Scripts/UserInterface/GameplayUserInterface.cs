using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUserInterface : UserInterface
{
    [Header("Shoot UI")]
    [SerializeField] private Button button;
    [SerializeField] private Image cooldownFillImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private Animator shootButtonAnimator;

    [Header("Shoot Cooldown")]
    [SerializeField] private float cooldownDuration = 5f;

    private Color _disabledIconColor;
    private Color _activeIconColor;

    private float cooldownTimer;
    private bool isCooldownActive;

    private void Awake()
    {
        _activeIconColor = iconImage.color;
        _disabledIconColor = iconImage.color;
        _disabledIconColor.a = 0.5f;
    }

    private void Start()
    {
        cooldownFillImage.fillAmount = 1f;
    }

    private void Update()
    {
        if (!isCooldownActive) return;

        cooldownTimer += Time.deltaTime;

        // Update radial fill
        cooldownFillImage.fillAmount = cooldownTimer / cooldownDuration;

        if (cooldownTimer >= cooldownDuration)
        {
            isCooldownActive = false;
            button.interactable = true;
            iconImage.color = _activeIconColor;
            cooldownFillImage.fillAmount = 1f;
            shootButtonAnimator.SetTrigger("pop");
            AudioController.Instance.PlaySFX(SfxSoundType.FireReady);
        }
    }

    public void OnShootButtonPressed()
    {
        if (isCooldownActive) return;

        Debug.Log("Ability Used");

        PlayerController.Instance.ShootFireball();

        isCooldownActive = true;
        cooldownTimer = 0;
        button.interactable = false;
        iconImage.color = _disabledIconColor;
        cooldownFillImage.fillAmount = 0f;
    }

    public override void Show()
    {
        base.Show();
        cooldownFillImage.fillAmount = 1f;
        //iconImage.color = _activeIconColor;
    }

    public override void Hide()
    {
        base.Hide();
    }
}
