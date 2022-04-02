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

    <ExportAPI("formula")>
    Public Function formulaString(meta As compounds) As String
        If meta.chemicalFormula.IsNullOrEmpty Then
            Return ""
        Else
            Return meta.chemicalFormula _
                .Select(Function(d)
                            Return d.Trim(" "c, "("c, ")"c).Replace(" ", "")
                        End Function) _
                .JoinBy("")
        End If
    End Function
End Module
