Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Public Class eggNOG : Implements INamedValue

    Public Property query As String Implements INamedValue.Key
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

            Dim s As New StringArrayPointer(line.Split(ControlChars.Tab))

            Yield New eggNOG With {
                .query = s.ReadString,
                .seed_ortholog = s.ReadString,
                .evalue = s.ReadDouble,
                .score = s.ReadDouble,
                .eggNOG_OGs = s.ReadString,
                .max_annot_lvl = s.ReadString,
                .COG_category = s.ReadString,
                .Description = s.ReadString,
                .Preferred_name = s.ReadString,
                .GOs = s.ReadString,
                .EC = s.ReadString,
                .KEGG_ko = s.ReadString,
                .KEGG_Pathway = s.ReadString,
                .KEGG_Module = s.ReadString,
                .KEGG_Reaction = s.ReadString,
                .KEGG_rclass = s.ReadString,
                .BRITE = s.ReadString,
                .KEGG_TC = s.ReadString,
                .CAZy = s.ReadString,
                .BiGG_Reaction = s.ReadString,
                .PFAMs = s.ReadString
            }
        Next
    End Function

End Class
