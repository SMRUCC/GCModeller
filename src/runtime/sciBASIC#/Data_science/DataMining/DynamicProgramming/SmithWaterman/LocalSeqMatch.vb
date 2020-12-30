Imports System.Runtime.CompilerServices

Namespace SmithWaterman

    Public Class LocalSeqMatch(Of T) : Inherits Match

        Public ReadOnly Property seq1 As T()
        Public ReadOnly Property seq2 As T()

        ReadOnly symbol As GenericSymbol(Of T)

        Sub New(m As Match, seq1 As T(), seq2 As T(), symbol As GenericSymbol(Of T))
            Call MyBase.New(m)

            Me.symbol = symbol
            Me.seq1 = seq1.Skip(fromA).Take(toA - fromA).ToArray
            Me.seq2 = seq2.Skip(fromB).Take(toB - fromB).ToArray
        End Sub

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