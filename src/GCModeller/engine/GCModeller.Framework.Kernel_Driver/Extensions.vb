Imports System.Runtime.CompilerServices

Public Module Extensions

    <Extension> Public Function GetHandle(Of T)(DynamicsExpression As Framework.Kernel_Driver.IDynamicsExpression(Of T)) As Framework.Kernel_Driver.DataStorage.FileModel.ObjectHandle
        Return New Framework.Kernel_Driver.DataStorage.FileModel.ObjectHandle With {
            .Identifier = DynamicsExpression.Identifier,
            .Handle = DynamicsExpression.Address
        }
    End Function
End Module
