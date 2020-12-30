Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.NeedlemanWunsch
Imports stdNum = System.Math

Namespace SmithWaterman

    Public Class LocalHSPMatch(Of T) : Inherits Match

        ''' <summary>
        ''' query的一部分片段
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property seq1 As T()
        ''' <summary>
        ''' subject的一部分片段
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property seq2 As T()

        ReadOnly symbol As GenericSymbol(Of T)

        ''' <summary>
        ''' 完整的query的长度
        ''' </summary>
        ''' <returns></returns>
        Public Property QueryLength As Integer
        ''' <summary>
        ''' 完整的subject的长度
        ''' </summary>
        ''' <returns></returns>
        Public Property SubjectLength As Integer

        ''' <summary>
        ''' length of the query fragment size
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property LengthQuery As Integer
            Get
                Return stdNum.Abs(toA - fromA)
            End Get
        End Property

        ''' <summary>
        ''' length of the hit fragment size
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property LengthHit As Integer
            Get
                Return stdNum.Abs(toB - fromB)
            End Get
        End Property

        Sub New(m As Match, seq1 As T(), seq2 As T(), symbol As GenericSymbol(Of T))
            Call MyBase.New(m)

            Me.symbol = symbol
            Me.seq1 = seq1.Skip(fromA).Take(toA - fromA).ToArray
            Me.seq2 = seq2.Skip(fromB).Take(toB - fromB).ToArray
        End Sub

        Public Function GetAlignment() As GlobalAlign(Of T)
            Dim gnw As New NeedlemanWunsch(Of T)(seq1, seq2, New ScoreMatrix(Of T)(symbol), symbol)
            Dim best As GlobalAlign(Of T) = gnw _
                .Compute() _
                .PopulateAlignments _
                .OrderByDescending(Function(a) a.score) _
                .FirstOrDefault

            Return best
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Function ToString(seq As T(), toChar As Func(Of T, Char)) As String
            Return seq.Select(toChar).CharString
        End Function

        Public Overrides Function ToString() As String
            Dim l1 As String = ToString(seq1, AddressOf symbol.ToChar)
            Dim l2 As String = ToString(seq2, AddressOf symbol.ToChar)

            Return {l1, l2}.JoinBy(vbCrLf)
        End Function

    End Class
End Namespace