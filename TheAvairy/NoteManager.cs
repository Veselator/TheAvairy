using System.ComponentModel.Design;
using System.IO.Enumeration;
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

        static public void SaveToFile(string fileName = "Avairy_Report.txt")
        {
            using (StreamWriter streamWriter = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                streamWriter.Write(AllNotes);
            }
        }
    }
}
