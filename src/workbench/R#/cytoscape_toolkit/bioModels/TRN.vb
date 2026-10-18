#Region "Microsoft.VisualBasic::b2f70964df426442b5b3b097b8a396dc, R#\cytoscape_toolkit\bioModels\TRN.vb"

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

'   Total Lines: 39
'    Code Lines: 29 (74.36%)
' Comment Lines: 4 (10.26%)
'    - Xml Docs: 75.00%
' 
'   Blank Lines: 6 (15.38%)
'     File Size: 1.62 KB


' Module TRN
' 
'     Function: edge_table, ExpressionConnections
' 
'     Sub: Main
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.IO.Compression
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Math.Matrix
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.WGCNA
Imports SMRUCC.genomics.Model.Network.Regulons
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' Transcription Regulation Network Builder Tools
''' </summary>
''' 
<Package("TRN")>
Module TRN

    Sub Main()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(Connection()), AddressOf edge_table)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Private Function edge_table(edges As Connection(), args As list, env As Environment) As dataframe
        Dim df As New dataframe With {.columns = New Dictionary(Of String, Array)}

        Call df.add("source", From e As Connection In edges Select e.gene1)
        Call df.add("target", From e As Connection In edges Select e.gene2)
        Call df.add("is_directly", From e As Connection In edges Select e.is_directly)
        Call df.add("cor", From e As Connection In edges Select e.cor)
        Call df.add("pval", From e As Connection In edges Select e.pval)
        Call df.add("interaction", From e As Connection In edges Select e.interaction)

        Return df
    End Function

    <ExportAPI("fpkm.connections")>
    Public Function ExpressionConnections(fpkm As DataSet(), Optional cutoff# = 0.65) As Connection()
        Return fpkm.CorrelationNetwork(cutoff).ToArray
    End Function

    ''' <summary>
    ''' compute the signed bicor correlation matrix once and persist it into a
    ''' random-access correlation store for repeated threshold filtering experiments.
    ''' </summary>
    ''' <param name="x">
    ''' the gene expression matrix (genes in rows, samples in columns) that the
    ''' robust biweight midcorrelation (bicor) is computed from. Should be
    ''' batch-normalized and variance-filtered already (see
    ''' <c>batch_normalize</c> and <c>top_variance</c>).
    ''' </param>
    ''' <param name="repo">
    ''' the data repository directory. Two files are written into it:
    ''' <c>{repo}/{corKind}.dat</c> (the row blocks of the correlation matrix,
    ''' named <c>bicor.dat</c> or <c>pearson.dat</c> after the correlation kind)
    ''' and <c>{repo}/index.dat</c> (the gene table + row directory). The WGCNA
    ''' module map sidecar cache is written as <c>{repo}/modules.dat</c>.
    ''' </param>
    ''' <param name="type">
    ''' the on-disk encoding of the correlation values:
    ''' <see cref="CorrelationEncodings.Float32"/> (4 bytes per value, ~1e-7 precision,
    ''' default) or <see cref="CorrelationEncodings.QuantizedInt16"/> (2 bytes per value,
    ''' ~3e-5 precision, about half of the storage size).
    ''' </param>
    ''' <param name="wgcnaOpts">
    ''' optional WGCNA blockwise configuration used by the module detection that
    ''' runs in the same pass. Nothing (default) uses the default configuration with
    ''' the GPU backend enabled and the network graph materialization disabled.
    ''' </param>
    ''' <param name="corKind">
    ''' the correlation estimator used for the matrix computation:
    ''' <see cref="CorrelationKind.Bicor"/> (robust biweight midcorrelation,
    ''' default) or <see cref="CorrelationKind.Pearson"/>. The kind is not stored
    ''' in the index but implied by the data file name.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>TRUE when the correlation store (and the module map cache) is written successfully.</returns>
    ''' <remarks>
    ''' computing the full N×N correlation matrix of a large expression matrix is
    ''' very expensive (hours for 50,000 genes), while all downstream network
    ''' generations are just threshold filters over this matrix. So this function
    ''' performs the expensive computation once and persists two artifacts:
    ''' 1. the raw unfiltered signed correlation matrix (open it later via
    ''' <c>open_bicor</c>);
    ''' 2. the WGCNA module map sidecar cache <c>{repo}/modules.dat</c> (consumed by
    ''' <c>build_grn</c>, so the WGCNA blockwise module detection is not re-run).
    ''' After this one-time computation, every different correlation threshold
    ''' experiment only costs a fast filtering over the cached matrix.
    ''' </remarks>
    ''' <example>
    ''' write_bicor(hsa, repo = "Z:/hsa_mat");
    ''' </example>
    <ExportAPI("write_bicor")>
    Public Function write_bicor(x As Matrix, repo As String,
                                Optional type As CorrelationEncodings = CorrelationEncodings.Float32,
                                Optional wgcnaOpts As WGCNAConfig = Nothing,
                                Optional corKind As CorrelationKind = CorrelationKind.Bicor,
                                Optional env As Environment = Nothing) As Object

        Call x.CreateMatrixStore(repo, type, corKind)
        ' 同一趟顺带缓存 WGCNA 模块映射（{repo}/modules.dat）：
        ' 后续 build_grn(bicor=...) 直接消费该边车缓存，不再重复运行 WGCNA blockwise
        Dim opts As WGCNAConfig = If(wgcnaOpts Is Nothing,
                                     New WGCNAConfig With {.useGpu = True, .buildGraph = False},
                                     wgcnaOpts.Clone())
        ' 让紧随其后的 WGCNA blockwise 直接复用刚写入的相关矩阵存储，避免相关矩阵被完整重算；
        ' 取值与 build_grn 的 ResolveModuleMap 侧保持同一规范化形式（数据文件的 ParentPath），
        ' 以保证模块映射边车缓存的指纹一致（克隆配置，不改写调用方对象）
        opts.correlationStore = $"{repo}/{CorrelationStoreFiles.DataFileOf(corKind)}".ParentPath

        Call WriteModuleMap(x, $"{repo}/modules.dat", opts)

        Return True
    End Function

    ''' <summary>
    ''' run the WGCNA blockwise module detection once and cache the module map
    ''' into the sidecar file
    ''' </summary>
    ''' <param name="x">the gene expression matrix (the same dataset as the bicor store)</param>
    ''' <param name="mapFile">the module map cache file path</param>
    ''' <param name="config">
    ''' the WGCNA blockwise configuration. Nothing (default) uses the default
    ''' configuration with the GPU backend enabled and the network graph
    ''' materialization disabled.
    ''' </param>
    ''' <returns>the module map object (the cache file has been written as well)</returns>
    Public Function WriteModuleMap(x As Matrix, mapFile As String, config As WGCNAConfig) As WGCNAModuleMap
        If config Is Nothing Then
            config = New WGCNAConfig With {.useGpu = True, .buildGraph = False}
        End If

        Dim t0 As Date = Now
        Dim wgcna As Result = Analysis.RunBlockwise(x, config)
        Dim fingerprint As String = WGCNAModuleMap.ComputeFingerprint(x.rownames, config)
        Dim map As WGCNAModuleMap = WGCNAModuleMap.FromWGCNA(wgcna, fingerprint)

        Call map.Save(mapFile)
        Call $"WGCNA module map cached in {StringFormats.ReadableElapsedTime(Now - t0)}".info

        Return map
    End Function

    ''' <summary>
    ''' re-open the persisted bicor correlation store that was written by <c>write_bicor</c>
    ''' </summary>
    ''' <param name="repo">
    ''' the data repository directory that was passed to <c>write_bicor</c>
    ''' (reads <c>{repo}/bicor.dat</c>, or <c>{repo}/pearson.dat</c> when the
    ''' store was written with <c>corKind = "pearson"</c>, and
    ''' <c>{repo}/index.dat</c>).
    ''' </param>
    ''' <param name="cache">
    ''' the hot-row LRU cache capacity in rows. Each cached row of a N = 50,000
    ''' matrix costs about 200 KB of memory. The default value 2048 means at most
    ''' about 400 MB of the hot row cache.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' an opened <see cref="CorrelationMatrixStore"/> object. Pass it to
    ''' <c>build_grn</c> via the <c>bicor</c> parameter to skip the expensive
    ''' correlation matrix re-computation.
    ''' </returns>
    ''' <remarks>
    ''' the returned store supports fast point queries (gene1, gene2 -> correlation
    ''' and p-value), neighborhood queries (gene + threshold -> all correlated genes)
    ''' and full-library streaming edge filters, all without loading the matrix
    ''' into memory.
    ''' </remarks>
    ''' <example>
    ''' let bicor = open_bicor("Z:/hsa_mat");
    ''' </example>
    <ExportAPI("open_bicor")>
    <RApiReturn(GetType(CorrelationMatrixStore))>
    Public Function open_bicor(repo As String, Optional cache As Integer = 2048, Optional env As Environment = Nothing) As Object
        ' 数据文件按相关类型命名（bicor.dat / pearson.dat）：优先 bicor，
        ' 仅当 bicor.dat 不存在而 pearson.dat 存在时才回退 pearson
        Dim dataFile As String = CorrelationStoreFiles.BicorDataFile

        If Not IO.File.Exists($"{repo}/{CorrelationStoreFiles.BicorDataFile}") AndAlso
           IO.File.Exists($"{repo}/{CorrelationStoreFiles.PearsonDataFile}") Then
            dataFile = CorrelationStoreFiles.PearsonDataFile
        End If

        Dim matrix = $"{repo}/{dataFile}".Open(FileMode.Open, doClear:=False, [readOnly]:=True)
        Dim index = $"{repo}/index.dat".Open(FileMode.Open, doClear:=False, [readOnly]:=True)

        ' storePath 用于边车缓存（WGCNA 模块映射 {storeFile}.modules）的自动定位
        Dim cor = CorrelationMatrixStore.Open(matrix, index, cacheRows:=cache, storePath:=$"{repo}/{dataFile}")

        Return cor
    End Function

    ''' <summary>
    ''' read the WGCNA module map sidecar cache that was written by <c>write_bicor</c>
    ''' </summary>
    ''' <param name="repo">
    ''' the same data repository directory that was passed to <c>write_bicor</c>
    ''' (reads <c>{repo}/modules.dat</c>).
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' the cached module map object, or Nothing when the cache file does not exist
    ''' (in that case <c>build_grn</c> will fall back to running the WGCNA blockwise
    ''' module detection and write the cache automatically).
    ''' </returns>
    ''' <remarks>
    ''' pass the returned object to <c>build_grn</c> via the <c>modules</c> parameter
    ''' to skip the WGCNA blockwise module detection. Note that the cache carries a
    ''' fingerprint (gene id row order + WGCNA configuration); when it does not match
    ''' the current data, <c>build_grn</c> will ignore it and rebuild automatically.
    ''' </remarks>
    ''' <example>
    ''' let modules = open_modules("Z:/hsa_mat");
    ''' let grn = WGCNA::build_grn(hsa, TF$Ensembl, opts, bicor = bicor, modules = modules);
    ''' </example>
    <ExportAPI("open_modules")>
    <RApiReturn(GetType(WGCNAModuleMap))>
    Public Function open_modules(repo As String, Optional env As Environment = Nothing) As Object
        Return WGCNAModuleMap.Load($"{repo}/modules.dat")
    End Function
End Module
