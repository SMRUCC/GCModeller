#Region "Microsoft.VisualBasic::7537e162d132a0ece2eab530097d2868, Bio.Assembly\ComponentModel\Locus\ExtensionMethods.vb"

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

    '     Module Loci_API
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
Imports SMRUCC.genomics.ComponentModel.Loci.Location
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace ComponentModel.Loci

    ''' <summary>
    ''' The extension method for the location object operations.
    ''' </summary>
    Public Module Loci_API

        Public Function InternalAssembler(source As IEnumerable(Of SegmentObject)) As NucleotideLocation()
            Dim ls = (From p In source Select p Order By p.Left Ascending).AsList

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
            a.Left = Math.Min(a.Left, b.Left)
            a.Right = Math.Max(a.Right, b.Right)
            Return a
        End Function

        <Extension> Public Function Group_p(Of TLocation As Location)(
                                               lc As IEnumerable(Of TLocation),
                                               Optional Length_Offset As Integer = 5) As TLocation()
            lc = (From lcl In lc Select lcl Order By lcl.Left Ascending)
            Dim GroupOperation = (From item In lc.AsParallel
                                  Let Possible_Duplicated = (From o As TLocation In lc
                                                             Where Math.Abs(o.Left - item.Left) < Length_Offset AndAlso
                                                                   Math.Abs(o.FragmentSize - item.FragmentSize) < Length_Offset
                                                             Select o
                                                             Order By o.Left).ToArray
                                  Let mLeft As Integer = (From o In Possible_Duplicated Select o.Left).Min
                                  Let mRight As Integer = (From o In Possible_Duplicated Select o.Right).Max
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
        ''' <typeparam name="TLocation"></typeparam>
        ''' <param name="lc"></param>
        ''' <param name="LenOffset"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Group(Of TLocation As Location)(lc As Generic.IEnumerable(Of TLocation), Optional LenOffset As Integer = 5) As TLocation()
            lc = (From lcl In lc Select lcl Order By lcl.Left Ascending).ToArray
            Dim GroupOperation = (From item In lc
                                  Let Possible_Duplicated = (From o As TLocation In lc
                                                             Where Math.Abs(o.Left - item.Left) < LenOffset AndAlso
                                                                   Math.Abs(o.FragmentSize - item.FragmentSize) < LenOffset
                                                             Select o
                                                             Order By o.Left).ToArray
                                  Select GroupTag = String.Format("{0}|{1}", (From o In Possible_Duplicated Select o.Left).ToArray.Min, (From o In Possible_Duplicated Select o.Right).ToArray.Max),
                                         Possible_Duplicated
                                  Order By GroupTag Ascending).ToArray
            Dim l_GroupOperation = (From GroupTag As String
                              In (From item In GroupOperation Select item.GroupTag Distinct).ToArray
                                    Let TagedLocation = (From item In GroupOperation Where String.Equals(GroupTag, item.GroupTag) Select item.Possible_Duplicated).ToArray.ToVector
                                    Select GroupTag, TagedLocation).ToArray
            lc = (From item In l_GroupOperation Select If(item.TagedLocation.Length = 1, item.TagedLocation.First, LocusExtensions.MergeJoins(item.TagedLocation))).ToArray
            Return lc.ToArray
        End Function

        ''' <summary>
        ''' 这个方法在Pfam蛋白质结构域分析的时候非常有用，
        ''' 请注意，这个方法仅仅会延伸片段的第一个对象，和第一个位点对象合并的位点都会出现在<see cref="Location.Extension"/>属性之中
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

                If current.Extension Is Nothing Then
                    ' 需要在离开前初始化，否则上一层调用函数会因为空引用出错
                    current.Extension = New ExtendedProps
                End If
                If raw.Count = 0 Then
                    Exit Do
                End If

                Do While current.InsideOrOverlapWith(n = raw(Scan0), WithOffSet:=lenOffset)
                    If current.Right < (+n).Right Then
                        current.Right = (+n).Right
                    End If

                    current.Extension.DynamicHashTable(
                        current.Extension.DynamicHashTable _
                        .Properties _
                        .Count) = +n

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
