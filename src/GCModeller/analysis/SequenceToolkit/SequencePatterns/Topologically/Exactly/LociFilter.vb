#Region "Microsoft.VisualBasic::5e9c80e7915206d4c9878a211426a33e, GCModeller\analysis\SequenceToolkit\SequencePatterns\Topologically\Exactly\LociFilter.vb"

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

    '   Total Lines: 180
    '    Code Lines: 125
    ' Comment Lines: 37
    '   Blank Lines: 18
    '     File Size: 7.24 KB


    ' Module LociFilter
    ' 
    '     Function: (+5 Overloads) RangeSelects
    '     Enum Compares
    ' 
    '         FromLoci, Interval
    ' 
    ' 
    ' 
    '  
    ' 
    '     Function: doFiltering, Filtering, FilteringRev
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract

Public Module LociFilter

    <Extension>
    Public Iterator Function RangeSelects(Of T As ILoci)(range As IntRange, data As IEnumerable(Of NamedValue(Of T())), lociProvider As Func(Of T, Integer())) As IEnumerable(Of NamedValue(Of T()))
        For Each part As NamedValue(Of T()) In data
            Dim out As New List(Of T)

            For Each x As T In part.Value
                If range.InsideAny(lociProvider(x)) Then
                    Call out.Add(x)
                End If
            Next

            Yield New NamedValue(Of T()) With {
                .Name = part.Name,
                .Value = out
            }
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function RangeSelects(range As IntRange, data As IEnumerable(Of NamedValue(Of RepeatsView()))) As IEnumerable(Of NamedValue(Of RepeatsView()))
        Return range.RangeSelects(data, Function(x) {x.Left})
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function RangeSelects(range As IntRange, data As IEnumerable(Of NamedValue(Of ReverseRepeatsView()))) As IEnumerable(Of NamedValue(Of ReverseRepeatsView()))
        Return range.RangeSelects(data, Function(x) {x.Left})
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function RangeSelects(range As IntRange, data As IEnumerable(Of NamedValue(Of PalindromeLoci()))) As IEnumerable(Of NamedValue(Of PalindromeLoci()))
        Return range.RangeSelects(data, Function(x) x.Start.Join({x.PalEnd, x.Mirror}).ToArray)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function RangeSelects(range As IntRange, data As IEnumerable(Of NamedValue(Of ImperfectPalindrome()))) As IEnumerable(Of NamedValue(Of ImperfectPalindrome()))
        Return range.RangeSelects(data, Function(x) {x.Left, x.Paloci})
    End Function

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
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function Filtering(Of T As RepeatsView)(
                              data As IEnumerable(Of T),
                           Optional interval As Integer = 2000,
                           Optional compare As Compares = Compares.Interval,
                           Optional returnsAll As Boolean = False,
                           Optional lenMin As Integer = 4) As IEnumerable(Of T)

        Return data.doFiltering(
            Function(l) l.Locis,
            Sub(view, l) view.Locis = l,
            interval,
            compare,
            returnsAll, lenMin
        )
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="data"></param>
    ''' <param name="getLocis"></param>
    ''' <param name="setLocis"></param>
    ''' <param name="interval"></param>
    ''' <param name="compare"></param>
    ''' <param name="returnsAll"></param>
    ''' <param name="lengthMin">重复片段的最小长度</param>
    ''' <returns></returns>
    <Extension>
    Private Iterator Function doFiltering(Of T As RepeatsView)(data As IEnumerable(Of T),
                                                               getLocis As Func(Of T, Integer()),
                                                               setLocis As Action(Of T, Integer()),
                                                               interval As Integer,
                                                               compare As Compares,
                                                               returnsAll As Boolean,
                                                               lengthMin As Integer) As IEnumerable(Of T)
        If compare = Compares.FromLoci Then
            For Each loci As T In data.Where(Function(x) x.SequenceData.Length >= lengthMin)
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
            For Each loci As T In data.Where(Function(x) x.SequenceData.Length >= lengthMin)
                Dim orders As Integer() = getLocis(loci).OrderBy(Function(x) x).ToArray
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
    ''' 根据<see cref="ReverseRepeatsView.reversed"/>来进行筛选
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="interval"></param>
    ''' <param name="compare"></param>
    ''' <param name="returnsAll"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function FilteringRev(data As IEnumerable(Of ReverseRepeatsView),
                                 Optional interval As Integer = 2000,
                                 Optional compare As Compares = Compares.Interval,
                                 Optional returnsAll As Boolean = False,
                                 Optional lenMin As Integer = 4) As IEnumerable(Of ReverseRepeatsView)
        Return data.doFiltering(Function(x) x.reversed,
                                Sub(x, rl) x.reversed = rl,
                                interval,
                                compare,
                                returnsAll, lenMin)
    End Function
End Module
