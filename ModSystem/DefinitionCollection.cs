using Izzy;
using System;
using System.Collections.Generic;


namespace Izzy.ModSystem
{
    public class DefinitionCollection<T> where T : DefinitionType, new()
    {
        const string debuggingLogContext = "DefinitionCollection_debugging";
        bool _firstLoad = true;
        T[] _definitionMap;
        Dictionary<string, T> _definitionMapByName = new Dictionary<string, T>();
        string _definitionType;
        T _fallbackDefinition;
        public bool StateLocked { get; private set; } = false;
        public DefinitionCollection(string typeName)
        {
            _definitionType = typeName;
            ModHandler.OnUnloadingMods += Unload;
            ModHandler.RebuildDynamicDefinitions += Rebuild;
            if (ModHandler.Loaded)
                Rebuild();
        }
        void Unload ()
        {
            StateLocked = false;
        }
        void Rebuild ()
        {
            T fallback = new T();
            fallback.SetupFallback();
            _fallbackDefinition = fallback;
            GameData[] definitions = GameData.GetAllOfType(_definitionType);
            _definitionMap = new T[definitions.Length];
            _definitionMapByName.Clear();
            if (_firstLoad)
            {
                for (int i = 0; i < definitions.Length; i++)
                {
                    GameData definition = definitions[i];
                    T definitionObj = new T();
                    definitionObj.DoSetup(definition, i);
                    _definitionMap[i] = definitionObj;
                    _definitionMapByName.Add(definitionObj.name, definitionObj);
                    DynamicLogger.Log($"Loaded {typeof(T).Name} {definition.name} with the id {i}", context:debuggingLogContext);
                }
            }
            else // If reloading data, take care to preserve existing indices
            {
                Dictionary<string, int> previousCardTypeIndices = new Dictionary<string, int>();
                Queue<int> freeIndices = new Queue<int>();
                foreach (T definitionObj in _definitionMap)
                {
                    previousCardTypeIndices.Add(definitionObj.name, definitionObj.id);
                }
                for (int i = 0; i < definitions.Length; i++)
                {
                    GameData definition = definitions[i];
                    if (previousCardTypeIndices.TryGetValue(definition.name, out int id))
                    {
                        T definitionObj = new T();
                        definitionObj.DoSetup(definition, id);
                        _definitionMap[id] = definitionObj;
                        _definitionMapByName.Add(definitionObj.name, definitionObj);
                        DynamicLogger.Log($"Loaded {typeof(T).Name} {definition.name} with the id {id}", context: debuggingLogContext);
                    }
                    else
                    {
                        freeIndices.Enqueue(i);
                    }
                }
                for (int i = 0; i < definitions.Length; i++)
                {
                    GameData definition = definitions[i];
                    int id = freeIndices.Dequeue();
                    T definitionObj = new T();
                    definitionObj.DoSetup(definition, id);
                    _definitionMap[id] = definitionObj;
                    _definitionMapByName.Add(definitionObj.name, definitionObj);
                    DynamicLogger.Log($"Loaded {typeof(T).Name} {definition.name} with the id {id}", context: debuggingLogContext);
                }
            }
            StateLocked = true;
        }
        public T Get (int index)
        {
            if (index < 0 || index >= _definitionMap.Length)
                return _fallbackDefinition;
            else
                return _definitionMap[index];
        }
        public T Get (string name)
        {
            if (_definitionMapByName.TryGetValue(name, out T definitionObj))
            {
                return definitionObj;
            }
            else
            {
                return _fallbackDefinition;
            }
        }
        public T[] All
        {
            get => _definitionMap;
        }
    }
}
