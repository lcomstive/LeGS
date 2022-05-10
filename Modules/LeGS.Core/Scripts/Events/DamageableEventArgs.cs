namespace LEGS
{
	/// <summary>
	/// When an <see cref="IDamageable"/>'s health changes
	/// </summary>
	public class EntityHealthChangeEventArgs : LEGEventArgs
	{
		/// <summary>
		/// Name of event in <see cref="EventManager"/>
		/// </summary>
		public static string EventName => "EntityHealthChange";

		/// <summary>
		/// How much health was added or removed from <see cref="IEntity"/>
		/// </summary>
		public float Amount { get; private set; }

		/// <summary>
		/// <see cref="IEntity"/> that caused health change
		/// </summary>
		public IEntity Sender { get; private set; }

		/// <summary>
		/// The <see cref="IDamageable"/> that had health changed
		/// </summary>
		public IDamageable Damageable { get; private set; }

		/// <param name="damageable"><see cref="IDamageable"/> whose health is being changed</param>
		/// <param name="sender"><see cref="IEntity"/> that caused the health change</param>
		/// <param name="changeAmount">Amount of health being altered</param>
		public EntityHealthChangeEventArgs(IDamageable damageable, IEntity sender, float changeAmount)
		{
			Damageable = damageable;
			Entity = damageable as IEntity;

			Amount = changeAmount;
			Sender = sender;
		}
	}

	/// <summary>
	/// When an <see cref="IEntity"/> gets created
	/// </summary>
	public class EntitySpawnEventArgs : LEGEventArgs
	{
		/// <summary>
		/// Name of event in <see cref="EventManager"/>
		/// </summary>
		public static string EventName => "EntitySpawn";
		
		/// <param name="entity"><see cref="IEntity"/> that spawned</param>
		public EntitySpawnEventArgs(IEntity entity) => Entity = entity;
	}
	
	/// <summary>
	/// When an <see cref="IEntity"/> gets destroyed
	/// </summary>
	public class EntityDeathEventArgs : LEGEventArgs
	{
		/// <summary>
		/// Name of event in <see cref="EventManager"/>
		/// </summary>
		public static string EventName => "EntityDeath";

		/// <summary>
		/// <see cref="IEntity"/> causing the demise
		/// </summary>
		public IEntity Killer { get; private set; }
	
		public EntityDeathEventArgs(IEntity entity, IEntity killer)
		{
			Entity = entity;
			Killer = killer;
		}
	}
}