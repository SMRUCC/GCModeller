Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class Project : Inherits ClassObject

    Public Property Summary As Summary
    Public Property Briefs As TableBrief()
    Public Property Ecards As Ecard()

    Public Function Write(EXPORT As String) As Boolean
        Call Summary.GetJson.SaveTo(EXPORT & "/" & NameOf(Summary) & ".json")
        Call Briefs.SaveTo(EXPORT & "/" & NameOf(Briefs) & ".Csv")
        Call Ecards.GetJson.SaveTo(EXPORT & "/" & NameOf(Ecards) & ".json")

        Return True
    End Function

    Public Shared Function Parser(DIR As String) As Project
        Dim details As String = DIR & "/basys_text_final/"
        Dim loads As IEnumerable(Of String) = ls - l - r - wildcards("*.ecard") <= details
        Dim proj As New Project With {
            .Summary = Summary.IndexParser(DIR & "/index.html"),
            .Briefs = TableBrief.TableParser(DIR & "/table.html")
        }
        Dim ecards As Ecard() =
            LinqAPI.Exec(Of Ecard) <=
 _
            From file As String
            In loads.AsParallel
            Select Ecard.Parser(file)

        proj.Ecards = ecards.ToArray

        Return proj
    End Function
End Class
