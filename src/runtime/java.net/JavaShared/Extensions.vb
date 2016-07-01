Module Extensions

    Public Function ReturnRectangularDoubleArray(size1 As Integer, size2 As Integer) As Double()()
        Dim newArray As Double()() = New Double(size1 - 1)() {}
        For array1 As Integer = 0 To size1 - 1
            newArray(array1) = New Double(size2 - 1) {}
        Next

        Return newArray
    End Function
End Module
