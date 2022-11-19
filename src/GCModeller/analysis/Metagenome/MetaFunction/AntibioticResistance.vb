#Region "Microsoft.VisualBasic::f799c2524e01774e0f4b57aaabf9c078, GCModeller\analysis\Metagenome\MetaFunction\AntibioticResistance.vb"

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

    '   Total Lines: 76
    '    Code Lines: 60
    ' Comment Lines: 7
    '   Blank Lines: 9
    '     File Size: 2.99 KB


    ' Module AntibioticResistance
    ' 
    '     Function: TaxonomyProfile
    ' 
    ' Class TaxonomyAntibioticResistance
    ' 
    '     Properties: AccessionID, antibiotic, antibiotic_resistance, ARO, Name
    '                 sp, Taxonomy
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.genomics.foundation.OBO_Foundry.IO.Models
Imports SMRUCC.genomics.foundation.OBO_Foundry.Tree

Public Module AntibioticResistance

    <Extension>
    Public Function TaxonomyProfile(seq As IEnumerable(Of SeqHeader),
                                    ARO As IEnumerable(Of RawTerm),
                                    taxonomy As NcbiTaxonomyTree,
                                    taxonomyID As IEnumerable(Of ncbi_taxomony)) As TaxonomyAntibioticResistance()

        Dim terms As Dictionary(Of String, GenericTree) = Builder.BuildTree(ARO)
        ' 基因对抗生素的抗性信息
        Dim antibiotic_resistance = terms.AntibioticResistanceRelationship
        ' 抗生素的名字等描述信息
        Dim antibiotic = terms.TravelAntibioticTree
        Dim resistances As New List(Of TaxonomyAntibioticResistance)
        ' 用于生成taxonomy的lineage信息
        Dim acc2taxonID = taxonomyID _
            .ToDictionary(Function(tax) tax.Name,
                          Function(tax) tax)
        Dim AROseqs = seq _
            .GroupBy(Function(s) s.ARO) _
            .ToDictionary(Function(a) a.Key,
                          Function(a) a.First)

        For Each rel In antibiotic_resistance _
            .EnumerateTuples _
            .Where(Function(relationship)
                       Return AROseqs.ContainsKey(relationship.name)
                   End Function)

            Dim ARO_id$ = rel.name
            Dim drugs$() = rel.obj
            Dim header As SeqHeader = AROseqs(ARO_id)
            Dim tax As ncbi_taxomony = acc2taxonID(header.species)
            Dim lineage$ = taxonomy.GetAscendantsWithRanksAndNames(Val(tax.Accession), only_std_ranks:=True).BuildBIOM

            For Each drug As String In drugs
                resistances += New TaxonomyAntibioticResistance With {
                    .AccessionID = header.AccessionID,
                    .antibiotic_resistance = drug,
                    .ARO = ARO_id,
                    .Name = header.name,
                    .sp = header.species,
                    .Taxonomy = lineage,
                    .antibiotic = antibiotic(drug).name
                }
            Next
        Next

        Return resistances
    End Function
End Module

Public Class TaxonomyAntibioticResistance
    Public Property AccessionID As String
    Public Property ARO As String
    Public Property sp As String
    Public Property Name As String
    ''' <summary>
    ''' 当前的这个菌株所具有的抗生素抗性
    ''' </summary>
    ''' <returns></returns>
    Public Property antibiotic_resistance As String
    Public Property antibiotic As String
    Public Property Taxonomy As String

    Public Overrides Function ToString() As String
        Return Name
    End Function
End Class
