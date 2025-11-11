using System.Net;
using System.Net.Http.Json;
using Entity.Dtos;

namespace GuitarLabMobile.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5000/api/";

        public ApiService()
        {
            var handler = new HttpClientHandler
            {
                CookieContainer = new CookieContainer(),
                UseCookies = true
            };
            _httpClient = new HttpClient(handler);
            _httpClient.BaseAddress = new Uri(BaseUrl);
        }

        // Auth methods
        public async Task<bool> LoginAsync(LoginRequestDto loginRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("Auth/Login", loginRequest);
            return response.IsSuccessStatusCode;
        }

        public async Task<UserDto?> GetCurrentUserAsync()
        {
            var response = await _httpClient.GetAsync("Auth/me");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserDto>();
            }
            return null;
        }

        // GuitaristLesson methods
        public async Task<List<GuitaristLessonDto>?> GetGuitaristLessonsAsync()
        {
            var response = await _httpClient.GetAsync("GuitaristLesson");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<GuitaristLessonDto>>();
            }
            return null;
        }

        // LessonExercise methods
        public async Task<List<LessonExerciseDto>?> GetLessonExercisesAsync(int lessonId)
        {
            var response = await _httpClient.GetAsync($"LessonExercise?lessonId={lessonId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<LessonExerciseDto>>();
            }
            return null;
        }

        // Exercise methods
        public async Task<ExerciseDto?> GetExerciseAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Exercise/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ExerciseDto>();
            }
            return null;
        }

        // Auth check
        public async Task<bool> IsAuthenticatedAsync()
        {
            var user = await GetCurrentUserAsync();
            return user != null;
        }
    }
}