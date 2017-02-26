Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language

Namespace Assembly.Uniprot.XML

    Public Module Extensions

        <Extension> Public Function proteinFullName(protein As entry) As String
            If protein.protein Is Nothing Then
                Return ""
            Else
                Return protein.protein.FullName
            End If
        End Function

        <Extension> Public Function ORF(protein As entry) As String
            If protein.gene Is Nothing OrElse Not protein.gene.HaveKey("ORF") Then
                Return Nothing
            Else
                Return protein.gene.ORF.First
            End If
        End Function

        <Extension>
        Public Function Term2Gene(uniprotXML As UniprotXML, Optional type$ = "GO") As IDMap()
            Dim out As New List(Of IDMap)

            For Each prot As entry In uniprotXML.entries
                Dim ID As String = prot.accession

                If prot.Xrefs.ContainsKey(type) Then
                    out += From term As dbReference
                           In prot.Xrefs(type)
                           Select New IDMap With {
                               .Key = term.id,
                               .Maps = ID
                           }
                End If
            Next

            Return out
        End Function
    End Module
End Namespace