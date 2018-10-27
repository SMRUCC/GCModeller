#Region "Microsoft.VisualBasic::001691a61f22e3fe9d26caddafb0ce35, Bio.Assembly\ComponentModel\Locus\Location.vb"

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

    '     Structure Position
    ' 
    '         Properties: Left, Right
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    '     Class Location
    ' 
    '         Properties: Center, FragmentSize, IsNormalized, Left, Right
    ' 
    '         Constructor: (+4 Overloads) Sub New
    '         Function: Clone, ContainSite, CreateObject, Equals, Inside
    '                   (+2 Overloads) InsideOrOverlapWith, Normalization, OffSet, ToString
    '         Operators: <>, =
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract

Namespace ComponentModel.Loci

    ''' <summary>
    ''' 百分比相对位置
    ''' </summary>
    Public Structure Position
        Public Property Left As Double
        Public Property Right As Double

        Sub New(loci As Location, len As Integer)
            Me.Left = loci.Left / len
            Me.Right = loci.Right / len
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure

    ''' <summary>
    ''' A location property on a sequence data. Please notice that if the loci value its left value greater than right value then this object will swap the value automaticaly.
    ''' (一个序列片段区域的位置，请注意，当Left的大小大于Right的时候，模块会自动纠正为Left小于Right的状态，这个对象可以同时用来表示核酸序列或者蛋白质序列上面的位置)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Location : Inherits IntRange
        Implements ILocationComponent
        Implements IKeyValuePairObject(Of Integer, Integer)

        ''' <summary>
        ''' <see cref="Location"/>: Gets or set the left start value of the segment on the target sequence.(目标片段的左端起始区域，与链的方向无关)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Left As Integer Implements ILocationComponent.Left, IKeyValuePairObject(Of Integer, Integer).Key
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
        <XmlAttribute> Public Property Right As Integer Implements ILocationComponent.Right, IKeyValuePairObject(Of Integer, Integer).Value
            Get
                Return MyBase.Max
            End Get
            Set(value As Integer)
                MyBase.Max = value
            End Set
        End Property

        Sub New()
        End Sub

        Sub New(Left As Integer, Right As Integer)
            Me.Left = Left
            Me.Right = Right
            Call Normalization()
        End Sub

        Sub New(loci As Location)
            Call Me.New(loci.Left, loci.Right)
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
                Return Left <= Right
            End Get
        End Property

        ''' <summary>
        ''' 使用这个方法更正数据，使位置数据始终是右大于左，(Return Me: 修改自身之后返回自身)
        ''' </summary>
        ''' <remarks></remarks>
        Public Function Normalization() As Location
            If Left > Right Then
                Call Min.SwapWith(Max)
            End If
            Return Me
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="loci"></param>
        ''' <param name="offsets">当这个大于零的时候会进行模糊匹配</param>
        ''' <returns></returns>
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
            Return (a.Left = b.Left AndAlso a.Right = b.Right)
        End Operator

        ''' <summary>
        ''' <paramref name="b"></paramref>在当前对象之中或者与当前对象重叠
        ''' </summary>
        ''' <param name="b"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsideOrOverlapWith(b As Location) As Boolean
            If b.Left >= Left AndAlso b.Right <= Right Then
                Return True         'b在当前的对象之中
            End If

            Return ContainSite(b.Left) OrElse ContainSite(b.Right)
        End Function

        ''' <summary>
        ''' <paramref name="loci"/> inside Me.(目标位点在当前的这个位点之中)
        ''' </summary>
        ''' <param name="loci"></param>
        ''' <returns></returns>
        Public Function Inside(loci As Location, offSet As Integer) As Boolean
            If ContainSite(loci.Left) AndAlso
                ContainSite(loci.Right) Then
                Return True
            Else
                For i As Integer = 1 To offSet
                    If ContainSite(loci.Left + i) AndAlso
                        ContainSite(loci.Right - i) Then
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
            If ContainSite(b.Left) OrElse ContainSite(b.Right) Then
                Return True ' at least is overlaps
            End If

            For i As Integer = 1 To WithOffSet
                If ContainSite(b.Left - i) OrElse ContainSite(b.Right + i) Then
                    Return True
                End If
            Next

            Return False
        End Function

        Public ReadOnly Property Center As Integer
            Get
                Return Left + (Right - Left) / 2
            End Get
        End Property

        ''' <summary>
        ''' Is the target site is on this location region?(目标位点是否被包含在当前的位置区域之中)
        ''' </summary>
        ''' <param name="p">这个点是没有指定链的方向的</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ContainSite(p As Integer) As Boolean
            'Dim Loci = {Left, Right}
            Return p >= Left AndAlso p <= Right
        End Function

        ''' <summary>
        ''' The segment length of this location object.(目标序列片段区域的片段长度)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property FragmentSize As Integer
            Get
                Return Math.Abs(Right - Left) + 1
            End Get
        End Property

        ''' <summary>
        ''' ``|<see cref="Left"/> ==> <see cref="Right"/>|``
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return String.Format("|{0} ==> {1}|", Left, Right)
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
            Return New Location(Left:=CLng(Val(Tokens.First)), Right:=CLng(Val(Tokens.Last)))
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
            Return New Location(Left + value, Right + value)
        End Function
    End Class
End Namespace
