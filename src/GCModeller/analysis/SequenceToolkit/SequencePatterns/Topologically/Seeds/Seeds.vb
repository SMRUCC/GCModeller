#Region "Microsoft.VisualBasic::5c4f6d9038ffdb15aff103564734acb9, analysis\SequenceToolkit\SequencePatterns\Topologically\Seeds\Seeds.vb"

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

    '     Module Seeds
    ' 
    '         Function: Combo, ExtendSequence, (+2 Overloads) InitializeSeeds, PopulateExistsSeeds
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language

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
            Return seeds.Where(Function(s) seq.IndexOf(s) > -1)
        End Function

        ''' <summary>
        ''' 延伸种子的长度
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="Chars"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ExtendSequence(source As IEnumerable(Of String), Chars As Char()) As List(Of String)
            Return LinqAPI.MakeList(Of String) _
 _
                () <= From s As String
                      In source.AsParallel
                      Select Seeds.Combo(s, Chars)
        End Function

        ''' <summary>
        ''' Initialize the nucleotide repeats seeds.(初始化序列片段的搜索种子)
        ''' </summary>
        ''' <param name="chars"></param>
        ''' <param name="length"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InitializeSeeds(chars As Char(), length%) As List(Of String)
            Dim tmp As New List(Of String) From {""}

            For i As Integer = 1 To length
                tmp = ExtendSequence(tmp, chars)
            Next

            Return LinqAPI.MakeList(Of String) _
 _
                () <= From s As String
                      In tmp
                      Select s
                      Order By s Descending
        End Function

        <Extension>
        Public Function InitializeSeeds(chars As Char(), length%, sequence$) As String()
            Dim buf As List(Of String) = InitializeSeeds(chars, length)
            Dim LQuery$() = LinqAPI.Exec(Of String) _
 _
                () <= From seed As String
                      In buf.AsParallel
                      Where InStr(sequence, seed, CompareMethod.Text) > 0
                      Select seed

            Return LQuery
        End Function

        ''' <summary>
        ''' Extend target <paramref name="seq"/> with one base character.
        ''' </summary>
        ''' <param name="seq$"></param>
        ''' <param name="Chars"></param>
        ''' <returns></returns>
        <Extension> Public Function Combo(seq$, Chars As Char()) As String()
            Return LinqAPI.Exec(Of String) _
 _
                () <= From ch As Char
                      In Chars
                      Select seq & CStr(ch)
        End Function
    End Module
End Namespace
