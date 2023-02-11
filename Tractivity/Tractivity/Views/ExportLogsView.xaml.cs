using Tractivity.Common.Environment;

namespace Tractivity.Views;

public partial class ExportLogsView : ContentPage
{
    private readonly EnvironmentManager _environmentManager;

    public ExportLogsView(EnvironmentManager environmentManager)
	{
		InitializeComponent();
        this._environmentManager = environmentManager;
    }

    private async void SubmitForm(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(this.recipientEmail.Text))
        {
            await DisplayAlert("Oops", "You must enter an email address.", "Ok");
            return;
        }

        if (Email.Default.IsComposeSupported)
        {
            string subject = "Tractivity Data Export";
            string body = "Here is your logged data!";
            string[] recipients = new[] { this.recipientEmail.Text.Trim() };

            var message = new EmailMessage
            {
                Subject = subject,
                Body = body,
                BodyFormat = EmailBodyFormat.PlainText,
                To = new List<string>(recipients)
            };

            string cacheDir = FileSystem.Current.CacheDirectory;
            string fileName = this._environmentManager.LogToFileName;
            string targetFile = System.IO.Path.Combine(cacheDir, fileName);
            
            message.Attachments.Add(new EmailAttachment(targetFile));

            await Email.Default.ComposeAsync(message);

            await DisplayAlert("Sent", this.recipientEmail.Text, "Got It");

            this.recipientEmail.Text = string.Empty;
        }
        else
        {
            await DisplayAlert("Ope!", "Sorry, email is not supported.", "Dang");
        }
    }
}