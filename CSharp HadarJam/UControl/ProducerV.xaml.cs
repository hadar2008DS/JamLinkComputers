public partial class ProducerV : UserControl
{
    private readonly Person currentUser;
    private readonly ApiService apiService = new ApiService();

    public ProducerV(Person user)
    {
        InitializeComponent();
        currentUser = user ?? throw new ArgumentNullException(nameof(user));
        Loaded += ProducerV_Loaded;
    }

    private async void ProducerV_Loaded(object s, RoutedEventArgs e) => await LoadData();

    private async Task LoadData()
    {
        var apps = await apiService.GetProducerApps(currentUser.Id); // implement in ApiService
        var segments = await apiService.GetUploadedSegments(currentUser.Id);
        AppsList.ItemsSource = apps;
        UploadedSegmentsList.ItemsSource = segments;
    }
}