Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base

<Package("mesh")>
Module mesh

    <ExportAPI("read.mesh_xml")>
    Public Function loadMeshXml(file As String) As DescriptorRecord()
        Return DescriptorRecordSet.ReadTerms(file).ToArray
    End Function
End Module
