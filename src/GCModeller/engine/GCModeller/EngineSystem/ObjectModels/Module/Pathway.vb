Imports System.Text
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements
Imports Microsoft.VisualBasic

Namespace EngineSystem.ObjectModels.Module

    Public Class Pathway : Inherits SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.FluxObject

        Protected Friend PathwayModel As SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Pathway
        <DumpNode> Private _MetabolismFluxes As EngineSystem.ObjectModels.Module.MetabolismFlux()

        Dim _FluxValue As Double

        Public Overrides Function Invoke() As Double
            _FluxValue = (From Flux In _MetabolismFluxes Select Global.System.Math.Abs(Flux.FluxValue)).ToArray.Sum
            Return FluxValue
        End Function

        Public Shared Function CreateObject(ModelBase As Metabolism.Pathway, Metabolism As EngineSystem.ObjectModels.SubSystem.MetabolismCompartment) As Pathway
            Dim FluxCollection As List(Of [Module].MetabolismFlux) = New List(Of MetabolismFlux)
            Dim Chunkbuffer = (From strId As String In ModelBase.MetabolismNetwork Select Metabolism.DelegateSystem.GetNetworkComponents(strId)).ToArray
            For Each Line In Chunkbuffer
                Call FluxCollection.AddRange(Line)
            Next
            Chunkbuffer = (From strId As String In ModelBase.MetabolismNetwork Select Metabolism.GetTransmembraneFluxs(strId)).ToArray
            For Each Line In Chunkbuffer
                Call FluxCollection.AddRange(Line)
            Next

            Dim Pathway As Pathway = New Pathway With {
                .Identifier = ModelBase.Identifier,
                ._MetabolismFluxes = (From Flux In FluxCollection.AsParallel Where Not Flux Is Nothing Select Flux).ToArray}
            Return Pathway
        End Function

        Public Overrides ReadOnly Property TypeId As ObjectModel.TypeIds
            Get
                Return TypeIds.Pathway
            End Get
        End Property

        Public Overrides ReadOnly Property FluxValue As Double
            Get
                Return _FluxValue
            End Get
        End Property
    End Class
End Namespace