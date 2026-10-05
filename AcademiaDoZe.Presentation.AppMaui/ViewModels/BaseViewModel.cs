using CommunityToolkit.Mvvm.ComponentModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    // Indica se uma operação está em andamento, útil para mostrar indicadores de carregamento na UI.
    private bool isBusy;
    public bool IsBusy
    {
        get => isBusy;
        set => SetProperty(ref isBusy, value);
    }

    // Título da ViewModel, pode ser usado para definir o título da página na UI.
    private string title = string.Empty;
    public string Title
    {
        get => title;
        set => SetProperty(ref title, value);
    }

    // Indica se a ViewModel está em estado de atualização, útil para pull-to-refresh na UI.
    private bool isRefreshing;
    public bool IsRefreshing
    {
        get => isRefreshing;
        set => SetProperty(ref isRefreshing, value);
    }
}

// ObservableObject implementa INotifyPropertyChanged, permitindo que a UI seja atualizada
// automaticamente quando o valor de uma propriedade é alterado.
