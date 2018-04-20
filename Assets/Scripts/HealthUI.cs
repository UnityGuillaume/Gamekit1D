using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    public GameObject healthIconPrefab;

    protected Animator[] m_HealthIconAnimators;

    protected readonly int m_HashActivePara = Animator.StringToHash("Active");
    protected readonly int m_HashInactiveState = Animator.StringToHash("Inactive");
    protected const float k_HeartIconAnchorWidth = 0.041f;

    protected int _lastHealth;

    IEnumerator Start()
    {
        yield return null;

        m_HealthIconAnimators = new Animator[PlayerController.Instance.maxHealth];

        _lastHealth = PlayerController.Instance.currentHealth;

        for (int i = 0; i < PlayerController.Instance.maxHealth; i++)
        {
            GameObject healthIcon = Instantiate(healthIconPrefab);
            healthIcon.transform.SetParent(transform);
            RectTransform healthIconRect = healthIcon.transform as RectTransform;
            healthIconRect.anchoredPosition = Vector2.zero;
            healthIconRect.sizeDelta = Vector2.zero;
            healthIconRect.anchorMin += new Vector2(k_HeartIconAnchorWidth, 0f) * i;
            healthIconRect.anchorMax += new Vector2(k_HeartIconAnchorWidth, 0f) * i;
            m_HealthIconAnimators[i] = healthIcon.GetComponent<Animator>();

            if (PlayerController.Instance.currentHealth < i + 1)
            {
                m_HealthIconAnimators[i].Play(m_HashInactiveState);
                m_HealthIconAnimators[i].SetBool(m_HashActivePara, false);
            }
        }
    }

    void Update()
    {
        if(_lastHealth != PlayerController.Instance.currentHealth)
            UpdateUI();
    }

    public void UpdateUI()
    {
        if (m_HealthIconAnimators == null)
            return;

        for (int i = 0; i < m_HealthIconAnimators.Length; i++)
        {
            m_HealthIconAnimators[i].SetBool(m_HashActivePara, PlayerController.Instance.currentHealth >= i + 1);
        }
    }
}
