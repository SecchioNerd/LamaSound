using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.IO;
using System.Collections.Generic;

namespace LamaSound;

public partial class MainWindow : Window
{
    private readonly AudioPlayer _player = new AudioPlayer();

    public List<String> librerie;
    public List<string> songs;
    public int stack = 0; //stack per gestire funzioi avanti e indietro
    public string libreriaName;
    public bool isRiproduzioneRandom = false;
    public bool isSceltaRiproduzione = false;
    public int indexDiRiproduzione = 0;
    

    public MainWindow()
    {
        InitializeComponent();
        
        librerie = GestioneMusica.InitLibrerie();
        Successivo.Content=">|";
        Pausa.Content="||";
        Precedente.Content="|<";
        MenuStart();      

    }

    private void MenuStart(){
        if(librerie != null){
            int numRighe = librerie.Count;
            if(numRighe%2==1)
                numRighe++;
            numRighe/=2;
            
            ImpostaNumeroRigheGrid(GridLibs,numRighe);

            int counterRow = 0, counterCol = 0;

            for(int i=0;i<librerie.Count;i++){
                var pul = new Button {
                    //faccio in substring perchè se no scrive tutto il percorse
                    Content = librerie[i].Substring(GestioneMusica.LibPath.Length+1),
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch,
                    Margin = new Avalonia.Thickness(5)
                };

                pul.Click += ClickLibreria;

                //coordinate del pulsante nella Griglia
                counterRow = i/2;
                counterCol = i%2;

                Grid.SetRow(pul, counterRow);
                Grid.SetColumn(pul, counterCol);
                
                GridLibs.Children.Add(pul);
            }
        }
    }

    private void ClickLibreria (object? sender, RoutedEventArgs e){
        //libreriaName = sender.Content;
        if (sender is Button button)
        {
            // Recuperiamo il Content convertendolo in stringa
            libreriaName = button.Content?.ToString() ?? string.Empty;
            // Ora puoi usare la variabile 'testoPulsante'
        }
        
        stack++;
        NomeLibreria.Text=libreriaName;
        AggiornamentoScheramta();
        return;
    }

    private void Indietro (object? sender, RoutedEventArgs e){
        stack--;
        AggiornamentoScheramta();
        _player.Stop();
    }

    private void AggiornamentoScheramta(){
        GridLibs.IsVisible = (stack == 0);
        GridRiproduzzione.IsVisible = (stack == 1);
        GridAscolto.IsVisible = (stack == 2);
        SezzioneSelezione.IsVisible = (stack == 3);
    }

    private void RiproduzioneNormale(object? sender, RoutedEventArgs e){
        isRiproduzioneRandom = false;
        isSceltaRiproduzione = false;
        Riproduzione();
    }

    private void RiproduzioneRandom(object? sender, RoutedEventArgs e){
        isRiproduzioneRandom = true;
        isSceltaRiproduzione = false;
        Riproduzione();

    }

    private void RiproduzioneScelta(object? sender, RoutedEventArgs e){
        isRiproduzioneRandom = false;
        isSceltaRiproduzione = true;
        Riproduzione();

    }

    public void Riproduzione(){
        indexDiRiproduzione = 0;
        stack++;
        songs = GestioneMusica.InitSongs(libreriaName);
        NomeSong.Text = libreriaName;
        AggiornamentoScheramta();

        if(isRiproduzioneRandom){
            songs = GestioneMusica.RandomizeSongs(songs);
            _player.PlayPlaylist(songs);
        }
        else if(isSceltaRiproduzione){
            stack++;
            Opzioni.Items.Clear();
            AggiornamentoScheramta();

            //aggiungo le CheckBox per la selezione
            foreach(string song in songs){
                var opzione =new CheckBox{
                    Content=song
                };
                Opzioni.Items.Add(opzione); 
            }

            //Avanti: funzione che aggiorna SONG leggendo le CheckBox e le cancella poi e diminuisce lo stack
        }
        else{
            _player.PlayPlaylist(songs);
        }      
    }

    public void SelezioneSongsClick(object? sender, RoutedEventArgs e){
        songs.Clear();
        
        foreach (var item in Opzioni.Items)
        {
            // Verifichiamo che l'elemento sia effettivamente una CheckBox
            if (item is CheckBox checkBox)
            {
                // Verifichiamo se è spuntata
                if (checkBox.IsChecked == true)
                {
                    songs.Add(checkBox.Content?.ToString() ?? string.Empty);
                }
            }
        }
        Opzioni.Items.Clear();
        stack--;

        AggiornamentoScheramta();
        _player.PlayPlaylist(songs);
    }

    private void PausaORipresa(object? sender, RoutedEventArgs e){
        Console.WriteLine("Pausa/ripresa");
        if(!_player.isInPausa){
            Pausa.Content = "|>";
            _player.Stop();
        }
        else{
            Pausa.Content = "||";
            _player.Riprendi();
        }
    }

    public void PassaPrecedente(object? sender, RoutedEventArgs e){
        _player.Precedente();
    }

    public void PassaSuccessivo(object? sender, RoutedEventArgs e){
        _player.Successivo();
    }

    private void ImpostaNumeroRigheGrid(Grid grid, int numeroRighe)
    {
        // Pulisce le righe esistenti
        grid.RowDefinitions.Clear();

        // Aggiunge il nuovo numero di righe
        for (int i = 0; i < numeroRighe; i++)
        {
            // GridLength.Star (*) distribuisce lo spazio in modo equo.
            // Usa GridLength.Auto se vuoi che l'altezza si adatti al contenuto.
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        }
    }
}