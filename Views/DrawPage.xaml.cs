using StudentDraw.Models;
using StudentDraw.Services;

namespace StudentDraw.Views
{
    public partial class DrawPage : ContentPage
    {
        Dictionary<string, List<Student>> students;

        public DrawPage()
        {
            InitializeComponent();

            students = Utils.LoadFromFile();
            if (students.Count == 0)
            {
                students = Utils.LoadDefaults();
            }

            List<string> classNames = new List<string> { "Wszystkie Klasy" };
            classNames.AddRange(students.Keys);
            ClassPicker.ItemsSource = classNames;
            ClassPicker.SelectedIndex = 0;
        }

        private void Draw(object sender, EventArgs e)
        {
            List<Student> drawPool = new List<Student>();

            if (ClassPicker.SelectedIndex == 0)
            {
                foreach (List<Student> classStudents in students.Values)
                {
                    drawPool.AddRange(classStudents);
                }
            }
            else
            {
                string selectedClass = ClassPicker.SelectedItem.ToString() ?? "";
                if (students.ContainsKey(selectedClass))
                {
                    drawPool.AddRange(students[selectedClass]);
                }
            }
            if (drawPool.Count == 0)
            {
                Result.Text = "Brak uczniów do losowania.";
                return;
            }

            Random rand = new Random();
            int index = rand.Next(drawPool.Count);
            Student selectedStudent = drawPool[index];
            Result.Text = $"Wylosowany uczeń: {selectedStudent.DisplayName}";
        }
    }
}
