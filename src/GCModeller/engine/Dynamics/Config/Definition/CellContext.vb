Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Definitions

    ''' <summary>
    ''' define the x0 status of the cell for run the simulation
    ''' </summary>
    Public Class CellContext : Implements INamedValue

        Public Property cell_id As String Implements INamedValue.Key

        ''' <summary>
        ''' [molecule => initial context status]
        ''' </summary>
        ''' <returns></returns>
        Public Property context As Dictionary(Of String, Double)

    End Class
End Namespace