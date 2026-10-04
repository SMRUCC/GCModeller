#Region "Microsoft.VisualBasic::b4fd248f4074842a588493329c7408a8, R#\TRNtoolkit\WGCNA.vb"

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

'   Total Lines: 139
'    Code Lines: 103 (74.10%)
' Comment Lines: 15 (10.79%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 21 (15.11%)
'     File Size: 5.71 KB


' Module WGCNA
' 
'     Function: CorrelationNetwork, (+2 Overloads) expr_cor, FilterRegulation
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Matrix
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.CellPhenotype
Imports SMRUCC.genomics.Analysis.CellPhenotype.RegulationNetwork
Imports SMRUCC.genomics.Analysis.HTS.WGCNA
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.WGCNA.Network
Imports SMRUCC.genomics.InteractionModel
Imports SMRUCC.genomics.Model.Network.Regulons
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports any = Microsoft.VisualBasic.Scripting
Imports HTSMatrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal
Imports std = System.Math

''' <summary>
''' Workflow for make TRN data model based on the WGCNA co-expression network and TF id list.
''' </summary>
<Package("WGCNA")>
<RTypeExport("GRN_opts", GetType(GRNBuildOptions))>
Module WGCNA

    ''' <summary>
    ''' filter regulation network by WGCNA result weights
    ''' </summary>
    ''' <param name="g"></param>
    ''' <param name="WGCNA"></param>
    ''' <param name="threshold"></param>
    ''' <returns></returns>
    <ExportAPI("shapeTRN")>
    <RApiReturn(GetType(NetworkGraph))>
    Public Function FilterRegulation(g As NetworkGraph, WGCNA As WGCNAWeight, Optional threshold As Double = 0.3) As Object
        Dim w As Double

        ' 20261004 due to the reason of needs RemoveEdge at the for loop
        ' so we use the toarray for avoid the possible
        ' internal collection modification error
        For Each edge As Edge In g.graphEdges.ToArray
            w = WGCNA.GetValue(edge.U.label, edge.V.label)

            If w < threshold Then
                g.RemoveEdge(edge)
            Else
                edge.weight = w
            End If
        Next

        Return g
    End Function

    ''' <summary>
    ''' append protein iteration network based on the WGCNA weights.
    ''' </summary>
    ''' <param name="g"></param>
    ''' <param name="WGCNA"></param>
    ''' <param name="modules"></param>
    ''' <param name="threshold"></param>
    ''' <returns></returns>
    <ExportAPI("interations")>
    <RApiReturn(GetType(NetworkGraph))>
    Public Function CorrelationNetwork(g As NetworkGraph, WGCNA As WGCNAWeight, modules As list, Optional threshold As Double = 0.3) As Object
        For Each conn As Weight In WGCNA.AsEnumerable.Where(Function(cn) cn.weight >= threshold)
            Dim u As Node = g.GetElementByID(conn.fromNode)
            Dim v As Node = g.GetElementByID(conn.toNode)

            If u Is Nothing OrElse v Is Nothing Then
                Continue For
            End If

            Dim edges As Edge() = g.GetEdges(u, v).SafeQuery.ToArray
            Dim data As New EdgeData
            Dim color1 As String = any.ToString(modules.slots(u.label))
            Dim color2 As String = any.ToString(modules.slots(v.label))

            data(NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE) = If(color1 = color2, color1, $"{color1}+{color2}")

            If edges.Length = 0 Then
                g.CreateEdge(u, v, conn.weight, data)
            Else
                For Each link In edges
                    link.weight += conn.weight
                Next
            End If
        Next

        Return g
    End Function

    ''' <summary>
    ''' Get the correlation matrix of the given expression data matrix, and then calculate the correlation value and p-value of the given id1 and id2.
    ''' </summary>
    ''' <param name="expr"></param>
    ''' <param name="id1"></param>
    ''' <param name="id2"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("expr_cor")>
    <RApiReturn(GetType(LazyCorrelationMatrix), GetType(dataframe))>
    Public Function expr_cor(expr As Object,
                             <RRawVectorArgument(TypeCodes.string)> Optional id1 As Object = Nothing,
                             <RRawVectorArgument(TypeCodes.string)> Optional id2 As Object = Nothing,
                             Optional env As Environment = Nothing) As Object

        Dim check_eval As Boolean = id1 IsNot Nothing AndAlso id2 IsNot Nothing

        If check_eval Then
            If Not TypeOf expr Is LazyCorrelationMatrix Then
                Return RInternal.debug.stop("the given data expr object should be a correlation matrix object when id1 and id2 is presentes!", env)
            End If

            Dim cor As LazyCorrelationMatrix = DirectCast(expr, LazyCorrelationMatrix)
            Dim idset1 As GetVectorElement = GetVectorElement.Create(Of String)(id1)
            Dim idset2 As GetVectorElement = GetVectorElement.Create(Of String)(id2)

            If Not GetVectorElement.DoesSizeMatch(idset1, idset2) Then
                Return RInternal.debug.stop($"the dimension size of id1({idset1.size}) is not matched with the dimension size of id2({idset2.size})!", env)
            Else
                Return expr_cor(cor, idset1, idset2)
            End If
        ElseIf TypeOf expr Is HTSMatrix Then
            Return New LazyCorrelationMatrix(DirectCast(expr, HTSMatrix))
        Else
            Return Message.InCompatibleType(GetType(HTSMatrix), expr.GetType, env)
        End If
    End Function

    Private Function expr_cor(cor As LazyCorrelationMatrix, idset1 As GetVectorElement, idset2 As GetVectorElement) As dataframe
        Dim eval As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }
        Dim id1vec As New List(Of String)
        Dim id2vec As New List(Of String)
        Dim corvec As New List(Of Double)
        Dim pvalvec As New List(Of Double)

        For Each tuple In TqdmWrapper.Wrap(GetVectorElement.Zip(idset1, idset2).ToArray)
            Dim x As String = CStr(tuple.Item1)
            Dim y As String = CStr(tuple.Item2)
            Dim corResult = cor.Correlation(x, y)

            Call id1vec.Add(x)
            Call id2vec.Add(y)
            Call corvec.Add(corResult.cor)
            Call pvalvec.Add(corResult.pval)
        Next

        Call eval.add("id1", id1vec)
        Call eval.add("id2", id2vec)
        Call eval.add("cor", corvec)
        Call eval.add("pval", pvalvec)

        Return eval
    End Function

    ''' <summary>
    ''' read network edges table which is save via igraph package
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="cor_thres"></param>
    ''' <returns></returns>
    <ExportAPI("read_wgcna_edges")>
    <RApiReturn(GetType(RelationshipScore))>
    Public Function readWGCNAInteractions(file As String, Optional cor_thres As Double = 0.65, Optional env As Environment = Nothing) As Object
        Return pipeline.CreateFromPopulator(
            From ie As RelationshipScore
            In NetworkFileIO.ReadEdges(Of RelationshipScore)(file)
            Where std.Abs(ie.Score) > cor_thres)
    End Function

    ''' <summary>
    ''' Build bnlearn prior network based on the WGCNA co-expression network and TF id list.
    ''' </summary>
    ''' <param name="edges"></param>
    ''' <param name="TF"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("prior_network")>
    <RApiReturn(GetType(PriorNetwork))>
    Public Function bnnet(<RRawVectorArgument(GetType(RelationshipScore))> edges As Object,
                          <RRawVectorArgument(TypeCodes.string)> TF As Object,
                          Optional env As Environment = Nothing) As Object

        Dim cor = pipeline.Stream(Of RelationshipScore)(edges, env)
        Dim tfids As String() = CLRVector.asCharacter(TF)

        If cor.isError Then
            Return cor.getError
        ElseIf tfids.IsNullOrEmpty Then
            Return RInternal.debug.stop("the required TF id list should not be empty!", env)
        End If

        Return cor.BuildPriorNetwork(New HashSet(Of String)(tfids))
    End Function

    ''' <summary>
    ''' build the GRN prior network from the expression matrix or the cached bicor correlation store.
    ''' </summary>
    ''' <param name="x">gene expression matrix (genes x samples)</param>
    ''' <param name="TF">transcription factor gene id vector</param>
    ''' <param name="opts">pipeline options</param>
    ''' <param name="bicor">
    ''' optional cached bicor correlation matrix store (from <c>open_bicor</c>).
    ''' When present, the candidate edges are read from the store instead of
    ''' re-computing the correlation matrix.
    ''' </param>
    ''' <param name="modules">
    ''' optional cached WGCNA module map (from <c>open_modules</c>). When Nothing and
    ''' <paramref name="bicor"/> is present, a sidecar cache <c>{store}.modules</c> is
    ''' tried automatically (with fingerprint validation); on cache miss the WGCNA
    ''' blockwise module detection runs once and the result is written back to the
    ''' sidecar file.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' the assembled <see cref="GRNBuildResult"/>: the prior regulatory network
    ''' split by the WGCNA modules (each module holds a
    ''' <see cref="PriorNetwork"/> of <see cref="RegulatoryEdge"/> edges
    ''' carrying TF, target gene, regulation type, confidence and evidence tags),
    ''' together with the build summary statistics. Use
    ''' <c>result.ToPriorNetwork()</c> / <c>result.ToPriorNetwork(minConfidence)</c>
    ''' to merge it into a single network for the downstream DBN or GNN modeling.
    ''' </returns>
    ''' <remarks>
    ''' performance notes: when the <c>bicor</c> store is present, the candidate
    ''' edges and p-values are read from the cached correlation matrix and the
    ''' WGCNA module map is resolved through a three-level cache (explicit
    ''' <c>modules</c> argument => <c>{storeFile}.modules</c> sidecar with
    ''' fingerprint validation => fresh blockwise run with automatic cache
    ''' write-back), so repeated calls with different thresholds do not re-run
    ''' either the correlation computation or the WGCNA module detection.
    ''' </remarks>
    ''' <example>
    ''' let opts = new("GRN_opts", minAbsCorrelation = 0.3, enableGpu = TRUE)
    ''' |&gt; string_links(string_db = "K:\hsa_grn\string-db");
    ''' let grn = WGCNA::build_grn(hsa, TF$Ensembl, opts, bicor = bicor);
    ''' </example>
    <ExportAPI("build_grn")>
    <RApiReturn(GetType(GRNBuildResult))>
    Public Function buildGRN(x As HTSMatrix,
                             <RRawVectorArgument(TypeCodes.string)>
                             TF As Object,
                             opts As GRNBuildOptions,
                             Optional bicor As CorrelationMatrixStore = Nothing,
                             Optional modules As WGCNAModuleMap = Nothing,
                             Optional env As Environment = Nothing) As Object

        Dim result As GRNBuildResult

        If bicor Is Nothing Then
            result = ExpressionGRNBuilder.Build(x, CLRVector.asCharacter(TF), opts)
        Else
            Dim map As WGCNAModuleMap = ResolveModuleMap(x, bicor, modules, opts)

            result = ExpressionGRNBuilder.Build(bicor, CLRVector.asCharacter(TF), map.modules, opts, expr:=x)
        End If

        Return result
    End Function

    ''' <summary>
    ''' resolve the WGCNA module map with the priority: explicit argument =>
    ''' sidecar cache file => fresh blockwise run (with fingerprint validation)
    ''' </summary>
    ''' <param name="x">the raw expression matrix (used for a fresh blockwise run on cache miss)</param>
    ''' <param name="bicor">the bicor correlation matrix store</param>
    ''' <param name="modules">the caller-provided module map cache (highest priority)</param>
    ''' <param name="opts">the pipeline options (WGCNA configuration and GPU switch are taken from it)</param>
    ''' <returns>a validated (or freshly computed) module map</returns>
    Private Function ResolveModuleMap(x As HTSMatrix,
                                      bicor As CorrelationMatrixStore,
                                      modules As WGCNAModuleMap,
                                      opts As GRNBuildOptions) As WGCNAModuleMap

        Dim config As WGCNAConfig = If(opts Is Nothing, Nothing, opts.wgcnaConfig)

        If config Is Nothing Then
            config = New WGCNAConfig With {.useGpu = True, .buildGraph = False}
        Else
            config.buildGraph = False
        End If

        config.useGpu = If(opts Is Nothing, True, opts.enableGpu)

        Dim fingerprint As String = WGCNAModuleMap.ComputeFingerprint(bicor.genes, config)

        ' ① 显式传入的模块缓存
        If modules IsNot Nothing Then
            If modules.Validate(bicor.genes, fingerprint) Then
                Call "module map: use caller-provided cache (fingerprint matched)".info
                Return modules
            End If

            Call "module map: caller-provided cache fingerprint mismatched, fallback to auto cache".warning
        End If

        Dim dir As String = bicor.storeFile.ParentPath
        Dim sidecar As String = $"{dir}/modules.dat"

        ' ② 自动定位边车缓存 {storeFile}.modules（文件模式才有路径）
        If bicor.storeFile IsNot Nothing Then
            Dim cached As WGCNAModuleMap = WGCNAModuleMap.Load(sidecar)

            If cached IsNot Nothing AndAlso cached.Validate(bicor.genes, fingerprint) Then
                Call $"module map: sidecar cache hit '{sidecar}' ({cached.ToString})".info
                Return cached
            End If

            If cached IsNot Nothing Then
                Call $"module map: sidecar cache '{sidecar}' fingerprint mismatched, rebuild...".warning
            End If
        End If

        ' ③ 缓存未命中：运行一次 WGCNA blockwise 模块划分，并自动写回边车缓存
        Call "module map cache miss: running WGCNA blockwise module detection...".info

        Dim wgcna As Result = Analysis.RunBlockwise(x, config)
        Dim map As WGCNAModuleMap = WGCNAModuleMap.FromWGCNA(wgcna, fingerprint)

        If bicor.storeFile IsNot Nothing Then
            Call map.Save(sidecar)
        End If

        Return map
    End Function

    ''' <summary>
    ''' locate the STRING protein interaction data files inside a STRING database
    ''' folder and attach them to the GRN pipeline options.
    ''' </summary>
    ''' <param name="opts">
    ''' the GRN pipeline options object (usually created via <c>new("GRN_opts", ...)</c>
    ''' and possibly piped through other option helpers). This function mutates and
    ''' returns the same options object.
    ''' </param>
    ''' <param name="string_db">
    ''' the directory of the extracted STRING database files (for example
    ''' <c>K:\hsa_grn\string-db</c>). The links file is picked with the priority:
    ''' compact <c>9606.protein.links.v*.txt</c> (3 columns) &gt; detailed version &gt;
    ''' any other <c>*protein.links*.txt</c>; the <c>.gz</c> archives and the
    ''' physical-subset files are always skipped. The aliases file
    ''' (<c>*protein.aliases*.txt</c>) is located as well and used to build the
    ''' Ensembl gene id => STRING protein id mapping automatically.
    ''' </param>
    ''' <returns>
    ''' the same <see cref="GRNBuildOptions"/> object with the
    ''' <c>stringLinks</c> and <c>stringAliases</c> file paths filled in.
    ''' When no links file is found, the STRING protein interaction evidence is
    ''' disabled (a warning is printed).
    ''' </returns>
    ''' <remarks>
    ''' STRING protein interactions are used by the GRN pipeline in two configurable
    ''' ways: as a confidence re-weighting evidence for the existing co-expression
    ''' edges, and/or as a topology completion source that adds the protein
    ''' interaction pairs missing in the co-expression network (see
    ''' <c>GRN_opts</c> fields <c>stringAddEdges</c>, <c>stringMinScore</c> and
    ''' <c>stringWeight</c>).
    ''' </remarks>
    ''' <example>
    ''' let opts = new("GRN_opts", minAbsCorrelation = 0.3, enableGpu = TRUE)
    ''' |&gt; string_links(string_db = "K:\hsa_grn\string-db");
    ''' </example>
    <ExportAPI("string_links")>
    Public Function stringLinks(opts As GRNBuildOptions, string_db As String) As GRNBuildOptions
        ' 自动发现 STRING 数据文件（优先精简版 links + aliases 别名表）
        Dim str As (links As String, aliases As String) = ExpressionGRNBuilder.FindStringLinks(string_db)

        If str.links IsNot Nothing Then
            Call $"STRING links   = '{str.links}'".info
        Else
            Call "STRING links not found, protein interaction evidence will be disabled.".warning
        End If

        If str.aliases IsNot Nothing Then
            Call $"STRING aliases = '{str.aliases}'".info
        End If

        opts.stringLinks = str.links
        opts.stringAliases = str.aliases

        Return opts
    End Function
End Module
