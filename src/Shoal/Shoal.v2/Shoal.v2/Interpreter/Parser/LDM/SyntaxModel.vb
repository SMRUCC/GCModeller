Imports System.Text
Imports System.Web.Script.Serialization
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.LDM.Expressions

Namespace Interpreter.LDM

    ''' <summary>
    ''' This object just the data model which was generated from the script parser, didn't contains any runtime information.
    ''' (只有解析出来的词元模型，，没有包含有函数指针的句柄信息，还不能够直接运行)
    ''' </summary>
    Public Class SyntaxModel

        ''' <summary>
        ''' The script lines in the script file.
        ''' </summary>
        ''' <returns></returns>
        Public Property Expressions As PrimaryExpression()
        ''' <summary>
        ''' Value是指向<see cref="Expressions"/>列表之中的元素的位置下表
        ''' </summary>
        ''' <returns></returns>
        Public Property GotoJumpsLabel As Dictionary(Of String, Integer)

        Public Overrides Function ToString() As String
            Try
                Return FilePath.ToFileURL
            Catch ex As Exception
                Return FilePath
            End Try
        End Function

        <XmlIgnore> <ScriptIgnore>
        Public Property FilePath As String

        Public Shared Function Parser(Script As String, ParserInvoke As Interpreter.Parser, File As String) As LDM.Expressions.PrimaryExpression()
            Dim Expressions = ParserInvoke(Script)

            '检查是否有语法错误
            Dim GetSyntaxError = (From Token In Expressions.AsParallel
                                  Where Token.ExprTypeID = LDM.Expressions.ExpressionTypes.SyntaxError
                                  Select Token.As(Of LDM.Expressions.Keywords.SyntaxError)).ToArray
            Dim SyntaxErrorChecks = (From Token In GetSyntaxError
                                     Where Token.IsSyntaxError
                                     Select Token).ToArray

            If Not SyntaxErrorChecks.IsNullOrEmpty Then Call ThrowSyntaxException(SyntaxErrorChecks, File) '有语法错误，则抛出错误

            Return Expressions
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ScriptText"></param>
        ''' <param name="File">脚本文件的文件路径</param>
        ''' <returns></returns>
        Public Shared Function ScriptParser(ScriptText As String, File As String) As SyntaxModel
            Dim Expressions = Interpreter.MSLParser(ScriptText).AsList
            Dim script As SyntaxModel = CreateObject(Expressions)
            script.FilePath = File
            Return script
        End Function

        Public Overloads Shared Function CreateObject(Expressions As List(Of LDM.Expressions.PrimaryExpression)) As SyntaxModel
            Dim GotoTags As Dictionary(Of String, Integer) =  ' GotoTag的下一行就是目标起始点
                New Dictionary(Of String, Integer)
            Dim p As i32 = -1

            Do While ++p < Expressions.Count - 1

                Dim Line = Expressions(index:=p)

                If Line.ExprTypeID = LDM.Expressions.ExpressionTypes.LineLable Then

                    '移除这一行，同时标记下一行的行号，由于移除了这一行，则下一行会上移一行，则行号就是这个i的值
                    Call Expressions.RemoveAt(p)
                    Call GotoTags.Add(DirectCast(Line, LDM.Expressions.ControlFlows.LineLabel).Label, p)
                End If
            Loop

            Return New SyntaxModel With {
                .Expressions = Expressions.ToArray,
                .GotoJumpsLabel = GotoTags
            }
        End Function

        Public Shared Function LoadFile(path As String) As SyntaxModel
            Dim Script As String = VisualBasic.FileIO.FileSystem.ReadAllText(path)
            Dim LDM = SyntaxModel.ScriptParser(Script, path)
            Return LDM
        End Function

        ''' <summary>
        ''' at Shoal.Testing.Debugger.Main() in G:\Shoal\Shoal.Testing\Debugger.vb:line 21
        ''' </summary>
        ''' <param name="Tokens"></param>
        ''' <param name="File"></param>
        Private Shared Sub ThrowSyntaxException(Tokens As LDM.Expressions.Keywords.SyntaxError(), File As String)
            If Not String.IsNullOrEmpty(File) Then
                File = $" {File}"
            Else
                File = " "
            End If

            Dim Details As String = String.Join(vbCrLf, (From Token As LDM.Expressions.Keywords.SyntaxError
                                                         In Tokens
                                                         Select str = $"  at {Token.PrimaryExpression}   in{File}:line {Token.LineNumber}").ToArray)
            Details = "The syntax in these statements is currently not support yet:" & vbCrLf & vbCrLf &
                Details
#If DEBUG Then
            Call Console.WriteLine(Details)
#End If
            Throw New SyntaxErrorException(Details)
        End Sub

        ''' <summary>
        ''' 在<see cref="LDM.Expressions.Keywords.SyntaxError"/>的命名空间之下的所有的类型都是脚本之中的关键词
        ''' </summary>
        ''' <returns></returns>
        Public Shared ReadOnly Property Keywords As String() = __getKeywords()

        Private Shared Function __getKeywords() As String()
            Dim Assembly As System.Reflection.Assembly = GetType(SyntaxModel).Assembly
            Dim Types = Assembly.GetTypes
            Dim KwNs As String = GetType(LDM.Expressions.Keywords.SyntaxError).FullName
            KwNs = Mid(KwNs, 1, Len(KwNs) - Len(NameOf(LDM.Expressions.Keywords.SyntaxError)))
            Dim LQuery = (From kwType In Types Let Name As String = kwType.FullName Where InStr(Name, KwNs) > 0 Select kwType.Description).ToArray
            Return LQuery
        End Function

        Public Function Save(FilePath As String, Optional Encoding As Encoding = Nothing) As Boolean
            Return Me.GetXml.SaveTo(FilePath, Encoding)
        End Function
    End Class
End Namespace