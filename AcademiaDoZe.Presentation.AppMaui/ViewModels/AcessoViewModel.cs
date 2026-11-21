using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
// Pedro Henrique dos Santos

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

/// <summary>Item exibido no seletor: representa tanto aluno quanto colaborador.</summary>
public record PessoaAcessoItem(int Id, string Nome, string Cpf, ArquivoDto? Foto)
{
    public override string ToString() => Nome;
}

public partial class AcessoViewModel : BaseViewModel
{
    private readonly IAcessoService _acessoService;
    private readonly IAlunoService _alunoService;
    private readonly IColaboradorService _colaboradorService;

    private List<PessoaAcessoItem> _todasPessoas = [];

    public ObservableCollection<PessoaAcessoItem> PessoasFiltradas { get; } = [];
    public ObservableCollection<AcessoDto> AcessosHoje { get; } = [];

    private bool _modoAluno = true;
    public bool ModoAluno
    {
        get => _modoAluno;
        set
        {
            if (SetProperty(ref _modoAluno, value))
            {
                OnPropertyChanged(nameof(ModoAtualTexto));
                OnPropertyChanged(nameof(TextoSelecao));
            }
        }
    }

    public string ModoAtualTexto => ModoAluno ? "Modo atual selecionado: Aluno" : "Modo atual selecionado: Colaborador";
    public string TextoSelecao => ModoAluno ? "Selecione o Aluno:" : "Selecione o Colaborador:";

    private string _termoBusca = string.Empty;
    public string TermoBusca
    {
        get => _termoBusca;
        set
        {
            if (SetProperty(ref _termoBusca, value))
                AplicarFiltro();
        }
    }

    private PessoaAcessoItem? _pessoaSelecionada;
    public PessoaAcessoItem? PessoaSelecionada
    {
        get => _pessoaSelecionada;
        set
        {
            if (SetProperty(ref _pessoaSelecionada, value))
            {
                OnPropertyChanged(nameof(TemPessoaSelecionada));
                OnPropertyChanged(nameof(NaoTemPessoaSelecionada));
                _ = AtualizarStatusAsync();
            }
        }
    }

    public bool TemPessoaSelecionada => PessoaSelecionada != null;
    public bool NaoTemPessoaSelecionada => PessoaSelecionada == null;

    private bool _estaDentro;
    public bool EstaDentro
    {
        get => _estaDentro;
        set
        {
            if (SetProperty(ref _estaDentro, value))
            {
                OnPropertyChanged(nameof(StatusTexto));
                OnPropertyChanged(nameof(BotaoRegistroTexto));
            }
        }
    }

    public string StatusTexto => EstaDentro ? "DENTRO DA ACADEMIA" : "FORA DA ACADEMIA";
    public string BotaoRegistroTexto => EstaDentro ? "Registrar Saída" : "Registrar Entrada";

    private bool _temResultado;
    public bool TemResultado { get => _temResultado; set => SetProperty(ref _temResultado, value); }

    private bool _resultadoAutorizado;
    public bool ResultadoAutorizado
    {
        get => _resultadoAutorizado;
        set
        {
            if (SetProperty(ref _resultadoAutorizado, value))
                OnPropertyChanged(nameof(ResultadoCor));
        }
    }

    // Verde quando liberou, vermelho quando negou.
    public Color ResultadoCor => ResultadoAutorizado ? Color.FromArgb("#1B7F5A") : Color.FromArgb("#B3261E");

    private string _resultadoTitulo = string.Empty;
    public string ResultadoTitulo { get => _resultadoTitulo; set => SetProperty(ref _resultadoTitulo, value); }

    private string _resultadoMensagem = string.Empty;
    public string ResultadoMensagem { get => _resultadoMensagem; set => SetProperty(ref _resultadoMensagem, value); }

    public AcessoViewModel(IAcessoService acessoService, IAlunoService alunoService, IColaboradorService colaboradorService)
    {
        _acessoService = acessoService;
        _alunoService = alunoService;
        _colaboradorService = colaboradorService;
        Title = "Controle de Acesso";
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        await CarregarPessoasAsync();
        await CarregarAcessosHojeAsync();
    }

    [RelayCommand]
    private async Task SelecionarAlunoAsync()
    {
        ModoAluno = true;
        await TrocarModoAsync();
    }

    [RelayCommand]
    private async Task SelecionarColaboradorAsync()
    {
        ModoAluno = false;
        await TrocarModoAsync();
    }

    [RelayCommand]
    private async Task RegistrarAsync()
    {
        if (PessoaSelecionada == null || IsBusy)
            return;

        try
        {
            IsBusy = true;
            var resultado = ModoAluno
                ? await _acessoService.RegistrarAcessoAlunoAsync(PessoaSelecionada.Id)
                : await _acessoService.RegistrarAcessoColaboradorAsync(PessoaSelecionada.Id);

            ResultadoAutorizado = resultado.Autorizado;
            ResultadoTitulo = resultado.Titulo;
            ResultadoMensagem = resultado.Mensagem;
            TemResultado = true;

            await AtualizarStatusAsync();
            await CarregarAcessosHojeAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Erro", $"Erro ao registrar acesso: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AtualizarAcessosAsync() => await CarregarAcessosHojeAsync();

    private async Task TrocarModoAsync()
    {
        TermoBusca = string.Empty;
        PessoaSelecionada = null;
        TemResultado = false;
        await CarregarPessoasAsync();
    }

    private async Task CarregarPessoasAsync()
    {
        try
        {
            IsBusy = true;
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            _todasPessoas = ModoAluno
                ? [.. (await _alunoService.ObterTodosAsync(cts.Token)).Select(a => new PessoaAcessoItem(a.Id, a.Nome, a.Cpf, a.Foto))]
                : [.. (await _colaboradorService.ObterTodosAsync(cts.Token)).Select(c => new PessoaAcessoItem(c.Id, c.Nome, c.Cpf, c.Foto))];

            AplicarFiltro();
        }
        catch (OperationCanceledException)
        {
            await Shell.Current.DisplayAlertAsync("Tempo Esgotado", "A conexão com o banco de dados expirou (timeout).", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Erro", $"Erro ao carregar pessoas: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void AplicarFiltro()
    {
        var termo = TermoBusca.Trim();
        var filtradas = string.IsNullOrEmpty(termo)
            ? _todasPessoas
            : [.. _todasPessoas.Where(p => p.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase) || p.Cpf.Contains(termo))];

        PessoasFiltradas.Clear();
        foreach (var p in filtradas) PessoasFiltradas.Add(p);

        // Se sobrou um único resultado, já seleciona (como na tela de referência: digitar "333" seleciona o Aluno 3).
        PessoaSelecionada = PessoasFiltradas.Count == 1 ? PessoasFiltradas[0] : (PessoaSelecionada != null && PessoasFiltradas.Contains(PessoaSelecionada) ? PessoaSelecionada : null);
    }

    private async Task AtualizarStatusAsync()
    {
        if (PessoaSelecionada == null)
        {
            EstaDentro = false;
            return;
        }

        try
        {
            EstaDentro = ModoAluno
                ? await _acessoService.AlunoEstaNaAcademiaAsync(PessoaSelecionada.Id)
                : await _acessoService.ColaboradorEstaNaAcademiaAsync(PessoaSelecionada.Id);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Erro", $"Erro ao consultar situação: {ex.Message}", "OK");
        }
    }

    private async Task CarregarAcessosHojeAsync()
    {
        try
        {
            var acessos = await _acessoService.ObterAcessosDeHojeAsync();
            AcessosHoje.Clear();
            foreach (var a in acessos) AcessosHoje.Add(a);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Erro", $"Erro ao carregar acessos de hoje: {ex.Message}", "OK");
        }
    }
}
