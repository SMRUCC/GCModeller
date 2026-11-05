Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Definitions

    ''' <summary>
    ''' define the x0 status of the cell for run the simulation
    ''' </summary>
    Public Class CellContext : Implements INamedValue

        Public Property name As String Implements INamedValue.Key

        ''' <summary>
        ''' [molecule => initial context status]
        ''' </summary>
        ''' <returns></returns>
        Public Property status As Dictionary(Of String, Double)

        Default Public ReadOnly Property Item(id As String) As Double
            Get
                Return status.TryGetValue(id)
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function CheckCompound(id As String) As Boolean
            Return status.ContainsKey(id)
        End Function

        Public Shared Narrowing Operator CType(context As CellContext) As Dictionary(Of String, Double)
            Return context.status
        End Operator

    End Class
End Namespace