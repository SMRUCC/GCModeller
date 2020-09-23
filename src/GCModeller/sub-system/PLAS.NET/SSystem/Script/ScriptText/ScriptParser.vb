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
        Private Function getScriptTokenGroups(script As String()) As Dictionary(Of ScriptTokens, ScriptToken())
            Dim tokens As ScriptToken() = TokenIcer.TryParse(script)
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

        <Extension>
        Private Function getTimeFinalConfig(tokens As Dictionary(Of ScriptTokens, ScriptToken()), env As ExpressionEngine) As Integer
            If Not tokens.ContainsKey(ScriptTokens.Time) Then
                Return 100
            Else
                Return tokens(ScriptTokens.Time) _
                    .First _
                    .DoCall(Function(t) env.Evaluate(t.text))
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

        <Extension>
        Private Sub setAliasNames(model As Model, tokens As Dictionary(Of ScriptTokens, ScriptToken()))
            Dim nameList As String()
            Dim name As String

            If tokens.ContainsKey(ScriptTokens.Alias) Then
                nameList = tokens(ScriptTokens.Alias) _
                    .Select(Function(x) x.text) _
                    .ToArray
            Else
                nameList = {}
            End If

            For Each s As String In nameList
                s = Mid(s, 7)
                name = s.Split.First
                model.FindObject(s).title = Mid(s, Len(name) + 2)
            Next
        End Sub

        <Extension>
        Private Sub setObjectComments(model As Model, tokens As Dictionary(Of ScriptTokens, ScriptToken()))
            Dim comments As String()
            Dim name As String

            If tokens.ContainsKey(ScriptTokens.SubsComments) Then
                comments = tokens(ScriptTokens.SubsComments).Select(Function(x) x.text).ToArray
            Else
                comments = {}
            End If

            For Each s As String In comments
                s = Mid(s, 9)
                name = s.Split.First
                model.FindObject(name).comment = Mid(s, Len(name) + 2)
            Next
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="scriptText">脚本的文本内容</param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function ParseScript(scriptText As String) As Model
            Dim tokens = scriptText.LineTokens.getScriptTokenGroups
            Dim equations = tokens(ScriptTokens.Reaction) _
                .Select(AddressOf sEquationParser) _
                .ToArray
            Dim title As String = tokens.getTitleOrDefault
            Dim disturbs As Experiment()
            Dim val As New ExpressionEngine
            Dim finalTime As Integer = tokens.getTimeFinalConfig(val)
            Dim c As NamedValue(Of String)() = val.setConstants(tokens)
            Dim inits = tokens(ScriptTokens.InitValue) _
                .Select(Function(x) var.TryParse(x.text, val)) _
                .ToArray

            If tokens.ContainsKey(ScriptTokens.Disturb) Then
                disturbs = tokens(ScriptTokens.Disturb) _
                    .Select(Function(x)
                                Return ExperimentParser(x.text)
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
        Public Function ParseStream(s As Stream) As Model
            Return ParseScript(New StreamReader(s).ReadToEnd)
        End Function

        <Extension>
        Public Function ParseFile(path As String) As Model
            Return ParseScript(path.ReadAllText)
        End Function
    End Module
End Namespace
