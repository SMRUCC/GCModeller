Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

''' <summary>
''' The microbiome KEGG pathway profile.
''' </summary>
Public Module PathwayProfile

    <Extension>
    Public Function PathwayProfile(metagenome As IEnumerable(Of TaxonomyRef), maps As MapRepository, Optional coverage# = 0.3) As Dictionary(Of String, Double)
        Dim profile As New Dictionary(Of String, Counter)

        For Each genome As TaxonomyRef In metagenome
            Dim KOlist$() = genome.KOTerms
            Dim pathways = maps _
                .QueryMapsByMembers(KOlist) _
                .Where(Function(map)
                           With map.Index.Objects.Where(Function(id) id.IsPattern("KO\d+")).ToArray
                               Return .Intersect(KOlist).Count / .Length >= coverage
                           End With
                       End Function) _
                .ToArray

            For Each map In pathways
                If Not profile.ContainsKey(map.MapID) Then
                    Call profile.Add(map.MapID, New Counter)
                End If

                Call profile(map.MapID).Hit()
            Next
        Next

        Return profile.AsNumeric
    End Function
End Module
