public partial class MusicianV : UserControl
{
    private readonly Person currentUser;
    private readonly ApiService apiService = new ApiService();

    public MusicianV(Person user)
    {
        InitializeComponent();
        currentUser = user ?? throw new ArgumentNullException(nameof(user));
        Loaded += MusicianV_Loaded;
    }

    private async void MusicianV_Loaded(object s, RoutedEventArgs e) => await LoadData();

    private async Task LoadData()
    {
        InstrumentsList.ItemsSource = await apiService.GetInstrumentsForUser(currentUser.Id);
        SegmentsList.ItemsSource = await apiService.GetMusicalSegmentsForUser(currentUser.Id);
        GenresList.ItemsSource = await apiService.GetGenresForUser(currentUser.Id);
    }
}