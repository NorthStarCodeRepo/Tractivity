namespace Tractivity;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

                // Helpful how-to use custom icons.
                // https://fonts.google.com/icons
                // https://cedricgabrang.medium.com/custom-fonts-material-design-icons-in-net-maui-acf59c9f98fe
                fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
            })
            .RegisterDependencies();

        return builder.Build();
    }
}