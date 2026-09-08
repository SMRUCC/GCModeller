Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.RetroPath
Imports SMRUCC.genomics.Analysis.RetroPath.Chem
Imports SMRUCC.genomics.Analysis.RetroPath.Model
Imports SMRUCC.genomics.Analysis.RetroPath.Search
Imports SMRUCC.genomics.Data.BioCyc

Public Class BioCycAdapter

    ReadOnly netwalk As Netwalk

    Sub New(biocyc As Workspace, Optional opts As SearchOptions = Nothing, Optional w As ScoreWeights = Nothing)
        Dim compounds As compounds() = biocyc.compounds.AsEnumerable.ToArray
        Dim reactions As reactions() = biocyc.reactions.AsEnumerable.ToArray
        Dim rules As New List(Of Rule)
        Dim sink As New List(Of (String, smiles As String))

        opts = If(opts, New SearchOptions)
        w = If(w, New ScoreWeights)
        netwalk = New Netwalk(rules, sink, opts, w)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function FindPathway(targetSmiles As String) As PathReport
        Return netwalk.Search(targetSmiles)
    End Function

End Class
