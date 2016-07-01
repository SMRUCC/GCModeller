Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.Services.DataAcquisition.DataAdapters

    Public Class Proteome : Inherits EngineSystem.Services.DataAcquisition.DataAdapter(Of EngineSystem.ObjectModels.SubSystem.ProteinAssembly)
        Implements IDataAdapter

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "proteome"
            End Get
        End Property

        Sub New(System As EngineSystem.ObjectModels.SubSystem.ProteinAssembly)
            Call MyBase.New(System)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Dim LQuery = From Protein In MyBase.System.Proteins Let value = Protein.DataSource Select value  '
            Return LQuery.ToArray
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Return (From item In MyBase.System.Proteins Select item.SerialsHandle).ToArray
        End Function
    End Class
End Namespace