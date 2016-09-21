Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically

Public Module LociFilter

    Public Enum Compares
        ''' <summary>
        ''' 比较当前点和最开始的位点的间隔
        ''' </summary>
        FromLoci
        ''' <summary>
        ''' 比较的是任意两点之间的间隔
        ''' </summary>
        Interval
    End Enum

    ''' <summary>
    ''' 筛选有效的重复片段位点
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="data"></param>
    ''' <param name="interval"></param>
    ''' <param name="compare"></param>
    ''' <param name="returnsAll"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Filtering(Of T As RepeatsView)(
                              data As IEnumerable(Of T),
                           Optional interval As Integer = 2000,
                           Optional compare As Compares = Compares.Interval,
                           Optional returnsAll As Boolean = False) As IEnumerable(Of T)
        Return data.__filtering(
            Function(x) x.Locis,
            Sub(x, l) x.Locis = l,
            interval,
            compare,
            returnsAll)
    End Function

    <Extension>
    Private Iterator Function __filtering(Of T As RepeatsView)(data As IEnumerable(Of T),
                                                               getLocis As Func(Of T, Integer()),
                                                               setLocis As Action(Of T, Integer()),
                                                               interval As Integer,
                                                               compare As Compares,
                                                               returnsAll As Boolean) As IEnumerable(Of T)
        If compare = Compares.FromLoci Then
            For Each loci As T In data
                Dim locis = LinqAPI.Exec(Of Integer) <=
                    From x As Integer
                    In getLocis(loci)
                    Where Math.Abs(x - loci.Left) <= interval
                    Select x

                setLocis(loci, locis)

                If locis.Length = 0 Then
                    If returnsAll Then
                        Yield loci
                    End If
                Else
                    Yield loci
                End If
            Next
        Else
            For Each loci As T In data
                Dim orders As Integer() =
                    getLocis(loci).OrderBy(Function(x) x).ToArray
                Dim locis As New List(Of Integer)
                Dim pre As Integer = loci.Left

                For Each x As Integer In orders
                    If x - pre <= interval Then
                        pre = x
                        locis += x
                    Else
                        Exit For
                    End If
                Next

                setLocis(loci, locis)

                If locis.Count = 0 Then
                    If returnsAll Then
                        Yield loci
                    End If
                Else
                    Yield loci
                End If
            Next
        End If
    End Function

    ''' <summary>
    ''' 根据<see cref="RevRepeatsView.RevLocis"/>来进行筛选
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="interval"></param>
    ''' <param name="compare"></param>
    ''' <param name="returnsAll"></param>
    ''' <returns></returns>
    <Extension>
    Public Function FilteringRev(data As IEnumerable(Of RevRepeatsView),
                                 Optional interval As Integer = 2000,
                                 Optional compare As Compares = Compares.Interval,
                                 Optional returnsAll As Boolean = False) As IEnumerable(Of RevRepeatsView)
        Return data.__filtering(Function(x) x.RevLocis,
                                Sub(x, rl) x.RevLocis = rl,
                                interval,
                                compare,
                                returnsAll)
    End Function
End Module
