#Region "Microsoft.VisualBasic::a69ba764c110831fd82f2c568f550444, core\Bio.Assembly\ComponentModel\Annotation\EC\ECNumber.vb"

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

    '   Total Lines: 201
    '    Code Lines: 112 (55.72%)
    ' Comment Lines: 62 (30.85%)
    '    - Xml Docs: 82.26%
    ' 
    '   Blank Lines: 27 (13.43%)
    '     File Size: 7.31 KB


    '     Class ECNumber
    ' 
    '         Properties: ECNumberString, serialNumber, subCategory, subType, type
    ' 
    '         Function: (+2 Overloads) Contains, HierarchicalECTerms, InternalHierarchicalECTerms, NumberParserInternal, ToString
    '                   ValidateValue, ValueParser
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
        ''' <remarks>nothing means -</remarks>
        <XmlAttribute> Public Property subType As Integer
        ''' <summary>
        ''' 该亚类之下的小分类
        ''' </summary>
        ''' <remarks>nothing means -</remarks>
        <XmlAttribute> Public Property subCategory As Integer

        ''' <summary>
        ''' 该小分类之下的序号
        ''' </summary>
        ''' <remarks>nothing means -</remarks>
        <XmlAttribute> Public Property serialNumber As Integer

        ''' <summary>
        ''' generates the EC number in format like: x.x.x.x
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ECNumberString As String
            Get
                Return New String() {
                    CInt(type).ToString,
                    If(subType <= 0, "-", subType.ToString),
                    If(subCategory <= 0, "-", subCategory.ToString),
                    If(serialNumber <= 0, "-", serialNumber.ToString)
                }.JoinBy(".")
            End Get
        End Property

        Public Function HierarchicalECTerms() As IEnumerable(Of String)
            Return InternalHierarchicalECTerms.Distinct
        End Function

        Private Iterator Function InternalHierarchicalECTerms() As IEnumerable(Of String)
            Yield $"{CInt(type)}.-.-.-"
            Yield $"{CInt(type)}.{If(subType = 0, "-", subType)}.-.-"
            Yield $"{CInt(type)}.{If(subType = 0, "-", subType)}.{If(subCategory = 0, "-", subCategory)}.-"
            Yield $"{CInt(type)}.{If(subType = 0, "-", subType)}.{If(subCategory = 0, "-", subCategory)}.{If(serialNumber = 0, "-", serialNumber)}"
        End Function

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

            ' 第一级比较
            If type <> ec.type Then
                Return False
            End If

            ' 第二级比较
            ' 如果当前是0（通配符），则包含所有；如果不一致则False；一致则继续
            If subType = 0 Then
                Return True
            ElseIf subType <> ec.subType Then
                Return False
            End If

            ' 第三级比较
            If subCategory = 0 Then
                Return True
            ElseIf subCategory <> ec.subCategory Then
                Return False
            End If

            ' 第四级比较
            ' 如果当前是0（通配符），则包含所有
            If serialNumber = 0 Then
                Return True
                ' 如果当前不为0，必须相等才算包含
            ElseIf serialNumber <> ec.serialNumber Then
                Return False
            End If

            ' 全部匹配，返回 True
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
        Public Const PatternECNumber$ = "\d(\.((\d+)|[-])){0,3}"

        Public Shared ReadOnly r As New Regex(PatternECNumber, RegexOptions.Compiled)

        ''' <summary>
        ''' 解析一个EC编号字符串，如果出现格式错误，则返回空值
        ''' </summary>
        ''' <param name="expr"></param>
        ''' <returns></returns>
        Public Shared Function ValueParser(expr As String) As ECNumber
            If expr.StringEmpty(, True) Then
                Return Nothing
            Else
                Dim m As Match = r.Match(expr)

                ' 格式错误，没有找到相应的编号格式字符串
                If Not m.Success Then
                    Call $"invalid EC_number to parse: {expr}".warning
                    Return Nothing
                Else
                    Return NumberParserInternal(m.Value, expr)
                End If
            End If
        End Function

        Private Shared Function NumberParserInternal(ec_num As String, expr As String) As ECNumber
            Dim tokens As String() = ec_num.Split("."c)

            If tokens.Length < 3 AndAlso Not expr.IsPattern("EC[-]" & PatternECNumber) Then
                Call $"probably none EC_number was parsed from expression: {expr}".warning
            End If

            Dim ecNum As New ECNumber With {
                .type = CInt(Val(tokens(0))),
                .subType = CInt(Val(tokens.ElementAtOrDefault(1))),
                .subCategory = CInt(Val(tokens.ElementAtOrDefault(2))),
                .serialNumber = CInt(Val(tokens.ElementAtOrDefault(3)))
            }

            If ecNum.type > 7 OrElse ecNum.type < 0 Then
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
            Return String.Format("[EC: {0}.{1}.{2}.{3}]",
                                 CInt(type),
                                 If(subType <= 0, "-", subType),
                                 If(subCategory <= 0, "-", subCategory),
                                 If(serialNumber <= 0, "-", serialNumber))
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
