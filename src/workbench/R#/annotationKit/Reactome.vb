Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.Reactome

<Package("Reactome")>
Module ReactomeTools

    Sub Main()

    End Sub

    ''' <summary>
    ''' get reactome pathway list
    ''' </summary>
    ''' <param name="taxname"></param>
    ''' <returns></returns>
    <ExportAPI("pathway_list")>
    Public Function loadPathwayList(Optional taxname As String = Nothing) As Hierarchy
        Return Hierarchy.LoadInternal(taxname)
    End Function

    <ExportAPI("jsonTree")>
    Public Function pathwayjsonTree(tree As Hierarchy) As String
        Return Hierarchy.TreeJSON(tree)
    End Function

End Module
