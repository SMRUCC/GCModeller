Namespace EngineSystem.ObjectModels.SubSystem

    Public Class Chemotaxis : Inherits EngineSystem.ObjectModels.SubSystem.CellComponentSystemFramework(Of EngineSystem.ObjectModels.Module.Chemotaxis)

        Sub New(Metabolism As SubSystem.MetabolismCompartment, CultivationMediums As SubSystem.CultivationMediums)
            Call MyBase.New(Metabolism)
        End Sub

        Public Overrides Function CreateServiceSerials() As Services.MySQL.IDataAcquisitionService()
            Throw New NotImplementedException
        End Function

        Public Overrides Function Initialize() As Integer
            Throw New NotImplementedException
        End Function

        Public Overrides Sub MemoryDump(Dir As String)

        End Sub

        Protected Overrides Function __innerTicks(KernelCycle As Integer) As Integer
            Return 0
        End Function
    End Class
End Namespace