Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.LDM
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.LDM.Expressions
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.LDM.Expressions.Keywords
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.TextTokenliser

Namespace Interpreter

    ''' <summary>
    ''' Parsing the script text into a LDM data model for the script executing. 
    ''' </summary>
    ''' <remarks>
    ''' 命令行输入的脚本行，一般认为是完整的一行，则直接使用<see cref="Interpreter.InternalExpressionParser(String)"/>来解析
    ''' 从脚本文件之中输入的脚本文件可能含有Delegate函数或者多行的数组定义，则脚本文件是使用<see cref="MSLTokens"/>来解析的
    ''' </remarks>
    Public Class Interpreter : Inherits Runtime.SCOM.RuntimeComponent

        Public ReadOnly Property SPMDevice As SPM.ShoalPackageMgr
        Public ReadOnly Property EPMDevice As Linker.EntryPoint

        Sub New(ScriptEngine As Runtime.ScriptEngine, SPMDevice As SPM.ShoalPackageMgr)
            Call MyBase.New(ScriptEngine)

            Me.SPMDevice = SPMDevice
            Me.EPMDevice = New Linker.EntryPoint(ScriptEngine)
        End Sub

        ''' <summary>
        ''' 使用<see cref="MSLTokens"/>来解析
        ''' </summary>
        ''' <param name="File">
        ''' The parser function supports both local file system url or http network file.
        ''' (文件路径)
        ''' </param>
        ''' <returns></returns>
        Public Function ParseFile(File As String) As SyntaxModel
#If Not DEBUG Then
            Try
#End If
            Return __parsingFile(File)
#If Not DEBUG Then
            Catch ex As Exception
                Call App.LogException(ex, $"{NameOf(Interpreter)}::{NameOf(ParseFile)}")

                Return New LDM.SyntaxModel With {
                    .Expressions = New PrimaryExpression() {}
                }
            End Try
#End If
        End Function

        ''' <summary>
        ''' 同时支持网络位置或者本地文件系统的位置
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Private Function __parsingFile(path As String) As SyntaxModel
            Dim Script As String = path.GET
            Return SyntaxModel.ScriptParser(Script, path)
        End Function

        Public Function TryGetAPI(Name As String) As Linker.APIHandler.APIEntryPoint
            Return EPMDevice.ImportedAPI(Name)
        End Function

        Protected Overrides Sub Dispose(disposing As Boolean)
            Call SPMDevice.Dispose()
            Call MyBase.Dispose(disposing)
        End Sub

        Delegate Function Parser(Script As String) As PrimaryExpression()

        ''' <summary>
        ''' 处理来自于文件之中的可能具有多行分行的脚本代码
        ''' </summary>
        ''' <param name="Script"></param>
        ''' <returns></returns>
        Public Shared Function MSLParser(Script As String) As PrimaryExpression()
            Dim lines As String() = Script.lTokens
            Dim Expressions As New List(Of PrimaryExpression)
            Dim ExprQueue As New Queue(Of String)(lines)

            Do While Not ExprQueue.IsNullOrEmpty
                Expressions += SyntaxParser.MSLParser(ExprQueue)
                Expressions.Last.LineNumber = Expressions.Count
            Loop

            Return Expressions
        End Function

        ''' <summary>
        ''' 处理来自于终端输入的只有一行的<see cref="ShoalShell.Interpreter.Parser.Tokens.InternalExpression"/>脚本代码
        ''' </summary>
        ''' <param name="Line"></param>
        ''' <returns></returns>
        Public Shared Function InternalExpressionParser(Line As String) As PrimaryExpression
            If String.IsNullOrEmpty(Line) Then
                Return SyntaxError.BlankCode
            Else

                Dim Parser As MSLTokens = New MSLTokens().Parsing(Line)
                Dim Expr As PrimaryExpression =
                    SyntaxParser.Parsing(Line, Parser.Tokens, Parser.Comments)
                Return Expr  ' 假若需要进行Tokens的类型的判断，则可以在得到表达式之后，通过表达式的类型的计算出Tokens的类型
            End If
        End Function
    End Class
End Namespace