namespace verse_interpreter.lib.Data.Variables
{
    public class TypeData
    {
        public TypeData(string typeName)
        {
            this.Name = typeName;
        }

        public string Name { get; set; }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(TypeData firstData, TypeData secondData)
        {
            return firstData.Name == secondData.Name;
        }

        public static bool operator !=(TypeData firstData, TypeData secondData)
        {
            return firstData.Name != secondData.Name;
        }
    }
}
