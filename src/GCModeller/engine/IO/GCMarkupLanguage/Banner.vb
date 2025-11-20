Imports System.IO

Public Module Banner

    Public Const BannerInformation As String = "

 Welcome To
      _____  _____ __  __           _      _ _           
     / ____|/ ____|  \/  |         | |    | | |          
    | |  __| |    | \  / | ___   __| | ___| | | ___ _ __ 
    | | |_ | |    | |\/| |/ _ \ / _` |/ _ \ | |/ _ \ '__|
    | |__| | |____| |  | | (_) | (_| |  __/ | |  __/ |   
     \_____|\_____|_|  |_|\___/ \__,_|\___|_|_|\___|_|   
                                                      
                      Virtual Cell Simulation Tool - 2025

                         https://biocad.innovation.ac.cn/
                         https://gcmodeller.org/

"

    Public Sub Print(console As TextWriter)
        For Each line As String In Banner.BannerInformation.LineTokens
            Call console.WriteLine(line)
        Next

        Call console.Flush()
    End Sub

End Module
