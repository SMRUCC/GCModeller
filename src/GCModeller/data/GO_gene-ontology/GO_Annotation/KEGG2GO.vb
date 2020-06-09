Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.Uniprot.XML

Public Module KEGG2GO

    <Extension>
    Public Iterator Function PopulateMappings(proteins As IEnumerable(Of entry)) As IEnumerable(Of (KO As String, GO As String()))
        For Each protein As entry In proteins
            Dim KO = protein.xrefs.TryGetValue("KO", [default]:=Nothing)
            Dim GO = protein.xrefs.TryGetValue("GO", [default]:=Nothing)

            If KO.IsNullOrEmpty Then
                Continue For
            ElseIf GO.IsNullOrEmpty Then
                Continue For
            End If

            For Each idRef As dbReference In KO
                Yield (idRef.id, GO.Select(Function(a) a.id).ToArray)
            Next
        Next
    End Function
End Module
