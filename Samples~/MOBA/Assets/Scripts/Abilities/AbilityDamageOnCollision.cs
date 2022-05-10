using LEGS;
using TMPro;
using UnityEngine;
using LEGS.Abilities;

namespace MOBAExample
{
	[RequireComponent(typeof(AbilityInfo))]
	public class AbilityDamageOnCollision : MonoBehaviour
	{
		[SerializeField] private GameObject m_HitEffectPrefab;
		[SerializeField] private GameObject m_DamagePopupPrefab;

		private MOBAAbility m_Ability;
		private MOBACharacter m_Character;

		private void Start()
		{
			AbilityInfo info = GetComponent<AbilityInfo>();
			m_Ability = (MOBAAbility)info.Ability;
			m_Character = (MOBACharacter)info.Caster;
		}

		private void OnCollisionEnter(Collision collision)
		{
			float damage = 0;
			// MOBACharacter
			if (collision.gameObject.TryGetComponent(out MOBACharacter character))
				damage = character.ApplyAbilityDamage(m_Ability, m_Character);

			//IDamageable
			else if (collision.gameObject.TryGetComponent(out IDamageable damageable))
			{
				// Calculate damage to apply based on attack type & character stats
				damage = m_Ability.AttackDamageScale;
				if (m_Ability.DamageType == DamageTypes.Magical)
					damage = m_Character.GetAttribute(CharacterTrait.AbilityPower).CurrentValue * m_Ability.AttackDamageScale;
				else if (m_Ability.DamageType == DamageTypes.Physical)
					damage = m_Character.GetAttribute(CharacterTrait.AttackDamage).CurrentValue * m_Ability.AttackDamageScale;

				damageable.ApplyDamage(damage, m_Character);
			}

			if(m_HitEffectPrefab)
				Instantiate(m_HitEffectPrefab, collision.contacts[0].point, m_HitEffectPrefab.transform.rotation);

			if (m_DamagePopupPrefab && damage != 0)
			{
				TMP_Text text = Instantiate(m_DamagePopupPrefab, collision.contacts[0].point, Quaternion.identity)?.GetComponent<TMP_Text>();
				if(text)
					text.text = damage.ToString("F0");
			}
		}
	}
}