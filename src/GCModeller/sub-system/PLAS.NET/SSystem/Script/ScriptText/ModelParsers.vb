#Region "Microsoft.VisualBasic::3fa03b02ed34f84512527acc5500da36, sub-system\PLAS.NET\SSystem\Script\ScriptText\ModelParsers.vb"

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

    '   Total Lines: 87
    '    Code Lines: 53 (60.92%)
    ' Comment Lines: 20 (22.99%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 14 (16.09%)
    '     File Size: 3.09 KB


    '     Module ModelParsers
    ' 
    '         Function: (+2 Overloads) ConstantParser, ExperimentParser, sEquationParser
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Linq
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels

Namespace Script

    Module ModelParsers

        ''' <summary>
        ''' 解析系统方程表达式
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        Public Function sEquationParser(x As ScriptToken) As SEquation
            Dim value = x.text.GetTagValue("=")

            Return New SEquation With {
                .x = value.Name,
                .Expression = value.Value
            }
        End Function

        ''' <summary>
        ''' 解析出系统的状态扰动实验表达式
        ''' </summary>
        ''' <param name="line"></param>
        ''' <returns></returns>
        Public Function ExperimentParser(line As String) As Experiment
            Return ExperimentParser(line, lineNumber:=-1)
        End Function

        ''' <summary>
        ''' 解析出系统的状态扰动实验表达式
        ''' </summary>
        ''' <param name="line">
        ''' STIMULATE 关键字之后的配置文本，形如：``OBJECT X2 START 5 KICKS -1 INTERVAL 2.5 VALUE --2``
        ''' </param>
        ''' <param name="lineNumber">当前行在脚本之中的行号，用于语法错误的定位</param>
        ''' <returns></returns>
        Public Function ExperimentParser(line As String, lineNumber As Integer) As Experiment
            Dim body As String = stripComment(line)
            Dim tokens As String() = body _
                .Split _
                .Where(Function(s) s.Length > 0) _
                .ToArray
            Dim cfgs As New Dictionary(Of String, String)
            Dim disturb As New Experiment

            ' 配置为 KEY VALUE 的键值对形式，关键字已经被词法器剥离掉
            ' 所以这里必须从第0个元素开始，步长为2
            For i As Integer = 0 To tokens.Length - 2 Step 2
                cfgs(tokens(i).ToUpper) = tokens(i + 1)
            Next

            disturb.Id = requireKey(cfgs, "OBJECT", line, lineNumber)
            disturb.Start = Val(requireKey(cfgs, "START", line, lineNumber))
            disturb.Interval = Val(requireKey(cfgs, "INTERVAL", line, lineNumber))
            disturb.Kicks = Val(requireKey(cfgs, "KICKS", line, lineNumber))

            Dim value As String = requireKey(cfgs, "VALUE", line, lineNumber)

            If InStr(value, "++") = 1 Then
                disturb.DisturbType = Types.Increase
                disturb.Value = Val(Mid(value, 3))
            ElseIf InStr(value, "--") = 1 Then
                disturb.DisturbType = Types.Decrease
                disturb.Value = Val(Mid(value, 3))
            Else
                disturb.DisturbType = Types.ChangeTo
                disturb.Value = Val(value)
            End If

            Return disturb
        End Function

        Private Function requireKey(cfgs As Dictionary(Of String, String), key As String, line As String, lineNumber As Integer) As String
            If cfgs.ContainsKey(key) Then
                Return cfgs(key)
            End If

            Throw New System.Data.SyntaxErrorException(
                $"Missing the required key ""{key}"" in STIMULATE config, the disturb experiment format should be: " &
                $"OBJECT <id> START <number> KICKS <integer> INTERVAL <number> VALUE <number|++number|--number>" &
                $"{If(lineNumber > 0, $" (script line {lineNumber})", "")}: {line}")
        End Function

        ''' <summary>
        ''' 去除掉脚本行末尾的行注释内容
        ''' </summary>
        ''' <param name="line"></param>
        ''' <returns></returns>
        Private Function stripComment(line As String) As String
            line = If(line, "")

            For Each commentFlag As String In {"#", "//", "'"}
                Dim offset As Integer = InStr(line, commentFlag)

                If offset > 0 Then
                    line = line.Substring(0, offset - 1)
                End If
            Next

            Return line.Trim
        End Function

        ''' <summary>
        ''' 函数会自动去除掉表达式末尾的注释
        ''' </summary>
        ''' <param name="expr"></param>
        ''' <returns></returns>
        Public Function ConstantParser(expr As Value(Of String)) As NamedValue(Of String)
            Dim name As String = (expr = (+expr).Trim).Split.First

            expr.Value = Mid(expr.Value, name.Length + 1).Trim
            expr = expr.Value _
                .GetTagValue("#", failureNoName:=False).Name _
                .GetTagValue("'", failureNoName:=False).Name _
                .GetTagValue("//", failureNoName:=False).Name

            Return New NamedValue(Of String) With {
                .Value = expr,
                .Name = name
            }
        End Function

        ''' <summary>
        ''' 这里只是进行解析，并没有立即进行求值
        ''' </summary>
        ''' <param name="expr"></param>
        ''' <returns></returns>
        Public Function ConstantParser(expr As ScriptToken) As NamedValue(Of String)
            Return ConstantParser(expr.text)
        End Function
    End Module
End Namespace
