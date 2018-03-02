Public Class DNAMatrix : Inherits Blosum

    ' In general, matches are assigned positive scores, And mismatches are 
    ' assigned relatively lower scores. 
    ' Take DNA sequence As an example. If matches Get +1, mismatches Get -1, 
    ' Then the substitution matrix Is:

    '    A	 G	 C	 T  *
    ' A	 1	-1	-1	-1  0
    ' G	-1	 1	-1	-1  0
    ' C	-1	-1	 1	-1  0
    ' T	-1	-1	-1	 1  0
    ' *  0   0   0   0  0

    Sub New()
        Matrix = {
            {+1, -1, -1, -1, 0},
            {-1, +1, -1, -1, 0},
            {-1, -1, +1, -1, 0},
            {-1, -1, -1, +1, 0},
            {+0, +0, +0, +0, 0}
        }.ToVectorList
    End Sub

    Protected Overrides Function getIndex(a As Char) As Integer
        Select Case Char.ToUpper(a)
            Case "A"c
                Return 0
            Case "G"c
                Return 1
            Case "C"c
                Return 2
            Case "T"c
                Return 3
            Case "*"c, "-"c, "N"c
                Return 4
            Case Else
                Throw New InvalidCastException(a)
        End Select
    End Function
End Class
