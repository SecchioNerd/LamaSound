using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace LamaSound;

class GestioneMusica{
    public static string LibPath = "Librerie";

    public static List<string> InitLibrerie(){
        List<string> librerie = new List<string>();
        if(Directory.Exists(LibPath))
            librerie = Directory.GetDirectories(LibPath+"/").ToList();

        return librerie;    
    }

    public static List<string> InitSongs(string libreria){
        List<string> listSongs = new List<string>();
        string fullPath = LibPath+"/"+libreria;
        if(Directory.Exists(fullPath)){
            listSongs = Directory.GetFiles(fullPath).ToList();
        }

        return listSongs;
    }

    public static List<string> RandomizeSongs(List<string> songs){
        Random r = new Random();

        List<string> randomSong = new List<string>();

        for(int i=0;i<songs.Count;i++){
            int index;
            bool isUsato;
            do{
                isUsato=false;
                index = r.Next(songs.Count);
                if(i!=0){
                    foreach (string s in randomSong){
                        if(s==songs[index]){
                            isUsato = true;
                            break;
                        }
                    }
                }
            }while(isUsato);
            randomSong.Add(songs[index]);
        }
        return randomSong;
    }
}