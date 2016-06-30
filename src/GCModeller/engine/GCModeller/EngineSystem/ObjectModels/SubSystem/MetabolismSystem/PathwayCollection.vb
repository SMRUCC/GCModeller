Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.RuntimeObjects
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.MySQL

Namespace EngineSystem.ObjectModels.SubSystem

    Public Class PathwayCollection : Inherits CellComponentSystemFramework(Of ObjectModels.Module.Pathway)
        Implements PlugIns.ISystemFrameworkEntry.ISystemFramework
        Implements IDrivenable

        Sub New(Metabolism As SubSystem.MetabolismCompartment)
            Call MyBase.New(Metabolism)
        End Sub

        Public Overrides Function CreateServiceSerials() As IDataAcquisitionService()
            If MyBase._DynamicsExprs.IsNullOrEmpty Then
                Call SystemLogging.WriteLine("There is no pathway data define in the data model, will not create the data service for the pathway collection.", "", Type:=Logging.MSG_TYPES.INF)
                Return New IDataAcquisitionService() {}
            Else
                Return New IDataAcquisitionService() {
          New DataAcquisitionService(New EngineSystem.Services.DataAcquisition.DataAdapters.PathwayCollection(Me), RuntimeContainer:=MyBase.get_RuntimeContainer)}
            End If
        End Function

        Public Overrides Function Initialize() As Integer
            Dim CellSystem = DirectCast(_CellComponentContainer, SubSystem.MetabolismCompartment)._CellSystem

            If CellSystem.DataModel.Metabolism.Pathways.IsNullOrEmpty Then
                MyBase._DynamicsExprs = New ObjectModels.Module.Pathway() {}
            Else
                MyBase._DynamicsExprs = (From item In CellSystem.DataModel.Metabolism.Pathways Select EngineSystem.ObjectModels.Module.Pathway.CreateObject(item, CellSystem.Metabolism)).ToArray
                Call SystemLogging.WriteLine(NetworkComponents.Count & " pathway objects was defined in the data model.", "", Type:=Logging.MSG_TYPES.INF)
            End If

            Return 0
        End Function

        Public Overrides Sub MemoryDump(Dir As String)
            Call Me.I_CreateDump.SaveTo(String.Format("{0}/{1}.log", Dir, Me.GetType.Name))
        End Sub

        Public ReadOnly Property EventId As String Implements IDrivenable.EventId
            Get
                Return "Pathway DataSource Driver"
            End Get
        End Property

        Protected Overrides Function __innerTicks(KernelCycle As Integer) As Integer Implements IDrivenable.__innerTicks
            Dim LQuery = (From data0expr In _DynamicsExprs.AsParallel Select data0expr.Invoke).ToArray
            Return 0
        End Function
    End Class
End Namespace