namespace verse_interpreter.lib.Data.Variables
{
	public class TypeData
	{
		public TypeData(string typeName)
		{
			this.Name = typeName;
		}

		public string Name { get; set; }

		public static bool operator ==(TypeData firstData, TypeData secondData)
		{
			return firstData.Name == secondData.Name;
		}

		public static bool operator !=(TypeData firstData, TypeData secondData)
		{
			return firstData.Name != secondData.Name;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			if (ReferenceEquals(obj, null))
			{
				return false;
			}

			throw new NotImplementedException();
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}
	}
}
