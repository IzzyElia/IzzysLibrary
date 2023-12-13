namespace Izzy.ModSystem
{
    public abstract class DefinitionType
    {
        public string name { get; private set; }
        public int id { get; private set; }
        public DefinitionType() { }
        public void DoSetup(GameData definition, int id)
        {
            this.id = id;
            this.name = definition.name;
            Setup(definition);
        }
        public void SetupFallback() => Fallback();
        /// <summary>
        /// Sets up an object to be used as a fallback in case an invalid index or key is used when
        /// referencing a definition
        /// </summary>
        protected abstract void Fallback();
        protected abstract void Setup(GameData definition);
    }
}
