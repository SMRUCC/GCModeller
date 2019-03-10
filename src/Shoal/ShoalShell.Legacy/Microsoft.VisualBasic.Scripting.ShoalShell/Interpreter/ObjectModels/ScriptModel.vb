Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.ObjectModels.Statements
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.ObjectModels.Tokens

Namespace Interpreter.ObjectModels

    ''' <summary>
    ''' 只有解析出来的词元模型，，没有包含有函数指针的句柄信息，还不能够直接运行
    ''' </summary>
    Public Class ScriptModel

        Public Property Expressions As Statement()
        ''' <summary>
        ''' Value是指向<see cref="Expressions"/>列表之中的元素的位置下表
        ''' </summary>
        ''' <returns></returns>
        Public Property GotoFlags As Dictionary(Of String, Integer)
        Public Property File As String

        ''' <summary>
        ''' 脚本文件的文件路径
        ''' </summary>
        ''' <param name="ScriptText"></param>
        ''' <param name="File"></param>
        ''' <returns></returns>
        Public Shared Function ScriptParser(ScriptText As String, File As String) As ScriptModel
            Dim s_Data As String() = ScriptText.Replace(vbLf, "").Split(CChar(vbCr))
            Dim LQuery = (From i As Integer In s_Data.Sequence'.AsParallel
                          Let Expr = SyntaxParser.Parsing(s_Data(i))?.InvokeSet(Of Integer)(NameOf(Statement.OriginalLineNumber), i)
                          Where Not Expr Is Nothing
                          Select i, Expr
                          Order By i Ascending).ToList

            '检查是否有语法错误
            Dim GetSyntaxError = (From Token In LQuery.AsParallel
                                  Where Token.Expr.TypeID = Statement.Types.SyntaxError
                                  Select Token.Expr.As(Of Statements.SyntaxError)).ToArray
            Dim SyntaxErrorChecks = (From Token In GetSyntaxError
                                     Where Token.IsSyntaxError
                                     Select Token).ToArray

            If Not SyntaxErrorChecks.IsNullOrEmpty Then Call ThrowSyntaxException(SyntaxErrorChecks, File) '有语法错误，则抛出错误

            Dim GotoTags As Dictionary(Of String, Integer) =  ' GotoTag的下一行就是目标起始点
                New Dictionary(Of String, Integer)
            Dim p As Integer = -1

            Do While p.MoveNext < LQuery.Count - 1

                Dim Line = LQuery(p)

                If Line.Expr.TypeID = Statement.Types.GotoTag Then

                    '移除这一行，同时标记下一行的行号，由于移除了这一行，则下一行会上移一行，则行号就是这个i的值
                    Call LQuery.RemoveAt(p)
                    Call GotoTags.Add(DirectCast(Line.Expr, GotoTag).TagData, p)
                End If
            Loop

            Return New ScriptModel With {
                .Expressions = (From Line In LQuery Select Line.Expr).ToArray,
                .GotoFlags = GotoTags,
                .File = File
            }
        End Function

        Public Shared Function LoadFile(path As String) As ScriptModel
            Dim Script As String = FileIO.FileSystem.ReadAllText(path)
            Return ScriptModel.ScriptParser(Script, path)
        End Function

        ''' <summary>
        ''' at Shoal.Testing.Debugger.Main() in G:\Shoal\Shoal.Testing\Debugger.vb:line 21
        ''' </summary>
        ''' <param name="Tokens"></param>
        ''' <param name="File"></param>
        Private Shared Sub ThrowSyntaxException(Tokens As Statements.SyntaxError(), File As String)
            If Not String.IsNullOrEmpty(File) Then
                File = $" {File}"
            Else
                File = " "
            End If

            Dim Details As String = String.Join(vbCrLf, (From Token As Statements.SyntaxError
                                                         In Tokens
                                                         Select str = $"  at {Token.Expression}   in{File}:line {Token.OriginalLineNumber}").ToArray)
            Details = "The syntax in these statements is currently not support yet:" & vbCrLf & vbCrLf &
                Details
#If DEBUG Then
            Call Console.WriteLine(Details)
#End If
            Throw New SyntaxErrorException(Details)
        End Sub
    End Class
End Namespace