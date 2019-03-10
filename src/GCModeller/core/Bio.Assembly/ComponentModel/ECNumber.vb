﻿#Region "Microsoft.VisualBasic::fae493a52bd9e3dd2da076cbfb8ade45, Bio.Assembly\ComponentModel\ECNumber.vb"

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

    '     Class ECNumber
    ' 
    ' 
    '         Enum ClassTypes
    ' 
    ' 
    ' 
    ' 
    '  
    ' 
    '     Properties: SerialNumber, SubCategory, SubType, Type
    ' 
    '     Function: ToString, ValidateValue, ValueParser
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization

Namespace ComponentModel

    ''' <summary>
    ''' Enzyme Commission Number
    ''' </summary>
    ''' <remarks>EC-6.1.1.10</remarks>
    Public Class ECNumber

        ''' <summary>
        ''' The enzyme types enumeration.
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum ClassTypes
            ''' <summary>
            ''' Oxido Reductase.(氧化还原酶)
            ''' </summary>
            ''' <remarks></remarks>
            OxidoReductase = 1
            ''' <summary>
            ''' Transferase.(转移酶)
            ''' </summary>
            ''' <remarks></remarks>
            Transferase = 2
            ''' <summary>
            ''' Hydrolase.(水解酶)
            ''' </summary>
            ''' <remarks></remarks>
            Hydrolase = 3
            ''' <summary>
            ''' Lyase.(裂合酶)
            ''' </summary>
            ''' <remarks></remarks>
            Lyase = 4
            ''' <summary>
            ''' Isomerase.(异构酶)
            ''' </summary>
            ''' <remarks></remarks>
            Isomerase = 5
            ''' <summary>
            ''' Synthetase.(合成酶)
            ''' </summary>
            ''' <remarks></remarks>
            Synthetase = 6
        End Enum

        ''' <summary>
        ''' EC编号里面的第一个数字代表酶的分类号
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Type As ECNumber.ClassTypes

        ''' <summary>
        ''' 该大类之下的亚分类
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property SubType As Integer
        ''' <summary>
        ''' 该亚类之下的小分类
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property SubCategory As Integer

        ''' <summary>
        ''' 该小分类之下的序号
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property SerialNumber As Integer

        Public Shared Widening Operator CType(s As String) As ECNumber
            Return ValueParser(s)
        End Operator

        Public Shared Function ValueParser(expr As String) As ECNumber
            Dim Regex As New Regex("/d[.]/d+[.]/d+[.]/d+")
            Dim m As Match = Regex.Match(expr)

            If Not m.Success Then Return Nothing ' 格式错误，没有找到相应的编号格式字符串

            Dim Tokens As String() = m.Value.Split(CChar("."))
            Dim _ec As New ECNumber With {
                .Type = CInt(Val(Tokens(0))),
                .SubType = CInt(Val(Tokens(1))),
                .SubCategory = CInt(Val(Tokens(2))),
                .SerialNumber = CInt(Val(Tokens(3)))
            }

            If _ec.Type > 6 OrElse _ec.Type < 0 Then
                Return Nothing  '格式错误
            Else
                Return _ec
            End If
        End Function

        ''' <summary>
        ''' IDE debug
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ToString() As String
            Return String.Format("EC-{0}.{1}.{2}.{3}", CInt(Type), SubType, SubCategory, SerialNumber)
        End Function

        ''' <summary>
        ''' ```
        ''' 1.2.3.4
        ''' 1.2.3.-
        ''' 1.2.-.-
        ''' ```
        ''' </summary>
        Public Const RegexEC$ = "\d+(\.((\d+)|[-]))+"

        ''' <summary>
        ''' 验证所输入的字符串的格式是否正确
        ''' </summary>
        ''' <param name="s$"></param>
        ''' <returns></returns>
        Public Shared Function ValidateValue(s$) As Boolean
            Return s.MatchPattern(regex:=RegexEC)
        End Function
    End Class
End Namespace
