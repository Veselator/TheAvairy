using System.Text;

namespace TheAvairy
{
    public class NoteManager
    {
        private static StringBuilder allNotes = new StringBuilder();
        public static string AllNotes { get => allNotes.ToString(); }

        static public void AddNote(string text)
        {
            allNotes.AppendLine(text);
        }
    }
}
