#Region "Microsoft.VisualBasic::8034016e9cdb444199537082be793512, engine\BootstrapLoader\Engine\VCell\OmicsDataAdapter.vb"

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


    ' Code Statistics:

    '   Total Lines: 87
    '    Code Lines: 74 (85.06%)
    ' Comment Lines: 2 (2.30%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 11 (12.64%)
    '     File Size: 4.18 KB


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
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process

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

        Public Shared Function GetMassTuples(ParamArray models As CellularModule()) As OmicsTuple(Of String())
            ' 因为rRNA和tRNA会存在重复
            ' 所以对于RNA分子需要做一下去重复
            Dim RNA = models.Select(Function(m) modelRNANames(m)).IteratesALL.Distinct.ToArray
            Dim protein = models.Select(Function(m) modelPolypeptides(m)).IteratesALL.Distinct.ToArray
            Dim metabolites = models.Select(Function(m) modelMetabolites(m)).IteratesALL.Distinct.ToArray

            Return New OmicsTuple(Of String())(RNA, protein, metabolites)
        End Function

        Private Shared Function modelMetabolites(model As CellularModule) As IEnumerable(Of String)
            Return model.Phenotype.fluxes _
                .Select(Iterator Function(flux) As IEnumerable(Of CompoundSpecieReference)
                            For Each c As CompoundSpecieReference In flux.equation.Reactants
                                Yield c
                            Next
                            For Each c As CompoundSpecieReference In flux.equation.Products
                                Yield c
                            Next
                        End Function) _
                .IteratesALL _
                .Select(Function(mass) mass.ID)
        End Function

        Private Shared Function modelPolypeptides(model As CellularModule) As IEnumerable(Of String)
            Return From gene As CentralDogma
                   In model.Genotype.centralDogmas
                   Where Not gene.IsRNAGene
                   Select gene.polypeptide
        End Function

        Private Shared Function modelRNANames(model As CellularModule) As IEnumerable(Of String)
            Return model.Genotype.centralDogmas.Select(Function(gene) gene.RNAName)
        End Function

        Public Shared Function GetFluxTuples(ParamArray models As CellularModule()) As OmicsTuple(Of String())
            Dim transcription As String() = DataHelper.getTranscription(models).ToArray
            Dim translation As String() = DataHelper.getTranslation(models).ToArray
            Dim proteinComplex = DataHelper.getProteinProcess(models).AsList
            Dim metabolism = DataHelper.getFluxIds(models).ToArray

            Return New OmicsTuple(Of String())(
                transcriptome:=transcription,
                proteome:=translation,
                metabolome:=proteinComplex + metabolism
            )
        End Function

        Public Sub ForwardRegulation(iteration As Integer, data As Dictionary(Of String, Double)) Implements IOmicsDataAdapter.ForwardRegulation

        End Sub

        Public Sub ReverseRegulation(iteration As Integer, data As Dictionary(Of String, Double)) Implements IOmicsDataAdapter.ReverseRegulation

        End Sub
    End Class
End Namespace
