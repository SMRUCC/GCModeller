#Region "Microsoft.VisualBasic::a702b3cdb037bc2cbeec1d13916a47e0, GCModeller\core\Bio.Assembly\ComponentModel\Annotation\EC\ECNumber.vb"

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

    '   Total Lines: 147
    '    Code Lines: 75
    ' Comment Lines: 50
    '   Blank Lines: 22
    '     File Size: 4.73 KB


    '     Class ECNumber
    ' 
    '         Properties: serialNumber, subCategory, subType, type
    ' 
    '         Function: (+2 Overloads) Contains, ToString, ValidateValue, ValueParser
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq

Namespace ComponentModel.Annotation

    ''' <summary>
    ''' Enzyme Commission Number
    ''' </summary>
    ''' <remarks>EC-6.1.1.10</remarks>
    Public Class ECNumber

        ''' <summary>
        ''' EC编号里面的第一个数字代表酶的分类号
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property type As EnzymeClasses

        ''' <summary>
        ''' 该大类之下的亚分类
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property subType As Integer
        ''' <summary>
        ''' 该亚类之下的小分类
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property subCategory As Integer

        ''' <summary>
        ''' 该小分类之下的序号
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property serialNumber As Integer

        Public Function Contains(ec As String) As Boolean
            Static parserCache As New Dictionary(Of String, ECNumber)

            Return parserCache _
                .ComputeIfAbsent(
                    key:=ec,
                    lazyValue:=AddressOf ValueParser
                ) _
                .DoCall(AddressOf Contains)
        End Function

        ''' <summary>
        ''' Contains or equals
        ''' </summary>
        ''' <param name="ec"></param>
        ''' <returns></returns>
        Public Function Contains(ec As ECNumber) As Boolean
            If ec Is Nothing Then
                Return False
            End If

            If Type <> ec.Type Then
                Return False
            End If

            If SubType = 0 Then
                Return True
            ElseIf SubType <> ec.SubType Then
                Return False
            End If

            If SubCategory = 0 Then
                Return True
            ElseIf SubCategory <> ec.SubCategory Then
                Return False
            End If

            If SerialNumber = 0 Then
                Return True
            ElseIf SerialNumber = ec.SerialNumber Then
                Return False
            End If

            Return True
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Widening Operator CType(s As String) As ECNumber
            Return ValueParser(s)
        End Operator

        ''' <summary>
        ''' ```
        ''' 1.2.3.4
        ''' 1.2.3.-
        ''' 1.2.-.-
        ''' ```
        ''' </summary>
        Public Const PatternECNumber$ = "\d(\.((\d+)|[-]))+"

        Shared ReadOnly r As New Regex(PatternECNumber)

        ''' <summary>
        ''' 解析一个EC编号字符串，如果出现格式错误，则返回空值
        ''' </summary>
        ''' <param name="expr"></param>
        ''' <returns></returns>
        Public Shared Function ValueParser(expr As String) As ECNumber
            Dim m As Match = r.Match(expr)

            ' 格式错误，没有找到相应的编号格式字符串
            If Not m.Success Then Return Nothing

            Dim tokens As String() = m.Value.Split("."c)
            Dim ecNum As New ECNumber With {
                .Type = CInt(Val(tokens(0))),
                .SubType = CInt(Val(tokens.ElementAtOrDefault(1))),
                .SubCategory = CInt(Val(tokens.ElementAtOrDefault(2))),
                .SerialNumber = CInt(Val(tokens.ElementAtOrDefault(3)))
            }

            If ecNum.Type > 7 OrElse ecNum.Type < 0 Then
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
            Return String.Format("[EC: {0}.{1}.{2}.{3}]", CInt(Type), SubType, SubCategory, SerialNumber)
        End Function

        ''' <summary>
        ''' 验证所输入的字符串的格式是否正确
        ''' </summary>
        ''' <param name="s$"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ValidateValue(s$) As Boolean
            Return s.MatchPattern(regex:=PatternECNumber)
        End Function
    End Class
End Namespace
