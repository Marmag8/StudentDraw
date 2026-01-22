using StudentDraw.Models;
using StudentDraw.Services;

namespace StudentDraw.Views
{
    public partial class DrawPage : ContentPage
    {
        public DrawPage()
        {
            InitializeComponent();

            Dictionary<string, List<Student>> dict = Utils.LoadFromFile();
            if (dict.Count == 0)
            {
                dict = Utils.LoadFromDefaults();
            }

            Utils.SaveToFile(dict);
        }
    }
}
