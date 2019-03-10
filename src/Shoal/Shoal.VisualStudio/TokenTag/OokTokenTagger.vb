Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.LDM.Expressions
Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Tagging
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.TextTokenliser
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter

Namespace OokLanguage

    Public NotInheritable Class OokTokenTagger : Implements ITagger(Of OokTokenTag)

        ReadOnly _buffer As ITextBuffer

        Public Sub New(buffer As ITextBuffer)
            _buffer = buffer
        End Sub

        Public Function GetTags(spans As NormalizedSnapshotSpanCollection) As IEnumerable(Of ITagSpan(Of OokTokenTag)) Implements ITagger(Of OokTokenTag).GetTags
            Dim tags As New List(Of TagSpan(Of OokTokenTag))

            For Each curSpan As SnapshotSpan In spans
                Dim containingLine As ITextSnapshotLine = curSpan.Start.GetContainingLine()
                Dim curLoc As Integer = containingLine.Start.Position
                Dim Expression As PrimaryExpression = Interpreter.InternalExpressionParser(curSpan.GetText)

                For Each ookToken As Token In Expression.GetTokens
                    Dim value As String = ookToken.GetTokenValue
                    Dim tokenSpan = New SnapshotSpan(curSpan.Snapshot, New Span(curLoc, value.Length))
                    If tokenSpan.IntersectsWith(curSpan) Then
                        Call tags.Add(New TagSpan(Of OokTokenTag)(tokenSpan, New OokTokenTag(ookToken.TokenType)))
                    End If

                    'add an extra char location because of the space
                    curLoc += value.Length + 1
                Next ookToken
            Next curSpan

            Return tags
        End Function

        Public Event TagsChanged(sender As Object, e As SnapshotSpanEventArgs) Implements ITagger(Of OokTokenTag).TagsChanged

    End Class
End Namespace