Imports LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.ObjectModels.Entity

    Public Class Peptide : Inherits IDisposableCompound

        <DumpNode> Protected Friend ProteinModelBase As GCML_Documents.XmlElements.Metabolism.Polypeptide

        Public Overrides ReadOnly Property DataSource As DataSource
            Get
                Return New DataSource(Handle, EntityBaseType.DataSource.value)
            End Get
        End Property

        Public Overrides Property Quantity As Double
            Get
                Return EntityBaseType.Quantity
            End Get
            Set(value As Double)
                EntityBaseType.Quantity = value
            End Set
        End Property

        Public ReadOnly Property ProteinType As GCML_Documents.XmlElements.Metabolism.Polypeptide.ProteinTypes
            Get
                Return ProteinModelBase.ProteinType
            End Get
        End Property
    End Class
End Namespace