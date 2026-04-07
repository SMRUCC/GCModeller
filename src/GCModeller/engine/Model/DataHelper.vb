#Region "Microsoft.VisualBasic::0c279e4fe2eddbe789bfd81d00e2b32a, engine\Model\DataHelper.vb"

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

    '   Total Lines: 52
    '    Code Lines: 44 (84.62%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 8 (15.38%)
    '     File Size: 2.26 KB


    ' Module DataHelper
    ' 
    '     Function: getFluxIds, GetProteinMatureId, getProteinProcess, getTranscription, GetTranscriptionId
    '               getTranslation, GetTranslationId
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Molecule
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process

Public Module DataHelper

    Public Function GetTranscriptionId(cd As CentralDogma, cellular_id As String) As String
        Return $"{cd.geneID}@{cellular_id}[Transcription]"
    End Function

    Public Function GetTranslationId(cd As CentralDogma, cellular_id As String) As String
        Return $"{cd.geneID}@{cellular_id}[Translation]"
    End Function

    Public Function GetProteinMatureId(protein As Protein, cellular_id As String) As String
        Return $"{protein.ProteinID}@{cellular_id}[Protein-Mature]"
    End Function

    Public Iterator Function getProteinProcess(models As CellularModule()) As IEnumerable(Of String)
        For Each model As CellularModule In models
            For Each prot As Protein In model.Phenotype.proteins
                Yield GetProteinMatureId(prot, model.CellularEnvironmentName)
            Next
        Next
    End Function

    Public Iterator Function getTranslation(models As CellularModule()) As IEnumerable(Of String)
        For Each model As CellularModule In models
            For Each gene As CentralDogma In model.Genotype.centralDogmas
                If gene.RNA.Value = RNATypes.mRNA Then
                    Yield GetTranslationId(gene, model.CellularEnvironmentName)
                End If
            Next
        Next
    End Function

    Public Iterator Function getTranscription(models As CellularModule()) As IEnumerable(Of String)
        For Each model As CellularModule In models
            For Each gene As CentralDogma In model.Genotype.centralDogmas
                Yield GetTranscriptionId(gene, model.CellularEnvironmentName)
            Next
        Next
    End Function

    Public Iterator Function getFluxIds(models As CellularModule()) As IEnumerable(Of String)
        For Each model As CellularModule In models
            For Each flux As Reaction In model.Phenotype.fluxes
                Yield flux.ID & "@" & model.CellularEnvironmentName
            Next
        Next
    End Function
End Module

