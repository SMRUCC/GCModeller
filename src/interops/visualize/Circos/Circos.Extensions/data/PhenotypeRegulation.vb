#Region "Microsoft.VisualBasic::e610f49fc0056f8440de4a10fa661f93, visualize\Circos\Circos.Extensions\data\PhenotypeRegulation.vb"

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

    '     Class PhenotypeRegulation
    ' 
    '         Properties: Size
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GenerateDocument
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.InteractionModel.Regulon
Imports SMRUCC.genomics.Visualize.Circos.Karyotype

Namespace Documents.Karyotype

    Public Class PhenotypeRegulation : Inherits SkeletonInfo

        ''' <summary>
        ''' Family, Regulators
        ''' </summary>
        ''' <remarks></remarks>
        Dim RegulatorFamilies As KeyValuePair(Of String, String())()
        ''' <summary>
        ''' Phenotype Class, GeneList
        ''' </summary>
        ''' <remarks></remarks>
        Dim PhenoTypeAssociations As KeyValuePair(Of String, String())()
        ''' <summary>
        ''' Regulator, Genes
        ''' </summary>
        ''' <remarks></remarks>
        Dim Regulations As KeyValuePair(Of String, String())()

        Public Overrides ReadOnly Property Size As Integer

        Sub New(Regulations As IEnumerable(Of IRegulon), Pathways As IEnumerable(Of bGetObject.Pathway))
            Dim PathwayGenes = (From Pathway In Pathways
                                Select PathwayId = Pathway.EntryId,
                                    PathwayGenesId = Pathway.GetPathwayGenes).ToArray
            RegulatorFamilies = Regprecise.FamilyStatics2(Regulations)

            Dim PathwayFunctions As Dictionary(Of String, BriteHEntry.Pathway) =
                BriteHEntry.Pathway.LoadDictionary

            Dim LQuery = (From Pathway As bGetObject.Pathway
                          In Pathways.AsParallel
                          Where Not Pathway.Genes.IsNullOrEmpty
                          Let PathwayId As String = Pathway.EntryId
                          Let [Class] As BriteHEntry.Pathway = PathwayFunctions(Regex.Match(PathwayId, "\d{5}").Value)
                          Select Phenotype = [Class].Category,
                              AssociationGenes = Pathway.GetPathwayGenes).ToArray
            PhenoTypeAssociations = (From Phenotype As String
                                     In (From item In LQuery Select item.Phenotype Distinct).ToArray
                                     Let AssociatedGene As String() = (From item In LQuery
                                                                       Where String.Equals(Phenotype, item.Phenotype)
                                                                       Select item.AssociationGenes).ToVector
                                     Select New KeyValuePair(Of String, String())(Phenotype, AssociatedGene)).ToArray
            Me.Regulations = (From Regulator As String
                              In (From item In Regulations Select item.TFlocusId Distinct).ToArray
                              Let RegulatedGene = (From item In Regulations Where String.Equals(item.TFlocusId, Regulator) Select item.RegulatedGenes).ToVector
                              Select New KeyValuePair(Of String, String())(Regulator, (From strId As String
                                                                                       In RegulatedGene
                                                                                       Select strId Distinct).ToArray)).ToArray
        End Sub

        Protected Overloads Function GenerateDocument() As String
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            Dim i As Integer = 0
            For Each PhenoType In PhenoTypeAssociations
                i += 1
                Call sBuilder.AppendLine(String.Format("chr - ch{0} {0} 0 {1} chr{0}", i, PhenoType.Value.Count))
            Next

            Return sBuilder.ToString
        End Function
    End Class
End Namespace
