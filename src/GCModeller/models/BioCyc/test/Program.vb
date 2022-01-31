Imports System
Imports System.IO
Imports BioCyc

Module Program
    Sub Main(args As String())
        Using file As StreamReader = "P:\2022_nar\25.5\data\protein-features.dat".OpenReader
            Dim data = AttrValDatFile.ParseFile(file)

            Pause()
        End Using
    End Sub
End Module
