Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Public Class ResidueScore

    Public ReadOnly Property residues As Char()

    Public Shared ReadOnly Property Protein As ResidueScore
        Get
            Return New ResidueScore("")
        End Get
    End Property

    Public Shared ReadOnly Property Gene As ResidueScore
        Get
            Return New ResidueScore("ATGC")
        End Get
    End Property

    Sub New(chars As IEnumerable(Of Char))
        residues = chars.ToArray
    End Sub

    Public Function Cos(a As Residue, b As Residue) As Double
        Dim v1 As New Vector(residues.Select(Function(c) a(c)))
        Dim v2 As New Vector(residues.Select(Function(c) b(c)))

        Return v1.SSM(v2)
    End Function

    Public Overrides Function ToString() As String
        Return residues.CharString
    End Function

End Class
