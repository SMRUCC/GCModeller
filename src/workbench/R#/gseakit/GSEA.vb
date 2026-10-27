#Region "Microsoft.VisualBasic::5edc7532802ac4fc14c977fe6153d99a, R#\gseakit\GSEA.vb"

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

    '   Total Lines: 428
    '    Code Lines: 288 (67.29%)
    ' Comment Lines: 103 (24.07%)
    '    - Xml Docs: 96.12%
    ' 
    '   Blank Lines: 37 (8.64%)
    '     File Size: 19.18 KB


    ' Module GSEA
    ' 
    '     Function: CreateEnrichmentObjects, CreateGOEnrichmentGraph, DrawGOEnrichmentGraph, Enrichment, enrichmentTable
    '               fisher, GOEnrichment, KOBASFormat, ReadEnrichmentTerms, SaveEnrichment
    '               toEnrichmentTerms
    ' 
    '     Sub: Main
    ' 
    ' Enum EnrichmentTableFormat
    ' 
    '     GCModeller, KOBAS
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.GO
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.HTS.GSEA.KnowledgeBase.Metabolism.Metpa
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Data.GeneOntology
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.Data.GeneOntology.obographs
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports dataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports REnv = SMRUCC.Rsharp.Runtime
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' ### The GCModeller GSEA toolkit
''' 
''' ##### Gene set enrichment analysis
''' 
''' Gene Set enrichment analysis (GSEA) (also called functional enrichment 
''' analysis Or pathway enrichment analysis) Is a method To identify classes
''' Of genes Or proteins that are over-represented In a large Set Of genes 
''' Or proteins, And may have an association With disease phenotypes. The
''' method uses statistical approaches To identify significantly enriched 
''' Or depleted groups Of genes. Transcriptomics technologies And proteomics 
''' results often identify thousands Of genes which are used For the analysis.
''' </summary>
<Package("GSEA", Category:=APICategories.ResearchTools)>
<RTypeExport("enrich", GetType(EnrichmentResult))>
Module GSEA

    Sub Main()
        RInternal.Object.Converts.makeDataframe.addHandler(GetType(EnrichmentResult()), AddressOf enrichmentTable)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Private Function enrichmentTable(result As EnrichmentResult(), args As list, env As Environment) As dataframe
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = result _
                .Select(Function(d) d.term) _
                .ToArray
        }

        table.columns("term") = result.Select(Function(d) d.term).ToArray
        table.columns("class") = result.Select(Function(d) d.class).ToArray
        table.columns("category") = result.Select(Function(d) d.category).ToArray

        table.columns("name") = result.Select(Function(d) d.name).ToArray
        table.columns("description") = result.Select(Function(d) d.description).ToArray
        table.columns("cluster") = result.Select(Function(d) d.cluster).ToArray
        table.columns("enriched") = result.Select(Function(d) d.enriched).ToArray
        table.columns("score") = result.Select(Function(d) d.score).ToArray
        table.columns("pvalue") = result.Select(Function(d) d.pvalue).ToArray
        table.columns("FDR") = result.Select(Function(d) d.FDR).ToArray
        table.columns("IDs") = result.Select(Function(d) d.IDs.JoinBy(";")).ToArray

        Return table
    End Function

    ''' <summary>
    ''' Read the enrichment analysis result table from a csv file.
    ''' </summary>
    ''' <param name="file">
    ''' the csv file path of the enrichment analysis result, the file should be written
    ''' by the ``write.enrichment`` function.
    ''' </param>
    ''' <returns>an array of the <see cref="EnrichmentResult"/> objects.</returns>
    <ExportAPI("read.enrichment")>
    Public Function ReadEnrichmentTerms(file As String) As EnrichmentResult()
        Return file.LoadCsv(Of EnrichmentResult)(mute:=True)
    End Function

    ''' <summary>
    ''' ### do GSEA enrichment analysis
    ''' 
    ''' Gene set enrichment analysis (GSEA) (also called functional 
    ''' enrichment analysis or pathway enrichment analysis) is a 
    ''' method to identify classes of genes or proteins that are 
    ''' over-represented in a large set of genes or proteins, and 
    ''' may have an association with disease phenotypes. The method 
    ''' uses statistical approaches to identify significantly enriched 
    ''' or depleted groups of genes. Transcriptomics technologies 
    ''' and proteomics results often identify thousands of genes 
    ''' which are used for the analysis.
    ''' </summary>
    ''' <param name="background">a <see cref="Background"/> model or <see cref="metpa"/> background model.</param>
    ''' <param name="geneSet">a given geneset id list</param>
    ''' <param name="expression">
    ''' the expression value of the given gene set. this parameter could be missing. 
    ''' default use the fisher test enrichment method for the given gene set when 
    ''' the expression vector is missing. GSEA method will be applied for the enrichment 
    ''' analysis if this parameter is not missing.
    ''' </param>
    ''' <param name="cut_size">
    ''' the minimum number of the member genes that a functional cluster should have,
    ''' the clusters with fewer members than this cutoff will be removed from the
    ''' enrichment analysis. Default ``3``.
    ''' </param>
    ''' <param name="outputAll">
    ''' keeps all of the clusters in the result table even if the cluster has no
    ''' intersection with the input gene set? Default ``TRUE``.
    ''' </param>
    ''' <param name="resize">
    ''' the size of the input gene set that is used as the ``n`` of the hypergeometric
    ''' test. Value less than or equals to zero means using the length of the
    ''' ``geneSet`` vector. Default ``-1``.
    ''' </param>
    ''' <param name="showProgress">prints the progress message? Default ``FALSE``.</param>
    ''' <param name="args">
    ''' the additional argument list that may be used, 
    ''' 
    ''' for metpa background model, ``topo`` parameter will be used for 
    ''' specific the network topology impact score source, value of the
    ''' ``topo`` parameter could be ``dgr`` or ``rbc``.
    ''' 
    ''' ``permutations`` parameter will be used for specific the permutation times of
    ''' the GSEA method when the ``expression`` parameter is not missing, default ``1000``.
    ''' </param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>
    ''' an array of the <see cref="EnrichmentResult"/> objects, which is sorted by
    ''' the ``FDR`` value in ascending order.
    ''' </returns>
    ''' <remarks>
    ''' the expression value could be the raw expression mean value, different expression 
    ''' foldchange value for the group comparision, or other related gene set sort scores.
    ''' </remarks>
    <ExportAPI("enrichment")>
    <RApiReturn(GetType(EnrichmentResult))>
    Public Function Enrichment(background As Object,
                               <RRawVectorArgument> geneSet As Object,
                               <RRawVectorArgument>
                               Optional expression As Object = Nothing,
                               Optional cut_size As Integer = 3,
                               Optional outputAll As Boolean = True,
                               Optional resize As Integer = -1,
                               Optional showProgress As Boolean = False,
                               <RListObjectArgument>
                               Optional args As list = Nothing,
                               Optional env As Environment = Nothing) As Object

        Dim enrich As IEnumerable(Of EnrichmentResult)
        Dim inputIdset As String() = CLRVector.asCharacter(geneSet)
        Dim expr As Double() = CLRVector.asNumeric(expression)

        If background Is Nothing Then
            Return RInternal.debug.stop("the required gsea background model could not be nothing!", env)
        End If

        Dim geneExpression As GeneExpressionRank() = Nothing
        Dim permutations As Integer = args.getValue("permutations", env, [default]:=1000)

        If Not expr.IsNullOrEmpty Then
            If expr.Length <> inputIdset.Length Then
                Return RInternal.debug.stop($"the given gene id set vector length({inputIdset.Length}) should be equals to the expression vector size({expr.Length})!", env)
            End If

            geneExpression = inputIdset _
                .Select(Function(id, i)
                            Return New GeneExpressionRank(id, expr(i))
                        End Function) _
                .ToArray
        End If

        If TypeOf background Is Background Then
            If Not expr.IsNullOrEmpty Then
                enrich = GSEACalculate.Enrichment(DirectCast(background, Background), geneExpression, permutations)
            Else
                enrich = DirectCast(background, Background).Enrichment(
                    list:=inputIdset,
                    outputAll:=outputAll,
                    showProgress:=showProgress,
                    resize:=resize,
                    cutSize:=cut_size
                )
            End If
        ElseIf TypeOf background Is metpa Then
            Dim topo As Topologys = [Enum].Parse(
                enumType:=GetType(Topologys),
                value:=args.getValue({"topo", "topology", "impacts", "impact"}, env, [default]:="rbc")
            )

            enrich = DirectCast(background, metpa).Enrichment(
                idset:=inputIdset,
                topo:=topo,
                resize:=resize,
                cutSize:=cut_size,
                outputAll:=outputAll,
                showProgress:=showProgress
            )
        Else
            Return Message.InCompatibleType(GetType(Background), background.GetType, env)
        End If

        Return enrich _
            .FDR _
            .OrderBy(Function(e) e.FDR) _
            .ToArray
    End Function

    ''' <summary>
    ''' Fisher exact test for the enrichment of a single gene set term.
    ''' </summary>
    ''' <param name="list">the gene id list that are observed in the target term.</param>
    ''' <param name="geneSet">all member gene ids of the target term.</param>
    ''' <param name="background">
    ''' the background size information, it could be an integer value to 
    ''' indicates that the total unique size of the enrichment background 
    ''' or a unique id character vector that contains all member 
    ''' information of the background.
    ''' </param>
    ''' <param name="term">the term id (or the term name) that will be written into the result object. Default ``unknown``.</param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>a <see cref="EnrichmentResult"/> object of the given term.</returns>
    <ExportAPI("fisher")>
    Public Function fisher(list As String(),
                           geneSet As String(),
                           <RRawVectorArgument> background As Object,
                           Optional term As String = "unknown",
                           Optional env As Environment = Nothing) As EnrichmentResult

        Dim info As New Cluster With {
            .ID = term,
            .description = term,
            .names = term,
            .members = New BackgroundGene(geneSet.Length - 1) {}
        }
        Dim backgroundSize As Integer

        If REnv.isVector(Of String)(background) Then
            backgroundSize = CLRVector.asCharacter(background).Distinct.Count
        Else
            backgroundSize = CLRVector.asInteger(background).FirstOrDefault
        End If

        Dim enrich As EnrichmentResult = info.calcResult(
            enriched:=list,
            inputSize:=list.Length,
            genes:=backgroundSize,
            outputAll:=True
        )

        Return enrich
    End Function

    ''' <summary>
    ''' Do the GO (Gene Ontology) GSEA enrichment analysis.
    ''' </summary>
    ''' <param name="background">the GSEA background model that contains the GO terms, for example the model created by ``dag.background``.</param>
    ''' <param name="geneSet">the gene id list for the enrichment analysis.</param>
    ''' <param name="go">the go database object can be the raw go obo data or the DAG graph model.</param>
    ''' <param name="showProgress">prints the progress message? Default ``TRUE``.</param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>
    ''' an array of the <see cref="EnrichmentResult"/> objects, which is sorted by
    ''' the ``FDR`` value in ascending order.
    ''' </returns>
    <ExportAPI("enrichment.go")>
    <RApiReturn(GetType(EnrichmentResult))>
    Public Function GOEnrichment(background As Background,
                                 geneSet$(),
                                 go As Object,
                                 Optional showProgress As Boolean = True,
                                 Optional env As Environment = Nothing) As Object
        If go Is Nothing Then
            Return RInternal.debug.stop(New ArgumentNullException("Missing the required GO database!"), env)
        ElseIf TypeOf go Is vbObject Then
            go = DirectCast(go, vbObject).target
        End If

        If go.GetType() Is GetType(GO_OBO) Then
            Return background.Enrichment(
                list:=geneSet,
                goClusters:=New DAG.Graph(DirectCast(go, GO_OBO).AsEnumerable),
                showProgress:=showProgress
            ).ToArray _
             .FDR _
             .OrderBy(Function(e) e.FDR) _
             .ToArray
        ElseIf go.GetType Is GetType(DAG.Graph) Then
            Return background _
                .Enrichment(geneSet, DirectCast(go, DAG.Graph), showProgress:=showProgress) _
                .ToArray _
                .FDR _
                .OrderBy(Function(e) e.FDR) _
                .ToArray
        Else
            Return RInternal.debug.stop(New InvalidProgramException(go.GetType.FullName), env)
        End If
    End Function

    ''' <summary>
    ''' Save the enrichment analysis result as a csv table file.
    ''' </summary>
    ''' <param name="enrichment">
    ''' should be a set of the enrichment analysis result, both of the
    ''' <see cref="EnrichmentResult"/> and the <see cref="EnrichmentTerm"/> array are
    ''' accepted.
    ''' </param>
    ''' <param name="file">the output csv file path.</param>
    ''' <param name="format">
    ''' the output table format of the <see cref="EnrichmentResult"/> data:
    ''' ``GCModeller`` (default) or ``KOBAS``.
    ''' </param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>this function returns ``TRUE`` when the file is saved successfully.</returns>
    <ExportAPI("write.enrichment")>
    <RApiReturn(GetType(Boolean))>
    Public Function SaveEnrichment(<RRawVectorArgument>
                                   enrichment As Object,
                                   file$,
                                   Optional format As EnrichmentTableFormat = EnrichmentTableFormat.GCModeller,
                                   Optional env As Environment = Nothing) As Object

        If enrichment Is Nothing Then
            Return RInternal.debug.stop(New ArgumentNullException(NameOf(enrichment)), env)
        End If

        Dim verbose As Boolean = env.globalEnvironment.options.verbose

        If REnv.isVector(Of EnrichmentResult)(enrichment) Then
            If format = EnrichmentTableFormat.GCModeller Then
                Return DirectCast(enrichment, EnrichmentResult()).SaveTo(file, silent:=Not verbose)
            ElseIf format = EnrichmentTableFormat.KOBAS Then
                Return KOBASFormat(enrichment).SaveTo(file, silent:=Not verbose)
            Else
                Return RInternal.debug.stop(New NotImplementedException(format), env)
            End If
        ElseIf REnv.isVector(Of EnrichmentTerm)(enrichment) Then
            Return DirectCast(enrichment, EnrichmentTerm()).SaveTo(file, silent:=Not verbose)
        Else
            Return RInternal.debug.stop(New InvalidProgramException(enrichment.GetType.FullName), env)
        End If
    End Function

    ''' <summary>
    ''' Convert GSEA enrichment result from GCModeller output format to KOBAS output format
    ''' </summary>
    ''' <param name="enrichment">the GCModeller enrichment analysis result set.</param>
    ''' <param name="database">the database label that will be written into the ``Database`` column of the KOBAS table. Default ``n/a``.</param>
    ''' <returns>an array of the <see cref="EnrichmentTerm"/> objects in the KOBAS table format.</returns>
    <ExportAPI("as.KOBAS_terms")>
    Public Function KOBASFormat(enrichment As EnrichmentResult(), Optional database$ = "n/a") As EnrichmentTerm()
        Return enrichment.Converts(database).ToArray
    End Function

    ''' <summary>
    ''' Create the network graph data of the GO enrichment result for the Cytoscape.
    ''' </summary>
    ''' <param name="go_enrichment">the GO enrichment analysis result set.</param>
    ''' <param name="go">the GO obo database that is used for creating the DAG graph.</param>
    ''' <returns>the network node and edge tables of the GO enrichment DAG graph.</returns>
    <ExportAPI("enrichment.go_dag")>
    Public Function CreateGOEnrichmentGraph(go_enrichment As EnrichmentResult(), go As GO_OBO) As NetworkTables
        Dim terms As NamedValue(Of Double)() = go_enrichment _
            .Select(Function(term)
                        Return New NamedValue(Of Double) With {
                            .Name = term.term,
                            .Value = -Math.Log10(term.pvalue)
                        }
                    End Function) _
            .ToArray
        Dim dag As NetworkGraph = go.CreateGraph(terms:=terms)
        Dim tables As NetworkTables = dag.DAGasTabular

        Return tables
    End Function

    ''' <summary>
    ''' Draw the GO enrichment DAG graph as a raster image.
    ''' </summary>
    ''' <param name="go_enrichment">the GO enrichment analysis result set.</param>
    ''' <param name="go">the GO obo database that is used for creating the DAG graph.</param>
    ''' <returns>the raster image data of the GO enrichment DAG graph.</returns>
    <ExportAPI("enrichment.draw.go_dag")>
    Public Function DrawGOEnrichmentGraph(go_enrichment As EnrichmentResult(), go As GO_OBO) As GraphicsData
        Dim terms As NamedValue(Of Double)() = go_enrichment _
            .Select(Function(term)
                        Return New NamedValue(Of Double) With {
                            .Name = term.term,
                            .Value = -Math.Log10(term.pvalue)
                        }
                    End Function) _
            .ToArray
        Dim dag As NetworkGraph = go.CreateGraph(terms:=terms)
        Dim image As GraphicsData = EnrichmentVisualize.DrawGraph(dag)

        Return image
    End Function

    ''' <summary>
    ''' Create the GCModeller enrichment analysis result objects from the raw data vectors.
    ''' </summary>
    ''' <param name="term">the term id vector.</param>
    ''' <param name="name">the term name vector, its length should be equals to the ``term`` vector.</param>
    ''' <param name="pvalue">the p-value vector of each term.</param>
    ''' <param name="geneIDs">
    ''' a named list of the member gene id vector of each term, the name of the list slot
    ''' should be the term id. Default ``NULL`` means no member gene id.
    ''' </param>
    ''' <param name="desc">the description vector of each term. Default ``NULL``.</param>
    ''' <param name="score">the enrichment score vector of each term. Default ``NULL``.</param>
    ''' <param name="fdr">the adjusted p-value (FDR) vector of each term. Default ``NULL``.</param>
    ''' <param name="cluster">the cluster size vector of each term. Default ``NULL``.</param>
    ''' <param name="enriched">the enriched gene id vector joined by ``;`` of each term. Default ``NULL``.</param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>an array of the <see cref="EnrichmentResult"/> objects.</returns>
    <ExportAPI("cast_enrichs")>
    Public Function CreateEnrichmentObjects(term As String(),
                                            name As String(),
                                            pvalue As Double(),
                                            Optional geneIDs As list = Nothing,
                                            Optional desc As String() = Nothing,
                                            Optional score As Double() = Nothing,
                                            Optional fdr As Double() = Nothing,
                                            Optional cluster As Integer() = Nothing,
                                            Optional enriched As String() = Nothing,
                                            Optional env As Environment = Nothing) As EnrichmentResult()
        If geneIDs Is Nothing Then
            geneIDs = list.empty
        End If

        Return term _
            .Select(Function(id, i)
                        Return New EnrichmentResult With {
                            .term = id,
                            .name = name(i),
                            .pvalue = pvalue(i),
                            .IDs = geneIDs.getValue(Of String())(id, env),
                            .FDR = fdr.ElementAtOrDefault(i),
                            .score = score.ElementAtOrDefault(i),
                            .description = desc.ElementAtOrDefault(i),
                            .cluster = cluster.ElementAtOrDefault(i),
                            .enriched = enriched.ElementAtOrDefault(i)
                        }
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' Cast a ``dataframe`` object into the CLR enrichment result objects.
    ''' </summary>
    ''' <param name="x">
    ''' the dataframe object that contains the enrichment result data, the row names
    ''' should be the term id and the required columns are ``name``, ``pvalue`` and
    ''' ``geneIDs`` (the member gene ids joined by ``;``).
    ''' </param>
    ''' <param name="env">the R# runtime environment.</param>
    ''' <returns>an array of the <see cref="EnrichmentResult"/> objects.</returns>
    <ExportAPI("to_enrichment_terms")>
    Public Function toEnrichmentTerms(x As dataframe, Optional env As Environment = Nothing) As EnrichmentResult()
        Dim terms As String() = x.getRowNames
        Dim name As String() = CLRVector.asCharacter(x.getColumnVector("name"))
        Dim pvalue As Double() = CLRVector.asNumeric(x.getColumnVector("pvalue"))
        Dim geneIDs As String() = CLRVector.asCharacter(x.getColumnVector("geneIDs"))
        Dim geneList As New list With {
            .slots = terms _
                .Select(Function(id, i)
                            Return (id, geneIDs(i).Split(";"c))
                        End Function) _
                .ToDictionary(Function(a) a.id,
                              Function(a)
                                  Return CObj(a.Item2)
                              End Function)
        }

        Return GSEA.CreateEnrichmentObjects(terms, name, pvalue, geneList, env:=env)
    End Function
End Module

Public Enum EnrichmentTableFormat
    GCModeller
    KOBAS
End Enum
