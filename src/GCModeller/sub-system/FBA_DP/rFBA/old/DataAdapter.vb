Namespace rFBA

    Public Class DataAdapter : Inherits SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.Kernel.DataAdapter(Of rFBA.NetworkModel)

        Sub New(ModelEngine As rFBA.Engine)
            Call MyBase.New(ModelEngine)
        End Sub

        Protected Overrides Function __getHeaders() As String()
            Throw New NotImplementedException
        End Function
    End Class
End Namespace