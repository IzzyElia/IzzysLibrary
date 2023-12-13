using Izzy.Serialization;
using System.Collections.Generic;
using System;
using IzzysConsole;
using Izzy;

namespace IzzysGameLibrary
{
    public class GameState
    {
        [ConsoleCommand("entities-metadata")]
        static void Console_CountEntities()
        {
            ConsoleManager.Log($"Count - {Active.entities.Count.ToString()}");
        }
        public static GameState Active { get; set; }
        [NonSerialized] private IDSystem<GameEntity> idSystem;
        HashSet<GameEntity> entities;
        public event Action<GameEntity> OnEntityCreated;
        public event Action<GameEntity> OnEntityDestroyed;
        public GameState()
        {
            idSystem = new IDSystem<GameEntity>();
            entities = new HashSet<GameEntity>();
        }
        public static GameState LoadFromFile(string directoryPath)
        {
            GameState gameState = Serializer.LoadFromBinary<GameState>(directoryPath);
            foreach (GameEntity entity in gameState.entities)
            {
                entity.RegisterIntoGameStateAfterLoading(gameState);
            }
            foreach (GameEntity entity in gameState.entities)
            {
                entity.OnWorldLoaded();
            }
            return gameState;
        }
        public void SaveToFile(string directoryPath)
        {
            DynamicLogger.Log($"Saving map to {directoryPath}");
            Serializer.SaveToBinary(directoryPath, this);
            DynamicLogger.Log($"Saved to {directoryPath}");

        }
        public void AssignIDAfterLoading (GameEntity entity, int id)
        {
            idSystem.ManuallyAssignID(entity, id);
        }
        public int RegisterGameEntityAndGetID(GameEntity entity)
        {
            int id = idSystem.AssignID(entity);
            entities.Add(entity);
            return id;
        }
        public void OnEntityCreationComplete (GameEntity entity)
        {
            OnEntityCreated(entity);
        }
        public void DeregisterGameEntity(GameEntity entity)
        {
            idSystem.UnassignID(entity.ID);
            entities.Remove(entity);
        }
        public void OnEntityBeingDestroyed (GameEntity entity)
        {
            OnEntityDestroyed(entity);
        }
        // Entity searching
        public int IDOfEntity(GameEntity entity)
        {
            return idSystem.IdOf(entity);
        }
        public GameEntity EntityWithID(int id)
        {
            try
            {
                return idSystem.WithID(id) as GameEntity;
            }
            catch (KeyNotFoundException)
            {
                DynamicLogger.Log($"No entity with ID {id}", LogType.warning);
                return null;
            }
        }
        public IGameEntity IEntityWithID(int id)
        {
            try
            {
                return idSystem.WithID(id) as IGameEntity;
            }
            catch (KeyNotFoundException)
            {
                DynamicLogger.Log($"No entity with ID {id}", LogType.warning);
                return null;
            }

        }
        public T EntityWithID<T>(int id) where T : GameEntity
        {
            try
            {
                return idSystem.WithID(id) as T;
            }
            catch (KeyNotFoundException)
            {
                DynamicLogger.Log($"No entity with ID {id}", LogType.warning);
                return null;
            }
        }
        public T IEntityWithID<T>(int id) where T : class, IGameEntity
        {
            try
            {
                return idSystem.WithID(id) as T;
            }
            catch (KeyNotFoundException)
            {
                DynamicLogger.Log($"No entity with ID {id}", LogType.warning);
                return null;
            }

        }
        public GameEntity[] EntitiesWithIDs(int[] ids)
        {
            GameEntity[] found = new GameEntity[ids.Length];
            for (int i = 0; i < ids.Length; i++)
            {
                found[i] = EntityWithID<GameEntity>(ids[i]);
            }
            return found;
        }
        public IGameEntity[] IEntitiesWithIDs(int[] ids)
        {
            IGameEntity[] found = new IGameEntity[ids.Length];
            for (int i = 0; i < ids.Length; i++)
            {
                found[i] = IEntityWithID<IGameEntity>(ids[i]);
            }
            return found;
        }
        public T[] EntitiesWithIDs<T>(int[] ids) where T : GameEntity
        {
            T[] found = new T[ids.Length];
            for (int i = 0; i < ids.Length; i++)
            {
                try
                {
                    found[i] = EntityWithID<T>(ids[i]);
                }
                catch (InvalidCastException)
                {

                    throw new InvalidCastException();
                }

            }
            return found;
        }
        public T[] EntitiesWithIDs<T>(IList<int> ids) where T : GameEntity
        {
            T[] found = new T[ids.Count];
            for (int i = 0; i < ids.Count; i++)
            {
                try
                {
                    found[i] = EntityWithID<T>(ids[i]);
                }
                catch (InvalidCastException)
                {

                    throw new InvalidCastException();
                }

            }
            return found;
        }
        public T[] IEntitiesWithIDs<T>(int[] ids) where T : class, IGameEntity
        {
            T[] found = new T[ids.Length];
            for (int i = 0; i < ids.Length; i++)
            {
                try
                {
                    found[i] = IEntityWithID<T>(ids[i]);
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return found;
        }
        public T[] IEntitiesWithIDs<T>(IList<int> ids) where T : class, IGameEntity
        {
            T[] found = new T[ids.Count];
            for (int i = 0; i < ids.Count; i++)
            {
                try
                {
                    found[i] = IEntityWithID<T>(ids[i]);
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return found;
        }
        public T[] AllEntitiesWithType<T>() where T : class, IGameEntity
        {
            List<T> found = new List<T>();
            foreach (GameEntity entity in entities)
            {
                T castEntity = entity as T;
                if (castEntity != null) { found.Add(castEntity); }
            }
            return found.ToArray();
        }
        public GameEntity[] AllAssignableTo(Type type)
        {
            List<GameEntity> found = new List<GameEntity>();
            foreach (GameEntity entity in entities)
            {
                if (type.IsAssignableFrom(entity.GetType())) { found.Add(entity); }
            }
            return found.ToArray();
        }
        public void Cleanup ()
        {
            foreach (GameEntity entity in new List<GameEntity>(entities))
            {
                entity.Cleanup();
            }
            idSystem.Flush();
            entities.Clear();
        }
    }
}
