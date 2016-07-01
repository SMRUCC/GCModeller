Imports System.Text
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.Services.DataAcquisition.DataAdapters

    ''' <summary>
    ''' 采集每一个反应对象的节点流量
    ''' </summary>
    ''' <remarks>所搜集的数据内容：每一种反应过程在每一次内核循环过程中的实时流量</remarks>
    Public Class DelegateSystem : Inherits EngineSystem.Services.DataAcquisition.DataAdapter(Of EngineSystem.ObjectModels.SubSystem.DelegateSystem)
        Implements IDataAdapter

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "metabolism_flux"
            End Get
        End Property

        Sub New(DataSource As EngineSystem.ObjectModels.SubSystem.DelegateSystem)
            Call MyBase.New(DataSource)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Return System.DataSource
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Dim ChunkBuffer = (From FluxObject As MetabolismFlux
                               In System.NetworkComponents
                               Select New HandleF With {
                                   .Identifier = GenerateId(FluxObject),
                                   .Handle = FluxObject.Handle}).ToArray
            Return ChunkBuffer
        End Function

        Private Shared Function GenerateId(flux As MetabolismFlux) As String
            Dim UniqueId As String = flux.Identifier
            If flux.Reversible = True Then
                UniqueId = String.Format("[{0}]", UniqueId)
            End If

            If flux.TypeId = ObjectModels.ObjectModel.TypeIds.EnzymaticFlux Then
                UniqueId = "*" & UniqueId
            End If
            Return UniqueId
        End Function
    End Class
End Namespace