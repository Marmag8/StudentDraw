using StudentDraw.Models;
using System.Text.Json;

namespace StudentDraw.Services
{
    internal class Utils
    {
        public static List<string> RecentlyDrawn { get; set; } = new();
        public static int LuckyNumber { get; private set; } = -1;

        public static void AssignIndexNumbers(Dictionary<string, List<Student>> dict)
        {
            foreach (var key in dict.Keys.ToList())
            {
                var sorted = dict[key].OrderBy(s => s.Surname).ThenBy(s => s.Name).ToList();
                for (int i = 0; i < sorted.Count; i++)
                {
                    sorted[i].IndexNumber = i + 1;
                }
                dict[key] = sorted;
            }
        }

        public static void RollLuckyNumber(Dictionary<string, List<Student>> dict)
        {
            if (LuckyNumber != -1) return;

            int maxStudents = 0;
            foreach (var list in dict.Values)
            {
                if (list.Count > maxStudents) maxStudents = list.Count;
            }

            if (maxStudents > 0)
            {
                LuckyNumber = new Random().Next(1, maxStudents + 1);
            }
            else
            {
                LuckyNumber = 0;
            }
        }

        static readonly string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LosowanieUcznia", "data.txt");
        public static void SaveToFile(Dictionary<string, List<Student>> dict)
        {
            AssignIndexNumbers(dict);

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

            File.AppendAllText(filePath, "RECENT" + Environment.NewLine);
            foreach (string recent in RecentlyDrawn)
            {
                File.AppendAllText(filePath, recent + Environment.NewLine);
            }
        }

        public static Dictionary<string, List<Student>> LoadDefaults()
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
            RecentlyDrawn.Clear();

            if (!File.Exists(filePath))
            {
                var defaults = LoadDefaults();
                AssignIndexNumbers(defaults);
                RollLuckyNumber(defaults);
                SaveToFile(defaults);
                return defaults;
            }

            Dictionary<string, List<Student>> dict = new();
            List<Student> currentStudents = new();
            string? currentClassSymbol = null;
            bool readingRecent = false;

            foreach (string rawLine in File.ReadAllLines(filePath))
            {
                string line = rawLine.Trim();

                if (line == "RECENT")
                {
                    if (currentClassSymbol is not null)
                    {
                        dict[currentClassSymbol] = new List<Student>(currentStudents);
                        currentStudents.Clear();
                        currentClassSymbol = null;
                    }
                    readingRecent = true;
                    continue;
                }

                if (readingRecent)
                {
                    if (line.Length > 0)
                    {
                        RecentlyDrawn.Add(line);
                    }
                    continue;
                }

                if (line.Length == 0)
                {
                    if (currentClassSymbol is not null)
                    {
                        dict[currentClassSymbol] = new List<Student>(currentStudents);
                        currentStudents.Clear();
                        currentClassSymbol = null;
                    }

                    continue;
                }

                if (!line.Contains(','))
                {
                    if (currentClassSymbol is not null)
                    {
                        dict[currentClassSymbol] = new List<Student>(currentStudents);
                        currentStudents.Clear();
                    }

                    currentClassSymbol = line;

                    if (!dict.ContainsKey(currentClassSymbol))
                    {
                        dict.Add(currentClassSymbol, new List<Student>());
                    }

                    continue;
                }

                currentStudents.Add(Student.FromString(line));
            }

            if (currentClassSymbol is not null)
            {
                dict[currentClassSymbol] = new List<Student>(currentStudents);
            }

            AssignIndexNumbers(dict);
            RollLuckyNumber(dict);

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
