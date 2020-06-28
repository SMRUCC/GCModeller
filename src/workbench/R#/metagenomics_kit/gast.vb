Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Metagenome.greengenes
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("gast")>
Module gast

    ''' <summary>
    ''' assign OTU its taxonomy result
    ''' </summary>
    ''' <returns></returns>
    <ExportAPI("OTU.taxonomy")>
    Public Function OTUgreengenesTaxonomy(<RRawVectorArgument>
                                          blastn As Object,
                                          OTUs As list,
                                          taxonomy As list,
                                          Optional min_pct# = 0.97,
                                          Optional env As Environment = Nothing) As pipeline
        Dim queries As pipeline

        If queries.isError Then
            Return queries
        End If

        Dim OTUTable = OTUs.slots.ToDictionary(Function(a) a.Key, Function(a) DirectCast(a.Value, NamedValue(Of Integer)))
        Dim taxonomyTable = taxonomy.slots.ToDictionary(Function(a) a.Key, Function(a) DirectCast(a.Value, otu_taxonomy))

        Return queries _
            .populates(Of Query) _
            .OTUgreengenesTaxonomy(OTUTable, taxonomyTable, min_pct) _
            .DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function
End Module
