
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports RDotNET.Extensions.GCModeller
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.Data

''' <summary>
''' The kegg metabolism model toolkit
''' </summary>
<Package("kegg.metabolism", Category:=APICategories.ResearchTools)>
Module metabolism

    ''' <summary>
    ''' Get compounds kegg id which is related to the given KO id list
    ''' </summary>
    ''' <param name="enzymes">KO id list</param>
    ''' <param name="reactions"></param>
    ''' <returns></returns>
    <ExportAPI("related.compounds")>
    Public Function GetAllCompounds(enzymes$(), reactions As ReactionRepository) As String()
        Return reactions _
            .GetByKOMatch(KO:=enzymes) _
            .Select(Function(r) r.GetSubstrateCompounds) _
            .IteratesALL _
            .Distinct _
            .ToArray
    End Function

    <ExportAPI("compound.origins")>
    Public Function CreateCompoundOriginModel(repo As String, Optional compoundNames As Dictionary(Of String, String) = Nothing) As OrganismCompounds
        If compoundNames Is Nothing Then
            Return OrganismCompounds.LoadData(repo)
        Else
            Return OrganismCompounds.LoadData(repo, compoundNames)
        End If
    End Function

    ''' <summary>
    ''' Removes invalid kegg compound id
    ''' </summary>
    ''' <param name="identified"></param>
    ''' <returns></returns>
    <ExportAPI("filter.invalid_keggIds")>
    Public Function filterInvalidCompoundIds(identified As String()) As String()
        Return identified _
            .Where(Function(id)
                       Return id.IsPattern(KEGGCompoundIDPatterns)
                   End Function) _
            .ToArray
    End Function
End Module
