Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI.MeSH
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI.MeSH.Tree
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("mesh")>
<RTypeExport("ncbi_mesh", GetType(MeSH.Tree.Term))>
Module meshTools

    Sub Main()

    End Sub

    <ExportAPI("read.mesh_xml")>
    Public Function loadMeshXml(file As String) As DescriptorRecord()
        Return DescriptorRecordSet.ReadTerms(file).ToArray
    End Function

    <ExportAPI("mesh_category")>
    Public Function mesh_category(term As MeSH.Tree.Term) As MeshCategory()
        Return term.category.ToArray
    End Function

    <ExportAPI("read.mesh_tree")>
    <RApiReturn(GetType(MeSH.Tree.Term))>
    Public Function loadMeshTree(<RRawVectorArgument> file As Object,
                                 Optional as_tree As Boolean = True,
                                 Optional env As Environment = Nothing) As Object

        Dim s = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

        If s Like GetType(Message) Then
            Return s.TryCast(Of Message)
        End If

        Using buf As Stream = s.TryCast(Of Stream)
            If as_tree Then
                Return MeSH.Tree.ParseTree(buf)
            Else
                Return MeSH.Tree.ReadTerms(buf).ToArray
            End If
        End Using
    End Function
End Module
