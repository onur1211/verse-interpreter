namespace verse_interpreter.lib.IO
{
	// EXAM_UPDATED
	public class FileReader
	{
		public string ReadFileToEnd(string path)
		{
			if (!path.EndsWith(".verse"))
			{
				throw new InvalidOperationException("The file must end with .verse");
			}

			using (StreamReader fs = new StreamReader(path))
			{
				return fs.ReadToEnd();
			}
		}
	}
}
