using Syncfusion.Licensing;

namespace EgoTournament;

public partial class App : Application
{
    private const string SyncFusionKey = "Ngo9BigBOggjHTQxAR8/V1NBaF1cXmhPYVVpR2Nbe053fldHalhSVAciSV9jS3pTc0ZjWXhccXBWQ2dYWQ==";

    public App()
	{
		InitializeComponent();

        //Register TRIAL Syncfusion license change to legit.
        SyncfusionLicenseProvider.RegisterLicense(SyncFusionKey);
        MainPage = new AppShell();
	}
}
