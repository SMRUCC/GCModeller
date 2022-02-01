Imports System
Imports System.IO
Imports BioCyc

Module Program
    Sub Main(args As String())

        Using file As Stream = "P:\2022_nar\25.5\data\pathways.dat".Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Dim data = AttrDataCollection(Of pathways).LoadFile(file)

            Pause()
        End Using

        Using file As StreamReader = "P:\2022_nar\25.5\data\protein-features.dat".OpenReader
            Dim data = AttrValDatFile.ParseFile(file)

            Pause()
        End Using
    End Sub
End Module
