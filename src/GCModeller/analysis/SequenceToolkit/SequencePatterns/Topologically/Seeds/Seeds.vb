#Region "Microsoft.VisualBasic::f3e5660345277df52bdb862441228fe3, GCModeller\analysis\SequenceToolkit\SequencePatterns\Topologically\Seeds\Seeds.vb"

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

    '   Total Lines: 87
    '    Code Lines: 39
    ' Comment Lines: 37
    '   Blank Lines: 11
    '     File Size: 3.29 KB


    '     Module Seeds
    ' 
    '         Function: Combo, ExtendSequence, (+2 Overloads) InitializeSeeds, PopulateExistsSeeds
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace Topologically.Seeding

    Public Module Seeds

        ''' <summary>
        ''' 将所输入的位于目标序列<paramref name="seq"/>之上的所有的有效的种子<paramref name="seeds"/>都拿出来
        ''' </summary>
        ''' <param name="seeds"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function PopulateExistsSeeds(seq$, seeds As IEnumerable(Of String)) As IEnumerable(Of String)
            Return seeds.AsParallel.Where(Function(s) seq.IndexOf(s) > -1)
        End Function

        ''' <summary>
        ''' 延伸种子的长度
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="Chars"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function ExtendSequence(source As IEnumerable(Of String), Chars As Char()) As IEnumerable(Of String)
            For Each seq As String In source
                For Each extend As String In Seeds.Combo(seq, Chars)
                    Yield extend
                Next
            Next
        End Function

        ''' <summary>
        ''' Initialize the nucleotide repeats seeds.(初始化序列片段的搜索种子)
        ''' </summary>
        ''' <param name="chars"></param>
        ''' <param name="length"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension>
        Public Function InitializeSeeds(chars As Char(), length%) As String()
            Dim seeds = {""}

            For i As Integer = 1 To length
                seeds = seeds.ExtendSequence(chars).ToArray
            Next

            Return seeds
        End Function

        ''' <summary>
        ''' 这个函数会过滤掉一部分在目标序列上不存在的种子序列
        ''' </summary>
        ''' <param name="chars"></param>
        ''' <param name="length%"></param>
        ''' <param name="sequence">
        ''' 因为过滤掉了一部分的不存在的种子序列
        ''' 所以这个理论上搜索效率会更高
        ''' </param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function InitializeSeeds(chars As Char(), length%, sequence$) As IEnumerable(Of String)
            Return sequence.PopulateExistsSeeds(seeds:=chars.InitializeSeeds(length))
        End Function

        ''' <summary>
        ''' Extend target <paramref name="seq"/> with one base character.
        ''' </summary>
        ''' <param name="seq$"></param>
        ''' <param name="chars"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Combo(seq$, chars As Char()) As String()
            Dim seeds As String() = New String(chars.Length - 1) {}

            ' 为了提升计算效率
            ' 在这里采用预分配内存的固定数组来获取序列种子拓展的数据
            For i As Integer = 0 To seeds.Length - 1
                seeds(i) = seq & chars(i)
            Next

            Return seeds
        End Function
    End Module
End Namespace
