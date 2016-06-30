Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace GridPBS

    ''' <summary>
    ''' 仅用于数据交换的对象
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure MetaboliteCompound
        Implements IAddressHandle
        Implements sIdEnumerable

        Public Property Handle As Integer Implements IAddressHandle.Address
        Public Property Identifier As String Implements sIdEnumerable.Identifier
        Public Property Quantity As Double

        Public Sub Dispose() Implements IDisposable.Dispose
            Return 'DO NOTHING
        End Sub
    End Structure
End Namespace