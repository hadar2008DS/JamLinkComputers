public partial class ProfileV : UserControl
{
    private Person currentUser;

    public ProfileV(Person user)
    {
        InitializeComponent();
        currentUser = user ?? throw new ArgumentNullException(nameof(user));
        LoadProfile();
    }

    private void LoadProfile()
    {
        UserNameText.Text = $"Username: {currentUser.Username}";
        StatusText.Text = currentUser.IsActive ? "Status: Active" : "Status: Inactive";
    }
}