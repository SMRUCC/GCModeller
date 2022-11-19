#Region "Microsoft.VisualBasic::0939ae53bd92ba8147278bd6f597916c, GCModeller\core\Bio.Assembly\ComponentModel\Locus\Nucleotide\NucleotideLocation.vb"

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

    '   Total Lines: 351
    '    Code Lines: 171
    ' Comment Lines: 143
    '   Blank Lines: 37
    '     File Size: 14.86 KB


    '     Class NucleotideLocation
    ' 
    '         Properties: Ends, IsValid, Start, Strand, tagStr
    ' 
    '         Constructor: (+12 Overloads) Sub New
    '         Function: Copy, (+3 Overloads) CreateObject, Equals, GetDownStream, GetRelationship
    '                   (+2 Overloads) GetUpStreamLoci, LociIsContact, MoveFrame, Normalization, Parse
    '                   ToString
    '         Operators: -, +
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract

Namespace ComponentModel.Loci

    ''' <summary>
    ''' Loci segment location information on an nucleotide sequence, this object added an <see cref="NucleotideLocation.Strand"></see> 
    ''' information on <see cref="Location"></see> data.(会自动根据LEFT和RIGHT的值来修正属性值)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class NucleotideLocation : Inherits Location

        ''' <summary>
        ''' 这个位点在哪一条DNA核酸链
        ''' 
        ''' ```
        ''' Forward =  1; 
        ''' Reverse = -1; 
        ''' Unknown =  0
        ''' ```
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Overridable Property Strand As Strands
        ''' <summary>
        ''' 用户对这个位点的自定义标注信息
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property tagStr As String

        ''' <summary>
        ''' <see cref="NucleotideLocation"/>: 实际的物理上面的位置，与核酸链的方向相关，
        ''' (假若需要使用原始的位置数据，则请使用<see cref="left"/>或者<see cref="right"/>属性，
        ''' 这两个属性值是和具体的链的方向无关的)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property Start As Integer
            Get
                If Strand = Strands.Forward Then
                    Return MyBase.left
                Else
                    Return MyBase.right  '因为在位置对象之中，左右位置自动更正为左小右大的情况
                End If
            End Get
        End Property

        ''' <summary>
        ''' <see cref="NucleotideLocation"/>: 实际的物理上面的位置，与核酸链的方向相关，
        ''' (假若需要使用原始的位置数据，则请使用<see cref="left"/>或者<see cref="right"/>属性，
        ''' 这两个属性值是和具体的链的方向无关的)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property Ends As Integer
            Get
                If Strand = Strands.Forward Then
                    Return MyBase.right
                Else
                    Return MyBase.left
                End If
            End Get
        End Property

        ''' <summary>
        ''' 获取当前的这个核酸片段位点的上游的指定距离的位点的位置
        ''' </summary>
        ''' <param name="distance"></param>
        ''' <returns></returns>
        Public Function GetUpStreamLoci(distance As UInteger) As Integer
            Call Me.Normalization()

            If Strand = Strands.Forward Then
                ' 正向链，则是上游在前面
                Return left - distance
            Else
                ' 反向链，则是上游在后面
                Return right + distance
            End If
        End Function

        Public Overloads Function Copy() As NucleotideLocation
            Return New NucleotideLocation(left, right, Strand)
        End Function

        ''' <summary>
        ''' 请放心，这个函数所得到的具体的位点是和链的方向相关的
        ''' </summary>
        ''' <param name="Distance"></param>
        ''' <returns></returns>
        Public Function GetUpStreamLoci(Distance As Integer) As NucleotideLocation
            Dim Loci = New NucleotideLocation(GetUpStreamLoci(CUInt(Distance)), Me.Start - 1, Me.Strand)
            Call Loci.Normalization()
            Return Loci
        End Function

        Public Function GetDownStream(Distance As Integer) As NucleotideLocation
            Dim loci As Integer = If(Me.Strand = Strands.Forward, right + Distance, left - Distance)
            Return New NucleotideLocation(Me.Ends + 1, loci, Strand)
        End Function

        Public Function MoveFrame(Offset As Integer) As NucleotideLocation
            right += Offset
            left += Offset
            Return Me
        End Function

        Public Sub New()
        End Sub

        ''' <summary>
        ''' Create location model from the exists loci data, and then assign a new <see cref="Strands"/> value for it.
        ''' </summary>
        ''' <param name="loci"></param>
        ''' <param name="strand">The new <see cref="Strands"/> value for the target loci.</param>
        Sub New(loci As Location, strand As Strands)
            Call loci.Normalization()

            MyBase.left = loci.left
            MyBase.right = loci.right
            Me.Strand = strand
        End Sub

        Sub New(loci As ILocationComponent, Optional strand As Strands = Strands.Forward)
            MyBase.left = loci.left
            MyBase.right = loci.right
            Me.Strand = strand
        End Sub

        Public Sub New(Copy As NucleotideLocation)
            MyBase.left = Copy.left
            MyBase.right = Copy.right
            Me.Strand = Copy.Strand
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(loc As IntRange, strand As Strands)
            Call Me.New(loc.Min, loc.Max, strand)
        End Sub

        ''' <summary>
        ''' 使用片段的位置进行初始化本位点对象
        ''' </summary>
        ''' <param name="LociStart"></param>
        ''' <param name="LociEnds"></param>
        ''' <param name="Strand">一般性的链的方向的描述性的字符串</param>
        Public Sub New(LociStart As Integer, LociEnds As Integer, Strand As String)
            Call Me.New(CLng(LociStart), CLng(LociEnds), Strand)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="LociStart"></param>
        ''' <param name="LociEnds"></param>
        ''' <param name="Strand">会自动通过<see cref="GetStrand(String)"/>函数来进行数据类型的转换的</param>
        Sub New(LociStart As Long, LociEnds As Long, Strand As String)
            MyBase.left = LociStart
            MyBase.right = LociEnds
            Me.Strand = GetStrand(Strand)
        End Sub

        ''' <summary>
        ''' Creates loci object from raw location data.
        ''' </summary>
        ''' <param name="start"><see cref="Location"/>: Gets or set the left start value of the segment on the target sequence.(目标片段的左端起始区域，与链的方向无关)</param>
        ''' <param name="[end]"><see cref="Location"/>: Gets or set the right stop value of the segment on the target sequence.(目标片段的右端终止区域，与链的方向无关)</param>
        ''' <param name="Strand">链的方向</param>
        Public Sub New(start As Integer, [end] As Integer, Strand As Strands)
            Call MyBase.New(start, [end])
            Me.Strand = Strand
        End Sub

        Public Sub New(LociStart As Long, LociEnds As Long, Strand As Strands)
            MyBase.left = LociStart
            MyBase.right = LociEnds
            Me.Strand = Strand
        End Sub

        ''' <summary>
        ''' 使用片段的位置进行初始化本位点对象
        ''' </summary>
        ''' <param name="_start"></param>
        ''' <param name="_ends"></param>
        ''' <param name="ComplementStrand">这个片段是否位于DNA上面的互补链或者是否为反向序列</param>
        ''' <remarks></remarks>
        Public Sub New(_start As Integer, _ends As Integer, ComplementStrand As Boolean)
            MyBase.left = _start
            MyBase.right = _ends
            Me.Strand = If(ComplementStrand, Strands.Reverse, Strands.Forward)
        End Sub

        ''' <summary>
        ''' 自动判断链的方向，假若开始小于结束，则是正链，反之为负义链
        ''' </summary>
        ''' <param name="start"></param>
        ''' <param name="Ends"></param>
        Public Sub New(start As Integer, Ends As Integer)
            MyBase.left = start
            MyBase.right = Ends
            If start < Ends Then
                Me.Strand = Strands.Forward '默认为正向链
            Else
                Me.Strand = Strands.Reverse
            End If
        End Sub

        ''' <summary>
        ''' 使用片段的位置进行初始化本位点对象
        ''' </summary>
        ''' <param name="_start"></param>
        ''' <param name="_ends"></param>
        ''' <param name="ComplementStrand">这个片段是否位于DNA上面的互补链或者是否为反向序列</param>
        ''' <remarks></remarks>
        Public Sub New(_start As Long, _ends As Long, ComplementStrand As Boolean)
            MyBase.left = _start
            MyBase.right = _ends
            Me.Strand = If(ComplementStrand, Strands.Reverse, Strands.Forward)
        End Sub

        ''' <summary>
        ''' <seealso cref="Location.Normalization()"/>
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Function Normalization() As NucleotideLocation
            Return DirectCast(MyBase.Normalization, NucleotideLocation)
        End Function

        Public Overloads Shared Function CreateObject(Start As Long, Length As Integer, Complement As Boolean) As NucleotideLocation
            If Complement Then
                Return New NucleotideLocation(Start, Start - Length, Complement)
            Else
                Return New NucleotideLocation(Start, Start + Length, False)
            End If
        End Function

        ''' <summary>
        ''' Create a nucleotide sequence loci location based on start position, segment length and segment strand.
        ''' </summary>
        ''' <param name="Start"></param>
        ''' <param name="Length"></param>
        ''' <param name="Strand"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Function CreateObject(Start As Long, Length As Integer, Strand As Strands) As NucleotideLocation
            Return NucleotideLocation.CreateObject(Start, Length, Strand = Strands.Reverse)
        End Function

        ''' <summary>
        ''' 会自动比较<paramref name="start"/>和<paramref name="ends"/>这两个参数的大小来确定链的方向
        ''' </summary>
        ''' <param name="Start"></param>
        ''' <param name="Ends"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Function CreateObject(Start As Long, Ends As Long) As NucleotideLocation
            Return New NucleotideLocation(Start, Ends, If(Start < Ends, Strands.Forward, Strands.Reverse))
        End Function

        ''' <summary>
        ''' 和当前的这个位点位置进行允许在一定的误差范围之内的模糊匹配
        ''' </summary>
        ''' <param name="Loci"></param>
        ''' <param name="AllowedOffset">当这个值为0的时候就是绝对相等</param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Function Equals(Loci As NucleotideLocation, Optional AllowedOffset As Integer = 10) As Boolean
            Return LocusExtensions.Equals(Me, Loci, AllowedOffset)
        End Function

        ''' <summary>
        ''' Gets the location relationship of two loci segments.(获取两个位点片段之间的位置关系，请注意，这个函数只依靠左右位置来判断关系，假若对核酸链的方向有要求在调用本函数之前请确保二者在同一条链之上)
        ''' </summary>
        ''' <param name="lcl">在计算之前请先调用<see cref="Location.Normalization()"/>方法来修正</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetRelationship(lcl As NucleotideLocation) As SegmentRelationships
            Return LocusExtensions.GetRelationship(Me, lcl)
        End Function

        ''' <summary>
        ''' 判断目标位点片段是否与本位点片段具有相互关系，这个函数是忽略掉了链的方向了的
        ''' </summary>
        ''' <param name="Loci"></param>
        ''' <returns></returns>
        Public Function LociIsContact(Loci As NucleotideLocation) As Boolean
            Dim r = Me.GetRelationship(Loci)

            Return r = SegmentRelationships.Equals OrElse
                r = SegmentRelationships.Inside OrElse
                r = SegmentRelationships.Cover OrElse
                r = SegmentRelationships.DownStreamOverlap OrElse
                r = SegmentRelationships.UpStreamOverlap
        End Function

        ''' <summary>
        ''' 这个函数的输出的字符串可以使用<see cref="LocusExtensions.TryParse(String)"/>方法进行解析
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return $"{left} ~ {right} #{Strings.LCase(Strand.ToString)}"
        End Function

        ''' <summary>
        ''' 当任意一个断点为0的时候就无效
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsValid As Boolean
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Not (left = 0 OrElse right = 0)
            End Get
        End Property

        ''' <summary>
        ''' 解析<see cref="ToString"/>的结果数据
        ''' </summary>
        ''' <param name="loci$"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Parse(loci$) As NucleotideLocation
            Return LocusExtensions.TryParse(loci)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Operator +(loci As NucleotideLocation, offset%) As NucleotideLocation
            Return New NucleotideLocation(loci.left + offset, loci.right + offset, loci.Strand) With {
                .tagStr = $"{loci.tagStr} ({offset}bp)"
            }
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Operator -(loci As NucleotideLocation, offset%) As NucleotideLocation
            Return New NucleotideLocation(loci.left - offset, loci.right - offset, loci.Strand) With {
                .tagStr = $"{loci.tagStr} ({offset}bp)"
            }
        End Operator
    End Class
End Namespace
