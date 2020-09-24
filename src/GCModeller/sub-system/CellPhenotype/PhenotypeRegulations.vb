#Region "Microsoft.VisualBasic::7e4d8f48f0efa1574c847152f84d27b3, sub-system\CellPhenotype\PhenotypeRegulations.vb"

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

'#Region "Microsoft.VisualBasic::7ea2b71b75664e07796e8c7b44eec8cf, sub-system\CellPhenotype\PhenotypeRegulations.vb"

'' Author:
'' 
''       asuka (amethyst.asuka@gcmodeller.org)
''       xie (genetics@smrucc.org)
''       xieguigang (xie.guigang@live.com)
'' 
'' Copyright (c) 2018 GPL3 Licensed
'' 
'' 
'' GNU GENERAL PUBLIC LICENSE (GPL3)
'' 
'' 
'' This program is free software: you can redistribute it and/or modify
'' it under the terms of the GNU General Public License as published by
'' the Free Software Foundation, either version 3 of the License, or
'' (at your option) any later version.
'' 
'' This program is distributed in the hope that it will be useful,
'' but WITHOUT ANY WARRANTY; without even the implied warranty of
'' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'' GNU General Public License for more details.
'' 
'' You should have received a copy of the GNU General Public License
'' along with this program. If not, see <http://www.gnu.org/licenses/>.



'' /********************************************************************************/

'' Summaries:

'' Module PhenotypeRegulations
'' 
''     Constructor: (+1 Overloads) Sub New
'' 
''     Function: __existsItem, __exportTCS_CrossTalks, __levelMapping, __phenotypeRegulats, __quantile
''               __ranking, AssignPhenotype, AssignPhenotype2, (+2 Overloads) CommandLineTools, CreateDefaultConfig
''               (+2 Overloads) CreateDynamicNetwork, CreateEmptyInput, CreateExpressionMatrix, CreateInput_AllRegulators, CreateMutationInit
''               ExportCytoscapeNetwork, ExportNetworkModel, ExportPfsNET, ExportTCSCrossTalksCytoscape, FamilyStatics
''               ImportantPhenotypeRegulators, ModelApplyingConfiguration, ModelSetupKernelLoops, ModelSetupMutation, MonteCarloExperiment
''               ReadInputStatus, SaveNetworkModel, Simulation, StaticsFamilyDistributions, WriteNetworkStateData
''               WriteRegulationState
'' 
''     Sub: CommandLineTools
''     Class CrossTalk
'' 
'' 
''         Class TCS_GeneObject
'' 
''             Properties: Quantity
'' 
'' 
'' 
''     Structure __phenotype
'' 
'' 
'' 
'' 
'' 
'' /********************************************************************************/

'#End Region

'Imports System.Text
'Imports System.Text.RegularExpressions
'Imports Microsoft.VisualBasic.CommandLine.Reflection
'Imports Microsoft.VisualBasic.Data.csv
'Imports Microsoft.VisualBasic.Data.csv.Extensions
'Imports Microsoft.VisualBasic.Data.csv.IO
'Imports Microsoft.VisualBasic.Data.visualize.Network
'Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
'Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
'Imports Microsoft.VisualBasic.Language
'Imports Microsoft.VisualBasic.Math
'Imports Microsoft.VisualBasic.Math.Quantile
'Imports Microsoft.VisualBasic.Scripting.MetaData
'Imports Microsoft.VisualBasic.Text
'Imports SMRUCC.genomics.Analysis.CellPhenotype.TRN
'Imports SMRUCC.genomics.Analysis.RNA_Seq
'Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.PfsNET
'Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.PfsNET.TabularArchives
'Imports SMRUCC.genomics.Assembly
'Imports SMRUCC.genomics.ComponentModel.Annotation
'Imports SMRUCC.genomics.Data.Regprecise
'Imports SMRUCC.genomics.GCModeller.Framework
'Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver
'Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.DataStorage.FileModel
'Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.DataStorage.FileModel.Extensions
'Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.Compiler.Components
'Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.IO
'Imports SMRUCC.genomics.InteractionModel.Regulon
'Imports SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat
'Imports SMRUCC.genomics.Visualize.Cytoscape.API.ImportantNodes
'Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.Serialization
'Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML
'Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML.File
'Imports KernelDriver = SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.KernelDriver(Of Integer,
'    SMRUCC.genomics.Analysis.CellPhenotype.TRN.KineticsModel.BinaryExpression,
'    SMRUCC.genomics.Analysis.CellPhenotype.TRN.BinaryNetwork)

'''' <summary>
'''' 将MEME所分析出来的调控信息附加到代谢途径的网络图之中
'''' </summary>
'''' <remarks></remarks>
'<[Namespace]("AnalysisTools.Phenotype.Regulations")>
'Public Module PhenotypeRegulations

'    <ExportAPI("Export.Network.Model")>
'    Public Function ExportNetworkModel(model As BinaryNetwork, saveto As String) As Boolean
'        Return model.SaveNetwork(saveto)
'    End Function

'    ''' <summary>
'    ''' 简要版本是没有长度映射的
'    ''' </summary>
'    ''' <param name="footprints"></param>
'    ''' <param name="Inits"></param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    <ExportAPI("binary_network.create_lite")>
'    Public Function CreateDynamicNetwork(Footprints As IEnumerable(Of RegulatesFootprints),
'                                         <Parameter("Kernel.Inits", "The inits node state value of the network kernel object.")>
'                                         Inits As IEnumerable(Of NetworkInput)) As BinaryNetwork

'#Const DEBUG = 1

'#If DEBUG Then
'        For i As Integer = 0 To Footprints.Count * 0.000001
'            Dim p = Rnd() * (Footprints.Count - 1)
'            Call $"Regulators  {If(String.IsNullOrEmpty(Footprints(p).Regulator), "NULL", Footprints(p).Regulator)}".__DEBUG_ECHO
'        Next
'#End If

'        Dim lMaps = (From rels As RegulatesFootprints
'                     In Footprints
'                     Select rels.ORF
'                     Distinct) _
'                        .ToDictionary(Function(obj) obj,
'                                      Function(obj) 1)
'        Dim network As BinaryNetwork =
'            EngineAPI.CreateObject(Footprints, Inits, lMaps)
'        Call Console.WriteLine(network.ToString)
'        Return network
'    End Function

'    <ExportAPI("Kernel.Config.Load", Info:="Loads the kernel configuration data and then applies on the kernel engine.")>
'    Public Function ModelApplyingConfiguration(<Parameter("Model.Applied", "The loaded configuration data will be applied on this cellular network model object.")>
'                                               Model As BinaryNetwork,
'                                               <Parameter("Conf.Path", "The file path of the kernel configuration.")>
'                                               Conf As String) As BinaryNetwork
'        Dim ConfData As Configs = Configuration.LoadConfiguration(Conf).GetConfigures
'        Call Model.SetConfigs(ConfData)
'        Return Model
'    End Function

'    ''' <summary>
'    ''' 将计算得到的矩阵数据之中导出TCS的CrossTalk的Cytoscape的网络XML文件，并分别保存至<paramref name="Export_to"></paramref>所指定的文件夹之下
'    ''' </summary>
'    ''' <param name="Model"></param>
'    ''' <param name="MAT"></param>
'    ''' <param name="Export_to"></param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    <ExportAPI("export.tcs_crosstalks")>
'    Public Function ExportTCSCrossTalksCytoscape(Model As XmlresxLoader, MAT As IO.File, Export_to As String) As Boolean
'        Dim Data = LoadData_Integer(MAT)
'        Dim TCS_HK As String() = (From item In Model.CrossTalksAnnotation Select item.Kinase).ToArray.Distinct.ToArray,
'            TCS_RR = (From item In Model.CrossTalksAnnotation Select item.Regulator).ToArray.Distinct.ToArray
'        Dim TCS_MAT = (From item In Data
'                       Where Array.IndexOf(TCS_HK, item.UniqueId) > -1 OrElse
'                           Array.IndexOf(TCS_RR, item.UniqueId) > -1
'                       Select item).ToDictionary(Function(item) item.UniqueId,
'                                                 Function(item) item.Samples)

'        TCS_HK = (From id As String In TCS_HK Where TCS_MAT.ContainsKey(id) Select id).ToArray
'        TCS_RR = (From id As String In TCS_RR Where TCS_MAT.ContainsKey(id) Select id).ToArray

'        Dim CrossTalks = (From item As CrossTalks
'                          In Model.CrossTalksAnnotation.AsParallel
'                          Where __existsItem(item, TCS_HK, TCS_RR)
'                          Select item).ToArray

'        For i As Integer = 0 To TCS_MAT.Count - 1
'            Dim p As Integer = i
'            Dim ChunkBuffer = (From item In TCS_MAT.AsParallel Select New KeyValuePair(Of String, Double)(item.Key, item.Value(p))).ToArray
'            Dim GraphFile = __exportTCS_CrossTalks("TimePhase_" & i, ChunkBuffer.ToDictionary(Function(item) item.Key,
'                                                                                                    Function(item) item.Value), CrossTalks)
'            Call GraphFile.Save(Export_to & "/" & i & ".xml", Encoding.ASCII)
'        Next

'        Return True
'    End Function

'    Private Function __existsItem(item As CrossTalks, TCS_HK As String(), TCS_RR As String()) As Boolean
'        For Each hk As String In TCS_HK
'            For Each rr As String In TCS_RR
'                If Abstract.Equals(item, hk, rr) Then
'                    Return True
'                End If
'            Next
'        Next

'        Return False
'    End Function

'    Private Function __exportTCS_CrossTalks(TAG As String, Data As Dictionary(Of String, Double), Network As CrossTalks()) As XGMMLgraph
'        Dim Edges = (From item In Network Select New CrossTalk With {.fromNode = item.Kinase, .toNode = item.Regulator, .value = Math.Min(Data(item.Kinase), Data(item.Regulator)) * item.Probability}).ToArray
'        Dim Nodes = (From item In Network Select {New CrossTalk.TCS_GeneObject With {.ID = item.Kinase, .Quantity = Data(item.Kinase), .NodeType = "Kinase"},
'                                                  New CrossTalk.TCS_GeneObject With {.ID = item.Regulator, .NodeType = "RR", .Quantity = Data(item.Regulator)}}).ToArray.ToVector

'        Return ExportToFile.Export(Nodes, Edges, "TCS_CrossTalks_Network =" & TAG)
'    End Function

'    Private Class CrossTalk : Inherits NetworkEdge

'        Public Class TCS_GeneObject : Inherits FileStream.Node
'            Public Property Quantity As Double
'        End Class
'    End Class

'    <ExportAPI("config.create_default")>
'    Public Function CreateDefaultConfig(Optional saveto As String = "") As Configuration
'        Dim conf = Configuration.DefaultValue
'        If Not String.IsNullOrEmpty(saveto) Then
'            Call conf.Save(saveto, Encodings.UTF8)
'        End If

'        Return conf
'    End Function

'    <ExportAPI("Binary_Network.Object.Create")>
'    Public Function CreateDynamicNetwork(Model As XmlresxLoader, Footprints As IEnumerable(Of RegulatesFootprints),
'                                         <Parameter("Network.Inits")>
'                                         Inits As IEnumerable(Of NetworkInput)) As BinaryNetwork
'        Dim NetworkModel As BinaryNetwork = EngineAPI.CreateObject(Footprints, Inits, Model)
'        Call ("The cellular network model created successfully!   // " & NetworkModel.ToString).__DEBUG_ECHO
'        Return NetworkModel
'    End Function

'    <ExportAPI("Setup.Mutation.Factor", Info:="Setup the mutation value to a specific gene object in the network model.")>
'    Public Function ModelSetupMutation(Model As BinaryNetwork,
'                                       <Parameter("Gene.ID", "The Id value of the target gene object that will be setup its mutation condition.")>
'                                       GeneID As String,
'                                       <Parameter("Mutation.Factor", "The mutation changes of the target gene, 0 for deletion mutation, " &
'                                                                          "1 for normal level expression and " &
'                                                                          "value greater than 1 is using for overexpression mutation.")>
'                                       Factor As Double) As BinaryNetwork
'        Call Model.SetMutationFactor(GeneID, Factor)
'        Return Model
'    End Function

'    <ExportAPI("setup.kernel_cycles")>
'    Public Function ModelSetupKernelLoops(<Parameter("network.cellular.model", "The created binary network model object.")>
'                                          Network As BinaryNetwork,
'                                          <Parameter("kel.cycles", "The kernel running cycle times.")>
'                                          KelCycle As Integer) As BinaryNetwork
'        Call Network.SetKernelLoops(KelCycle)
'        Return Network
'    End Function

'    <ExportAPI("Binary_Network.Run")>
'    Public Function Simulation(<Parameter("Cellular.Network.Model", "The virtual cellular network model object.")>
'                               NetworkModel As BinaryNetwork,
'                               <Parameter("Kel.Cycles", "The total kernel cycle ticks of the kernel model object will running.")>
'                               KernelLoop As Integer) As IO.File

'        Using NetworkKernelDriver As KernelDriver = New KernelDriver

'            Call NetworkModel.SetKernelLoops(KernelLoop)
'            Call NetworkModel.Initialize()
'            Call NetworkKernelDriver.LoadEngineKernel(Kernel:=NetworkModel)
'            Call NetworkKernelDriver.SetFilterHandles(NetworkModel.NonRegulationHandles)

'            Call Console.WriteLine("Driver start to run the kernel...")
'            Call NetworkKernelDriver.Run()

'            Using DataResultAdapter = New MsCsvChunkBuffer(Of Integer)()
'                Dim ChunkBuffer = NetworkKernelDriver.get_DataSerials
'                Dim Data0Expr = DataResultAdapter.get_Result(ChunkBuffer:=ChunkBuffer)
'                Return Data0Expr
'            End Using
'        End Using
'    End Function

'    ''' <summary>
'    ''' 
'    ''' </summary>
'    ''' <param name="network"></param>
'    ''' <param name="counts">实验的重复次数，建议至少1000次以上</param>
'    ''' <param name="export">实验数据的导出位置</param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    ''' 
'    <ExportAPI("experiment.monte_carlo", Info:="Please initialize all of the parameter before calling this function.")>
'    Public Function MonteCarloExperiment(network As BinaryNetwork, counts As Integer, export As String) As Boolean

'        Call network.Initialize()

'        For i As Integer = 0 To counts

'            Using driver As New Kernel_Driver.KernelDriver(Of Integer, KineticsModel.BinaryExpression, BinaryNetwork)
'                Call driver.LoadEngineKernel(Kernel:=network)
'                Call driver.SetFilterHandles(network.NonRegulationHandles)
'                Call driver.Run()

'                Using csvAdapter = New MsCsvChunkBuffer(Of Integer)()
'                    Dim chunkBuffer = driver.get_DataSerials
'                    Dim data = csvAdapter.get_Result(ChunkBuffer:=chunkBuffer)

'                    Call data.Save(String.Format("{0}/Expression/n_{1}.csv", export, i), False)
'                    Call network.WriteNodeStates(url:=String.Format("{0}/States/n_{1}.csv", export, i))
'                End Using
'            End Using

'            Call network.Reset()
'        Next

'        Return True
'    End Function

'    ''' <summary>
'    ''' 从蒙特卡洛实验的计算数据之中生成实验样本，即将多个计算样本合并为一个矩阵（数据预处理阶段），最后生成的数据都是没有时间标签的
'    ''' </summary>
'    ''' <param name="dir"></param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    <ExportAPI("ExpressionMatrix.Create", Info:="Please notice that all of the sample value its name start with character # will be ignored by this function as the character # is the annotation tag for the data.")>
'    Public Function CreateExpressionMatrix(<Parameter("Dir.Matrix.Source", "The csv matrix value in this directory should comes from the same monte carlo experiment.")>
'                                           dir As String,
'                                           <Parameter("Ranking.Mapping.Level", "The rankning mapping level counts for gene real-time " &
'                                                                                    "expression level matrix ranking mapping.")>
'                                           Optional Level As Double = 0.7) As IO.File
'        Dim Cache = (From path As String
'                     In FileIO.FileSystem.GetFiles(dir, FileIO.SearchOption.SearchTopLevelOnly, "*.csv").AsParallel
'                     Let Csv = FileLoader.FastLoad(path, parallel:=False)
'                     Select Csv).ToArray '获取同一批次的蒙特卡洛计算实验之中所得到的所有的计算样本
'        Dim RankMapping = (From obj In (From Csv In Cache.AsParallel Select (From row In Csv Let ID As String = row.First Where ID.First <> "#"c Let data As Double() = (From s As String In row.Skip(1) Select Val(s)).ToArray Select New SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.DataStorage.FileModel.CHUNK_BUFFER_EntityQuantities With {.UniqueId = ID, .Samples = data}).ToArray).ToArray.Unlist Select obj Group By obj.UniqueId Into Group).ToArray
'        Dim GetModalLevel = (From GeneObject In RankMapping.AsParallel Select __levelMapping(GeneObject.Group.ToArray, p:=Level)).ToArray
'        '重新生成一个Csv文件
'        Dim CsvMatrix As File =
'            (From GeneObject As CHUNK_BUFFER_StateEnumerations
'             In GetModalLevel.AsParallel
'             Let IDCol As String() = {GeneObject.UniqueId}
'             Let DataCols As String() = (From n As Integer In GeneObject.Samples Select CStr(n)).ToArray
'             Let Row As String()() = {IDCol, DataCols}
'             Let RowData = Row.Unlist
'             Select CType(RowData, RowObject)).ToArray
'        Return CsvMatrix
'    End Function

'    ''' <summary>
'    '''
'    ''' </summary>
'    ''' <param name="DataChunk">都是对同一个基因的分组数据</param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Private Function __levelMapping(DataChunk As DataSerials(Of Double)(), p As Double) As CHUNK_BUFFER_StateEnumerations
'        Dim LQuery = (From i As Integer In DataChunk.First.Samples.Sequence
'                      Let ChunkBuffer As Double() = (From data0Expr In DataChunk Select data0Expr.Samples(i)).ToArray
'                      Select i, ExprLevel = __quantile(ChunkBuffer, p)
'                      Order By i Ascending).ToArray
'        Call Trace.WriteLine(String.Join(vbTab, (From obj In LQuery Select CStr(obj.ExprLevel)).ToArray))
'        Dim Value As New CHUNK_BUFFER_StateEnumerations With {
'            .Samples = (From data0Expr In LQuery Select CInt(data0Expr.ExprLevel)).ToArray,
'            .UniqueId = DataChunk.First.UniqueId
'        }
'        Return Value
'    End Function

'    Private Function __quantile(X As Double(), p As Double) As Double
'        Dim q As QuantileEstimationGK = X.GKQuantile
'        Dim cutoff As Double = q.Query(p)
'        Return cutoff
'    End Function

'    ''' <summary>
'    ''' 
'    ''' </summary>
'    ''' <param name="Matrix"></param>
'    ''' <param name="Level">映射的等级数目</param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Private Function __ranking(Matrix As IO.File, Level As Integer) As DataSerials(Of Integer)()
'        '首先生成每一个基因的表达曲线
'        Dim DataChunk = (From row As RowObject
'                         In Matrix
'                         Let ID As String = row.First
'                         Where ID.First <> "#"c
'                         Let Data As Double() = (From s As String In row.Skip(1) Select Val(s)).ToArray
'                         Select New CHUNK_BUFFER_EntityQuantities With {.UniqueId = ID, .Samples = Data}).ToArray
'        Dim Vector = (From data0Expr In DataChunk Select data0Expr.Samples).ToArray.Unlist
'        Dim Ranking As Integer() = GenerateMapping(Vector, Level)
'        Dim ChunkBuffer As Integer() = New Integer(DataChunk.First.Samples.Length - 1) {}
'        Dim ChunkList As New List(Of DataSerials(Of Integer))
'        Dim p As i32 = 0

'        For i As Integer = 0 To Ranking.Count - 1 Step ChunkBuffer.Length
'            Call Array.ConstrainedCopy(Ranking, i, ChunkBuffer, 0, ChunkBuffer.Length)

'            i += ChunkBuffer.Length

'            Call ChunkList.Add(New DataSerials(Of Integer) With {.UniqueId = DataChunk(++p).UniqueId, .Samples = ChunkBuffer})
'        Next

'        Return ChunkList.ToArray
'    End Function

'    <ExportAPI("write.csv.node_states")>
'    Public Function WriteNetworkStateData(network As BinaryNetwork, saveto As String) As Boolean
'        Return network.WriteNodeStates(url:=saveto)
'    End Function

'    <ExportAPI("Write.Csv.Regulations")>
'    Public Function WriteRegulationState(Model As BinaryNetwork, <Parameter("SaveTo.Path")> SaveToPath As String) As Boolean
'        Return Model.WriteRegulationValues(url:=SaveToPath)
'    End Function

'    <ExportAPI("write.xml.binary_network")>
'    Public Function SaveNetworkModel(network As BinaryNetwork, saveto As String) As Boolean
'        Return network.get_Model.GetXml.SaveTo(saveto)
'    End Function

'    ''' <summary>
'    ''' 生成网络计算模型的状态输入数据
'    ''' </summary>
'    ''' <param name="NetworkModel"></param>
'    ''' <param name="SaveToPath"></param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    <ExportAPI("Binary_Network.Empty_Inputs.Create()")>
'    Public Function CreateEmptyInput(<Parameter("Network.Model")> NetworkModel As IEnumerable(Of RegulatesFootprints),
'                                     <Parameter("Path.SaveTo",
'                                                "Optional parameter, is this path value not is null then the program " &
'                                                                    "will save the inputs data to the specific file which was indicates by this value.")>
'                                     Optional SaveToPath As String = "") As NetworkInput()
'        Dim Nodes As String() = BinaryNetwork.AnalysisMonteCarloTopLevelInput(NetworkModel)
'        Dim ChunkBuffer As NetworkInput() =
'            (From NodeID As String In Nodes.AsParallel
'             Let InputNode As NetworkInput = New NetworkInput With
'                 {
'                     .locusId = NodeID, .Level = True, .InitQuantity = 1, .NoneRegulation = True}
'             Select InputNode).ToArray
'        If Not String.IsNullOrEmpty(SaveToPath) Then Call ChunkBuffer.SaveTo(SaveToPath, False)
'        Return ChunkBuffer
'    End Function

'    <ExportAPI("binary_network.empty_input.create_all_regulators")>
'    Public Function CreateInput_AllRegulators(model As IEnumerable(Of RegulatesFootprints), saveto As String) As Boolean
'        Dim Nodes = BinaryNetwork.AnalysisMonteCarloTopLevelInput(model)
'        Dim ChunkBuffer = (From id As String In Nodes.AsParallel Select New NetworkInput With {
'                                                                     .locusId = id, .Level = True, .InitQuantity = 1, .NoneRegulation = True}).ToArray
'        Return ChunkBuffer.SaveTo(saveto, False)
'    End Function

'    <ExportAPI("read.csv.network_init_input")>
'    Public Function ReadInputStatus(path As String) As NetworkInput()
'        Return path.LoadCsv(Of NetworkInput)(False).ToArray
'    End Function

'    ''' <summary>
'    ''' 将野生型的蒙特卡洛实验数据之中的后半部分的稳定状态的数据转换为网络输入
'    ''' </summary>
'    ''' <param name="inits">在本参数之中，仅含有<paramref name="MAT"></paramref>之中的一部分数据</param>
'    ''' <param name="MAT"></param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    <ExportAPI("init.generate")>
'    Public Function CreateMutationInit(inits As IEnumerable(Of NetworkInput), MAT As File, Optional samples As Integer = 10) As NetworkInput()
'        Dim SamplesMAT = MAT.LoadData_Double
'        SamplesMAT = SamplesMAT.TakeSamples(start:=SamplesMAT.SampleCounts - samples)  '取最后的samples参数做指定的时间点的数据为计算数据

'        Dim SamplesAverage = (From item In SamplesMAT Select item.UniqueId, item.Samples.Average).ToArray
'        Dim LQuery = (From item In SamplesAverage
'                      Let input = New NetworkInput With {
'                          .locusId = item.UniqueId,
'                          .InitQuantity = item.Average,
'                          .Level = False,
'                          .NoneRegulation = False
'                      }
'                      Select input).ToArray
'        '将inits之中的顶层的没有调控因子的基因进行合并之后返回结果
'        Dim SamplesId = (From item In SamplesAverage Select item.UniqueId).ToArray '获取所有基因的初始输入
'        Dim Uniques = (From item In inits Where Array.IndexOf(SamplesId, item.locusId) = -1 Select item).ToArray

'        Return {LQuery, Uniques}.ToVector      '返回并集
'    End Function

'    Sub New()
'        Call Settings.Initialize()
'    End Sub

'    ''' <summary>
'    ''' 生成指定的基因的蒙特卡洛突变实验的计算实验的所需要的命令行
'    ''' </summary>
'    ''' <param name="GeneIDList">所需要进行突变的基因的编号列表，假若这个参数为空，则只计算野生型的数据</param>
'    ''' <param name="shell"></param>
'    ''' <param name="factor">突变的因素大小，只能够取值0-正无穷之间，当小于0的时候，系统认为是正常表达</param>
'    ''' <param name="Repeats">蒙特卡罗实验的计算次数，默认计算20次</param>
'    ''' <remarks></remarks>
'    <ExportAPI("Monte_Carlo.Commandlines")>
'    Public Sub CommandLineTools(<Parameter("Url.GCML.Csvx", "The file path of the GCModeller xml resource entry file.")> ModelUrl As String,
'                                <Parameter("Kel.Conf", "The kernel configuration file path.")> Conf As String,
'                                <Parameter("Footprints.Source", "The file path of the virtual footprints csv data file.")> Footprints As String,
'                                <Parameter("Shell.Exe", "The executable file path of the shoal shell.")> Shell As String,
'                                <Parameter("Dir.Export", "The directory location of the script file will be export to, this function " &
'                                                              "will export both Windows batch ""MonteCarlo.bat"" and Linux shell " &
'                                                              """MonteCarlo.sh"" for invoke this calculation experiment.")> Export As String,
'                                <Parameter("Kel.Cycles", "The total kernel runtime ticks, default is 3000 cycles.")> Optional KernelCycles As Integer = 3000,
'                                <Parameter("Monte_Carlo.Repeats", "The repeats number of the Monte carlo experiment will running.")> Optional Repeats As Integer = 20,
'                                <Parameter("List.Gene.ID", "A string collection of the gene will be mutation in this experiment.")> Optional GeneIDList As IEnumerable(Of String) = Nothing,
'                                <Parameter("Mutation.Factor", "The mutation factor of the gene in the id list parameter, 0 for deletion mutation, " &
'                                                                   "value greater than 1 will be mutation as overexpression, and any value smaller " &
'                                                                   "than 0 will be treat as normal state.")> Optional Factor As Double = -1)
'        Dim sBatBuilder As StringBuilder = New StringBuilder(1024)
'        Dim sLShBuilder As StringBuilder = New StringBuilder(1024)

'        If Not FileIO.FileSystem.FileExists(ModelUrl) Then
'            Throw New Exception("GCML Csvx xml resource entry file is not found at  " & ModelUrl.ToFileURL)
'        End If

'        If Not FileIO.FileSystem.FileExists(Conf) Then
'            Throw New Exception("GCModeller DFL kernel configuration file is not found at   " & Conf.ToFileURL)
'        End If

'        If Not FileIO.FileSystem.FileExists(Shell) Then
'            Throw New Exception("Shoal Shell executable program file is not found at   " & Shell.ToFileURL)
'        End If

'        If Not FileIO.FileSystem.FileExists(Footprints) Then
'            Throw New Exception("DFL vritual footprints Excel data file is not found at   " & Footprints.ToFileURL)
'        End If

'        '计算野生型的数据
'        If GeneIDList Is Nothing OrElse Factor = 1.0R OrElse Factor < 0 Then
'            Dim ScriptFile As String = "./KernelDriver.shl"

'            Const BATCH_SCRIPT_ARGVS As String = """{0}"" Footprints ""{1}"" Model ""{2}"" Conf ""{3}"" Kernel.Cycles {4} Dir.Export_Result ""{5}"""

'            For i As Integer = 0 To Repeats
'                Dim argvs As String = String.Format(BATCH_SCRIPT_ARGVS, ScriptFile, Footprints, ModelUrl, Conf, KernelCycles, Export)

'                Call sLShBuilder.AppendLine(String.Format("""{0}"" {1} &", Shell, argvs))
'                Call sBatBuilder.AppendLine(String.Format("start /b {0} {1}", Shell, argvs))
'            Next

'            Call ScriptFile.SetValue(Export & "/KernelDriver.shl")
'            Call My.Resources.KernelDriver.SaveTo(ScriptFile)
'        Else
'            Dim ScriptFile As String = "./KernelDriver.[MonteCarloMutation].shl"

'            Const BATCH_SCRIPT_ARGVS As String = """{0}"" Footprints ""{1}"" Model ""{2}"" Conf ""{3}"" Kernel.Cycles {4} Dir.Export_Result ""{5}"" ID.Gene {6} m.Factor {7}"

'            For Each GeneID As String In GeneIDList

'                Dim argvs As String = String.Format(BATCH_SCRIPT_ARGVS, ScriptFile, Footprints, ModelUrl, Conf, KernelCycles, Export, GeneID, Factor)

'                For i As Integer = 0 To Repeats
'                    Call sBatBuilder.AppendLine(String.Format("start /b {0} {1}", Shell, argvs))
'                    Call sLShBuilder.AppendLine(String.Format("""{0}"" {1} &", Shell, argvs))
'                Next
'            Next

'            Call ScriptFile.SetValue(Export & "/KernelDriver.[MonteCarloMutation].shl")
'            Call My.Resources.KernelDriver___GeneMutation__MonteCarlo.SaveTo(ScriptFile)
'        End If

'        Call sBatBuilder.ToString.SaveTo(Export & "/MonteCarlo.Run.bat")
'        Call sLShBuilder.ToString.SaveTo(Export & "/MonteCarlo.Run.sh")
'    End Sub

'    <ExportAPI("monte_carlo.export_commandlines")>
'    Public Function CommandLineTools(dir As String, shell As String) As String
'        Dim Dirs = FileIO.FileSystem.GetDirectories(dir, FileIO.SearchOption.SearchTopLevelOnly)
'        Dim sBuilder As StringBuilder = New StringBuilder(1024)
'        Dim ScriptFile As String = dir & "/MAT.shl"

'        Call My.Resources.ExpressionMAT.SaveTo(ScriptFile)

'        For Each sd As String In Dirs
'            Dim gene As String = FileIO.FileSystem.GetDirectoryInfo(sd).Name

'            Call sBuilder.AppendLine(String.Format("start /b {0} ""{1}"" dir ""{2}/Expression"" gene ""{3}""", shell, ScriptFile, sd, gene))
'        Next

'        Return sBuilder.ToString
'    End Function

'    ''' <summary>
'    ''' 
'    ''' </summary>
'    ''' <param name="dir">计算数据的Export文件夹</param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    ''' 
'    <ExportAPI("MonteCarlo.pfsnet.commandlines")>
'    Public Function CommandLineTools(<Parameter("dir.source")> Dir As String,
'                                     <Parameter("shell.path", "The program file bin path of the shoal shell command")> Shell As String,
'                                     <Parameter("dir.export", "The directory to export the calculation data.")> Export As String,
'                                     <Parameter("kegg.pathways")> KEGGPathways As String,
'                                     <Parameter("java.filter.class")> JavaFilter As String) As String
'        Dim Script As String = My.Resources.pfsnet_kegg_pathways.Replace("[pathways]", KEGGPathways).Replace("[java_filter]", JavaFilter)
'        Dim ScriptPath As String = Export & "/PfsNET.shl"
'        Dim sBuilder As StringBuilder = New StringBuilder(1024)

'        Call Script.SaveTo(ScriptPath)

'        For Each Path As String In FileIO.FileSystem.GetFiles(Dir, FileIO.SearchOption.SearchTopLevelOnly, "*.csv")
'            Dim scriptLine As String = String.Format("start /b {0} ""{1}"" chipdata ""{2}"" save ""{3}/Saved/{4}.txt""", Shell, ScriptPath, Path, Export, FileIO.FileSystem.GetFileInfo(Path).Name)
'            Call sBuilder.AppendLine(scriptLine)
'        Next

'        Return sBuilder.ToString
'    End Function

'    <ExportAPI("MC_PFSNet.Export")>
'    Public Function ExportPfsNET(dir As String, <Parameter("KEGG.Pathways")> KEGGPathways As IEnumerable(Of KEGG.Archives.Csv.Pathway), saveto As String) As Boolean
'        Dim Result = (From file As String
'                      In FileIO.FileSystem.GetFiles(dir, FileIO.SearchOption.SearchTopLevelOnly).AsParallel
'                      Select PfsNET = RTools.PfsNET.LoadResult(file), Regulator = FileIO.FileSystem.GetFileInfo(file).Name).ToArray
'        Dim PathwayDict = (From item In KEGGPathways Select CType(item, PathwayBrief)).ToDictionary(Function(t) t.EntryId)
'        Dim Csv = (From item In Result Select SubNetTable.CreateObject(item.PfsNET, PhenotypeName:=item.Regulator, PathwayBrief:=PathwayDict))
'        Dim Output = (From item As SubNetTable()
'                      In Csv.AsParallel
'                      Select RTools.PfsNET.KEGGPathwaysPhenotypeAnalysis(item, KEGGPathways)).ToArray.ToVector
'        Output = KEGGPhenotypes.CalculateContributions(Output)
'        Call ExportCytoscape(Output, FileIO.FileSystem.GetParentPath(saveto) & "/Network/")
'        Return Output.SaveTo(saveto, False)
'    End Function

'    '<Command("pathway.assign_gene_regulations")>
'    'Public Function AssignPathwayRegulation(Network As Microsoft.VisualBasic.DataVisualization.Network.FileStream.Network,
'    '                                        Regulations As Generic.IEnumerable(Of MEME.MatchedResult),
'    '                                        Pathways As Generic.IEnumerable(Of SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Pathway)) _
'    '    As Microsoft.VisualBasic.DataVisualization.Network.FileStream.Network

'    '    Dim LQuery = (From RegulationRelation As MEME.MatchedResult
'    '                  In Regulations
'    '                  Select (From PathwayId As String In (From strId As String
'    '                                                       In RegulationRelation.OperonGeneIds
'    '                                                       Select (From Pathway In Pathways Where Pathway.ContainsGene(strId) Select Pathway.EntryId).ToArray).ToArray.MatrixToVector
'    '                          Select New Microsoft.VisualBasic.DataVisualization.Network.FileStream.NetworkNode With
'    '                                 {
'    '                                     .FromNode = RegulationRelation.TF, .ToNode = PathwayId, .InteractionType = "Regulation"}).ToArray).ToArray.MatrixToVector
'    '    Dim Regulators = (From tfId As String
'    '                      In (From item As MEME.MatchedResult In Regulations Select item.TF Distinct).ToArray
'    '                      Select New Microsoft.VisualBasic.DataVisualization.Network.FileStream.Node With
'    '                             {
'    '                                 .NodeType = "TF", .Identifier = tfId}).ToArray

'    '    Network.Edges = {Network.Edges, LQuery}.MatrixToVector
'    '    Network.Nodes = {Network.Nodes, Regulators}.MatrixToVector
'    '    Return Network
'    'End Function

'    '<Command("pathway.assign_phenotype")>
'    'Public Function AssignPhenotype(Network As Microsoft.VisualBasic.DataVisualization.Network.FileStream.Network) As Pathway()
'    '    Dim Pathways = (From NodeItem As Microsoft.VisualBasic.DataVisualization.Network.FileStream.Node
'    '                    In Network.Nodes
'    '                    Where String.Equals(NodeItem.NodeType, PATHWAY_NODE_TYPE)
'    '                    Select NodeItem.Identifier).ToArray
'    '    Dim PathwayFunctions As Dictionary(Of String, SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.PathwayBriteMenuText) =
'    '        SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.PathwayBriteMenuText.LoadFromResource.ToDictionary(
'    '            Function(item As SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.PathwayBriteMenuText) item.EntryId)
'    '    Dim LQuery = (From PathwayId As String
'    '                  In Pathways
'    '                  Let [Class] As SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.PathwayBriteMenuText = PathwayFunctions(Regex.Match(PathwayId, "\d{5}").Value)
'    '                  Select New Pathway With
'    '                         {
'    '                             .Identifier = PathwayId, .NodeType = PATHWAY_NODE_TYPE, .PhenotypeCategory = [Class].Category, .PhenotypeClass = [Class].Class}).ToArray
'    '    Return LQuery
'    'End Function

'    'Public Class Phenotype : Inherits Microsoft.VisualBasic.Datavisualization.Network.FileStream.Node
'    '    Public Property PathwayCounts As Integer
'    '    Public Property RegulatorCounts As Integer
'    '    Public Property RegulatorFamilyDistributions As String()
'    'End Class

'    ''' <summary>
'    ''' 仅仅只是根据调控数据将表型调控赋值给指定的调控因子,没有计算得分的过程
'    ''' </summary>
'    ''' <param name="Regulations"></param>
'    ''' <param name="Pathways"></param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    <ExportAPI("phenotype.regulations")>
'    Public Function AssignPhenotype(Regulations As IEnumerable(Of IRegulatorRegulation), Pathways As IEnumerable(Of KEGG.Archives.Csv.Pathway)) As File
'        Dim RegulationData = Regulations.Trim
'        Dim Regulators As String() = RegulationData.GetRegulators
'        Dim Scores = RegulationData.SignificantModel
'        Dim Phenotypes = (From strPhenotype As String In (From item In Pathways Select item.Category Distinct).AsParallel
'                          Let PhenotypeRelateGene = (From item In Pathways Where String.Equals(item.Category, strPhenotype)
'                                                     Where Not item.PathwayGenes.IsNullOrEmpty
'                                                     Select item.PathwayGenes).ToVector.Distinct.ToArray
'                          Let PhenoTypeRegulators = (From locusId As String
'                                                     In PhenotypeRelateGene
'                                                     Let GeneRegulators = (From item In Scores Where String.Equals(item.InteractorB, locusId) Select item).ToArray
'                                                     Select GeneRegulators).ToVector
'                          Select New __phenotype With {
'                              .Phenotype = strPhenotype,
'                              .regulateGenes = PhenotypeRelateGene,
'                              .numOfRegulators = (From item In PhenoTypeRegulators Select item.InteractorA Distinct).Count,
'                              .regulators = PhenoTypeRegulators
'                          }).ToArray
'        Dim GenerateCsv = (From RegulatorId As String In Regulators Select __phenotypeRegulats(RegulatorId, Phenotypes)).ToArray
'        Dim Head As RowObject = New RowObject From {"Regulator"}
'        Call Head.AddRange((From item In Phenotypes Select item.Phenotype).ToArray)
'        Dim Csv As File = New File
'        Call Csv.AppendLine(Head)
'        Call Csv.AppendRange(GenerateCsv)

'        Return Csv
'    End Function

'    Private Structure __phenotype
'        Dim Phenotype As String
'        Dim regulateGenes As String()
'        Dim numOfRegulators As Integer
'        Dim regulators As RelationshipScore()
'    End Structure

'    Private Function __phenotypeRegulats(regulatorId As String, phenotypes As __phenotype()) As RowObject
'        Dim Row As RowObject = New RowObject From {regulatorId}
'        Call Row.AddRange((From Phenotype As __phenotype
'                           In phenotypes
'                           Let score As Double() = (From regulator As RelationshipScore
'                                                    In Phenotype.regulators
'                                                    Where String.Equals(regulator.InteractorA, regulatorId) AndAlso
'                                                        Array.IndexOf(Phenotype.regulateGenes, regulator.InteractorB) > -1
'                                                    Select regulator.Score).ToArray
'                           Select If(score.IsNullOrEmpty, "0", CStr(score.Sum))).ToArray)
'        Return Row
'    End Function

'    ''' <summary>
'    ''' 仅仅只是根据调控数据将表型调控赋值给指定的调控因子,没有计算得分的过程
'    ''' </summary>
'    ''' <param name="Regulations"></param>
'    ''' <param name="Pathways"></param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    <ExportAPI("regulators.assign.phenotype", Info:="function only use for assign the phenotype regulation information, no scoring.")>
'    Public Function AssignPhenotype2(Regulations As IEnumerable(Of IRegulatorRegulation), Pathways As IEnumerable(Of KEGG.Archives.Csv.Pathway), EXPORT As String) As Boolean
'        Dim RegulationData = Regulations.Trim
'        Dim Regulators As String() = RegulationData.GetRegulators
'        Dim Phenotypes = (From strPhenotype As String
'                          In (From item In Pathways Select item.Category Distinct).ToArray.AsParallel
'                          Let PhenotypeRelateGene = (From item In Pathways Where String.Equals(item.Category, strPhenotype) Where Not item.PathwayGenes.IsNullOrEmpty Select item.PathwayGenes).ToArray.ToVector.Distinct.ToArray
'                          Let PhenoTypeRegulators = (From GeneId As String In PhenotypeRelateGene Let GeneRegulators = (From item In RegulationData Where String.Equals(item.LocusId, GeneId) Let RegulatorIdColection = item.Regulators Select RegulatorIdColection).ToArray.ToVector Select GeneRegulators).ToArray.ToVector
'                          Select Phenotype = strPhenotype, PhenotypeRelateGene, RegulatorCounts = PhenoTypeRegulators.Count, PhenoTypeRegulators).ToArray
'        Dim LQuery = (From phenotype In Phenotypes
'                      Select (From RegulatorId As String In phenotype.PhenoTypeRegulators Select New NetworkEdge With {.fromNode = RegulatorId, .interaction = "PhenotypeRegulation", .toNode = phenotype.Phenotype}).ToArray).ToArray.ToVector
'        Dim RegulatorNodes = (From RegulatorId As String In Regulators Select New FileStream.Node With {.ID = RegulatorId, .NodeType = "Regulator"}).ToArray
'        Dim PhenotypeNodes = (From phenotype In Phenotypes Select New FileStream.Node With {.ID = phenotype.Phenotype, .NodeType = "Cell.Phenotype"}).ToArray

'        Call LQuery.SaveTo(EXPORT & "/Edges.csv", False)
'        Call {RegulatorNodes, PhenotypeNodes}.ToVector.SaveTo(EXPORT & "/Nodes.csv", False)

'        Return True
'    End Function

'    <ExportAPI("ranked.phenotype_regulations", Info:="Rank_score_cutoff, 0-1, 0 for export all relationships")>
'    Public Function ImportantPhenotypeRegulators(RankedRegulations As IEnumerable(Of RankRegulations),
'                                                 Export As String,
'                                                 Pathways As IEnumerable(Of KEGG.Archives.Csv.Pathway),
'                                                 Optional Rank_score_cutoff As Double = 0.65) As KeyValuePair(Of String, KeyValuePair(Of Integer, String)())()

'        Dim Phenotypes = (From strPhenotype As String
'                          In (From item In Pathways Select item.Category Distinct).ToArray.AsParallel
'                          Let PhenotypeRelateGene As String() = (From item As KEGG.Archives.Csv.Pathway
'                                                                 In Pathways
'                                                                 Where String.Equals(item.Category, strPhenotype) AndAlso
'                                                                     Not item.PathwayGenes.IsNullOrEmpty
'                                                                 Select item.PathwayGenes).ToVector.Distinct.ToArray
'                          Let PhenoTypeRankedRegulators = (From GeneId As String In PhenotypeRelateGene
'                                                           Let GeneRegulators As RankRegulations() = (From item As RankRegulations In RankedRegulations
'                                                                                                      Where Array.IndexOf(item.GeneCluster, GeneId) > -1
'                                                                                                      Select item).ToArray
'                                                           Select GeneRegulators).ToVector
'                          Let PhenotypeRegulators = (From item As RankRegulations
'                                                     In PhenoTypeRankedRegulators
'                                                     Select RankScore = item.RankScore,
'                                                         Regulators = item.Regulators).ToArray
'                          Select Phenotype = strPhenotype,
'                              PhenotypeRelateGene,
'                              numOfRegulator = PhenotypeRegulators.Length,
'                              PhenotypeRegulators).ToArray
'        Dim EvaluateRanks = (From Phenotype In Phenotypes Let Regulators = (From RegulatorId As String
'                                                                            In (From item In Phenotype.PhenotypeRegulators Select item.Regulators).ToArray.ToVector.Distinct.ToArray
'                                                                            Let rs = (From item In Phenotype.PhenotypeRegulators Where Array.IndexOf(item.Regulators, RegulatorId) > -1 Select item.RankScore).ToArray.Sum
'                                                                            Select RankedScore = rs, Regulator = RegulatorId).ToArray
'                             Let cutoff = If(Regulators.IsNullOrEmpty, 0, (From Regulator In Regulators Select Regulator.RankedScore).ToArray.Max * Rank_score_cutoff)
'                             Select Phenotype = Phenotype.Phenotype,
'                                    Regulators = (From Regulator In Regulators Where Regulator.RankedScore >= cutoff Select Regulator)).ToArray

'        Dim PhenotypeRegulations = (From Phenotype In EvaluateRanks
'                                    Select (From Regulator
'                                            In Phenotype.Regulators
'                                            Select New FileStream.NetworkEdge With {
'                                                .fromNode = Regulator.Regulator,
'                                                .toNode = Phenotype.Phenotype,
'                                                .value = Regulator.RankedScore,
'                                                .interaction = "Phenotype.Regulations"})).ToVector
'        Dim PhenotypeNodes = (From Phenotype
'                              In EvaluateRanks
'                              Select New FileStream.Node With {
'                                  .ID = Phenotype.Phenotype, .NodeType = "Cell.Phenotype"}).ToArray
'        Dim PhenotypeRegulatorNodes = (From Phenotype
'                                       In EvaluateRanks
'                                       Select (From Regulator
'                                               In Phenotype.Regulators
'                                               Select New FileStream.Node With {
'                                                   .ID = Regulator.Regulator,
'                                                   .NodeType = "Regulator"}).ToArray).ToArray.ToVector

'        Call PhenotypeRegulations.SaveTo(Export & "/Edges.csv", False)
'        Call {PhenotypeNodes, PhenotypeRegulatorNodes}.ToVector.SaveTo(Export & "/Nodes.csv", False)

'        Dim DetailsCsv As New IO.File
'        Call DetailsCsv.Add(New String() {"Phenotype", "Rank Scores"})
'        Call DetailsCsv.AppendRange(From Phenotype In EvaluateRanks
'                                    Let Regulators = (From item In Phenotype.Regulators Select String.Format("{0} ({1})", item.Regulator, item.RankedScore)).ToArray
'                                    Let row = New RowObject({Phenotype.Phenotype, String.Join("; ", Regulators)})
'                                    Select row)

'        Call DetailsCsv.Save(Export & "/RankScores.csv", False)

'        Return (From row In EvaluateRanks
'                Let ScoredRegulators = (From item In row.Regulators Select New KeyValuePair(Of Integer, String)(item.RankedScore, item.Regulator)).ToArray
'                Select New KeyValuePair(Of String, KeyValuePair(Of Integer, String)())(row.Phenotype, ScoredRegulators)).ToArray
'    End Function

'    <ExportAPI("family.statics")>
'    Public Function FamilyStatics(ranks As IEnumerable(Of KeyValuePair(Of String, KeyValuePair(Of Integer, String)())), bh As IEnumerable(Of RegpreciseMPBBH)) As IO.File
'        Dim RegulatorFamilies = (From RegulatorId As String
'                                 In (From item In bh
'                                     Where Not String.IsNullOrEmpty(item.HitName)
'                                     Select item.QueryName
'                                     Distinct).ToArray.AsParallel
'                                 Let Matches = (From item In bh Where String.Equals(item.QueryName, RegulatorId) Select item.Family Distinct).ToArray
'                                 Select RegulatorId, Family = Matches).ToArray.ToDictionary(Function(m) m.RegulatorId)
'        Dim LQuery = (From Phenotype In ranks.AsParallel
'                      Let Families = (From item
'                                      In Phenotype.Value
'                                      Let FamilyList = RegulatorFamilies(item.Value).Family
'                                      Select (From Family
'                                              In FamilyList
'                                              Select Family, item.Key).ToArray).ToArray.ToVector
'                      Let UniqueFamily = (From item In Families Select item.Family.ToLower Distinct).ToArray
'                      Select phenotype = Phenotype.Key, FamilyScores = (From Family As String
'                                                                        In UniqueFamily
'                                                                        Let ScoredData = (From item In Families Where String.Equals(Family, item.Family, StringComparison.OrdinalIgnoreCase) Select item).ToArray
'                                                                        Where Not ScoredData.IsNullOrEmpty
'                                                                        Select ScoredData(0).Family, Scores = (From item In ScoredData Select item.Key).ToArray.Sum Order By Scores Descending).ToArray).ToArray
'        Dim Result As IO.File = New IO.File
'        Call Result.Add(New String() {"Phenotype", "Regulator-Family-Scores"})
'        Call Result.AppendRange(From Phenotype In LQuery
'                                Let Scores = (From item In Phenotype.FamilyScores Select String.Format("{0} ({1})", item.Family, item.Scores)).ToArray
'                                Select New RowObject({Phenotype.phenotype, String.Join("; ", Scores)}))
'        Return Result
'    End Function

'    '<Command("significant_phenotype_regulators")>
'    'Public Function SignificantPhenotypeRegulator(RegulatorPhenotypeRegulations As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File) _
'    '    As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File

'    '    Dim PhenotypeRegulators As String()() = (From row In RegulatorPhenotypeRegulations Select row.ToArray).ToArray.MatrixTranspose
'    '    Dim Regulators = PhenotypeRegulators.First.Skip(1).ToArray
'    '    Dim Count = Regulators.Count * 0.05

'    '    Dim Head As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Row = New DocumentFormat.Csv.DocumentStream.File.Row From {"Phenotype", "Possible-significant-regulators"}
'    '    Dim Csv As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File = New DocumentFormat.Csv.DocumentStream.File From {Head}

'    '    Call Csv.AppendRange((From Line In PhenotypeRegulators.Skip(1) Let RegulatorScores = (From i As Integer
'    '                                                                                          In Regulators.Sequence
'    '                                                                                          Let Score = Val(Line(i + 1))
'    '                                                                                          Where Score >= 0.6
'    '                                                                                          Select Regulator = Regulators(i), Score Order By Score Descending).ToArray.Take(Count).ToArray
'    '                                                                    Let GenerateLine = Function() As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Row
'    '                                                                                           Dim Row As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File.Row = New DocumentFormat.Csv.DocumentStream.File.Row From {Line.First}
'    '                                                                                           Call Row.Add(String.Join("; ", (From item In RegulatorScores Select String.Format("{0}({1})", item.Regulator, item.Score)).ToArray))
'    '                                                                                           Return Row
'    '                                                                                       End Function Select GenerateLine()).ToArray)

'    '    Return Csv
'    'End Function

'    <ExportAPI("Export.Net.Cytoscape")>
'    Public Function ExportCytoscapeNetwork(SignificantPhenotypeRegulations As File, ExportDIR As String) As Boolean
'        Dim PhenotypeRegulations = (From Line In SignificantPhenotypeRegulations.Skip(1).AsParallel Where Not String.IsNullOrEmpty(Line.Last) Select Phenotype = Line.First, Regulators = (From item In Strings.Split(Line.Last, "; ") Select Regulator = item.Split(CChar("(")).First, Score = Val(Regex.Match(item, "\d+\.\d+").Value)).ToArray).ToArray
'        Dim Phenotypes = (From item In PhenotypeRegulations.AsParallel Select New FileStream.Node With {.ID = item.Phenotype, .NodeType = "PhenotypePathway"}).ToArray
'        Dim PhenotypeSignificantRegulators = (From RegulatorId As String In (From item In PhenotypeRegulations Select (From Regulator In item.Regulators Select Regulator.Regulator).ToArray).ToArray.ToVector.Distinct.ToArray.AsParallel Select New FileStream.Node With {.ID = RegulatorId, .NodeType = "Regulator"}).ToArray
'        Dim PhenotypeRegulationsEdges = (From Phenotype In PhenotypeRegulations.AsParallel
'                                         Select (From Regulator In Phenotype.Regulators
'                                                 Select New NetworkEdge With {
'                                                     .fromNode = Regulator.Regulator,
'                                                     .toNode = Phenotype.Phenotype,
'                                                     .interaction = "PhenotypeRegulation",
'                                                     .value = Regulator.Score}).ToArray).ToArray.ToVector

'        Call PhenotypeRegulationsEdges.SaveTo(String.Format("{0}/Edges.csv", ExportDIR), False)
'        Call {Phenotypes, PhenotypeSignificantRegulators}.ToVector.SaveTo(ExportDIR & "/Nodes.csv", False)

'        Return True
'    End Function

'    Private Function StaticsFamilyDistributions(Distribution As KeyValuePair(Of String, String)()) As String()
'        Dim Families = (From item In Distribution Select item.Value Distinct).ToArray
'        Dim FamilyDistributions = (From Family As String In Families.AsParallel
'                                   Let Regulators As String() = (From item In Distribution Where String.Equals(item.Value, Family) Select item.Key Distinct).ToArray
'                                   Select String.Join("=", Family, Regulators.Count)).ToArray
'        Return FamilyDistributions
'    End Function

'    'Public Class Reaction : Inherits Microsoft.VisualBasic.Datavisualization.Network.FileStream.Node
'    '    Public Property EC As String

'    'End Class

'    '<Command("get.kegg_reactions")>
'    'Public Function GetKEGGReactions(KEGGPathways As Generic.IEnumerable(Of SMRUCC.genomics.Assembly.KEGG.WebServices.Modules.Module),
'    '                                 Reactions As Generic.IEnumerable(Of SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Reaction)) _
'    '    As SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Reaction()

'    '    Dim ReactionDict = Reactions.ToDictionary(Function(Reaction As SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Reaction) Reaction.Entry)
'    '    Dim ReactionList = (From Pathway In KEGGPathways.AsParallel Where Not Pathway.Reaction.IsNullOrEmpty Select (From item In Pathway.Reaction Select item.Key).ToArray).ToArray.MatrixToVector.Distinct.ToArray
'    '    Dim LQuery = (From itemId As String In ReactionList Select ReactionDict(itemId)).ToArray
'    '    Return LQuery
'    'End Function

'    'Public Function ModuleInteractions(KEGGModules As Generic.IEnumerable(Of SMRUCC.genomics.Assembly.KEGG.WebServices.Modules.Module),
'    '                                   ReactionInteractions As Generic.IEnumerable(Of Microsoft.VisualBasic.Datavisualization.Network.FileStream.NetworkNode)) _
'    '    As Microsoft.VisualBasic.Datavisualization.Network.FileStream.NetworkNode()

'    '    Dim LQuery = (From [Module] As SMRUCC.genomics.Assembly.KEGG.WebServices.Modules.Module
'    '                  In KEGGModules
'    '                  Where Not [Module].Reaction.IsNullOrEmpty
'    '                  Select (From InteractedModule As String In (From Reaction As SMRUCC.genomics.ComponentModel.KeyValuePair
'    '                                                              In [Module].Reaction
'    '                                                              Select (From ModuleItem As SMRUCC.genomics.Assembly.KEGG.WebServices.Modules.Module
'    '                                                                      In KEGGModules
'    '                                                                      Where ModuleItem.ContainsReaction(Reaction.Key)
'    '                                                                      Select [Module].EntryId).ToArray).ToArray.MatrixToVector
'    '                          Select New Microsoft.VisualBasic.Datavisualization.Network.FileStream.NetworkNode With
'    '                                 {
'    '                                     .FromNode = [Module].EntryId, .ToNode = InteractedModule, .InteractionType = "Module.Reaction.Interactions"}).ToArray
'    '                  ).ToArray.MatrixToVector
'    '    Return LQuery
'    'End Function

'    '<Command("build.reaction_interactions")>
'    'Public Function AssemblyInteractions(KEGGReactions As Generic.IEnumerable(Of SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Reaction)) _
'    '    As Microsoft.VisualBasic.Datavisualization.Network.FileStream.Network

'    '    Dim Reactions = (From item In KEGGReactions Select item.Entry, Substrates = item.GetSubstrateCompounds).ToArray
'    '    Dim Compounds = (From item In Reactions Select item.Substrates).ToArray.MatrixToVector.Distinct.ToArray
'    '    Dim ReactionInteractions As Microsoft.VisualBasic.Datavisualization.Network.FileStream.NetworkNode() = (
'    '        From Pair As KeyValuePair(Of String, String)
'    '        In (From CompoundId As String
'    '            In Compounds
'    '            Let ReactionList = (From item In Reactions
'    '                                Where Array.IndexOf(item.Substrates, CompoundId) > -1
'    '                                Select item.Entry).ToArray
'    '            Select Microsoft.VisualBasic.ComponentModel.Comb(Of String).CreateObject(ReactionList).CombList).ToArray.MatrixToVector.MatrixToVector
'    '        Select New Microsoft.VisualBasic.Datavisualization.Network.FileStream.NetworkNode With
'    '               {
'    '                   .FromNode = Pair.Key, .ToNode = Pair.Value, .InteractionType = "Reaction.Interactions"}).ToArray

'    '    Dim ReactionNodes As Microsoft.VisualBasic.Datavisualization.Network.FileStream.Node() = (
'    '        From item In Reactions
'    '        Select New Microsoft.VisualBasic.Datavisualization.Network.FileStream.Node With
'    '               {
'    '                   .Identifier = item.Entry, .NodeType = "Reaction"}).ToArray

'    '    Return New Microsoft.VisualBasic.Datavisualization.Network.FileStream.Network With {.Nodes = ReactionNodes, .Edges = ReactionInteractions}
'    'End Function
'End Module
