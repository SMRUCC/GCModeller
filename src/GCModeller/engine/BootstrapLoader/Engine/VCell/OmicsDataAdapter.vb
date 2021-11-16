#Region "Microsoft.VisualBasic::28ac001909a5c3253a1b9599180f940a, engine\BootstrapLoader\Engine\VCell\OmicsDataAdapter.vb"

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

    '     Class OmicsDataAdapter
    ' 
    '         Properties: flux, mass
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetFluxTuples, GetMassTuples
    ' 
    '         Sub: FluxSnapshot, MassSnapshot
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.ModelLoader
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular

Namespace Engine

    Public Class OmicsDataAdapter : Implements IOmicsDataAdapter

        Public ReadOnly Property mass As OmicsTuple(Of String()) Implements IOmicsDataAdapter.mass
        Public ReadOnly Property flux As OmicsTuple(Of String())

        Dim saveMass As OmicsTuple(Of SnapshotDriver)
        Dim saveFlux As OmicsTuple(Of SnapshotDriver)

        Sub New(model As CellularModule, mass As OmicsTuple(Of SnapshotDriver), flux As OmicsTuple(Of SnapshotDriver))
            Me.saveMass = mass
            Me.saveFlux = flux
            Me.mass = GetMassTuples(model)
            Me.flux = GetFluxTuples(model)
        End Sub

        Public Sub MassSnapshot(iteration As Integer, data As Dictionary(Of String, Double)) Implements IOmicsDataAdapter.MassSnapshot
            Call saveMass.transcriptome(iteration, data.Subset(mass.transcriptome))
            Call saveMass.proteome(iteration, data.Subset(mass.proteome))
            Call saveMass.metabolome(iteration, data.Subset(mass.metabolome))
        End Sub

        Public Sub FluxSnapshot(iteration As Integer, data As Dictionary(Of String, Double)) Implements IOmicsDataAdapter.FluxSnapshot
            Call saveFlux.transcriptome(iteration, data.Subset(flux.transcriptome))
            Call saveFlux.proteome(iteration, data.Subset(flux.proteome))
            Call saveFlux.metabolome(iteration, data.Subset(flux.metabolome))
        End Sub

        Public Shared Function GetMassTuples(model As CellularModule) As OmicsTuple(Of String())
            ' 因为rRNA和tRNA会存在重复
            ' 所以对于RNA分子需要做一下去重复
            Dim RNA = model.Genotype.centralDogmas _
                .Select(Function(gene) gene.RNAName) _
                .Distinct _
                .ToArray
            Dim protein = model.Genotype.centralDogmas _
                .Where(Function(gene) Not gene.IsRNAGene) _
                .Select(Function(gene) gene.polypeptide) _
                .ToArray
            Dim metabolites = model.Phenotype.fluxes _
                .Select(Function(flux)
                            Return flux.products.AsList + flux.substrates
                        End Function) _
                .IteratesALL _
                .Select(Function(mass) mass.text) _
                .Distinct _
                .ToArray

            Return New OmicsTuple(Of String())(RNA, protein, metabolites)
        End Function

        Public Shared Function GetFluxTuples(model As CellularModule) As OmicsTuple(Of String())
            Dim transcription As String() = model.Genotype.centralDogmas _
                .Select(AddressOf Loader.GetTranscriptionId) _
                .ToArray
            Dim translation As String() = model.Genotype.centralDogmas _
                .Where(Function(cd) Not cd.IsRNAGene) _
                .Select(AddressOf Loader.GetTranslationId) _
                .ToArray
            Dim proteinComplex = model.Phenotype.proteins _
                .Select(AddressOf Loader.GetProteinMatureId) _
                .AsList
            Dim metabolism = model.Phenotype.fluxes _
                .Select(Function(r) r.ID) _
                .ToArray

            Return New OmicsTuple(Of String())(
                transcriptome:=transcription,
                proteome:=translation,
                metabolome:=proteinComplex + metabolism
            )
        End Function
    End Class
End Namespace
