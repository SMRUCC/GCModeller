#Region "Microsoft.VisualBasic::28f647c043f729527387759df8e6bbb4, sub-system\CellPhenotype\PfsNET_FootprintMappingPathway_API.vb"

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

    ' 
    ' /********************************************************************************/

#End Region

'#Region "Microsoft.VisualBasic::a3b3d760d34745634f035a1f18838a2e, sub-system\CellPhenotype\PfsNET_FootprintMappingPathway_API.vb"

'    ' Author:
'    ' 
'    '       asuka (amethyst.asuka@gcmodeller.org)
'    '       xie (genetics@smrucc.org)
'    '       xieguigang (xie.guigang@live.com)
'    ' 
'    ' Copyright (c) 2018 GPL3 Licensed
'    ' 
'    ' 
'    ' GNU GENERAL PUBLIC LICENSE (GPL3)
'    ' 
'    ' 
'    ' This program is free software: you can redistribute it and/or modify
'    ' it under the terms of the GNU General Public License as published by
'    ' the Free Software Foundation, either version 3 of the License, or
'    ' (at your option) any later version.
'    ' 
'    ' This program is distributed in the hope that it will be useful,
'    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
'    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    ' GNU General Public License for more details.
'    ' 
'    ' You should have received a copy of the GNU General Public License
'    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



'    ' /********************************************************************************/

'    ' Summaries:

'    ' Module PFSNet
'    ' 
'    '     Function: Evaluate
'    ' 
'    ' Module PfsNET_FootprintMappingPathway_API
'    ' 
'    '     Constructor: (+1 Overloads) Sub New
'    ' 
'    '     Function: __createNetwork, __fileName, AnalysisFootprintPathway, CreateExpressionWindows, ExportCSV
'    '               GenomeProgrammingAnalysis, Initialize
'    ' 
'    '     Sub: SetPfsNETHandle
'    ' 
'    ' /********************************************************************************/

'#End Region

'Imports Microsoft.VisualBasic.CommandLine
'Imports Microsoft.VisualBasic.CommandLine.Reflection
'Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
'Imports Microsoft.VisualBasic.Data.csv
'Imports Microsoft.VisualBasic.Language
'Imports Microsoft.VisualBasic.Linq
'Imports Microsoft.VisualBasic.Scripting.MetaData
'Imports SMRUCC.genomics.Analysis.PFSNet
'Imports SMRUCC.genomics.Analysis.RNA_Seq
'Imports SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT
'Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.PfsNET
'Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.PfsNET.TabularArchives
'Imports SMRUCC.genomics.Assembly
'Imports SMRUCC.genomics.ComponentModel.Annotation
'Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly
'Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular
'Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.Compiler.Components
'Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.IO
'Imports SMRUCC.genomics.Visualize.Cytoscape.NetworkModel.PfsNET
'Imports __KEGG_NETWORK_ = Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic.Network(Of
'    SMRUCC.genomics.Visualize.Cytoscape.NetworkModel.PfsNET.Enzyme,
'    SMRUCC.genomics.Visualize.Cytoscape.NetworkModel.PfsNET.Interaction)

'''' <summary>
'''' The calculation invoke interface of the both R scription version and VB version engine.
'''' (R脚本版本的PfsNET计算模块和VB版本的PfsNET计算模块的计算接口)
'''' </summary>
'''' <remarks></remarks>
'Module PFSNet

'    ''' <summary>
'    ''' The default PFSNet engine is the R script version.(默认是使用R脚本的计算版本)
'    ''' </summary>
'    ''' <remarks></remarks>
'    Public PFSNet_EvaluateHandle As Func(Of String, String, String, String, String, String, String, PFSNetResultOut) = AddressOf PfsNETModuleAPI.Evaluate

'    Public Function Evaluate(file1 As String, file2 As String, file3 As String, b As Double, t1 As Double, t2 As Double, n As Double) As DataStructure.PFSNetResultOut
'        Return PFSNet_EvaluateHandle(file1, file2, file3, b, t1, t2, n)
'    End Function
'End Module

'<Package("PfsNET.Footprint.Mapping.Pathway",
'                    Description:="This module determines the specific pathway for the motif mapping to a specific cell phenotype.",
'                    Publisher:="xie.guigang@gcmodeller.org")>
'Module PfsNET_FootprintMappingPathway_API

'    ''' <summary>
'    ''' Current process id to avoid the disruption of the PfsNET calculation data.
'    ''' </summary>
'    ''' <remarks></remarks>
'    ReadOnly PID As Integer = Process.GetCurrentProcess.Id

'    <ExportAPI("Set.Handle.PfsNET.Evaluate",
'               Info:="Setups the PfsNET evaluation handle, you can using this method to choosing the R scripting version engine or the VB version engine.")>
'    Public Sub SetPfsNETHandle(<Parameter("Handle.PfsNET.Evaluation",
'                                          "The PfsNET evaluate handle interface, you can choosing the engine handle interface from: " & vbCrLf &
'                                          "PfsNET Get.Handle.PfsNET_Evaluate(R_Implements)   or" & vbCrLf &
'                                          "PfsNET Get.Handle.PfsNET_Evaluate(VB_Implements)")> Handle As Func(Of String, String, String, String, String, String, String, PFSNetResultOut))
'        PFSNet.PFSNet_EvaluateHandle = Handle
'    End Sub

'    Private Function __fileName(key As String) As String
'        Return String.Format("{0}/__{1}___{2}_{3}.txt", App.AppSystemTemp, key, PID, Now.ToNormalizedPathString)
'    End Function

'    <ExportAPI("PfsNET.Analyzer.Init()", Info:="Initialize the R version pfsnet analysis engine for the program.")>
'    Public Function Initialize(<Parameter("Java.Class.Path", "The file path of the java filter class object.")> Optional Java_Class As String = "",
'                               <Parameter("R_HOME", "")> Optional R_HOME As String = "") As Boolean
'        Return RTools.PfsNET.InitializeSession(Java_Class, R_HOME)
'    End Function

'    ''' <summary>
'    ''' footprint途径是经过比较野生型和突变体在同一个滑窗的时间段之内的表达差异而得到的
'    ''' </summary>
'    ''' <param name="WT">必须是经过<see cref="CreateExpressionWindows"></see>方法所创建出来的滑窗数据</param>
'    ''' <param name="netModel">用于生成文件三</param>
'    ''' <param name="Mutation">必须是经过<see cref="CreateExpressionWindows"></see>方法所创建出来的滑窗数据</param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    ''' 
'    <ExportAPI("analysis.footprint_pathway")>
'    Public Function AnalysisFootprintPathway(netModel As XmlresxLoader,
'                                             WT As ExprSamples()(),
'                                             Mutation As ExprSamples()(),
'                                             Optional b As String = "0.5",
'                                             Optional t1 As String = "0.95",
'                                             Optional t2 As String = "0.85",
'                                             Optional n As String = "1000") As DataStructure.PFSNetResultOut()

'        Dim Network As __KEGG_NETWORK_ = __createNetwork(netModel)
'        Dim GeneIdList As String() = (From nnn In WT.First Select nnn.locusId).ToArray    '筛选网络数据
'        Dim FiltedNetwork As String() = (From item In Network.Edges
'                                         Where Array.IndexOf(GeneIdList, item.FromNode) > -1 AndAlso
'                                             Array.IndexOf(GeneIdList, item.ToNode) > -1
'                                         Select (From pwyID As String
'                                                 In item.Pathways
'                                                 Select String.Join(vbTab, pwyID, item.FromNode, item.ToNode)).ToArray).IteratesALL.Distinct.ToArray
'        Dim file3 As String = __fileName("FOOTPRINT_PATHWAY_NETWORK_file3")

'        Dim InternalCreateMatrix = Function(data As ExprSamples()) As String()
'                                       Dim LQuery = (From Line As ExprSamples In data.AsParallel
'                                                     Let Expressions As String = String.Join(vbTab, Line.Values)
'                                                     Select Line.locusId & vbTab & Expressions).ToArray
'                                       Return LQuery
'                                   End Function
'        Dim InternalSaveFile = Sub(data As String(), file As String) data.SaveTo(file, System.Text.Encoding.ASCII)
'        Dim ChunkList As List(Of DataStructure.PFSNetResultOut) = New List(Of DataStructure.PFSNetResultOut)

'        Call InternalSaveFile(FiltedNetwork, file3)

'        For i As Integer = 0 To WT.Count - 1
'            Dim m_WT = WT(i), m_Mutant = Mutation(i)
'            '生成文件一二，在讲参数传递给R之后进行计算
'            Dim file1 As String = __fileName("footprint_pathway_MAT_file1")
'            Dim file2 As String = __fileName("footprint_pathway_MAT_file2")
'            Dim MAT As String()

'            MAT = InternalCreateMatrix(m_WT) : Call InternalSaveFile(MAT, file1)
'            MAT = InternalCreateMatrix(m_Mutant) : Call InternalSaveFile(MAT, file2)

'            Dim Result = PFSNet.Evaluate(file1, file2, file3, b, t1, t2, n)
'            Result.tag = "$Time=" & i
'            Call ChunkList.Add(Result)

'            Call Console.WriteLine("[DONE] {0}%", 100 * i / WT.Count)
'        Next

'        Return ChunkList.ToArray
'    End Function

'    <ExportAPI("pfsnet.export.csv")>
'    Public Function ExportCSV(Result As IEnumerable(Of DataStructure.PFSNetResultOut), Model As XmlresxLoader) As KEGGPhenotypes()
'        Dim Pathways = Model.KEGG_Pathways.GetAllPathways _
'            .ToDictionary(Function(x) x.EntryId,
'                          Function(x) DirectCast(x, PathwayBrief))
'        Dim LQuery = (From item As DataStructure.PFSNetResultOut
'                      In Result
'                      Let p1 = SubNetTable.CreateObject(item.phenotype1, item.tag & ".Class1", Pathways, 1)
'                      Let p2 = SubNetTable.CreateObject(item.phenotype2, item.tag & ".Class2", Pathways, 2)
'                      Select {p1, p2}.Unlist).ToArray.ToVector
'        Dim KEGGCategory = KEGGPhenotypes.PhenotypeAssociations(LQuery, KEGG.Archives.Csv.Pathway.CreateObjects(Model.KEGG_Pathways))
'        Return KEGGCategory
'    End Function

'    ''' <summary>
'    ''' 除了KEGG pathway的信息，还会在这里面包含有完整的细胞网络之中的调控信息和互作信息
'    ''' </summary>
'    ''' <param name="Model"></param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Private Function __createNetwork(Model As GCTabular.FileStream.IO.XmlresxLoader) As __KEGG_NETWORK_
'        Dim Network As __KEGG_NETWORK_ = GeneInteractions.__exportPathwayGraph(Model.KEGG_Pathways, True)
'        Dim Regulations As List(Of GeneInteractions.Interaction) =
'            LinqAPI.MakeList(Of GeneInteractions.Interaction) <=
'                From regulator As FileStream.Regulator
'                In Model.Regulators
'                Select New GeneInteractions.Interaction With {
'                    .FromNode = regulator.ProteinId,
'                    .ToNode = regulator.get_TargetGeneId,
'                    .Pathways = New String() {regulator.ProteinId & "-TF.Regulon"}
'                }
'        Dim CrossTalks As GeneInteractions.Interaction() =
'            LinqAPI.Exec(Of GeneInteractions.Interaction) <= From CrossTalk As CrossTalks
'                                                             In Model.CrossTalksAnnotation
'                                                             Select New GeneInteractions.Interaction With {
'                                                                 .FromNode = CrossTalk.Kinase,
'                                                                 .ToNode = CrossTalk.Regulator,
'                                                                 .Pathways = {"Cross-Talks"}
'                                                             }
'        Network.Edges = Regulations + Network.Edges + CrossTalks
'        Return Network
'    End Function

'    Sub New()
'        Call Settings.Session.Initialize()
'    End Sub

'    ''' <summary>
'    ''' 基因组的编程信息则是比较同一个细胞之内的不同滑窗时间点之间的表达差异性而得到的，在计算出差异性之后在比较野生型和突变体之间的差异既可以得到基因组因为motif突变而产生的重编程的现象了
'    ''' </summary>
'    ''' <param name="chipdata"></param>
'    ''' <param name="NetworkModel"></param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    ''' 
'    <ExportAPI("Analysis.Genome.Programming_Info",
'               Info:="Analysis the real time genome wide regulation profile changes in the cell cycle.")>
'    Public Function GenomeProgrammingAnalysis(
' _
'                <Parameter("ChipData.Realtime.Matrix", "This matrix object should be a real time expression data, " &
'                                                            "and this matrix is also can be obtained from the GCModeller simulation experiment.")>
'                ChipData As ExprSamples()(),
'                <Parameter("Network.Model", "The network model object for assign the cell phenotype regulation analysis.")>
'                NetworkModel As XmlresxLoader,
'                Shell As String,
'                Export As String,
'                <Parameter("b", "The beta threshold of the pfsnet calculation.")> Optional b As String = "0.5",
'                Optional t1 As String = "0.95",
'                Optional t2 As String = "0.85",
'                <Parameter("n", "The permuation testing count in the pfsnet.")> Optional n As String = "1000", Optional Skip As Integer = -1) As Boolean

'        Dim Network = __createNetwork(NetworkModel)
'        Dim GeneIdList As String() = (From RowData In ChipData.First Select RowData.locusId).ToArray    '筛选网络数据
'        Dim FiltedNetwork As String() = (From Edge As SMRUCC.genomics.Visualize.Cytoscape.NetworkModel.PfsNET.Interaction
'                                         In Network.Edges
'                                         Where Array.IndexOf(GeneIdList, Edge.FromNode) > -1 AndAlso Array.IndexOf(GeneIdList, Edge.ToNode) > -1
'                                         Select (From pwyID As String In Edge.Pathways Select String.Join(vbTab, pwyID, Edge.FromNode, Edge.ToNode)).ToArray).ToArray.ToVector.Distinct.ToArray

'        Dim File3 As String = __fileName("GenomeProgrammingInfoNetwork.File3")
'        Dim InternalCreateMatrix = Function(data As ExprSamples()) As String()
'                                       Dim LQuery = (From Line In data.AsParallel
'                                                     Let Expressions As String = String.Join(vbTab, Line.Values)
'                                                     Select Line.locusId & vbTab & Expressions).ToArray
'                                       Return LQuery
'                                   End Function
'        Dim InternalSaveFile = Sub(data As String(), file As String) Call data.SaveTo(file, System.Text.Encoding.ASCII)
'        Dim PfsNETProcessHandles As New Dictionary(Of System.IAsyncResult, Func(Of Integer))

'        Call InternalSaveFile(FiltedNetwork, File3)

'        Export = FileIO.FileSystem.GetDirectoryInfo(Export).FullName

'        If Skip <= 0 Then Skip = ChipData.Count * 2

'        For i As Integer = 1 To ChipData.Count - 1

'            If i >= Skip Then
'                Exit For
'            End If

'            Dim DataSlideWindow_Pre = ChipData(i - 1)
'            Dim DataSlideWindow_Current = ChipData(i)

'            '生成文件一二，在讲参数传递给R之后进行计算
'            Dim File1 As String = __fileName("#" & i & "GenomeProgrammingInfoMAT.File1")
'            Dim File2 As String = __fileName("#" & i & "GenomeProgrammingInfoMAT.File2")
'            Dim MAT As String()

'            MAT = InternalCreateMatrix(DataSlideWindow_Pre) : Call InternalSaveFile(MAT, File1)
'            MAT = InternalCreateMatrix(DataSlideWindow_Current) : Call InternalSaveFile(MAT, File2)

'            Dim Script As String = String.Format(My.Resources.PfsNETCellularNetwork, File1, File2, File3, Export, "#" & i)
'            Dim ScriptPath As String = Settings.DataCache & "/#" & i & ".shl"

'            Call Script.SaveTo(ScriptPath)

'            Dim argvs As String = String.Format("""{0}"" Beta {1} t1 {2} t2 {3} n {4}", ScriptPath, b, t1, t2, n)
'            Dim Invoke = Function() New IORedirect(Shell, argvs).Start(waitForExit:=True, displaDebug:=True)
'            Call PfsNETProcessHandles.Add(Invoke.BeginInvoke(Nothing, Nothing), Invoke)
'        Next

'        Dim EndInvokeLQuery = (From Handle In PfsNETProcessHandles.AsParallel Select Handle.Value.EndInvoke(Handle.Key)).ToArray

'        Return True
'    End Function

'    ''' <summary>
'    ''' 
'    ''' </summary>
'    ''' <param name="chipdata"></param>
'    ''' <param name="windowSize"></param>
'    ''' <param name="offset">大于0的一个数</param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    <ExportAPI("Data.Serials.Create.SlideWindows", Info:="The offset parameter should be a integer and bigger than the number ZERO.")>
'    Public Function CreateExpressionWindows(<Parameter("Expression.Matrix.ChipData", "This data is comes from the GCModeller calculation result or the Monte Carlo created matrix data.")>
'                                            ChipData As IO.File,
'                                            <Parameter("Window.Size")> WindowSize As Integer,
'                                            <Parameter("Index.OffSets", "The offset parameter should be a integer and bigger than the number ZERO.")>
'                                            Optional OffSet As Integer = 1) As ExprSamples()()

'        If OffSet <= 0 Then
'            Throw New DataException(String.Format("The slide window offset value '{0}' is not valid!", OffSet))
'        End If

'        Dim Matrix As ExprSamples() = MatrixAPI.ToSamples(ChipData, True)
'        Dim LQuery = (From GeneSamples As ExprSamples
'                      In Matrix.AsParallel
'                      Select GeneSamples.locusId,
'                          SlideWindowSamples = GeneSamples.Values.CreateSlideWindows(WindowSize, OffSet)).ToArray '对每一个基因的表达曲线数据都创建了滑窗数据
'        Dim ChunkBuffer As ExprSamples()() = (
'            From i As Integer
'            In LQuery.First.SlideWindowSamples.Sequence
'            Select (From GeneSample In LQuery
'                    Select New ExprSamples With {
'                        .locusId = GeneSample.locusId,
'                        .Values = GeneSample.SlideWindowSamples(i).Items}).ToArray).ToArray
'        Return ChunkBuffer
'    End Function
'End Module
