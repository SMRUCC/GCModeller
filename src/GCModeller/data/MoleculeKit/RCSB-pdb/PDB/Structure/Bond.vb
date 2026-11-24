Namespace Structures

    ''' <summary>
    ''' The covalent bond between two atom sites.
    ''' </summary>
    Public Structure Bond

        ''' <summary>
        ''' The atom index of the first bond end.
        ''' </summary>
        Public A As Int32
        ''' <summary>
        ''' The atom index of the second bond end.
        ''' </summary>
        Public B As Int32
        ''' <summary>
        ''' The bond order value: 1 / 1.5(aromatic) / 2 / 3.
        ''' </summary>
        Public Order As Double

        ''' <summary>
        ''' Create a new covalent bond model.
        ''' </summary>
        ''' <param name="a">The atom index of the first bond end.</param>
        ''' <param name="b">The atom index of the second bond end.</param>
        ''' <param name="order">The bond order value: 1 / 1.5(aromatic) / 2 / 3.</param>
        Public Sub New(a As Int32, b As Int32, order As Double)
            Me.A = a
            Me.B = b
            Me.Order = order
        End Sub

        Public Overrides Function ToString() As String
            Return $"{A}-{B} ({Order})"
        End Function

    End Structure
End Namespace
