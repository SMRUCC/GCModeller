#Region "Microsoft.VisualBasic::cc8dc4941ed37df4fc4e16ae4fdc0492, localblast\LocalBLAST\LocalBLAST\BlastOutput\Common\Parameter.vb"

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

    '   Total Lines: 104
    '    Code Lines: 80 (76.92%)
    ' Comment Lines: 6 (5.77%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 18 (17.31%)
    '     File Size: 3.96 KB


    '     Structure Parameter
    ' 
    '         Function: __parserCommon, ToString, TryParseBlastPlusBlastn, TryParseBlastPlusParameters
    '         Delegate Function
    ' 
    '             Function: __blastnParser, __newParameter
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON
Imports r = System.Text.RegularExpressions.Regex

Namespace LocalBLAST.BLASTOutput.ComponentModel

    Public Structure Parameter

        <XmlAttribute> Dim Lambda, K, H, a, alpha, sigma As Double

        Public Const MATCHED As String = "Lambda.+$"

        Public Overrides Function ToString() As String
            If sigma < 0 Then
                Return Me.GetJson
            Else
                Return $"[Gapped] " & Me.GetJson
            End If
        End Function

#Region "TryParseBlastPlusParameters"

        Public Shared Function TryParseBlastPlusParameters(Text As String) As Parameter()
            Return Parameter.__parserCommon(Text, AddressOf Parameter.__newParameter)
        End Function

        Public Shared Function TryParseBlastPlusBlastn(s As String) As Parameter()
            Return Parameter.__parserCommon(s, AddressOf Parameter.__blastnParser)
        End Function

        Private Shared Function __parserCommon(line As String, Parser As Parameter.ParameterParser) As Parameter()
            Dim match As String = r.Match(line, Parameter.MATCHED, RegexOptions.Singleline).Value
            Dim tokens As String() = match.LineTokens

            If tokens.IsNullOrEmpty Then
NULL:           ' 如果序列的长度是零的时候，会出现无参数的情况
                Dim zeroLen = r.Match(line, "Length[=]0", RegexICSng) _
                    .Value _
                    .GetTagValue("=") _
                    .Value = "0"

                If Not zeroLen Then
                    Call $"[{line}] ===> {NameOf(tokens)}:=null".debug
                End If

                Return {
                    New Parameter,
                    New Parameter
                }
            ElseIf tokens.Length >= 6 Then
                Return New Parameter() {Parser(tokens(1)), Parser(tokens(5))}
            ElseIf tokens.Length >= 2 Then
                Call $"[{line}] ===> {NameOf(tokens)}:={tokens.Length}".Warning
                Return New Parameter() {Parser(tokens(1)), New Parameter}
            Else
                GoTo NULL
            End If
        End Function

        Delegate Function ParameterParser(line As String) As Parameter

        Private Shared Function __newParameter(line As String) As Parameter
            Dim t As List(Of Double) =
                (From k As String
                 In line.Split
                 Where Not String.IsNullOrEmpty(k)
                 Select Val(k)).Join(__null) ' 防止出现索引出界的异常

            Dim p As New Parameter With {
                .Lambda = t(0),
                .K = t(1),
                .H = t(2),
                .a = t(3),
                .alpha = t(4),
                .sigma = t(5)
            }
            Return p
        End Function

        Private Shared ReadOnly __null As Double() = {-1.0R, -1.0R, -1.0R, -1.0R, -1.0R, -1.0R}

        ''' <summary>
        ''' Blastn和Blastp的分数的项目是不一样的
        ''' </summary>
        ''' <param name="line"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function __blastnParser(line As String) As Parameter
            Dim Tokens As List(Of Double) = (From Token As String
                                             In line.Split
                                             Where Not String.IsNullOrEmpty(Token)
                                             Select Val(Token)).Join(__null) '防止出现索引出界的异常
            Dim p As Parameter = New Parameter With {
                .Lambda = Tokens(0),
                .K = Tokens(1),
                .H = Tokens(2)
            }
            Return p
        End Function
#End Region

    End Structure
End Namespace
