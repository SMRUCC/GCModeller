Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.genomics.Data.Functional
Imports SMRUCC.genomics.foundation.OBO_Foundry

Public Module AntibioticResistance

    <Extension>
    Public Function TaxonomyProfile(seq As IEnumerable(Of SeqHeader),
                                    ARO As IEnumerable(Of RawTerm),
                                    taxonomy As NcbiTaxonomyTree,
                                    taxonomyID As IEnumerable(Of ncbi_taxomony)) As TaxonomyAntibioticResistance()

        Dim terms As Dictionary(Of String, GenericTree) = GenericTree.BuildTree(ARO)
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
            .GroupBy(Function(s)
                         Return s.AccessionID.Split("."c).First
                     End Function) _
            .ToDictionary(Function(a) a.Key,
                          Function(a) a.First)

        For Each rel In antibiotic_resistance.EnumerateTuples
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
                    .Taxonomy = lineage
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
    Public Property Taxonomy As String
    ''' <summary>
    ''' 当前的这个菌株所具有的抗生素抗性
    ''' </summary>
    ''' <returns></returns>
    Public Property antibiotic_resistance As String

    Public Overrides Function ToString() As String
        Return Name
    End Function
End Class
