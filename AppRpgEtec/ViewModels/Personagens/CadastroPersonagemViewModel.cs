using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppRpgEtec.Models;
using AppRpgEtec.Models.Enuns;
using AppRpgEtec.Services.Personagens;


namespace AppRpgEtec.ViewModels.Personagens
{
    [QueryProperty("PersonagemSelecionadoId", "pId")]
    public class CadastroPersonagemViewModel : BaseViewModel
    {

        public ICommand SalvarCommand { get; }
        public ICommand CancelarCommand { get; set; }
        private PersonagemService pService;

        public CadastroPersonagemViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            pService = new PersonagemService(token);
            _ = ObterClasses();

            SalvarCommand = new Command(async () => { await SalvarPersonagem(); });
            CancelarCommand = new Command(async => CancelarCadastro());
        }

        private ObservableCollection<TipoClasse> listaTiposClasse;
        public ObservableCollection<TipoClasse> ListaTiposClasse
        {
            get { return listaTiposClasse; }
            set
            {
                if (value != null)
                {
                    ListaTiposClasse = value;
                    OnPropertyChanged();
                }
            }
        }

        private TipoClasse tipoClasseSelecionado;
        public TipoClasse TipoClasseSelecionado
        {
            get { return tipoClasseSelecionado; }
            set
            {
                if(value != null)
                {
                    tipoClasseSelecionado = value;
                    OnPropertyChanged();
                }
            }
        }

        public async Task ObterClasses()
        {
            try
            {
                ListaTiposClasse = new ObservableCollection<TipoClasse>();
                ListaTiposClasse.Add(new TipoClasse() { Id = 1, Descricao = "Cavaleiro " });
                ListaTiposClasse.Add(new TipoClasse() { Id = 2, Descricao = "Mago" });
                ListaTiposClasse.Add(new TipoClasse() { Id = 3, Descricao = "Clerigo" });
                OnPropertyChanged(nameof(ListaTiposClasse));
            }
            catch(Exception ex)
            {
                await Application.Current.MainPage
                        .DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        public async Task SalvarPersonagem()
        {
            try
            {
                Personagem model = new Personagem()
                {
                    Nome = this.nome,
                    PontosVida = this.pontosVida,
                    Defesa = this.defesa,
                    Derrotas = this.derrotas,
                    Disputas = this.disputas,
                    Forca = this.forca,
                    Inteligencia = this.Inteligencia,
                    Vitorias = this.vitorias,
                    Id = this.id,
                    Classe = (ClasseEnum)tipoClasseSelecionado.Id

                };

                if (model.Id == 0)
                    await pService.PostPersonagemAsync(model);
                else
                    await pService.PutPersonagemAsync (model);

                await Application.Current.MainPage
                      .DisplayAlert("Mensagem", "Dados salvos com sucesso!", "Ok");

                await Shell.Current.GoToAsync(".."); // Remove a página atual da pilha de páginas
            }
            catch(Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + "Detalhes: " + ex.InnerException, "Ok");
            }
        }

        private async void CancelarCadastro()
        {
            await Shell.Current.GoToAsync("..");
        }

        public async void CarregarPersonagem()
        {
            try
            {
                Personagem p = await
                pService.GetPersonagemAsync(int.Parse(personagemSelecionadoId));

                this.nome = p.Nome;
                this.pontosVida = p.PontosVida;
                this.defesa = p.Defesa;
                this.derrotas = p.Derrotas;
                this.forca = p.Forca;
                this.Inteligencia = p.Inteligencia;
                this.Vitorias = p.Vitorias;
                this.Id = p.Id;

                TipoClasseSelecionado = this.ListaTiposClasse
                    .FirstOrDefault(tClasse => tClasse.Id == (int)p.Classe);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + "Detalhes: " + ex.InnerException, "Ok");
            }
        }

        private string personagemSelecionadoId;
        private string nome;
        private int disputas;
        private string Nome;

        public string PersonagemSelecionadoId
        {
            set
            {
                if (value != null)
                {
                    personagemSelecionadoId = Uri.UnescapeDataString(value);
                    CarregarPersonagem();
                }
            }
        }

        public int pontosVida { get; private set; }
        public int defesa { get; private set; }
        public int derrotas { get; private set; }
        public int forca { get; private set; }

        private int Inteligencia;
        private int Vitorias;
        private int Id;
        private int vitorias;
        private int id;
    }
}
