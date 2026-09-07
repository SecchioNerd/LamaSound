using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using Avalonia.Controls;

namespace LamaSound;

public class AudioPlayer
{
    private readonly LibVLC _libVLC;
    private readonly MediaPlayer _mediaPlayer;
    private List<string> _playlist = new();
    private int _currentIndex = 0;
    public bool isInPausa;
    public TextBlock _nomeSong;

    public AudioPlayer()
    {
        Core.Initialize();
        _libVLC = new LibVLC();
        _mediaPlayer = new MediaPlayer(_libVLC);

        _mediaPlayer.EndReached += OnSongEnded;
    }

    public void PlayPlaylist(List<string> songs)
    {
        if (songs == null || songs.Count == 0) return;

        _playlist = songs;
        _currentIndex = 0;
        PlayCurrentSong();
    }

    private void PlayCurrentSong()
    {
        if (_currentIndex < 0 || _currentIndex >= _playlist.Count) return;

        String songPath = _playlist[_currentIndex];

        using var media = new Media(_libVLC, songPath, FromType.FromPath);
        _mediaPlayer.Play(media);
    }

    private void OnSongEnded(object? sender, EventArgs e)
    {
        _currentIndex++;

        if (_currentIndex >= _playlist.Count)
            _currentIndex=0;
        
        Task.Run(async () =>
        {
            // Attesa di 1 secondo prima di riprodurre la traccia successiva
            await Task.Delay(500); 
            PlayCurrentSong();
        });
    }

    public void Stop()
    {
        _mediaPlayer.Stop();
        isInPausa = true;
    }

    public void Riprendi(){
        _mediaPlayer.Play();
        isInPausa = false;
    }

    public void Precedente(){
        
        _mediaPlayer.Stop();
        _currentIndex--;
        if(_currentIndex==-1)
            _currentIndex=_playlist.Count-1;
        
        PlayCurrentSong();

    }

    public void Successivo(){
        
        _mediaPlayer.Stop();
        _currentIndex++;
        if(_currentIndex==_playlist.Count)
            _currentIndex=0;
        
        PlayCurrentSong();
    
    }
}