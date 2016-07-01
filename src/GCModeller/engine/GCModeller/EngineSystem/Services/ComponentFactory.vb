Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.RuntimeObjects

Namespace EngineSystem.Services

    Public Class ComponentFactory : Inherits RuntimeObject

        Dim _ModelSource As EngineSystem.ObjectModels.SubSystem.CellSystem

        Sub New(ModelSource As EngineSystem.ObjectModels.SubSystem.CellSystem)
            _ModelSource = ModelSource
        End Sub

#Region "EngineSystem.ObjectModels.Entity"
        Public Function CreateObject() As EngineSystem.ObjectModels.Entity.Compound
            Throw New NotImplementedException
        End Function
#End Region
    End Class
End Namespace