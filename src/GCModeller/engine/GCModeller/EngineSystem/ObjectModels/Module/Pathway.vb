#Region "Microsoft.VisualBasic::48bd2cc4256e4837425933acdb5cb1ff, engine\GCModeller\EngineSystem\ObjectModels\Module\Pathway.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '     Class Pathway
    ' 
    '         Properties: FluxValue, TypeId
    ' 
    '         Function: CreateObject, Invoke
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Serialization
Imports SMRUCC.genomics.GCModeller.Assembly
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements

Namespace EngineSystem.ObjectModels.Module

    Public Class Pathway : Inherits SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.FluxObject

        Protected Friend PathwayModel As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Pathway
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
