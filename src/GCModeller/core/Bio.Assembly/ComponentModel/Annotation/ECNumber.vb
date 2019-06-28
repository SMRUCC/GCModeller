#Region "Microsoft.VisualBasic::e4dc09317a3bdcfea964bd62ae6d1212, Bio.Assembly\ComponentModel\Annotation\ECNumber.vb"

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

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization

Namespace ComponentModel.Annotation

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

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Widening Operator CType(s As String) As ECNumber
            Return ValueParser(s)
        End Operator

        ''' <summary>
        ''' 解析一个EC编号字符串，如果出现格式错误，则返回空值
        ''' </summary>
        ''' <param name="expr"></param>
        ''' <returns></returns>
        Public Shared Function ValueParser(expr As String) As ECNumber
            Dim r As New Regex("/d[.]/d+[.]/d+[.]/d+")
            Dim m As Match = r.Match(expr)

            ' 格式错误，没有找到相应的编号格式字符串
            If Not m.Success Then Return Nothing

            Dim tokens As String() = m.Value.Split(CChar("."))
            Dim ecNum As New ECNumber With {
                .Type = CInt(Val(tokens(0))),
                .SubType = CInt(Val(tokens(1))),
                .SubCategory = CInt(Val(tokens(2))),
                .SerialNumber = CInt(Val(tokens(3)))
            }

            If ecNum.Type > 6 OrElse ecNum.Type < 0 Then
                ' 格式错误
                Return Nothing
            Else
                Return ecNum
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
        Public Const PatternECNumber$ = "\d+(\.((\d+)|[-]))+"

        ''' <summary>
        ''' 验证所输入的字符串的格式是否正确
        ''' </summary>
        ''' <param name="s$"></param>
        ''' <returns></returns>
        Public Shared Function ValidateValue(s$) As Boolean
            Return s.MatchPattern(regex:=PatternECNumber)
        End Function
    End Class
End Namespace
