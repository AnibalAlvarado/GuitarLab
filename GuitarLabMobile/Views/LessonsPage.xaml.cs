using System.Collections.ObjectModel;
using GuitarLabMobile.Dtos;
using GuitarLabMobile.Services;
using Microsoft.Maui.Controls;

namespace GuitarLabMobile.Views
{
    public partial class LessonsPage : ContentPage
    {
        public ObservableCollection<GuitaristLessonDto> Lessons { get; set; } = new();
        private readonly ApiService _apiService;

        public LessonsPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            LessonsCollectionView.ItemsSource = Lessons;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            bool isAuthenticated = await _apiService.IsAuthenticatedAsync();
            if (!isAuthenticated)
            {
                await Shell.Current.GoToAsync("//LoginPage");
                return;
            }
            await LoadLessonsAsync();
        }

        private async Task LoadLessonsAsync()
        {
            var lessons = await _apiService.GetGuitaristLessonsAsync();
            if (lessons != null)
            {
                Lessons.Clear();
                foreach (var lesson in lessons)
                {
                    Lessons.Add(lesson);
                }
            }
        }

        private async void OnLessonSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is GuitaristLessonDto selectedLesson)
            {
                // Navigate to ExercisesPage with selected lesson
                await Shell.Current.GoToAsync($"ExercisesPage?lessonId={selectedLesson.LessonId}");
                LessonsCollectionView.SelectedItem = null;
            }
        }
    }
}