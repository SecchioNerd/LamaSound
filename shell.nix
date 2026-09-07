{ pkgs ? import <nixpkgs> {} }:

let
  # Definizione delle librerie condivise da riutilizzare in LD_LIBRARY_PATH
  libs = with pkgs; [
    vlc
    fontconfig
    freetype
    xorg.libX11
    xorg.libXcursor
    xorg.libXrandr
    xorg.libXi
    xorg.libICE
    xorg.libSM
    libglvnd
    glib
    icu
    openssl
  ];

  # Pacchetto .NET 10 configurato con il workload Android
  dotnetWithAndroidWorkload = pkgs.dotnetCorePackages.combinePackages [
    pkgs.dotnetCorePackages.sdk_10_0
    pkgs.dotnetCorePackages.workloads.android
  ];
in
pkgs.mkShell {
  buildInputs = libs ++ [
    dotnetWithAndroidWorkload
    pkgs.android-tools      # Fornisce adb, fastboot, ecc.
    pkgs.android-studio     # Fornisce l'Android SDK
    pkgs.jdk17              # Richiesto dal sistema di build Android
  ];

  shellHook = ''
    export LD_LIBRARY_PATH="${pkgs.lib.makeLibraryPath libs}:$LD_LIBRARY_PATH"
    
    # Imposta la variabile d'ambiente ANDROID_HOME per la CLI di .NET
    export ANDROID_HOME="${pkgs.android-studio}/share/android-studio/sdk"
  '';
}
