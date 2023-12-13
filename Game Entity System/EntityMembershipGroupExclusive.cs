using System.Collections.Generic;
using System.Runtime.Serialization;
using Izzy.DataStructures.HashsetDictionary;

namespace Izzy.GameEntitySystem
{
	/// <summary>
	/// Tracks memberships of <typeparamref name="TMember"/> so that each <typeparamref name="TMember"/> can be part of one (and only one) <typeparamref name="TController"/>
	/// </summary>
	/// <typeparam name="TController">The type of entity that can hold members</typeparam>
	/// <typeparam name="TMember">The type of entity that can be a member of <typeparamref name="TController"/></typeparam>
	/// <remarks>This class This does not derive from <see cref="GameEntity"/> and so will not be serialized by default. A <see cref="GameEntity"/> or other serialized object needs to hold a refernce to this for it to be saved</remarks>
	[System.Serializable]
	public class EntityMembershipGroupExclusive<TController, TMember> where TController : class, IGameEntity where TMember : class, IGameEntity
	{
		GameState gameState;
		public EntityMembershipGroupExclusive(GameState gameState)
		{
			this.gameState = gameState;
			gameState.OnEntityCreated += OnNewGameEntity;
			gameState.OnEntityDestroyed += OnGameEntityDestroyed;
		}
		~EntityMembershipGroupExclusive()
		{
			gameState.OnEntityCreated -= OnNewGameEntity;
            gameState.OnEntityDestroyed -= OnGameEntityDestroyed;
		}
		[OnDeserialized]
		void OnDeserialized(StreamingContext context)
		{
            // WIP / DEBUG - do we need to resubscribe to events on deserialization?
            // If not, also change GameEntity!
            gameState.OnEntityCreated += OnNewGameEntity;
            gameState.OnEntityDestroyed += OnGameEntityDestroyed;
		}

		Dictionary<int, int> _entityMembership = new Dictionary<int, int>(); //<member, controller>
		HashsetDictionary<int, int> _groupMembers = new HashsetDictionary<int, int>(); //<controller, members>

		void OnNewGameEntity(GameEntity entity)
		{
			if (entity is TMember)
				_entityMembership.Add(entity.ID, -1);
			if (entity is TController)
				_groupMembers.EnsureKey(entity.ID);
		}
		void OnGameEntityDestroyed(GameEntity entity)
		{
			if (entity is TMember)
				_entityMembership.Remove(entity.ID);
			if (entity is TController)
				_groupMembers.RemoveAllFromKey(entity.ID);
		}

		// Accessors
		public void SetMembership(TMember member, TController controller)
		{
			_groupMembers.RemoveAllOfValue(member.ID);
			if (controller != null)
            {
				_groupMembers.Add_CertainOfKey(controller.ID, member.ID);
				_entityMembership[member.ID] = controller.ID;
			}
			else 
            {
				_entityMembership[member.ID] = -1;
            }
		}
		public void RemoveMemberIfMember(TMember member, TController controller)
		{
			if (_entityMembership[member.ID] == controller.ID)
			{
				_groupMembers.Remove_CertainOfKey(controller.ID, member.ID);
				_entityMembership[member.ID] = -1;
			}
		}
		public void RemoveMembersMembership(TMember member)
		{
			_groupMembers.RemoveAllOfValue(member.ID);
			_entityMembership[member.ID] = -1;
		}
		public TController Membership(TMember member)
		{
			int iMembership = _entityMembership[member.ID];
			return iMembership == -1 ? null : GameEntity.ActiveGameState.IEntityWithID<TController>(iMembership);
		}
		public IReadOnlyCollection<TMember> Members(TController controller)
		{
			return GameEntity.ActiveGameState.IEntitiesWithIDs<TMember>(_groupMembers.Get(controller.ID));
		}
		public IReadOnlyCollection<TSubtype> MembersOfType<TSubtype>(TController controller) where TSubtype : class, TMember
		{
			return GameEntity.ActiveGameState.IEntitiesWithIDs<TSubtype>(_groupMembers.Get(controller.ID));
		}
	}
}
