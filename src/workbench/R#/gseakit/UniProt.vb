Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.genomics.Assembly.Uniprot.XML

''' <summary>
''' The uniprot background model handler
''' </summary>
''' 
<Package("UniProt")>
Module UniProt

    <ExportAPI("subcellular_location")>
    <RApiReturn(GetType(Background))>
    Public Function SubcellularLocation(<RRawVectorArgument> uniprot As Object,
                                        Optional env As Environment = Nothing) As Object

        Dim base = pipeline.TryCreatePipeline(Of entry)(uniprot, env)

        If base.isError Then
            Return base.getError
        Else
            Return base.populates(Of entry)(env).SubcellularLocation
        End If
    End Function

    <ExportAPI("uniprot_keywords")>
    <RApiReturn(GetType(Background))>
    Public Function uniprotKeywords(<RRawVectorArgument> uniprot As Object,
                                    Optional env As Environment = Nothing) As Object

        Dim base = pipeline.TryCreatePipeline(Of entry)(uniprot, env)

        If base.isError Then
            Return base.getError
        Else
            Return base.populates(Of entry)(env).UniprotKeywordsModel
        End If
    End Function
End Module
