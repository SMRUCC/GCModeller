#Region "Microsoft.VisualBasic::eda204d8d27e3644f61c6fbfdc2f88bb, ..\interops\localblast\LocalBLAST\LocalBLAST\BlastOutput\Common\Parameter.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

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
            Dim Match As String = Regex.Match(line, Parameter.MATCHED, RegexOptions.Singleline).Value
            Dim Tokens As String() = Match.lTokens

            If Tokens.IsNullOrEmpty OrElse StringHelpers.IsNullOrEmpty(Tokens) Then
NULL:           Call $"[{line}] ===> {NameOf(Tokens)}:=null".__DEBUG_ECHO
                Return New Parameter() {New Parameter, New Parameter}
            ElseIf Tokens.Length >= 6 Then
                Return New Parameter() {Parser(Tokens(1)), Parser(Tokens(5))}
            ElseIf Tokens.Length >= 2 Then
                Call $"[{line}] ===> {NameOf(Tokens)}:={Tokens.Length}".__DEBUG_ECHO
                Return New Parameter() {Parser(Tokens(1)), New Parameter}
            Else
                GoTo NULL
            End If
        End Function

        Delegate Function ParameterParser(line As String) As Parameter

        Private Shared Function __newParameter(Line As String) As Parameter
            Dim Tokens As List(Of Double) = (From Token As String
                                             In Line.Split
                                             Where Not String.IsNullOrEmpty(Token)
                                             Select Val(Token)).Join(__null) '防止出现索引出界的异常
            Dim p As Parameter = New Parameter With {
                .Lambda = Tokens(0),
                .K = Tokens(1),
                .H = Tokens(2),
                .a = Tokens(3),
                .alpha = Tokens(4),
                .sigma = Tokens(5)
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
