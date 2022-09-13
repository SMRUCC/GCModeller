#Region "Microsoft.VisualBasic::8ad779fb38713ae0f7e8cb4e53be8ad5, GCModeller\core\Bio.Assembly\ComponentModel\Locus\ExtensionMethods.vb"

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

    '   Total Lines: 166
    '    Code Lines: 113
    ' Comment Lines: 29
    '   Blank Lines: 24
    '     File Size: 7.52 KB


    '     Module LociAPI
    ' 
    '         Function: __assembly, FragmentAssembly, Group, Group_p, InternalAssembler
    '                   Merge
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.My.JavaScript
Imports SMRUCC.genomics.ComponentModel.Loci.Location
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports stdNum = System.Math

Namespace ComponentModel.Loci

    ''' <summary>
    ''' The extension method for the location object operations.
    ''' </summary>
    Public Module LociAPI

        Public Function InternalAssembler(source As IEnumerable(Of SegmentObject)) As NucleotideLocation()
            Dim ls = (From p In source Select p Order By p.left Ascending).AsList

            For i As Integer = 0 To ls.Count - 1
                If i = ls.Count - 1 Then
                    Exit For
                End If

                Dim [next] = ls(i + 1)
                Dim currt = ls(i)

                If currt.InsideOrOverlapWith([next]) Then
                    Call ls.Remove([next])
                    i -= 1
                    Call currt.Merge([next])
                End If
            Next

            Return (From item In ls Select item.ToLoci).ToArray
        End Function

        ''' <summary>
        ''' 将<paramref name="b"></paramref>合并至<paramref name="a"></paramref>之中并返回位点<paramref name="a"></paramref>，合并失败则只会返回a，其不会被做任何修改
        ''' </summary>
        ''' <typeparam name="TLoci"></typeparam>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension> Public Function Merge(Of TLoci As Location)(a As TLoci, b As TLoci) As TLoci
            a.left = stdNum.Min(a.left, b.left)
            a.right = stdNum.Max(a.right, b.right)
            Return a
        End Function

        <Extension> Public Function Group_p(Of TLocation As Location)(
                                               lc As IEnumerable(Of TLocation),
                                               Optional Length_Offset As Integer = 5) As TLocation()
            lc = (From lcl In lc Select lcl Order By lcl.left Ascending)
            Dim GroupOperation = (From item In lc.AsParallel
                                  Let Possible_Duplicated = (From o As TLocation In lc
                                                             Where stdNum.Abs(o.left - item.left) < Length_Offset AndAlso
                                                                   stdNum.Abs(o.FragmentSize - item.FragmentSize) < Length_Offset
                                                             Select o
                                                             Order By o.left).ToArray
                                  Let mLeft As Integer = (From o In Possible_Duplicated Select o.left).Min
                                  Let mRight As Integer = (From o In Possible_Duplicated Select o.right).Max
                                  Select GroupTag = $"{mLeft}|{mRight}",
                                      Possible_Duplicated
                                  Order By GroupTag Ascending).ToArray
            Dim l_GroupOperation = (From GroupTag As String
                                    In (From item In GroupOperation Select item.GroupTag Distinct).AsParallel
                                    Let TagedLocation = (From item In GroupOperation Where String.Equals(GroupTag, item.GroupTag) Select item.Possible_Duplicated).ToVector
                                    Select GroupTag, TagedLocation).ToArray
            lc = (From item In l_GroupOperation.AsParallel
                  Select If(item.TagedLocation.Count = 1, item.TagedLocation.First, LocusExtensions.MergeJoins(item.TagedLocation))).ToArray
            Return lc.ToArray
        End Function

        ''' <summary>
        ''' 非并行版本，为上一级是并行的LINQ查询所准备的
        ''' </summary>
        ''' <typeparam name="Loci"></typeparam>
        ''' <param name="lc"></param>
        ''' <param name="lenOffset"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Iterator Function Group(Of Loci As {New, Location})(lc As IEnumerable(Of Loci), Optional lenOffset% = 5) As IEnumerable(Of Loci)
            ' group by loci left at first
            Dim groups = lc.GroupBy(Function(l) l.left, lenOffset).ToArray
            Dim left%, right%

            ' and then group by right
            For Each leftGroup In groups
                Dim rightGroup = leftGroup.GroupBy(Function(l) l.right, lenOffset).ToArray

                For Each lociGroup As NamedCollection(Of Loci) In rightGroup
                    left = Aggregate lo In lociGroup Into Min(lo.left)
                    right = Aggregate lo In lociGroup Into Max(lo.right)

                    Yield New Loci With {
                        .left = left,
                        .right = right
                    }
                Next
            Next
        End Function

        ''' <summary>
        ''' 这个方法在Pfam蛋白质结构域分析的时候非常有用，
        ''' 请注意，这个方法仅仅会延伸片段的第一个对象，和第一个位点对象合并的位点都会出现在<see cref="Location.Tag"/>属性之中
        ''' </summary>
        ''' <param name="source">必须是已经按照<see cref="Left"></see>进行从小到大排序操作的数据</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension>
        Public Function FragmentAssembly(source As IEnumerable(Of Location), lenOffset As Integer) As Location()
            If source Is Nothing Then
                Return New Location() {}
            End If

            With source.ToArray
                If .Length = 1 Then
                    Return .ByRef
                Else
                    Return .__assembly(lenOffset)
                End If
            End With
        End Function

        <Extension>
        Private Function __assembly(source As IEnumerable(Of Location), lenOffset As Integer) As Location()
            Dim lstLoci As New List(Of Location)
            Dim current As Location
            Dim raw As New List(Of Location)(source)
            Dim n As New Value(Of Location)

            Do While raw.Count > 0
                current = raw(Scan0)
                raw.RemoveAt(Scan0)

                If current.Tag Is Nothing Then
                    ' 需要在离开前初始化，否则上一层调用函数会因为空引用出错
                    current.Tag = New JavaScriptObject
                End If
                If raw.Count = 0 Then
                    Exit Do
                End If

                Do While current.InsideOrOverlapWith(n = raw(Scan0), WithOffSet:=lenOffset)
                    If current.right < (+n).right Then
                        current.right = (+n).right
                    End If

                    current.Tag(current.Tag.length) = +n

                    Call raw.RemoveAt(Scan0)

                    If raw.Count = 0 Then
                        Exit Do
                    End If
                Loop

                Call lstLoci.Add(current)
            Loop

            Return lstLoci.ToArray
        End Function
    End Module
End Namespace
