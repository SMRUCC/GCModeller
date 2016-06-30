Imports Microsoft.VisualBasic.Scripting.TokenIcer
Imports Microsoft.VisualBasic
Imports System.Runtime.CompilerServices

Namespace Script

    Public Module TokenIcer

        Public ReadOnly Property Tokens As IReadOnlyDictionary(Of String, Tokens) =
            New Dictionary(Of String, Tokens) From {
 _
            {Script.Tokens.Alias.Description, Script.Tokens.Alias},
            {Script.Tokens.Comment.Description, Script.Tokens.Comment},
            {Script.Tokens.Constant.Description, Script.Tokens.Constant},
            {Script.Tokens.InitValue.Description, Script.Tokens.InitValue},
            {Script.Tokens.Reaction.Description, Script.Tokens.Reaction},
            {Script.Tokens.Time.Description, Script.Tokens.Time},
            {Script.Tokens.Title.Description, Script.Tokens.Title}
        }

        Public Function TryParse(script As IEnumerable(Of String)) As Token(Of Tokens)()
            Dim LQuery = (From line As String In script
                          Let token As Token(Of Tokens) = line.Trim.__tokenParser()  ' 类型的前缀已经被切割掉了
                          Where Not token Is Nothing
                          Select token).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' 会忽略掉注释信息
        ''' </summary>
        ''' <param name="line"></param>
        ''' <returns></returns>
        <Extension> Private Function __tokenParser(line As String) As Token(Of Tokens)
            Dim x As String = line.Split.First.ToUpper

            If Not Tokens.ContainsKey(x) Then
                Return Nothing
            End If

            Dim type As Tokens = Tokens(x)
            If type = Script.Tokens.Comment Then
                Return Nothing
            End If
            Dim text As String = Mid(line, x.Length + 1).Trim
            Return New Token(Of Tokens)(type, text)
        End Function
    End Module
End Namespace