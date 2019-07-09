Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Public Module Loader

    ''' <summary>
    ''' 因为累加效应在系统很庞大的时候可能会非常的大,所以在这里全部都是用零来进行初始化的
    ''' </summary>
    ''' <param name="width"></param>
    ''' <returns></returns>
    Public Function EmptyGridSystem(width As Integer) As GridSystem
        Return New GridSystem With {
            .A = Vector.Zero(width),
            .C = width.SeqIterator _
                .Select(Function(null)
                            Return New Correlation With {
                                .B = Vector.Zero(width)
                            }
                        End Function) _
                .ToArray
        }
    End Function
End Module
