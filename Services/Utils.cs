using StudentDraw.Models;
using System.Text.Json;

namespace StudentDraw.Services
{
    internal class Utils
    {
        static readonly string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LosowanieUcznia", "data.txt");
        public static void SaveToFile(Dictionary<string, List<Student>> dict)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }

            File.WriteAllText(filePath, string.Empty);

            foreach (KeyValuePair<string, List<Student>> entry in dict)
            {
                string classSymbol = entry.Key;

                File.AppendAllText(filePath, classSymbol + Environment.NewLine);

                foreach (Student student in entry.Value)
                {
                    File.AppendAllText(filePath, student.ToString() + Environment.NewLine);
                }

                File.AppendAllText(filePath, Environment.NewLine);
            }
        }

        public static Dictionary<string, List<Student>> LoadFromDefaults()
        {
            using Stream stream = FileSystem.OpenAppPackageFileAsync("Defaults.json").GetAwaiter().GetResult();
            using StreamReader reader = new StreamReader(stream);
            string json = reader.ReadToEnd();

            Dictionary<string, List<Student>> dict = JsonSerializer.Deserialize<Dictionary<string, List<Student>>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return dict ?? new Dictionary<string, List<Student>>();
        }

        public static Dictionary<string, List<Student>> LoadFromFile()
        {
            Dictionary<string, List<Student>> dict = new Dictionary<string, List<Student>>();
            List<Student> students = new List<Student>();
            string classSymbol = "";
            bool inClassNode = false;

            if (!File.Exists(filePath))
            {
                return dict;
            }

            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                if (line.Trim() == "")
                {
                    inClassNode = false;
                }
                else if (line.Trim().Length == 2)
                {
                    inClassNode = true;
                    classSymbol = line;
                }
                else
                {
                    students.Add(Student.FromString(line));
                }

                if (!inClassNode && students.Count > 0)
                {
                    dict.Add(classSymbol, new List<Student>(students));
                    students.Clear();
                }
            }

            return dict;
        }

        public static Dictionary<string, List<Student>> LoadFromFile(string classSymbol)
        {
            Dictionary<string, List<Student>> dict = new Dictionary<string, List<Student>>();
            List<Student> students = new List<Student>();
            bool inClassNode = false;

            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                if (line.Trim() == classSymbol)
                {
                    inClassNode = true;
                    continue;
                }

                if (inClassNode)
                {
                    if (line.Trim() == "")
                    {
                        inClassNode = false;
                        break;
                    }
                    else
                    {
                        students.Add(Student.FromString(line));
                    }
                }
            }

            dict.Add(classSymbol, new List<Student>(students));
            return dict;
        }
    }
}
