using System.Text.Json;

namespace Billiards.Base.Validations
{
    public class ValidatedFolder
    {
        public ValidatedFileCollection ValidatedFiles { get; set; } = new ();

        public static ValidatedFolder? Load(string path)
        {
            string json = File.ReadAllText(path);
            ValidatedFolder? folder = JsonSerializer.Deserialize<ValidatedFolder>(json);
            return folder;
        }

        public void Save(string path)
        {
            JsonSerializer.Serialize<ValidatedFolder>(File.OpenWrite(path), this);
        }

    }
}
