#Region "Microsoft.VisualBasic::d8efdf74d7e79f8777c61345fb7d614e, ..\GCModeller\analysis\SequenceToolkit\SequencePatterns\Topologically\Seeds.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language

Namespace Topologically.Seeding

    Public Module Seeds

        ''' <summary>
        ''' 延伸种子的长度
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="Chars"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ExtendSequence(source As IEnumerable(Of String), Chars As Char()) As List(Of String)
            Return LinqAPI.MakeList(Of String) <= From s As String
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
        Public Function InitializeSeeds(chars As Char(), length As Integer) As List(Of String)
            Dim tmp As List(Of String) = New List(Of String) From {""}

            For i As Integer = 1 To length
                tmp = ExtendSequence(tmp, chars)
            Next

            Return LinqAPI.MakeList(Of String) <=
                From s As String
                In tmp
                Select s
                Order By s Descending
        End Function

        <Extension>
        Public Function InitializeSeeds(chars As Char(), length As Integer, sequence As String) As String()
            Dim buf As List(Of String) = InitializeSeeds(chars, length)
            Dim LQuery As String() =
                LinqAPI.Exec(Of String) <= From seed As String
                                           In buf.AsParallel
                                           Where InStr(sequence, seed, CompareMethod.Text) > 0
                                           Select seed
            Return LQuery
        End Function

        <Extension>
        Public Function Combo(Sequence As String, Chars As Char()) As String()
            Return LinqAPI.Exec(Of String) <=
                From ch As Char
                In Chars
                Select Sequence & CStr(ch)
        End Function
    End Module
End Namespace
