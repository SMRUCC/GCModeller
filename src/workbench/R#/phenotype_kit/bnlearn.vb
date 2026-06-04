Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention.InterventionComparisonExporter
Imports SMRUCC.genomics.Analysis.BNLearn.IO
Imports SMRUCC.genomics.Analysis.BNLearn.StructureLearning
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

<Package("bnlearn")>
<RTypeExport("struct_learn_params", GetType(StructureLearningParams))>
Module bnlearn

    <ExportAPI("bnlearn")>
    <RApiReturn(GetType(BNLearnWorkflow))>
    Public Function bnlearn(exprData As Matrix, <RRawVectorArgument> priorNet As Object,
                            Optional max_itrs As Integer = 500,
                            Optional env As Environment = Nothing) As Object
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of RegulatoryEdge)(priorNet, env)

        If pull.isError Then
            Return pull.getError
        End If

        Dim workflow As New BNLearnWorkflow() With {
            .ExpressionData = BnIO.ReadGeneExpressionMatrix(exprData),
            .PriorNetwork = BnIO.ReadPriorNetwork(pull.populates(Of RegulatoryEdge)(env))
        }

        workflow.StructureParams.MaxIterations = max_itrs
        ' 3. 结构学习（MMHC + 白名单先验）
        workflow.LearnStructure()
        ' 4. 参数学习（高斯BN MLE）
        workflow.LearnParameters()

        Return workflow
    End Function

    <ExportAPI("knockouts")>
    <RApiReturn(GetType(InterventionResult))>
    Public Function KnockoutGene(bnlearn As BNLearnWorkflow, <RRawVectorArgument(TypeCodes.string)> geneNames As Object) As Object
        Dim result As New List(Of InterventionResult)

        For Each geneName As String In CLRVector.asCharacter(geneNames)
            Call result.Add(bnlearn.KnockoutGene(geneName))
        Next

        Return result.ToArray
    End Function

    <ExportAPI("overexpress")>
    <RApiReturn(GetType(InterventionResult))>
    Public Function overexpress(bnlearn As BNLearnWorkflow, <RRawVectorArgument(TypeCodes.string)> geneNames As Object) As Object
        Dim result As New List(Of InterventionResult)

        For Each geneName As String In CLRVector.asCharacter(geneNames)
            Call result.Add(bnlearn.OverexpressGene(geneName))
        Next

        Return result.ToArray
    End Function

    <ExportAPI("make_exports")>
    Public Function make_exports(<RRawVectorArgument> results As Object, dir As String,
                                 <RRawVectorArgument>
                                 Optional pathway_info As list = Nothing,
                                 Optional top_n As Integer = 50,
                                 Optional env As Environment = Nothing) As Object

        Dim pull As pipeline = pipeline.TryCreatePipeline(Of InterventionResult)(results, env)
        Dim pathways As Dictionary(Of String, PathwayInfo) = Nothing

        If pull.isError Then
            Return pull.getError
        End If
        If pathway_info IsNot Nothing Then
            pathways = pathway_info.AsGeneric(Of PathwayInfo)(env)
        End If

        Call New InterventionComparisonExporter(pull.populates(Of InterventionResult)(env)).ExportAll(dir, pathways, topN:=top_n)

        Return True
    End Function

End Module
