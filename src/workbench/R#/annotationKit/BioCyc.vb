Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.BioCyc

<Package("BioCyc")>
Public Module BioCycRepository

    ''' <summary>
    ''' open a directory path as the biocyc workspace
    ''' </summary>
    ''' <param name="repo"></param>
    ''' <returns></returns>
    <ExportAPI("open.biocyc")>
    Public Function openBioCyc(repo As String) As Workspace
        Return New Workspace(repo)
    End Function

    <ExportAPI("getCompounds")>
    Public Function getCompounds(repo As Workspace) As compounds()
        Return repo.compounds.features.ToArray
    End Function
End Module
