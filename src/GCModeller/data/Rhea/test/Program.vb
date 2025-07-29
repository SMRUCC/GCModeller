Imports System
Imports SMRUCC.genomics.Data.Rhea

Module Program

    Const test_file = "E:\biodeep\biodeepdb_v3\rhea.rdf"

    Sub Main(args As String())
        Dim data As RheaRDF = RheaRDF.Load(test_file)
        Dim reactions As Reaction() = data.GetReactions().ToArray

        Pause()
    End Sub
End Module
