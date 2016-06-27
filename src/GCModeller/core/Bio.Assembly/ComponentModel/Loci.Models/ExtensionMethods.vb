Imports LANS.SystemsBiology.ComponentModel.Loci.Location
Imports System.Runtime.CompilerServices
Imports LANS.SystemsBiology.SequenceModel.NucleotideModels
Imports Microsoft.VisualBasic

Namespace ComponentModel.Loci

    ''' <summary>
    ''' The extension method for the location object operations.
    ''' </summary>
    Public Module Loci_API

        Public Function InternalAssembler(source As IEnumerable(Of SegmentObject)) As NucleotideLocation()
            Dim ls = (From p In source Select p Order By p.Left Ascending).ToList

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
                                    Let TagedLocation = (From item In GroupOperation Where String.Equals(GroupTag, item.GroupTag) Select item.Possible_Duplicated).MatrixToVector
                                    Select GroupTag, TagedLocation).ToArray
            lc = (From item In l_GroupOperation.AsParallel
                  Select If(item.TagedLocation.Count = 1, item.TagedLocation.First, LociAPI.Merge(item.TagedLocation))).ToArray
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
                                    Let TagedLocation = (From item In GroupOperation Where String.Equals(GroupTag, item.GroupTag) Select item.Possible_Duplicated).ToArray.MatrixToVector
                                    Select GroupTag, TagedLocation).ToArray
            lc = (From item In l_GroupOperation Select If(item.TagedLocation.Count = 1, item.TagedLocation.First, LociAPI.Merge(item.TagedLocation))).ToArray
            Return lc.ToArray
        End Function

        ''' <summary>
        ''' 这个方法在Pfam蛋白质结构域分析的时候非常有用
        ''' </summary>
        ''' <param name="source">必须是已经按照<see cref="Left"></see>进行从小到大排序操作的数据</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function FragmentAssembly(source As Generic.IEnumerable(Of Location), lenOffset As Integer) As Location()
            If source.IsNullOrEmpty Then
                Return New Location() {}
            End If

            If source.Count = 1 Then
                Return source.ToArray
            Else
                Return __assembly(source, lenOffset)
            End If
        End Function

        Private Function __assembly(source As Generic.IEnumerable(Of Location), lenOffset As Integer) As Location()
            Dim lstLoci As List(Of Location) = New List(Of Location)
            Dim current As Location = source.First
            Dim getNext As Boolean = False

            For Each n As Location In source.Skip(1)

                If getNext Then
                    current = n : getNext = False
                    Continue For
                End If

                If current.InsideOrOverlapWith(n, WithOffSet:=lenOffset) Then
                    If current.Right < n.Right Then
                        current.Right = n.Right
                    End If
                Else
                    Call lstLoci.Add(current)
                    getNext = True
                End If
            Next

            Call lstLoci.Add(current)

            Return lstLoci.ToArray
        End Function
    End Module
End Namespace