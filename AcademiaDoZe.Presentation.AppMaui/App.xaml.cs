using AcademiaDoZe.Presentation.AppMaui.Message;
using CommunityToolkit.Mvvm.Messaging;

namespace AcademiaDoZe.Presentation.AppMaui;

// O namespace AcademiaDoZe.Application conflita com a classe Application do MAUI.
// Por isso a classe base é referenciada com o nome completo Microsoft.Maui.Controls.Application.
public partial class App : Microsoft.Maui.Controls.Application
{
    public App()
    {
        InitializeComponent();

        // aplicar o tema salvo nas preferências
        AplicarTema();

        // assinar para receber mensagens de alteração de tema:
        // toda vez que o usuário salvar um tema, o tema é reaplicado imediatamente
        WeakReferenceMessenger.Default.Register<TemaPreferencesUpdatedMessage>(this, (r, m) =>
        {
            AplicarTema();
        });
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }

    private void AplicarTema()
    {
        UserAppTheme = Preferences.Get("Tema", "system") switch
        {
            "light" => AppTheme.Light,
            "dark" => AppTheme.Dark,
            _ => AppTheme.Unspecified,
        };
    }
}
