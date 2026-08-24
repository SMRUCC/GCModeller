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
Imports SMRUCC.genomics.Analysis.BNLearn.Core
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

<Package("bnlearn")>
<RTypeExport("struct_learn_params", GetType(StructureLearningParams))>
<RTypeExport("knowledges", GetType(Dictionary(Of String, MetabolicPathway)))>
Module bnlearn

    <ExportAPI("bnlearn")>
    <RApiReturn(GetType(BNLearnWorkflow))>
    Public Function bnlearn(exprData As matrix,
                            <RRawVectorArgument(GetType(RegulatoryEdge))>
                            Optional priorNet As Object = Nothing,
                            Optional max_itrs As Integer = 500,
                            Optional env As Environment = Nothing) As Object

        Dim pull As pipeline = pipeline.TryCreatePipeline(Of RegulatoryEdge)(priorNet, env, nullPipe:=True)

        If pull IsNot Nothing AndAlso pull.isError Then
            Return pull.getError
        End If

        Dim workflow As New BNLearnWorkflow() With {
            .ExpressionData = BnIO.ReadGeneExpressionMatrix(exprData),
            .PriorNetwork = BnIO.ReadPriorNetwork(pull?.populates(Of RegulatoryEdge)(env))
        }

        workflow.StructureParams.MaxIterations = max_itrs
        ' 3. 结构学习（MMHC + 白名单先验）
        workflow.LearnStructure()
        ' 4. 参数学习（高斯BN MLE）
        workflow.LearnParameters()

        Return workflow
    End Function

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
    Public Function overexpress(bnlearn As BNLearnWorkflow, <RRawVectorArgument(TypeCodes.string)> geneNames As Object, Optional env As Environment = Nothing) As Object
        Dim result As New List(Of InterventionResult)

        For Each geneName As String In CLRVector.asCharacter(geneNames)
            Call result.Add(bnlearn.OverexpressGene(geneName))
        Next

        Return result.ToArray
    End Function

    <ExportAPI("knockdown")>
    <RApiReturn(GetType(InterventionResult))>
    Public Function knockdownGene(bnlearn As BNLearnWorkflow, <RRawVectorArgument(TypeCodes.string)> geneNames As Object) As Object
        Dim result As New List(Of InterventionResult)

        For Each geneName As String In CLRVector.asCharacter(geneNames)
            Call result.Add(bnlearn.KnockDownGene(geneName))
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

