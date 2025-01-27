using Microsoft.Extensions.Logging;
using Acr.UserDialogs;
using Serilog;

namespace qController
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseUserDialogs()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("qfont.ttf", "qfont");
                });

#if DEBUG
            builder.Logging.AddDebug();

            Log.Logger = new LoggerConfiguration()
              .Enrich.FromLogContext()
              .MinimumLevel.Debug()
              .WriteTo.Debug()
              .CreateLogger();
            builder.Logging.AddSerilog(dispose: true);


#endif

            return builder.Build();
        }
    }
}
