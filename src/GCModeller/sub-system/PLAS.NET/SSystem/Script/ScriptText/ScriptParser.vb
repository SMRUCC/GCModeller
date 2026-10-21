#Region "Microsoft.VisualBasic::061626b13533a231ca1db558ba99e059, sub-system\PLAS.NET\SSystem\Script\ScriptText\ScriptParser.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 189
    '    Code Lines: 154 (81.48%)
    ' Comment Lines: 11 (5.82%)
    '    - Xml Docs: 81.82%
    ' 
    '   Blank Lines: 24 (12.70%)
    '     File Size: 7.11 KB


    '     Module ScriptParser
    ' 
    '         Function: getCommentText, getScriptTokenGroups, getTimeFinalConfig, getTitleOrDefault, getUserFunctionOrDefault
    '                   ParseFile, ParseScript, ParseStream, setConstants
    ' 
    '         Sub: setAliasNames, setObjectComments
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression
Imports SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels

Namespace Script

    Public Module ScriptParser

        <Extension>
        Private Function getScriptTokenGroups(script As String(), ByRef errors As List(Of ScriptError)) As Dictionary(Of ScriptTokens, ScriptToken())
            Dim tokens As ScriptToken() = TokenIcer.TryParse(script, errors)
            Dim tokenGroups As Dictionary(Of ScriptTokens, ScriptToken()) =
                (From x As ScriptToken
                 In tokens
                 Select x
                 Group x By x.name Into Group) _
                     .ToDictionary(Function(x) x.name,
                                   Function(x)
                                       Return x.Group.ToArray
                                   End Function)
            Return tokenGroups
        End Function

        <Extension>
        Private Function setConstants(env As ExpressionEngine, tokens As Dictionary(Of ScriptTokens, ScriptToken())) As NamedValue(Of String)()
            Dim c As NamedValue(Of String)()

            If tokens.ContainsKey(ScriptTokens.Constant) Then
                c = tokens(ScriptTokens.Constant) _
                    .Select(AddressOf ModelParsers.ConstantParser) _
                    .ToArray
            Else
                c = {}
            End If

            For Each x As NamedValue(Of String) In c
                Call env.SetSymbol(x.Name, x.Value)
            Next

            Return c
        End Function

        ''' <summary>
        ''' 解析脚本之中的 FINALTIME 配置值，FINALTIME 允许为浮点数值，
        ''' 故这里不可以将配置值截断为整数，否则 FINALTIME 4.5 会被舍入掉。
        ''' </summary>
        ''' <param name="tokens"></param>
        ''' <param name="env"></param>
        ''' <returns></returns>
        <Extension>
        Private Function getTimeFinalConfig(tokens As Dictionary(Of ScriptTokens, ScriptToken()), env As ExpressionEngine) As Double
            If Not tokens.ContainsKey(ScriptTokens.Time) Then
                Return 100
            Else
                Return tokens(ScriptTokens.Time) _
                    .First _
                    .DoCall(Function(t) CDbl(env.Evaluate(t.text)))
            End If
        End Function

        <Extension>
        Private Function getTitleOrDefault(tokens As Dictionary(Of ScriptTokens, ScriptToken())) As String
            If Not tokens.ContainsKey(ScriptTokens.Title) Then
                Return "UNNAMED TITLE"
            Else
                Return tokens(ScriptTokens.Title).First.text
            End If
        End Function

        <Extension>
        Private Function getCommentText(tokens As Dictionary(Of ScriptTokens, ScriptToken())) As String
            If tokens.ContainsKey(ScriptTokens.Comment) Then
                Return tokens(ScriptTokens.Comment) _
                    .Select(Function(x) x.text) _
                    .JoinBy(vbCrLf)
            Else
                Return ""
            End If
        End Function

        <Extension>
        Private Function getUserFunctionOrDefault(tokens As Dictionary(Of ScriptTokens, ScriptToken())) As [Function]()
            If tokens.ContainsKey(ScriptTokens.Function) Then
                Return tokens(ScriptTokens.Function) _
                    .Select(Function(x) CType(x.text, [Function])) _
                    .ToArray
            Else
                Return {}
            End If
        End Function

        ''' <summary>
        ''' 设置脚本之中所申明的变量的别名：``NAMED &lt;var&gt; &lt;alias&gt;``
        ''' </summary>
        ''' <param name="model"></param>
        ''' <param name="tokens">
        ''' 词法器已经将 NAMED 关键字剥离掉，所以这里的 text 就是 ``&lt;var&gt; &lt;alias&gt;``
        ''' </param>
        <Extension>
        Private Sub setAliasNames(model As Model, tokens As Dictionary(Of ScriptTokens, ScriptToken()))
            Dim nameList As String()

            If tokens.ContainsKey(ScriptTokens.Alias) Then
                nameList = tokens(ScriptTokens.Alias) _
                    .Select(Function(x) x.text) _
                    .ToArray
            Else
                nameList = {}
            End If

            For Each s As String In nameList
                Dim name As String = s.Split.First
                Dim target As var = model.FindObject(name)

                If target Is Nothing Then
                    Call divertMissing(s, $"alias name target ""{name}"" was not declared in this script")
                Else
                    target.title = Mid(s, Len(name) + 2).Trim()
                End If
            Next
        End Sub

        ''' <summary>
        ''' 设置脚本之中所申明的变量的注释信息：``REM-SUB &lt;var&gt; &lt;comment&gt;``
        ''' </summary>
        ''' <param name="model"></param>
        ''' <param name="tokens">
        ''' 词法器已经将 REM-SUB 关键字剥离掉，所以这里的 text 就是 ``&lt;var&gt; &lt;comment&gt;``
        ''' </param>
        <Extension>
        Private Sub setObjectComments(model As Model, tokens As Dictionary(Of ScriptTokens, ScriptToken()))
            Dim comments As String()

            If tokens.ContainsKey(ScriptTokens.SubsComments) Then
                comments = tokens(ScriptTokens.SubsComments).Select(Function(x) x.text).ToArray
            Else
                comments = {}
            End If

            For Each s As String In comments
                Dim name As String = s.Split.First
                Dim target As var = model.FindObject(name)

                If target Is Nothing Then
                    Call divertMissing(s, $"comment target ""{name}"" was not declared in this script")
                Else
                    target.comment = Mid(s, Len(name) + 2).Trim()
                End If
            Next
        End Sub

        ''' <summary>
        ''' 别名或者注释所指向的目标对象并没有在脚本之中被申明，这里仅仅给出警告信息而不是让程序崩溃
        ''' </summary>
        ''' <param name="line"></param>
        ''' <param name="message"></param>
        Private Sub divertMissing(line As String, message As String)
            Call Console.WriteLine($"[WARN] Ignore {message}: {line}")
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="scriptText">脚本的文本内容</param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function ParseScript(scriptText As String, Optional strict As Boolean = False) As Model
            Dim errors As List(Of ScriptError) = Nothing
            Dim tokens = scriptText.LineTokens.getScriptTokenGroups(errors)

            If Not errors Is Nothing AndAlso errors.Count > 0 Then
                For Each syntax As ScriptError In errors
                    Call Console.WriteLine($"[WARN] script syntax: {syntax.ToString}")

                    If strict Then
                        Throw New System.Data.SyntaxErrorException(syntax.ToString)
                    End If
                Next
            End If

            If Not tokens.ContainsKey(ScriptTokens.Reaction) Then
                Throw New System.Data.SyntaxErrorException(
                    "There is no reaction channel (RXN) was declared in the target plas script! At least one state variable should have a dynamical equation.")
            End If

            Dim equations = tokens(ScriptTokens.Reaction) _
                .Select(AddressOf sEquationParser) _
                .ToArray
            Dim title As String = tokens.getTitleOrDefault
            Dim disturbs As Experiment()
            Dim val As New ExpressionEngine
            Dim finalTime As Double = tokens.getTimeFinalConfig(val)
            Dim c As NamedValue(Of String)() = val.setConstants(tokens)
            Dim inits = tokens(ScriptTokens.InitValue) _
                .Select(Function(x) var.TryParse(x.text, val)) _
                .ToArray

            If tokens.ContainsKey(ScriptTokens.Disturb) Then
                disturbs = tokens(ScriptTokens.Disturb) _
                    .Select(Function(x)
                                Return ExperimentParser(x.text, x.lineNumber)
                            End Function) _
                    .ToArray
            Else
                disturbs = {}
            End If

            Dim model As New Model With {
                .sEquations = equations,
                .Vars = inits,
                .Experiments = disturbs,
                .Comment = tokens.getCommentText,
                .FinalTime = finalTime,
                .Title = title,
                .Constant = c,
                .UserFunc = tokens.getUserFunctionOrDefault
            }

            Call model.setAliasNames(tokens)
            Call model.setObjectComments(tokens)

            Return model
        End Function

        ''' <summary>
        ''' 从文件指针或者网络数据之中解析出脚本模型
        ''' </summary>
        ''' <param name="s"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ParseStream(s As Stream, Optional strict As Boolean = False) As Model
            Return ParseScript(New StreamReader(s).ReadToEnd, strict)
        End Function

        <Extension>
        Public Function ParseFile(path As String, Optional strict As Boolean = False) As Model
            Return ParseScript(path.ReadAllText, strict)
        End Function
    End Module
End Namespace
