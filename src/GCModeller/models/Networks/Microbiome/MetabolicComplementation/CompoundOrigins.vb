Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy

''' <summary>
''' Organism KEGG CompoundOrigins profiles dataset
''' 
''' ID property of the dataset is the ncbi taxonomy id or kegg organism id 
''' </summary>
Public Class CompoundOrigins : Inherits OTUTable

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="organism">The directory path which contains the kegg organism information files.</param>
    ''' <returns></returns>
    Public Shared Function CreateEmptyCompoundsProfile(taxonomy As NcbiTaxonomyTree, organism As String) As CompoundOrigins
        Dim info = organism.DoCall(AddressOf getIndexJson).LoadJSON(Of OrganismInfo)
        Dim compoundID As String() = $"{organism}/kegg_compounds.txt".ReadAllLines
        Dim empty As New CompoundOrigins With {
            .ID = info.code
        }
        Dim taxonomyEntry As String = Strings.Trim(info.Taxonomy)

        If taxonomyEntry.IsPattern("TAX[:]\s*\d+") Then
            ' is ncbi taxonomy id
            Dim taxid As String = taxonomyEntry.Split(":"c).Last.Trim
            Dim lineage As Metagenomics.Taxonomy = taxonomy _
                .GetAscendantsWithRanksAndNames(Integer.Parse(taxid), only_std_ranks:=True) _
                .DoCall(Function(nodes)
                            Return New Metagenomics.Taxonomy(nodes)
                        End Function)

            empty.taxonomy = lineage
        End If

        For Each id As String In compoundID
            empty(id) = 1
        Next

        Return empty
    End Function

    Private Shared Function getIndexJson(repo As String) As String
        If $"{repo}/kegg.json".FileExists Then
            Return $"{repo}/kegg.json"
        Else
            Return $"{repo}/{repo.BaseName}.json"
        End If
    End Function
End Class
