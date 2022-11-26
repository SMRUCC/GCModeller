#Region "Microsoft.VisualBasic::1292f43efeb4bc407e595fc921bedee8, GCModeller\core\Bio.Assembly\ComponentModel\Locus\Location.vb"

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

    '   Total Lines: 277
    '    Code Lines: 132
    ' Comment Lines: 112
    '   Blank Lines: 33
    '     File Size: 10.41 KB


    '     Class Location
    ' 
    '         Properties: Center, FragmentSize, IsNormalized, left, right
    '                     Tag
    ' 
    '         Constructor: (+4 Overloads) Sub New
    '         Function: Clone, CreateObject, Equals, GetOverlapSize, Inside
    '                   (+2 Overloads) InsideOrOverlapWith, Normalization, OffSet, ToString
    '         Operators: <>, =
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.My.JavaScript
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract
Imports stdNum = System.Math

Namespace ComponentModel.Loci

    ''' <summary>
    ''' A location property on a sequence data. Please notice that if the loci value its left value 
    ''' greater than right value then this object will swap the value automaticaly.
    ''' (一个序列片段区域的位置，请注意，当Left的大小大于Right的时候，模块会自动纠正为Left小于Right的状态，
    ''' 这个对象可以同时用来表示核酸序列或者蛋白质序列上面的位置)
    ''' </summary>
    ''' <remarks>
    ''' 字符串中的字符在计算机程序语言中是从零开始的
    ''' 但是生物分子序列中的残基的起始是从1开始的
    ''' </remarks>
    Public Class Location : Inherits IntRange
        Implements IRange(Of Integer)
        Implements ILocationComponent
        Implements IKeyValuePairObject(Of Integer, Integer)

        ''' <summary>
        ''' <see cref="Location"/>: Gets or set the left start value of the segment on the target sequence.(目标片段的左端起始区域，与链的方向无关)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property left As Integer Implements ILocationComponent.left, IKeyValuePairObject(Of Integer, Integer).Key
            Get
                Return MyBase.Min
            End Get
            Set(value As Integer)
                MyBase.Min = value
            End Set
        End Property

        ''' <summary>
        ''' <see cref="Location"/>: Gets or set the right ends value of the segment on the target sequence.(目标片段的右端结束区域，与链的方向无关)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property right As Integer Implements ILocationComponent.right, IKeyValuePairObject(Of Integer, Integer).Value
            Get
                Return MyBase.Max
            End Get
            Set(value As Integer)
                MyBase.Max = value
            End Set
        End Property

        Public Property Tag As JavaScriptObject

        Sub New()
        End Sub

        Sub New(left As Integer, right As Integer)
            Me.left = left
            Me.right = right

            Call Normalization()
        End Sub

        Sub New(loci As Location)
            Call Me.New(loci.left, loci.right)
        End Sub

        Sub New(base As IntRange)
            Call Me.New(base.Min, base.Max)
        End Sub

        ''' <summary>
        ''' 当前的这个位置数据是否为左边小于右边的正常状态
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsNormalized As Boolean
            Get
                Return left <= right
            End Get
        End Property

        ''' <summary>
        ''' 使用这个方法更正数据，使位置数据始终是右大于左，(Return Me: 修改自身之后返回自身)
        ''' </summary>
        ''' <remarks></remarks>
        Public Function Normalization() As Location
            If left > right Then
                Call Min.Swap(Max)
            End If
            Return Me
        End Function

        ''' <summary>
        ''' 获取当前的位点片段和一个指定的位点片段之间的重叠区域的长度，
        ''' 没有重叠的时候会返回-1值
        ''' </summary>
        ''' <param name="loci"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 对于核酸位置数据，这个函数不考虑链的方向问题,只是单纯的计算线段的重叠长度的问题
        ''' 
        ''' > https://stackoverflow.com/questions/16691524/calculating-the-overlap-distance-of-two-1d-line-segments
        ''' </remarks>
        Public Function GetOverlapSize(loci As Location) As Integer
            ' me
            ' |----------------------------|
            '           |----------------------| loci
            '           |<--overlap_size-->|

            ' def overlap(min1, max1, min2, max2)
            '    return Max(0, Min(max1, max2) - Max(min1, min2))

            ' >>> overlap(0, 10, 80, 90)
            ' 0
            ' >>> overlap(0, 50, 40, 90)
            ' 10
            ' >>> overlap(0, 50, 40, 45)
            ' 5
            ' >>> overlap(0, 100, 0, 20)
            ' 20

            Return stdNum.Max(0, stdNum.Min(Me.right, loci.right) - stdNum.Max(Me.left, loci.right))
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="loci"></param>
        ''' <param name="offsets">当这个大于零的时候会进行模糊匹配</param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Function Equals(loci As Location, Optional offsets As Integer = 0) As Boolean
            Return LocusExtensions.Equals(loci, Me, offsets)
        End Function

        Public Shared Operator <>(a As Location, b As Location) As Boolean
            Return Not a = b
        End Operator

        ''' <summary>
        ''' Position equals
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator =(a As Location, b As Location) As Boolean
            If a Is Nothing OrElse b Is Nothing Then
                Return False
            End If
            Return (a.left = b.left AndAlso a.right = b.right)
        End Operator

        ''' <summary>
        ''' <paramref name="b"></paramref>在当前对象之中或者与当前对象重叠
        ''' </summary>
        ''' <param name="b"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsideOrOverlapWith(b As Location) As Boolean
            If b.left >= left AndAlso b.right <= right Then
                ' b在当前的对象之中
                Return True
            End If

            Return IsInside(b.left) OrElse IsInside(b.right)
        End Function

        ''' <summary>
        ''' <paramref name="loci"/> inside Me.(目标位点在当前的这个位点之中)
        ''' </summary>
        ''' <param name="loci"></param>
        ''' <returns></returns>
        Public Function Inside(loci As Location, offSet As Integer) As Boolean
            If IsInside(loci.left) AndAlso IsInside(loci.right) Then
                Return True
            Else
                For i As Integer = 1 To offSet
                    If IsInside(loci.left + i) AndAlso IsInside(loci.right - i) Then
                        Return True
                    End If
                Next
            End If

            Return False
        End Function

        ''' <summary>
        ''' <paramref name="b"></paramref>在当前对象之中或者与当前对象重叠
        ''' </summary>
        ''' <param name="b"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsideOrOverlapWith(b As Location, WithOffSet As Integer) As Boolean
            If IsInside(b.left) OrElse IsInside(b.right) Then
                ' at least is overlaps
                Return True
            End If

            For i As Integer = 1 To WithOffSet
                If IsInside(b.left - i) OrElse IsInside(b.right + i) Then
                    Return True
                End If
            Next

            Return False
        End Function

        Public ReadOnly Property Center As Integer
            Get
                Return left + (right - left) / 2
            End Get
        End Property

        ''' <summary>
        ''' The segment length of this location object.(目标序列片段区域的片段长度)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property FragmentSize As Integer
            Get
                Return stdNum.Abs(right - left) + 1
            End Get
        End Property

        ''' <summary>
        ''' ``|<see cref="Left"/> ==> <see cref="Right"/>|``
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return String.Format("|{0} ==> {1}|", left, right)
        End Function

        Public Function Clone() As Location
            Return DirectCast(Me.MemberwiseClone, Location)
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="strData"></param>
        ''' <param name="Delimiter"></param>
        ''' <returns></returns>
        Public Shared Function CreateObject(strData As String, Delimiter As String) As Location
            Dim Tokens As String() = Strings.Split(strData, Delimiter)
            Return New Location(left:=CLng(Val(Tokens.First)), right:=CLng(Val(Tokens.Last)))
        End Function

        Public Overloads Shared Widening Operator CType(loci As Integer()) As Location
            If loci.IsNullOrEmpty Then
                Return New Location
            ElseIf loci.Length = 1 Then
                Return New Location(1, loci(Scan0))
            Else
                Return New Location(loci.Min, loci.Max)
            End If
        End Operator

        ''' <summary>
        ''' <see cref="Left"/>, <see cref="Right"/> offset a length value and 
        ''' then construct a new <see cref="Location"/> value.
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Function OffSet(value As Integer) As Location
            Return New Location(left + value, right + value)
        End Function
    End Class
End Namespace
