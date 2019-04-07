Imports System.Runtime.CompilerServices

Namespace SequenceModel

    Public Module TypeExtensions

#Region "Constants"

        ''' <summary>
        ''' Enumerate all of the amino acid.(字符串常量枚举所有的氨基酸分子)
        ''' </summary>
        ''' <remarks></remarks>
        Public Const AA_CHARS_ALL As String = "BDEFHIJKLMNOPQRSVWXYZ"
        ''' <summary>
        ''' Enumerate all of the nucleotides.(字符串常量枚举所有的核苷酸分子类型)
        ''' </summary>
        ''' <remarks></remarks>
        Public Const NA_CHARS_ALL As String = "ATGCU"
#End Region

        <Extension>
        Public Function GetSeqType(seq As IPolymerSequenceModel) As SeqTypes
            If IsProteinSource(seq) Then
                Return SeqTypes.Protein
            ElseIf seq.SequenceData.Any(Function(c) c = "U"c) Then
                Return SeqTypes.RNA
            Else
                Return SeqTypes.DNA
            End If
        End Function

        ''' <summary>
        ''' 目标序列数据是否为一条蛋白质序列
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function IsProteinSource(seq As IPolymerSequenceModel) As Boolean
            Return seq.SequenceData _
                .ToUpper _
                .Where(Function(c) c <> "N"c AndAlso AA_CHARS_ALL.IndexOf(c) > -1) _
                .FirstOrDefault _
                .CharCode > 0
        End Function
    End Module
End Namespace