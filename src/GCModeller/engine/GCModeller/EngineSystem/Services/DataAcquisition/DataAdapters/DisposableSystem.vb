Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.Services.DataAcquisition.DataAdapters

    Public Class DisposableSystem(Of MolecularType As EngineSystem.ObjectModels.Entity.IDisposableCompound)
        Inherits EngineSystem.Services.DataAcquisition.DataAdapter(Of EngineSystem.ObjectModels.SubSystem.DisposerSystem(Of MolecularType))
        Implements IDataAdapter

        Dim Type As EngineSystem.ObjectModels.Entity.IDisposableCompound.DisposableCompoundTypes

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "Disposal_Of_" & Type.ToString
            End Get
        End Property

        Sub New(System As EngineSystem.ObjectModels.SubSystem.DisposerSystem(Of MolecularType), Type As EngineSystem.ObjectModels.Entity.IDisposableCompound.DisposableCompoundTypes)
            Call MyBase.New(System)
            Me.Type = Type
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Return System.DataSource
        End Function

        Public Overrides Function DefHandles() As HandleF()
            If Type = ObjectModels.Entity.IDisposableCompound.DisposableCompoundTypes.Transcripts Then
                Dim ChunkBuffer = (From item In MyBase.System.NetworkComponents Select New HandleF With {.Handle = item.Handle, .Identifier = item.Identifier.Replace(".TRANSCRIPT", "")}).ToArray
                Return ChunkBuffer
            Else
                Return MyBase.System.get_DataSerializerHandles
            End If
        End Function
    End Class
End Namespace