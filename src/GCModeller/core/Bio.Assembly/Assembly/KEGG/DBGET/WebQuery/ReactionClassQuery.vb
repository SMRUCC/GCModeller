Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Terminal.ProgressBar
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers
Imports SMRUCC.genomics.ComponentModel

Namespace Assembly.KEGG.DBGET.WebQuery

    Public Class ReactionClassQuery : Inherits WebQuery(Of BriteHEntry.ReactionClass)

        Public Sub New(<CallerMemberName>
                       Optional cache As String = Nothing,
                       Optional interval As Integer = -1,
                       Optional offline As Boolean = False)

            MyBase.New(
                url:=AddressOf rcURL,
                contextGuid:=AddressOf getRCNumber,
                parser:=AddressOf ParseHtml,
                prefix:=Nothing,
                cache:=cache,
                interval:=interval,
                offline:=offline
            )
        End Sub

        Private Shared Function getRCNumber(entry As BriteHEntry.ReactionClass) As String
            Return entry.RCNumber
        End Function

        Private Shared Function rcURL(entry As BriteHEntry.ReactionClass) As String
            Return $"https://www.kegg.jp/dbget-bin/www_bget?rc:{entry.RCNumber}"
        End Function

        Private Shared Function ParseHtml(html$, schema As Type) As Object
            Return ReactionClassWebQuery.ParseReactionClass(html)
        End Function
    End Class

    Public Module ReactionClassWebQuery

        Public Function DownloadReactionClass(export$, Optional cache$ = Nothing) As IEnumerable(Of String)
            Dim web As New ReactionClassQuery(cache Or $"{export}/.cache/".When(cache.StringEmpty))
            Dim numbers As BriteHEntry.ReactionClass() = BriteHEntry.ReactionClass _
                .LoadFromResource _
                .ToArray
            Dim failures As New List(Of String)

            Using progressbar As New ProgressBar("Download reaction class numbers...", 1, True)
                Dim progress As New ProgressProvider(numbers.Length)
                Dim save$
                Dim rcnumber As ReactionClass

                For Each number As BriteHEntry.ReactionClass In numbers
                    save = $"{export}/{number.GetPathComponents}/{number.RCNumber}.xml"
                    rcnumber = web.Query(Of ReactionClass)(number, ".html")

                    Call rcnumber _
                        .GetXml _
                        .SaveTo(save)
                Next
            End Using

            Return failures
        End Function

        Public Function GetReactionClass(id As String, Optional cache$ = "./br08204/") As ReactionClass
            Static handler As New Dictionary(Of String, ReactionClassQuery)

            Return handler.ComputeIfAbsent(
                key:=cache,
                lazyValue:=Function()
                               Return New ReactionClassQuery(cache)
                           End Function) _
                .Query(Of ReactionClass)(New BriteHEntry.ReactionClass With {.RCNumber = id}, ".html")
        End Function

        Friend Function ParseReactionClass(html As String) As ReactionClass
            Dim web As New WebForm(html)
            Dim [class] As New ReactionClass

            Return [class]
        End Function
    End Module
End Namespace