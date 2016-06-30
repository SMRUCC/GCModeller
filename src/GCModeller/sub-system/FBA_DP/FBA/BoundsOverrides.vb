Imports Microsoft.VisualBasic.Linq

''' <summary>
''' 复写模型之中的流的约束条件
''' </summary>
Public Class BoundsOverrides

    ReadOnly __UPPER_BOUNDS As IBoundsOverrides
    ReadOnly __LOWER_BOUNDS As IBoundsOverrides

    Sub New(up As IBoundsOverrides, lower As IBoundsOverrides)
        __UPPER_BOUNDS = up
        __LOWER_BOUNDS = lower
    End Sub

    Public Function OverridesUpper(fluxs As IEnumerable(Of String), bounds As Double()) As Double()
        Dim n As Double() = __overrides(fluxs, bounds, __UPPER_BOUNDS)
        Return n
    End Function

    Private Shared Function __overrides(fluxs As IEnumerable(Of String),
                                        bounds As Double(),
                                        ibo As IBoundsOverrides) As Double()
        Return (From rxn As SeqValue(Of String)
                In fluxs.SeqIterator
                Select ibo(rxn.obj, bounds(rxn.i))).ToArray
    End Function

    Public Function OverridesLower(fluxs As IEnumerable(Of String), bounds As Double()) As Double()
        Dim n As Double() = __overrides(fluxs, bounds, __LOWER_BOUNDS)
        Return n
    End Function

End Class

Public Delegate Function IBoundsOverrides(rxn As String, curr As Double) As Double
