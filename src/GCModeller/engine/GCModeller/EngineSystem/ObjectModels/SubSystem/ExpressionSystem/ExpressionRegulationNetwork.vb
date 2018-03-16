#Region "Microsoft.VisualBasic::388ca4b685c242ebd726b15045032bb1, engine\GCModeller\EngineSystem\ObjectModels\SubSystem\ExpressionSystem\ExpressionRegulationNetwork.vb"

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

    '     Class ExpressionRegulationNetwork
    ' 
    '         Properties: _, BasalExpression, DataSource, ExpressionKinetics, NetworkComponents
    '                     RegulationCoverage, RegulationEntitiesCount, RibosomeAssembly, RNAPolymeraseAssembly, TranslationDataSource
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: CreateServiceSerials, get_DataSerializerHandles, Initialize
    ' 
    '         Sub: MemoryDump, SetupMutation
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Serialization
Imports SMRUCC.genomics.GCModeller.Assembly
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL
Imports SMRUCC.genomics.GCModeller.ModellingEngine.PlugIns

Namespace EngineSystem.ObjectModels.SubSystem.ExpressionSystem

    <ISystemFrameworkEntry(ModelName:="svd", Type:=SMRUCC.genomics.GCModeller.ModellingEngine.PlugIns.ISystemFrameworkEntry.Types.ExpressionRegulationNetwork,
       Authors:="xie.guigang@gmail.com", Description:="A mathematical implement for the svd gene expression regulation network model.")>
    Public Class ExpressionRegulationNetwork : Inherits SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.SubSystemContainer(Of EngineSystem.ObjectModels.Module.CentralDogmaInstance.CentralDogma)
        Implements SMRUCC.genomics.GCModeller.ModellingEngine.PlugIns.ISystemFrameworkEntry.ISystemFramework

        <DumpNode>
        Protected Friend _InternalTranscriptsPool As SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Entity.Transcript()
        ''' <summary>
        ''' <see cref="ExpressionRegulationNetwork._InternalTranscriptsPool">RNA分子</see>的降解系统
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend _InternalTranscriptDisposableSystem As SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.DisposerSystem(Of Entity.Transcript)

        Public Property ExpressionKinetics As MathematicsModels.EnzymeKinetics.ExpressionKinetics

        ''' <summary>
        ''' 主要是为了方便进行基因突变的实验而设置的一个对象列表
        ''' </summary>
        ''' <remarks></remarks>
        <DumpNode> Protected Friend _InternalEvent_Transcriptions As EngineSystem.ObjectModels.Module.CentralDogmaInstance.Transcription()
        <DumpNode> Protected Friend ReadOnly Property _
                                    _InternalEvent_Translations__ As EngineSystem.ObjectModels.Module.CentralDogmaInstance.Translation()
            Get
                Return BasalExpression.BasalTranslationFluxs
            End Get
        End Property

        ''' <summary>
        ''' 转录所需要的RNA聚合酶复合物
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend RNAPolymerase As EngineSystem.ObjectModels.Feature.MetabolismEnzyme()
        ''' <summary>
        ''' 翻译所需要的核糖体复合物
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend RibosomeAssemblyCompound As EngineSystem.ObjectModels.Feature.MetabolismEnzyme
        Public Property RibosomeAssembly As EngineSystem.ObjectModels.SubSystem.RibosomalAssembly
        Public Property RNAPolymeraseAssembly As EngineSystem.ObjectModels.SubSystem.RNAPolymeraseAssembly
        Public Property BasalExpression As BasalExpressionKeeper

        Protected Friend _TranscriptionK1 As Double, _TranslationK1 As Double

        ''' <summary>
        ''' 返回每一个基因的转录量的和
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property DataSource As DataSource()
            Get
                Dim LQuery = (From ExpressionObject As EngineSystem.ObjectModels.Module.CentralDogmaInstance.CentralDogma
                              In MyBase._DynamicsExprs
                              Let sum = (From item In ExpressionObject.Transcriptions Select item.FluxValue).Sum
                              Select New DataSource(ExpressionObject.Handle, sum)).ToArray
                Return LQuery
            End Get
        End Property

        ''' <summary>
        ''' 获取每一种mRNA分子的实时表达量
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TranslationDataSource As KeyValuePair(Of Long, Double)()
            Get
                Dim LQuery = (From TranslationEvent As [Module].CentralDogmaInstance.Translation
                              In _InternalEvent_Translations__
                              Select New KeyValuePair(Of Long, Double)(TranslationEvent.Handle, TranslationEvent.FluxValue)).ToArray
                Return LQuery
            End Get
        End Property

        Public Overrides ReadOnly Property NetworkComponents As SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.CentralDogmaInstance.CentralDogma()
            Get
                Return MyBase._DynamicsExprs
            End Get
        End Property

        Sub New(CellSystem As SubSystem.CellSystem)
            Call MyBase.New(CellSystem)
        End Sub

        Public Overrides Function Initialize() As Integer
            Me._ExpressionKinetics = New MathematicsModels.EnzymeKinetics.ExpressionKinetics(_CellSystem.Metabolism)
            Me._TranscriptionK1 = Val(_CellSystem.get_runtimeContainer.SystemVariable(GCMarkupLanguage.GCML_Documents.ComponentModels.SystemVariables.PARA_TRANSCRIPTION))
            Me._TranslationK1 = Val(_CellSystem.get_runtimeContainer.SystemVariable(GCMarkupLanguage.GCML_Documents.ComponentModels.SystemVariables.PARA_TRANSLATION))

            If Me._TranscriptionK1 = 0.0R Then
                Throw New DataException("Kinetics parameter for transcription is ZERO, could' not initialize!")
            End If
            If Me._TranslationK1 = 0.0R Then
                Throw New DataException("Kinetics parameter for translation is ZERO, could' not initialize!")
            End If

            Dim n = New EngineSystem.ObjectModels.SubSystem.ExpressionSystem.Initializer(System:=Me).Invoke
            Dim EventTranscriptionList As New List(Of EngineSystem.ObjectModels.Module.CentralDogmaInstance.Transcription)
            Dim EventTranslationList As New List(Of EngineSystem.ObjectModels.Module.CentralDogmaInstance.Translation)

            For Each item As EngineSystem.ObjectModels.Module.CentralDogmaInstance.CentralDogma In MyBase._DynamicsExprs
                Call EventTranscriptionList.AddRange(item.Transcriptions)
            Next

            Me._InternalEvent_Transcriptions = EventTranscriptionList.WriteAddress.ToArray

            Me._InternalTranscriptDisposableSystem = New DisposerSystem(Of Entity.Transcript)(Me._InternalTranscriptsPool, Me._CellSystem.Metabolism, Entity.IDisposableCompound.DisposableCompoundTypes.Transcripts)
            Call Me._InternalTranscriptDisposableSystem.Initialize()
            Me.RibosomeAssembly = New RibosomalAssembly(_CellSystem.Metabolism)
            Call Me.RibosomeAssembly.Initialize()
            Me.RNAPolymeraseAssembly = New RNAPolymeraseAssembly(_CellSystem.Metabolism)
            Call Me.RNAPolymeraseAssembly.Initialize()
            '    Call New MathematicsModels.SVD(Me).CreateMatrix()
            Call _CellSystem._InternalEventDriver.JoinEvents(_InternalTranscriptDisposableSystem.NetworkComponents)
            Call _CellSystem._InternalEventDriver.JoinEvents(Me.NetworkComponents)

            Call _SystemLogging.WriteLine(String.Format("Expression regulation coverage in this virtual cell model is {0}%({1}/{2})", RegulationCoverage, RegulationEntitiesCount, _CellSystem.DataModel.BacteriaGenome.OperonCounts))

            Return n
        End Function

        ''' <summary>
        ''' 在本计算模型之中的基因表达调控网络之中的被调控的基因的数目
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property RegulationCoverage As Double
            Get
                Return 100 * RegulationEntitiesCount / _CellSystem.DataModel.BacteriaGenome.OperonCounts
            End Get
        End Property

        ''' <summary>
        ''' Gets the gene object counts value in current gene expression regulation network.(获取被调控网络对象之中的被调控的基因的节点对象的数目)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property RegulationEntitiesCount As Integer
            Get
                Return (From data0expr In _DynamicsExprs Where Not data0expr.MotifSites.IsNullOrEmpty Select 1).ToArray.Length
            End Get
        End Property

        ''' <summary>
        ''' 用于调试的监视变量
        ''' </summary>
        ''' <remarks></remarks>
        Dim MutatedGenes As List(Of Entity.Transcript) = New List(Of Entity.Transcript)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="UniqueId"></param>
        ''' <param name="Factor">大于0的一个数字，当值为0的时候表示缺失突变，当值为1的时候表示正常表达，当值大于1的时候表示过量表达</param>
        ''' <remarks></remarks>
        Public Sub SetupMutation(UniqueID As String, Factor As Double)
            If Factor < 0 Then
                Call MyBase.SystemLogging.WriteLine(String.Format("Factor could not be set negative!  {0} -> {1}x", UniqueID, Factor), "Expression_Network_Initialize_Mutation()", Type:=MSG_TYPES.ERR)
                Return
            End If

            Dim Transcriptions = (From item In Me._InternalEvent_Transcriptions Where String.Equals(UniqueID, item.Product.Identifier, StringComparison.OrdinalIgnoreCase) Select item).ToArray

            If Factor > 1 Then
                Call MyBase.SystemLogging.WriteLine(String.Format("  >> {0} was setup to over express by factor {1}", UniqueID, Factor))
            Else
                If Factor = 0.0R Then
                    Call MyBase.SystemLogging.WriteLine(String.Format("  >> {0} was setup to deleted mutation", UniqueID))
                Else
                    Call MyBase.SystemLogging.WriteLine(String.Format("  >> {0} was setup to suppressed express by factor {1}", UniqueID, Factor))
                End If
            End If

            For i As Integer = 0 To Transcriptions.Count - 1
                Transcriptions(i).CoefficientFactor = Factor
            Next

            Call MutatedGenes.Add(Me._InternalTranscriptsPool.GetItem(UniqueID))
        End Sub

        Public Overrides Function CreateServiceSerials() As IDataAcquisitionService()
            Dim ServiceList = New IDataAcquisitionService() _
                {
                    New DataAcquisitionService(
                        Adapter:=New Services.DataAcquisition.DataAdapters.Transcriptome(Me), RuntimeContainer:=Me.IRuntimeContainer),
 _
                    New DataAcquisitionService(
                        Adapter:=New Services.DataAcquisition.DataAdapters.TranscriptionFlux(Me), RuntimeContainer:=Me.IRuntimeContainer)}.AsList
            Call ServiceList.AddRange(Me._InternalTranscriptDisposableSystem.CreateServiceSerials)
            Call ServiceList.Add(New DataAcquisitionService(
                                 Adapter:=New EngineSystem.Services.DataAcquisition.DataAdapters.TranscriptionRegulation(Me),
                                 RuntimeContainer:=Me.IRuntimeContainer))
            Call ServiceList.Add(New DataAcquisitionService(
                               Adapter:=New EngineSystem.Services.DataAcquisition.DataAdapters.TranslationRegulation(Me),
                               RuntimeContainer:=Me.IRuntimeContainer))

            Call ServiceList.Add(New DataAcquisitionService(New EngineSystem.Services.DataAcquisition.DataAdapters.RibosomeAssembly(Me.RibosomeAssembly), IRuntimeContainer))
            Call ServiceList.Add(New DataAcquisitionService(New EngineSystem.Services.DataAcquisition.DataAdapters.RibosomeAssemblyCompounds(Me.RibosomeAssembly), IRuntimeContainer))
            Call ServiceList.Add(New DataAcquisitionService(New EngineSystem.Services.DataAcquisition.DataAdapters.RNAPolymeraseAssembly(Me.RNAPolymeraseAssembly), IRuntimeContainer))
            Call ServiceList.Add(New DataAcquisitionService(New EngineSystem.Services.DataAcquisition.DataAdapters.RNAPolymeraseAssemblyCompounds(Me.RNAPolymeraseAssembly), IRuntimeContainer))
            Call ServiceList.Add(New DataAcquisitionService(New EngineSystem.Services.DataAcquisition.DataAdapters.TFRegulators(Me), IRuntimeContainer))
            Call ServiceList.AddRange(BasalExpression.CreateServiceSerials)

            Return ServiceList.ToArray
        End Function

        Public Overrides Sub MemoryDump(Dir As String)
            Call _InternalTranscriptDisposableSystem.MemoryDump(Dir)
            Call Me.I_CreateDump.SaveTo(String.Format("{0}/{1}.log", Dir, Me.GetType.Name))
        End Sub

        Public Overrides Function get_DataSerializerHandles() As HandleF()
            Return (From Item In MyBase._DynamicsExprs Select Item.SerialsHandle).ToArray
        End Function
    End Class
End Namespace
