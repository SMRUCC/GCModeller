Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace GCModeller.FileSystem.KEGG

#If ENABLE_API_EXPORT Then
    <PackageNamespace("GCModeller.Repository.KEGG.Directory", Publisher:="amethyst.asuka@gcmodeller.org")>
    Module Directories
#Else
    Module Directories
#End If
        ''' <summary>
        ''' /KEGG/Reactions/
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <ExportAPI("GetReactions")>
        Public Function GetReactions() As String
            Return FileSystem.GetRepositoryRoot & "/KEGG/Reactions/"
        End Function

        <ExportAPI("GetMetabolites")>
        Public Function GetMetabolites() As String
            Return FileSystem.GetRepositoryRoot & "/KEGG/Metabolites/"
        End Function

        <ExportAPI("GetCompounds")>
        Public Function GetCompounds() As String
            Return Directories.GetMetabolites & "/Compounds/"
        End Function

        <ExportAPI("GetGlycan")>
        Public Function GetGlycan() As String
            Return Directories.GetMetabolites() & "/Glycan/"
        End Function

        <ExportAPI("GetPathways")>
        Public Function GetPathways() As String
            Return FileSystem.GetRepositoryRoot & "/KEGG/Pathways/"
        End Function
    End Module
End Namespace