Imports Microsoft.VisualBasic.Linq.LDM.Statements
Imports Microsoft.VisualBasic.Scripting.TokenIcer

Namespace Script.Tokens

    Public Class [Imports] : Inherits TokenBase
        Implements IEnumerable(Of String)

        Public ReadOnly Property Namespaces As String()

        Sub New(source As IEnumerable(Of Token(Of TokenIcer.Tokens)))
            Call MyBase.New(source)

            Namespaces = (From x As Token(Of TokenIcer.Tokens)
                          In source
                          Where Not x.Type = TokenIcer.Tokens.WhiteSpace
                          Select x.Text).ToArray
        End Sub

        Public Overrides Function ToString() As String
            Return "Imports " & String.Join(", ", Namespaces)
        End Function

        ''' <summary>
        ''' 枚举所有导入的命名空间
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function GetEnumerator() As IEnumerator(Of String) Implements IEnumerable(Of String).GetEnumerator
            For Each ns As String In Namespaces
                If Not String.IsNullOrEmpty(ns) Then
                    Yield ns
                End If
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace