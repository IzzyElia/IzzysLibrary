using Izzy.DataStructures;
using Izzy.DataStructures.HashsetDictionary;
using Izzy.ModSystem;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System;
using System.Reflection;
using Izzy.ForcedInitialization;
using IzzysConsole;
using Izzy;

namespace Izzy.GameEntitySystem
{
    /// <summary>
    /// An entity that exists as part of the game state.
    /// Any class that derives from this will be serialized on game save
    /// </summary>
    [Serializable]
    [ForceInitialize]
    public abstract class GameEntity : IGameEntity
    {
        // CONSOLE

        [ConsoleCommand("entities")]
        static void Console_WriteAllEntities(int page = 0, string type = "")
        {
            const int perPage = 100;
            List<GameEntity> entities;
            if (type == "")
            {
                entities = new List<GameEntity>();
            }
            else
            {
                entities = new List<GameEntity>(ActiveGameState.AllEntitiesWithType<GameEntity>());
            }

            for (int i = 0 + (perPage * page); i < Mathfi.Min(entities.Count, (perPage * page) + perPage); i++)
            {
                GameEntity entity = entities[i];
                ConsoleManager.Log(($"{entity.ID} - {entity.GetType()} '{entity.ToString()}'"));
            }
        }
        [ConsoleCommand("info", "Gives detailed info about the selected entity. [filter] is optional and may be used to get more specific information for some entity types, see the entity type's documentation for more details")]
        static void Console_GetEntityInfo(int id, string filter = "")
        {
            GameEntity withId = ActiveGameState.EntityWithID(id);
            if (withId == null) { ConsoleManager.Log($"No entity with id {id}"); return; }
            ConsoleManager.Log(withId.Info(filter));
        }
        public virtual string Info(string filter = null)
        {
            return $"{this.GetType().FullName}\nID = {ID}\n";
        }
        // -----------------------
        public static GameState ActiveGameState
        {
            get { return GameState.Active; }
            set { GameState.Active = value; }
        }


        // Construction

        protected GameEntity(GameState gameState, string definition = null)
        {

            // Initialize
            if (definition != null) this.DefinitionKey = definition;
            id = gameState.RegisterGameEntityAndGetID(this);
            SubscribeToEvents();
            gameState.OnEntityCreationComplete(this);
        }

        [NonSerialized] GameState gameState;
        public GameState GameState => gameState;
        private int id;
        public int ID => id;

        // Definition
        [NonSerialized] DataNode _definition;
        /// <summary>
        /// The definition key stored in a field so it can be serialized/deserialized
        /// </summary>
        string _definition_key;
        /// <summary>
        /// A DataNode definition for the entity loaded from GameData
        /// </summary>
        public DataNode Definition
        {
            get { return this._definition; }
        }
        public string DefinitionKey
        {
            get { return _definition_key; }
            set
            {
                _definition_key = value;
                ReloadDefiition();
            }
        }
        void ReloadDefiition()
        {
            if (_definition_key == null) { this._definition = null; return; }
            DataNode definition = GameData.Get(_definition_key);
            if (definition != null)
            {
                this._definition = definition;
                OnDefinitionChanged();
            }
            else
            {
                throw new ArgumentException($"Definition '{_definition_key}' not found");
            }
        }
        protected virtual void OnDefinitionChanged() { }
        // Render overrides
        protected delegate Property PropertyValueOverride();
        Dictionary<string, PropertyValueOverride> renderPropertyOverrides = new Dictionary<string, PropertyValueOverride>();

        // Destruction and Cleanup
        public void Destroy()
        {
            gameState.OnEntityBeingDestroyed(this);
            OnDestroy();
            Cleanup();
        }
        /// <summary>
        /// What should happen when the entity is destroyed in the game world (ex death)
        /// </summary>
        protected virtual void OnDestroy()
        {
            gameState.DeregisterGameEntity(this);
        }
        /// <summary>
        /// What should happen when the entity is removed (ex when the world is unloaded)
        /// </summary>
        public virtual void Cleanup()
        {
            UnsubscribeFromEvents();
        }

        //Serialization
        public void RegisterIntoGameStateAfterLoading (GameState gameState)
        {
            if (gameState == null)
                throw new ArgumentNullException();

            if (id < 0)
                throw new ArgumentOutOfRangeException("id");

            this.gameState = gameState;
            gameState.AssignIDAfterLoading(this, id);
        }
        /// <summary>
        /// Called when every game entity has been loaded
        /// </summary>
        public virtual void OnWorldLoaded ()
        {

        }
        [OnSerializing]
        void _OnSerializing(StreamingContext context)
        {
            OnSerializing(context);
        }
        protected virtual void OnSerializing(StreamingContext context)
        {

        }
        [OnDeserializing]
        void _OnDeserializing(StreamingContext context)
        {
            // Internal deserialization here
            OnDeserializing(context);
        }
        protected virtual void OnDeserializing(StreamingContext context)
        {

        }
        [OnDeserialized]
        void _OnDeserialized(StreamingContext context)
        {
            //Setting DefinitionKey also sets the definition field
            ReloadDefiition();
            // WIP / DEBUG - we may or may not need to readd the event on deserialization???
            ModHandler.OnModStateChanged += ReloadDefiition;
            OnDeserialized(context);
        }
        protected virtual void OnDeserialized(StreamingContext context)
        {
            SubscribeToEvents();
        }

        // Event subscriptions
        protected virtual void SubscribeToEvents() { }
        protected virtual void UnsubscribeFromEvents() { }
    }
}
