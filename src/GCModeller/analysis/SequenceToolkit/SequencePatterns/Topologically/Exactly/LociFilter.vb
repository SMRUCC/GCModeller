Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
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
    Public Iterator Function Filtering(Of T As RepeatsView)(
                                       data As IEnumerable(Of T),
                           Optional interval As Integer = 2000,
                           Optional compare As Compares = Compares.Interval,
                           Optional returnsAll As Boolean = False) As IEnumerable(Of T)

        If compare = Compares.FromLoci Then
            For Each loci As T In data
                Dim locis = LinqAPI.Exec(Of Integer) <=
                    From x As Integer
                    In loci.Locis
                    Where Math.Abs(x - loci.Left) <= interval
                    Select x

                loci.Locis = locis

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
                Dim orders = loci.Locis.OrderBy(Function(x) x).ToArray
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

                loci.Locis = locis

                If loci.Locis.Length = 0 Then
                    If returnsAll Then
                        Yield loci
                    End If
                Else
                    Yield loci
                End If
            Next
        End If
    End Function
End Module
