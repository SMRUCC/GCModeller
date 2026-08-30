#Region "Microsoft.VisualBasic::0a84bf1f85437d0009dd4da6da08e55d, R#\phenotype_kit\bnlearn.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 145
    '    Code Lines: 111 (76.55%)
    ' Comment Lines: 8 (5.52%)
    '    - Xml Docs: 75.00%
    ' 
    '   Blank Lines: 26 (17.93%)
    '     File Size: 6.11 KB


    ' Module bnlearn
    ' 
    '     Function: bnlearn, knockdownGene, KnockoutGene, make_exports, overexpress
    '               prior_network, save_model
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.BNLearn.Core.WGCNADBN
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.BNLearn.IO
Imports SMRUCC.genomics.Analysis.BNLearn.StructureLearning
Imports SMRUCC.genomics.MetabolicModel
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix

''' <summary>
''' Bayesian network learning and the in silico gene perturbation toolkit
''' </summary>
''' 
''' <remarks>
''' This R# package module provides the toolkit for learn the gene regulatory 
''' bayesian network from the gene expression data, and then run the in silico 
''' gene perturbation experiment based on the learned network model:
''' 
''' + ``prior_network`` and ``as.prior_net``: create the prior knowledge 
'''   regulatory network(TF -&gt; target gene) which is used as the whitelist of 
'''   the network structure learning;
''' + ``bnlearn``: learn the network structure(MMHC algorithm with the whitelist 
'''   prior) and the network parameters(Gaussian bayesian network MLE);
''' + ``knockouts``, ``overexpress`` and ``knockdown``: run the in silico gene 
'''   perturbation experiment on the learned network model;
''' + ``make_exports``: export the perturbation experiment result as a set of the 
'''   csv table files;
''' + ``save_model``: save the learned bayesian network model as the tsv table 
'''   files.
''' </remarks>
<Package("bnlearn")>
<RTypeExport("struct_learn_params", GetType(StructureLearningParams))>
<RTypeExport("knowledges", GetType(Dictionary(Of String, MetabolicPathway)))>
<RTypeExport("subnet", GetType(WGCNASubnetworkPipeline))>
Module bnlearn

    ''' <summary>
    ''' learn the gene regulatory bayesian network from the gene expression data
    ''' </summary>
    ''' <param name="exprData">
    ''' the gene expression matrix object, could be load from csv file via ``geneExpression::load.expr`` api
    ''' </param>
    ''' <param name="priorNet">
    ''' a collection of the prior knowledge regulatory edge data 
    ''' (<see cref="RegulatoryEdge"/>), which is used as the whitelist of the 
    ''' network structure learning: only the regulation relation that is described 
    ''' in this prior network will be considered in the structure learning.
    ''' 
    ''' this parameter is optional, the network structure will be learned from the 
    ''' expression data alone if the prior network is not provided.
    ''' </param>
    ''' <param name="max_itrs">
    ''' the max iteration numbers of the network structure learning, by default is 
    ''' 500.
    ''' </param>
    ''' <param name="strict">
    ''' the strict option of the in silico perturbation experiment: if this 
    ''' parameter is TRUE, then an error will be thrown when the target gene of the 
    ''' perturbation is missing from the learned network; if this parameter is 
    ''' FALSE, then a warning message will be printed and the wildtype expression 
    ''' data will be returned as the perturbation result with the ``Undefined`` flag 
    ''' marked as TRUE.
    ''' 
    ''' if this parameter is not specified, then the strict option of the R# 
    ''' runtime environment will be used.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a <see cref="BNLearnWorkflow"/> object that contains the learned bayesian 
    ''' network model, which implements the 
    ''' <see cref="InsilicoPerturbationExperiment"/> interface, so that it can be 
    ''' used by the ``knockouts``, ``overexpress`` and ``knockdown`` api for run 
    ''' the in silico gene perturbation experiment;
    ''' 
    ''' this function returns a R# error message object if the given prior network 
    ''' data can not be cast to a collection of the 
    ''' <see cref="RegulatoryEdge"/> data.
    ''' </returns>
    ''' 
    ''' <remarks>
    ''' this function runs the network learning in two steps:
    ''' 
    ''' 1. the structure learning: the MMHC algorithm with the whitelist prior 
    '''    network;
    ''' 2. the parameter learning: the maximum likelihood estimation(MLE) of the 
    '''    Gaussian bayesian network.
    ''' </remarks>
    <ExportAPI("bnlearn")>
    <RApiReturn(GetType(BNLearnWorkflow))>
    Public Function bnlearn(exprData As matrix,
                            <RRawVectorArgument(GetType(RegulatoryEdge))>
                            Optional priorNet As Object = Nothing,
                            Optional max_itrs As Integer = 500,
                            Optional strict As Boolean? = Nothing,
                            Optional env As Environment = Nothing) As Object

        Dim pull As PipeIterator(Of RegulatoryEdge) = pipeline.Stream(Of RegulatoryEdge)(priorNet, env, nullPipe:=True)

        If pull IsNot Nothing AndAlso pull.isError Then
            Return pull.getError
        End If

        Dim workflow As New BNLearnWorkflow() With {
            .ExpressionData = BnIO.ReadGeneExpressionMatrix(exprData),
            .PriorNetwork = BnIO.ReadPriorNetwork(pull),
            .Strict = env.strictOption(opt:=strict)
        }

        workflow.StructureParams.MaxIterations = max_itrs
        ' 3. 结构学习（MMHC + 白名单先验）
        workflow.LearnStructure()
        ' 4. 参数学习（高斯BN MLE）
        workflow.LearnParameters()

        Return workflow
    End Function

    ''' <summary>
    ''' build prior network object based on a given vector of the knowledge network edges data
    ''' </summary>
    ''' <param name="priorNet">
    ''' a collection of the regulatory edge data, which can be a vector of the 
    ''' <see cref="RegulatoryEdge"/> object, the output of the ``prior_network`` 
    ''' api, or a pipeline object that produces a set of the 
    ''' <see cref="RegulatoryEdge"/> data.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a <see cref="PriorNetwork"/> object that contains all of the given 
    ''' regulatory edges, the TF name set and the target gene name set;
    ''' 
    ''' this function returns a R# error message object if the given data can not 
    ''' be cast to a collection of the <see cref="RegulatoryEdge"/> data.
    ''' </returns>
    <ExportAPI("as.prior_net")>
    <RApiReturn(GetType(PriorNetwork))>
    Public Function buildNetwork(<RRawVectorArgument(GetType(RegulatoryEdge))> priorNet As Object, Optional env As Environment = Nothing) As Object
        Dim pull As PipeIterator(Of RegulatoryEdge) = pipeline.Stream(Of RegulatoryEdge)(priorNet, env, nullPipe:=True)

        If pull IsNot Nothing AndAlso pull.isError Then
            Return pull.getError
        Else
            Return BnIO.ReadPriorNetwork(pull)
        End If
    End Function

    ''' <summary>
    ''' create prior knowledge netwoek edges from the given vector data
    ''' </summary>
    ''' <param name="TF"></param>
    ''' <param name="target_gene"></param>
    ''' <param name="regulation_type"></param>
    ''' <param name="confidence"></param>
    ''' <param name="evidence"></param>
    ''' <returns></returns>
    <ExportAPI("prior_network")>
    <RApiReturn(GetType(RegulatoryEdge))>
    Public Function prior_network(<RRawVectorArgument(TypeCodes.string)> TF As Object,
                                  <RRawVectorArgument(TypeCodes.string)> target_gene As Object,
                                  <RRawVectorArgument(TypeCodes.string)> regulation_type As Object,
                                  <RRawVectorArgument(TypeCodes.string)> confidence As Object,
                                  <RRawVectorArgument(TypeCodes.string)> evidence As Object) As Object

        Dim tfs As String() = CLRVector.asCharacter(TF)
        Dim targets As String() = CLRVector.asCharacter(target_gene)
        Dim reg_types As String() = CLRVector.asCharacter(regulation_type)
        Dim confs As Double() = CLRVector.asNumeric(confidence)
        Dim evidences As String() = CLRVector.asCharacter(evidence)
        Dim priorNet As RegulatoryEdge() = New RegulatoryEdge(tfs.Length - 1) {}

        For i As Integer = 0 To tfs.Length - 1
            priorNet(i) = New RegulatoryEdge With {
                .Confidence = confs(i),
                .Evidence = evidences(i),
                .RegulationType = reg_types(i),
                .TargetGene = targets(i),
                .TF = tfs(i)
            }
        Next

        Return priorNet
    End Function

    ' permutation

    <ExportAPI("knockouts")>
    <RApiReturn(GetType(InterventionResult))>
    Public Function KnockoutGene(bnlearn As InsilicoPerturbationExperiment, <RRawVectorArgument(TypeCodes.string)> geneNames As Object) As Object
        Dim result As New List(Of InterventionResult)

        For Each geneName As String In CLRVector.asCharacter(geneNames)
            Call result.Add(bnlearn.KnockoutGene(geneName))
        Next

        Return result.ToArray
    End Function

    <ExportAPI("overexpress")>
    <RApiReturn(GetType(InterventionResult))>
    Public Function overexpress(bnlearn As InsilicoPerturbationExperiment, <RRawVectorArgument(TypeCodes.string)> geneNames As Object, Optional env As Environment = Nothing) As Object
        Dim result As New List(Of InterventionResult)

        For Each geneName As String In CLRVector.asCharacter(geneNames)
            Call result.Add(bnlearn.OverexpressGene(geneName))
        Next

        Return result.ToArray
    End Function

    <ExportAPI("knockdown")>
    <RApiReturn(GetType(InterventionResult))>
    Public Function knockdownGene(bnlearn As InsilicoPerturbationExperiment, <RRawVectorArgument(TypeCodes.string)> geneNames As Object) As Object
        Dim result As New List(Of InterventionResult)

        For Each geneName As String In CLRVector.asCharacter(geneNames)
            Call result.Add(bnlearn.KnockDownGene(geneName))
        Next

        Return result.ToArray
    End Function

    ''' <summary>
    ''' export the virtual permutation result as csv table files
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="dir"></param>
    ''' <param name="pathway_info"></param>
    ''' <param name="top_n"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("make_exports")>
    Public Function make_exports(<RRawVectorArgument(GetType(InterventionResult))> results As Object, dir As String,
                                 <RRawVectorArgument>
                                 Optional pathway_info As list = Nothing,
                                 Optional top_n As Integer = 50,
                                 Optional env As Environment = Nothing) As Object

        Dim pull As pipeline = pipeline.TryCreatePipeline(Of InterventionResult)(results, env)
        Dim pathways As Dictionary(Of String, MetabolicPathway) = Nothing

        If pull.isError Then
            Return pull.getError
        End If
        If pathway_info IsNot Nothing Then
            pathways = pathway_info.AsGeneric(Of MetabolicPathway)(env)
        End If

        Call New InterventionComparisonExporter(pull.populates(Of InterventionResult)(env)).ExportAll(dir, pathways, topN:=top_n)

        Return True
    End Function

    ''' <summary>
    ''' save bnlearn model
    ''' </summary>
    ''' <param name="bnlearn"></param>
    ''' <param name="dir"></param>
    ''' <returns></returns>
    <ExportAPI("save_model")>
    Public Function save_model(bnlearn As BNLearnWorkflow, dir As String) As Object
        Call bnlearn.SaveResults(dir)
        Return True
    End Function
End Module

