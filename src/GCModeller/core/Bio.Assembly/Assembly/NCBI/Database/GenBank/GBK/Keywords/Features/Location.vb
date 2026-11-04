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

Imports System.Text
Imports System.Xml.Serialization
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract
Imports std = System.Math

Imports Microsoft.VisualBasic.Linq

Namespace Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES

    ''' <summary>
    ''' GenBank feature 的位点信息。
    ''' </summary>
    ''' <remarks>
    ''' 支持的 location 语法包括:
    ''' 
    ''' ```
    ''' 1..100
    ''' &lt;1..100
    ''' 1..&gt;100
    ''' complement(1..100)
    ''' join(1..100,201..300)
    ''' complement(join(1..100,201..300))
    ''' order(1..100,201..300)
    ''' join(complement(1..100),complement(201..300))
    ''' 100
    ''' 100^101
    ''' ```
    ''' </remarks>
    Public Class Location : Implements ILocationSegment

        ''' <summary>
        ''' 对于环状的DNA分子，当某一个特性位点跨越了终点的时候，会有一个这个属性，本属性此时不会为空值
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement> Public Property JoinLocation As RegionSegment

        ''' <summary>
        ''' 这个基因的位置是否在互补链
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Complement As Boolean

        ''' <summary>
        ''' 是否为``order(...)``类型的位点: 表示各个片段之间的顺序是不确定的
        ''' </summary>
        <XmlAttribute> Public Property IsOrder As Boolean
        ''' <summary>
        ''' 位点是否为一个不完整的片段(存在于序列的两端位置被截断的情况)
        ''' </summary>
        <XmlAttribute> Public Property IsPartial As Boolean

        ''' <summary>
        ''' join mutliple location for the extron.
        ''' join(...,...,...)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 这个主要是应用于记录真核生物的mRNA外显子区域的位置，基于这个列表进行相应的序列片段提取，构建出一段完整的mRNA序列数据
        ''' </remarks>
        <XmlAttribute> Public Property Locations As RegionSegment()
            Get
                Return _locis
            End Get
            Set(value As RegionSegment())
                _locis = value

                If Not value.IsNullOrEmpty Then
                    _left = value.Select(Function(l) l.Left).Min
                    _right = value.Select(Function(l) l.Right).Max
                Else
                    _left = 0
                    _right = 0
                End If
            End Set
        End Property

        Dim _locis As RegionSegment()
        Dim _left, _right As Long

        ''' <summary>
        ''' 用于提取序列片段的位点列表：按照原始的书写顺序返回，并且兼容了旧版本
        ''' 之中用于处理环状分子跨起点的<see cref="JoinLocation"/>属性
        ''' </summary>
        Public ReadOnly Property SequenceSegments As RegionSegment()
            Get
                If JoinLocation Is Nothing Then
                    Return If(_locis, New RegionSegment() {})
                End If

                Dim list As New List(Of RegionSegment)

                If Complement Then
                    Call list.Add(JoinLocation)
                    Call list.AddRange(_locis.SafeQuery)
                Else
                    Call list.AddRange(_locis.SafeQuery)
                    Call list.Add(JoinLocation)
                End If

                Return list.ToArray
            End Get
        End Property

        ''' <summary>
        ''' 假若目标对象是真核生物基因组的话，则可能会因为内含子的原因出现不连续的片段，
        ''' 故而此时的<see cref="Locations"></see>属性会有多个值，这个属性会尝试将连续
        ''' 的区域返回。对于原核生物而言，也可以直接使用这个属性来获取特性位点的在
        ''' 基因组序列之上的位置
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ContiguousRegion As NucleotideLocation
            Get
                Return New NucleotideLocation(_left, _right, Complement)
            End Get
        End Property

        ''' <summary>
        ''' 起始位置
        ''' </summary>
        Public ReadOnly Property Left As Long
            Get
                Return _left
            End Get
        End Property

        ''' <summary>
        ''' 结束位置
        ''' </summary>
        Public ReadOnly Property Right As Long
            Get
                Return _right
            End Get
        End Property

        Public ReadOnly Property Location As ComponentModel.Loci.Location Implements ILocationSegment.Location
            Get
                Return ContiguousRegion
            End Get
        End Property

        Public ReadOnly Property UniqueId As String Implements ILocationSegment.UniqueId
            Get
                Return ToString()
            End Get
        End Property

        Public ReadOnly Property HasJoinLocation As Boolean
            Get
                Return Not JoinLocation Is Nothing
            End Get
        End Property

        ''' <summary>
        ''' 片段的数量
        ''' </summary>
        Public ReadOnly Property SegmentCount As Integer
            Get
                Return If(_locis Is Nothing, 0, _locis.Length)
            End Get
        End Property

        Public Iterator Function JoinLocations() As IEnumerable(Of NucleotideLocation)
            For Each loc As RegionSegment In SequenceSegments
                Yield New NucleotideLocation(loc.Left, loc.Right, Complement)
            Next
        End Function

        ''' <summary>
        ''' parse the location string
        ''' </summary>
        ''' <param name="strData"></param>
        ''' <returns></returns>
        Public Shared Widening Operator CType(strData As String) As Location
            Return LocationParser.Parse(strData)
        End Operator

        ''' <summary>
        ''' 按照NCBI的标准格式重新生成location的文本表示
        ''' </summary>
        Public Overrides Function ToString() As String
            Dim segments As RegionSegment() = SequenceSegments
            Dim sb As New StringBuilder()

            If segments.Length > 1 Then
                If IsOrder Then
                    Call sb.Append("order(")
                Else
                    Call sb.Append("join(")
                End If
            End If

            For i As Integer = 0 To segments.Length - 1
                If i > 0 Then
                    Call sb.Append(",")
                End If

                Call sb.Append(__segment(segments(i)))
            Next

            If segments.Length > 1 Then
                Call sb.Append(")")
            End If

            Dim s As String = sb.ToString()

            If Complement Then
                s = $"complement({s})"
            End If

            Return s
        End Function

        Private Shared Function __segment(seg As RegionSegment) As String
            Dim s As String

            If seg.Left = seg.Right Then
                s = seg.Left.ToString()
            Else
                s = $"{If(seg.LeftPartial, "<", "")}{seg.Left}..{If(seg.RightPartial, ">", "")}{seg.Right}"
            End If

            If seg.Complement Then
                s = $"complement({s})"
            End If

            Return s
        End Function

        Public Shared Narrowing Operator CType(loc As Location) As NucleotideLocation
            If loc Is Nothing Then
                Return Nothing
            Else
                Return loc.ContiguousRegion
            End If
        End Operator
    End Class

    ''' <summary>
    ''' A site region on the sequence.
    ''' </summary>
    ''' <remarks>(序列上面的一个位点)</remarks>
    Public Class RegionSegment

        <XmlAttribute> Public Property Left As Long
        <XmlAttribute> Public Property Right As Long
        ''' <summary>
        ''' 这一个片段是否是在互补链上面的(``join(complement(1..10),20..30)``这种写法)
        ''' </summary>
        <XmlAttribute> Public Property Complement As Boolean
        ''' <summary>
        ''' 片段的起始位置是否为一个不完整的端点(``&lt;1..100``)
        ''' </summary>
        <XmlAttribute> Public Property LeftPartial As Boolean
        ''' <summary>
        ''' 片段的结束位置是否为一个不完整的端点(``1..&gt;100``)
        ''' </summary>
        <XmlAttribute> Public Property RightPartial As Boolean

        ''' <summary>
        ''' 片段所包含的碱基数量
        ''' </summary>
        Public ReadOnly Property RegionLength As Integer
            Get
                Return CInt(std.Abs(Right - Left) + 1)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}, {1}", Left, Right)
        End Function
    End Class
End Namespace
