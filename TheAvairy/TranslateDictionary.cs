namespace TheAvairy
{
    internal class TranslateDictionary
    {
        private static readonly Dictionary<string, string> InnerDictionary = new Dictionary<string, string>
        {
            { "none", "Ніхто" },
            { "gorilla", "Горила" },
            { "cat", "Кіт" },
            { "hedgehog", "Їжак" },
            { "squirell", "Білка" },
        };

        public static string Translate(string key)
        {
            try
            {
                return InnerDictionary[key];
            }
            catch
            {
                return key;
            }
        }
    }
}
