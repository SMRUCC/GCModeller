Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Linq

Namespace Specifics.MetaCyc

    Public Enum FluxProperties
        BIOCYC
        ECNumber
        SUBSYSTEM
        GENE_ASSOCIATION
        ConfidenceLevel
    End Enum

    Public Enum SpeciesProperties
        BIOCYC
        INCHI
        CHEBI
        HMDB
        CHEMSPIDER
        PUBCHEM
        DRUGBANK
        CAS
        METABOLIGHTS
        KEGG
        FORMULA
        CHARGE
    End Enum

    Public Module PropertyParser

        Public ReadOnly Property SpeciesKeyMaps As IReadOnlyDictionary(Of SpeciesProperties, String) =
            New Dictionary(Of SpeciesProperties, String) From {
 _
                {SpeciesProperties.BIOCYC, "BIOCYC"},
                {SpeciesProperties.CAS, "CAS"},
                {SpeciesProperties.CHARGE, "CHARGE"},
                {SpeciesProperties.CHEBI, "CHEBI"},
                {SpeciesProperties.CHEMSPIDER, "CHEMSPIDER"},
                {SpeciesProperties.DRUGBANK, "DRUGBANK"},
                {SpeciesProperties.FORMULA, "FORMULA"},
                {SpeciesProperties.HMDB, "HMDB"},
                {SpeciesProperties.INCHI, "INCHI"},
                {SpeciesProperties.KEGG, "KEGG"},
                {SpeciesProperties.METABOLIGHTS, "METABOLIGHTS"},
                {SpeciesProperties.PUBCHEM, "PUBCHEM"}
        }

        Public ReadOnly Property FluxKeyMaps As IReadOnlyDictionary(Of FluxProperties, String) =
            New Dictionary(Of FluxProperties, String) From {
 _
                {FluxProperties.BIOCYC, "BIOCYC"},
                {FluxProperties.ECNumber, "EC Number"},
                {FluxProperties.SUBSYSTEM, "SUBSYSTEM"},
                {FluxProperties.GENE_ASSOCIATION, "GENE_ASSOCIATION"},
                {FluxProperties.ConfidenceLevel, "Confidence level"}
        }

        ''' <summary>
        ''' Example: 
        ''' GENE_ASSOCIATION: (XC_3424) or (XC_4096)
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Function GetGenes(value As String) As String()
            Dim ms As String() = Regex.Matches(value, "\(.+?\)").ToArray
            ms = ms.ToArray(Function(s) Mid(s, 2, s.Length - 2))
            Return ms
        End Function

        ''' <summary>
        ''' Example:
        ''' EC Number: 2.3.1.85/2.3.1.86/4.2.1.59
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Function GetEcList(value As String) As String()
            Return value.Split("/"c)
        End Function
    End Module
End Namespace