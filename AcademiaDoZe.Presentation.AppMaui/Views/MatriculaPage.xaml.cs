using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Presentation.AppMaui.ViewModels;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class MatriculaPage : ContentPage
{
    public MatriculaPage(MatriculaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is MatriculaViewModel viewModel)
        {
            await viewModel.InitializeAsync();

            // Garante que a data fim seja calculada ao carregar dados existentes
            CalculateFimFromPlanoAndInicio();
        }
    }

    private void PlanoPicker_SelectedIndexChanged(object? sender, EventArgs e)
    {
        CalculateFimFromPlanoAndInicio();
    }

    private void InicioDatePicker_DateSelected(object? sender, DateChangedEventArgs e)
    {
        CalculateFimFromPlanoAndInicio();
    }

    // Data fim = data início + duração do plano (mensal 1 mês, trimestral 3, semestral 6, anual 1 ano)
    private void CalculateFimFromPlanoAndInicio()
    {
        if (BindingContext is not MatriculaViewModel vm) return;

        var plano = vm.Matricula.Plano;
        var inicio = vm.Matricula.DataInicio;

        var fim = plano switch
        {
            AppMatriculaPlano.Mensal => inicio.AddMonths(1),
            AppMatriculaPlano.Trimestral => inicio.AddMonths(3),
            AppMatriculaPlano.Semestral => inicio.AddMonths(6),
            AppMatriculaPlano.Anual => inicio.AddYears(1),
            _ => inicio,
        };

        // Atualiza o DTO e o DatePicker somente leitura
        vm.Matricula.DataFim = fim;

        try
        {
            FimDatePicker.Date = fim.ToDateTime(TimeOnly.MinValue);
        }
        catch { }
    }
}
