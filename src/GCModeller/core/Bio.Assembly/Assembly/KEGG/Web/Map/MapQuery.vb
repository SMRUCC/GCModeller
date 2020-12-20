Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Net.Http
Imports PathwayEntry = SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.Pathway
Imports r = System.Text.RegularExpressions.Regex

Namespace Assembly.KEGG.WebServices

    Public Class MapQuery : Inherits WebQuery(Of PathwayEntry)

        Public Sub New(briteFile$,
                       <CallerMemberName>
                       Optional cache As String = Nothing,
                       Optional interval As Integer = -1,
                       Optional offline As Boolean = False)

            Call MyBase.New(
                createUrl(briteFile),
                AddressOf getID(briteFile).Invoke,
                AddressOf ParseHtml,
                prefix:=Nothing,
                cache:=cache,
                interval:=interval,
                offline:=offline
            )
        End Sub

        Friend Shared Function getID(briteFile As String) As Func(Of PathwayEntry, String)
            Return Function(entry As PathwayEntry)
                       If briteFile.StringEmpty Then
                           Return "map" & entry.EntryId
                       Else
                           Dim s = entry.entry.text
                           s = r.Match(s, "\[PATH:.+?\]", RegexICSng).Value
                           s = s.GetStackValue("[", "]").Split(":"c).Last
                           Return s
                       End If
                   End Function
        End Function

        Private Shared Function ParseHtml(html$, schema As Type) As Object
            Return Map.ParseHTML(html)
        End Function

        Private Shared Function createUrl(briteFile As String) As Func(Of PathwayEntry, String)
            Dim getID = MapQuery.getID(briteFile)

            Return Function(entry)
                       Return $"http://www.genome.jp/kegg-bin/show_pathway?{getID(entry)}"
                   End Function
        End Function
    End Class
End Namespace