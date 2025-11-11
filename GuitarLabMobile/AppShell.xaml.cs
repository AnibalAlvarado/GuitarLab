using GuitarLabMobile.Services;

namespace GuitarLabMobile;

public partial class AppShell : Shell
{
	private readonly ApiService _apiService;

	public AppShell()
	{
		InitializeComponent();
		_apiService = new ApiService();
		_ = InitializeAsync();
	}

	private async Task InitializeAsync()
	{
		bool isAuthenticated = await _apiService.IsAuthenticatedAsync();
		if (isAuthenticated)
		{
			await GoToAsync("//LessonsPage");
		}
		else
		{
			await GoToAsync("//LoginPage");
		}
	}
}
