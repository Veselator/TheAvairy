namespace TheAvairy
{
    internal class TranslateDictionary
    {
        private readonly Dictionary<string, string> InnerDictionary = new Dictionary<string, string>
        {
            { "None", "Ніхто" },
            { "Gorilla", "Горила" },
            { "Cat", "Кіт" },
            { "Hedgehog", "Їжак" },
            { "Squirell", "Білка" },
        };

        // TODO:
        // Переводы
        // Записи в NoteManager
        // Проверить, работают ли состояния на данный момент

        public string this[string i]
        {
            get
            {
                return InnerDictionary[i];
            }
        }
    }
}
