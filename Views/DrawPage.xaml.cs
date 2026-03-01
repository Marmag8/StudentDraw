using StudentDraw.Models;
using StudentDraw.Services;

namespace StudentDraw.Views
{
    public partial class DrawPage : ContentPage
    {
        Dictionary<string, List<Student>> students = new();

        public DrawPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            students = Utils.LoadFromFile();

            if (Utils.LuckyNumber > 0)
            {
                LuckyNumberLabel.Text = $"Szczęśliwy numerek: {Utils.LuckyNumber}";
            }
            else
            {
                LuckyNumberLabel.Text = "";
            }

            List<string> classNames = new List<string> { "Wszystkie Klasy" };
            classNames.AddRange(students.Keys);

            string? selectedClass = ClassPicker.SelectedItem?.ToString();

            ClassPicker.ItemsSource = classNames;

            if (selectedClass != null && classNames.Contains(selectedClass))
            {
                ClassPicker.SelectedItem = selectedClass;
            }
            else
            {
                ClassPicker.SelectedIndex = 0;
            }
        }

        private void Draw(object sender, EventArgs e)
        {
            List<Student> drawPool = new List<Student>();

            if (ClassPicker.SelectedIndex == 0)
            {
                foreach (List<Student> classStudents in students.Values)
                {
                    drawPool.AddRange(classStudents.Where(s => s.IsPresent && s.IndexNumber != Utils.LuckyNumber));
                }
            }
            else
            {
                string selectedClass = ClassPicker.SelectedItem.ToString() ?? "";
                if (students.ContainsKey(selectedClass))
                {
                    drawPool.AddRange(students[selectedClass].Where(s => s.IsPresent && s.IndexNumber != Utils.LuckyNumber));
                }
            }
            if (drawPool.Count == 0)
            {
                Result.Text = "Brak uczniów do losowania.";
                return;
            }

            List<Student> availablePool = drawPool
                .Where(s => !Utils.RecentlyDrawn.Contains(s.ToString()))
                .ToList();

            if (availablePool.Count == 0)
            {
                Result.Text = "Wszyscy obecni uczniowie zostali niedawno wylosowani.";

                Utils.RecentlyDrawn.Add("");
                if (Utils.RecentlyDrawn.Count > 3)
                {
                    Utils.RecentlyDrawn.RemoveAt(0);
                }
                Utils.SaveToFile(students);

                return;
            }

            Random rand = new Random();
            int index = rand.Next(availablePool.Count);
            Student selectedStudent = availablePool[index];

            Utils.RecentlyDrawn.Add(selectedStudent.ToString());
            if (Utils.RecentlyDrawn.Count > 3)
            {
                Utils.RecentlyDrawn.RemoveAt(0);
            }

            Utils.SaveToFile(students);

            Result.Text = $"Wylosowany uczeń: {selectedStudent.DisplayName}";
        }
    }
}
