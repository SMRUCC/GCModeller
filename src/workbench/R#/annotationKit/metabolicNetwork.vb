Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.RetroPath.Model
Imports SMRUCC.genomics.MetabolicModel
Imports SMRUCC.genomics.Model.Metabolic
Imports SMRUCC.genomics.Model.Metabolic.RouterAdapter
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("metabolic_network")>
<RTypeExport("metabolic_network", GetType(RouterAdapter.MetabolicNetwork))>
Public Module metabolicNetwork

    <ExportAPI("find_pathway")>
    <RApiReturn(GetType(PathReport))>
    Public Function find_pathway(router As IRouter, target As String) As Object
        Return router.FindPathway(target)
    End Function

    <ExportAPI("router")>
    <RApiReturn(GetType(MetabolicAdapter))>
    Public Function network_router(<RRawVectorArgument(GetType(MetabolicCompound))> compounds As Object,
                                   <RRawVectorArgument(GetType(MetabolicReaction))> reactions As Object,
                                   Optional env As Environment = Nothing) As Object

        Dim meta As PipeIterator(Of MetabolicCompound) = pipeline.Stream(Of MetabolicCompound)(compounds, env)
        Dim rxns As PipeIterator(Of MetabolicReaction) = pipeline.Stream(Of MetabolicReaction)(reactions, env)

        If meta.isError Then
            Return meta.getError
        ElseIf rxns.isError Then
            Return rxns.getError
        End If

        Dim router As New MetabolicAdapter(meta, rxns)
        Return router
    End Function
End Module

