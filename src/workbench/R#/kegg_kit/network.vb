Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

<Package("network")>
Module network

    <ExportAPI("fromCompounds")>
    Public Function fromCompoundId(compoundsId As String(), graph As Reaction(), Optional compounds As Compound() = Nothing) As NetworkGraph

    End Function
End Module
