Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash

Public Class Project : Inherits ClassObject

    Public Property Summary As Summary
    Public Property Briefs As TableBrief()
    Public Property Ecards As Ecard()

    Public Function Write(EXPORT As String) As Boolean

    End Function

    Public Shared Function Parser(DIR As String) As Project
        Dim details As String = DIR & "/basys_text_final/"
        Dim loads As IEnumerable(Of String) = ls - l - r - wildcards("*.ecard") <= details
        Dim ecards As Ecard() =
            LinqAPI.Exec(Of Ecard) <=
 _
            From file As String In loads.AsParallel Select Ecard.Parser(file)

        Return New Project With {
            .Summary = Summary.IndexParser(DIR & "/index.html"),
            .Briefs = TableBrief.TableParser(DIR & "/table.html"),
            .Ecards = ecards.ToArray
        }
    End Function
End Class
