Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Parallel.Linq

Namespace Topologically

    Public Module Seeds

        ''' <summary>
        ''' 延伸种子的长度
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="Chars"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ExtendSequence(source As IEnumerable(Of String), Chars As Char()) As List(Of String)
            Return LinqAPI.MakeList(Of String) <=
                LQuerySchedule.LQuery(
                source,
                Function(s) Seeds.Combo(s, Chars), 200000)
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