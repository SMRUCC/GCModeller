Imports System.IO
Imports StringReader = Microsoft.VisualBasic.ComponentModel.DataSourceModel.StringReader

Public Class eggNOG

    Public Property query As String
    Public Property seed_ortholog As String
    Public Property evalue As Double
    Public Property score As Double
    Public Property eggNOG_OGs As String
    Public Property max_annot_lvl As String
    Public Property COG_category As String
    Public Property Description As String
    Public Property Preferred_name As String
    Public Property GOs As String
    Public Property EC As String
    Public Property KEGG_ko As String
    Public Property KEGG_Pathway As String
    Public Property KEGG_Module As String
    Public Property KEGG_Reaction As String
    Public Property KEGG_rclass As String
    Public Property BRITE As String
    Public Property KEGG_TC As String
    Public Property CAZy As String
    Public Property BiGG_Reaction As String
    Public Property PFAMs As String

    Public Shared Iterator Function ParseTable(file As Stream) As IEnumerable(Of eggNOG)
        For Each line As String In file.ReadAllLines
            If line.StartsWith("#"c) Then
                Continue For
            End If

            Dim t As String() = line.Split(ControlChars.Tab)
            Dim cols As New StringReader

            Yield New eggNOG With {
                .query = t(0),
                .seed_ortholog = t(1),
                .evalue = t(),
                .
            }
        Next
    End Function

End Class
