Namespace Kernel.ObjectModels

    Public Enum Types
        Increase
        Decrease
        ChangeTo
    End Enum

    Public Module DisturbAPI

        Public ReadOnly Property Methods As IReadOnlyDictionary(Of Types, Func(Of Double, Double, Double)) =
            New Dictionary(Of Types, Func(Of Double, Double, Double)) From {
 _
            {Types.Increase, Function(x, d) x + d},
            {Types.Decrease, Function(x, d) x - d},
            {Types.ChangeTo, Function(x, d) x}
        }
    End Module
End Namespace