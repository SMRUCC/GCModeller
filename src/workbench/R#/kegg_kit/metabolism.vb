
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
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
End Module
