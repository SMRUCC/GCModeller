Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.Services.DataAcquisition.DataAdapters

    Public Class RibosomeAssembly : Inherits EngineSystem.Services.DataAcquisition.DataAdapter(Of EngineSystem.ObjectModels.SubSystem.RibosomalAssembly)
        Implements IDataAdapter

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "RibosomalAssemblyFlux"
            End Get
        End Property

        Sub New(System As EngineSystem.ObjectModels.SubSystem.RibosomalAssembly)
            Call MyBase.New(System)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Return System.DataSource
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Return (From item In System.NetworkComponents Select item.SerialsHandle).ToArray
        End Function
    End Class

    Public Class RibosomeAssemblyCompounds : Inherits EngineSystem.Services.DataAcquisition.DataAdapter(Of EngineSystem.ObjectModels.SubSystem.RibosomalAssembly)
        Implements IDataAdapter

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "RibosomalAssemblyCompounds"
            End Get
        End Property

        Sub New(System As EngineSystem.ObjectModels.SubSystem.RibosomalAssembly)
            Call MyBase.New(System)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Return (From item In System.RibosomalComplexes Let value = item.DataSource Select value).ToArray
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Return (From item In System.RibosomalComplexes Select item.SerialsHandle).ToArray
        End Function
    End Class

    Public Class RNAPolymeraseAssembly : Inherits EngineSystem.Services.DataAcquisition.DataAdapter(Of EngineSystem.ObjectModels.SubSystem.RNAPolymeraseAssembly)
        Implements IDataAdapter

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "RNAPolymeraseAssemblyFlux"
            End Get
        End Property

        Sub New(System As EngineSystem.ObjectModels.SubSystem.RNAPolymeraseAssembly)
            Call MyBase.New(System)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Return System.DataSource
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Return (From item In System.NetworkComponents Select item.SerialsHandle).ToArray
        End Function
    End Class

    Public Class RNAPolymeraseAssemblyCompounds : Inherits EngineSystem.Services.DataAcquisition.DataAdapter(Of EngineSystem.ObjectModels.SubSystem.RNAPolymeraseAssembly)
        Implements IDataAdapter

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "RNAPolymeraseAssemblyCompounds"
            End Get
        End Property

        Sub New(System As EngineSystem.ObjectModels.SubSystem.RNAPolymeraseAssembly)
            Call MyBase.New(System)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Return (From item In System.RNAPolymerase Let value = item.DataSource Select value).ToArray
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Return (From item In System.RNAPolymerase Select item.SerialsHandle).ToArray
        End Function
    End Class


    Public Class TransmembraneTransportation : Inherits EngineSystem.Services.DataAcquisition.DataAdapter(Of EngineSystem.ObjectModels.SubSystem.CultivationMediums)
        Implements IDataAdapter

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "TransmembraneTransportation"
            End Get
        End Property

        Sub New(Mediums As EngineSystem.ObjectModels.SubSystem.CultivationMediums)
            Call MyBase.New(Mediums)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Return (From item In System._TransmembraneFluxes Select New DataSource(item.Handle, item.FluxValue)).ToArray
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Return (From item As PassiveTransportationFlux
                    In System._TransmembraneFluxes
                    Select New HandleF With {
                        .Identifier = GenerateId(item),
                        .Handle = item.Handle}).ToArray
        End Function

        Private Shared Function GenerateId(flux As PassiveTransportationFlux) As String
            Dim UniqueId As String = flux.Identifier

            If flux.Reversible Then
                UniqueId = String.Format("[{0}]", UniqueId)
            End If

            If flux.TypeId = ObjectModels.ObjectModel.TypeIds.ActiveTransportationFlux Then
                UniqueId = "*" & UniqueId
            End If
            Return UniqueId
        End Function
    End Class
End Namespace