Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Data.Functional
Imports SMRUCC.genomics.foundation.OBO_Foundry

Public Module AntibioticResistance

    <Extension>
    Public Function TaxonomyProfile(seq As IEnumerable(Of SeqHeader), ARO As IEnumerable(Of RawTerm)) As TaxonomyAntibioticResistance()
        Dim terms As Dictionary(Of String, GenericTree) = GenericTree.BuildTree(ARO)
        Dim antibiotic_resistance = terms.AntibioticResistanceRelationship

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
End Class
