Namespace Linq

    Public Structure SliceRange
        Dim start As Integer
        Dim length As Integer
        Dim steps As Integer
    End Structure

    Public Module SliceExtensions

        Public Function Slice(Optional start% = Integer.MinValue, Optional length% = Integer.MinValue) As SliceRange
            Return New SliceRange With {.start = start, .length = length}
        End Function
    End Module
End Namespace