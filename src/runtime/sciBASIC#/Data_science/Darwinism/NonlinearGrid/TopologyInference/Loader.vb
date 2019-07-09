Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Public Module Loader

    Public Function EmptyGridSystem(width As Integer) As GridSystem
        Return New GridSystem With {
            .A = Vector.rand(width),
            .C = width.SeqIterator _
                .Select(Function(null)
                            Return New Correlation With {
                                .B = Vector.rand(width)
                            }
                        End Function) _
                .ToArray
        }
    End Function
End Module
