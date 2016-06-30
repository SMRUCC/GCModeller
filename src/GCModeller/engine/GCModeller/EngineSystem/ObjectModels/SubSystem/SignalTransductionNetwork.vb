
Imports System.Text.RegularExpressions
Imports LANS.SystemsBiology.DatabaseServices.SabiorkKineticLaws.TabularDump
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.MySQL

Namespace EngineSystem.ObjectModels.SubSystem

    Public Class SignalTransductionNetwork : Inherits DelegateSystem

        ''' <summary>
        ''' [MCP][CH3] -> MCP + -CH3  Enzyme:[CheB][PI]
        ''' 
        ''' Protein L-glutamate O(5)-methyl ester + H(2)O = protein L-glutamate + methanol
        ''' C00132
        '''
        ''' METOH
        '''
        ''' </summary>
        ''' <remarks></remarks>
        <DumpNode> Public Property CheBMethylesterase As ObjectModels.Module.MetabolismFlux()
        ''' <summary>
        ''' MCP + -CH3 -> [MCP][CH3]   Enzyme:CheR
        ''' S-adenosyl-L-methionine
        ''' S-ADENOSYLMETHIONINE
        ''' C00019
        ''' 
        ''' S-ADENOSYLMETHIONINE                              ADENOSYL-HOMO-CYS
        ''' S-adenosyl-L-methionine + protein L-glutamate = S-adenosyl-L-homocysteine + protein L-glutamate methyl ester.
        ''' </summary>
        ''' <remarks></remarks>
        <DumpNode> Public Property CheRMethyltransferase As ObjectModels.Module.MetabolismFlux()
        ''' <summary>
        ''' CheB + [ChA][PI] -> [CheB][PI] + CheA
        ''' </summary>
        ''' <remarks></remarks>
        <DumpNode> Public Property CheBPhosphate As ObjectModels.Module.MetabolismFlux()
        ''' <summary>
        ''' [MCP][CH3] + Inducer &lt;--&gt; [MCP][CH3][Inducer]
        ''' </summary>
        ''' <remarks></remarks>
        <DumpNode> Public Property ChemotaxisSensing As ObjectModels.Module.PassiveTransportationFlux()
        ''' <summary>
        ''' CheAHK + ATP -> [CheAHK][PI] + ADP   Enzyme: [MCP][CH3][Inducer]
        ''' </summary>
        ''' <remarks></remarks>
        <DumpNode> Public Property HkAutoPhosphorus As ObjectModels.Module.MetabolismFlux()
        ''' <summary>
        ''' [CheAHK][PI] + RR -> [RR][PI] + CheAHK
        ''' [CheAHK][PI] + OCS -> CheAHK + [OCS][PI]
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        <DumpNode> Public Property CrossTalk As ObjectModels.Module.MetabolismFlux()
        <DumpNode> Public Property OCSSensing As ObjectModels.Module.MetabolismFlux()

        ''' <summary>
        ''' 连接信号转导网络和调控模型的属性
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DumpNode> Public Property TFActive As ObjectModels.Module.MetabolismFlux()

        <DumpNode> Public Property ProteinComplexes As EngineSystem.ObjectModels.Entity.Compound()

        Dim CellSystem As EngineSystem.ObjectModels.SubSystem.CellSystem

        Sub New(CellSystem As SubSystem.CellSystem)
            Call MyBase.New(CellSystem.Metabolism)
            Me.CellSystem = CellSystem
        End Sub

        Public Overrides Function Initialize() As Integer
            Dim CultivationMediums = DirectCast(IRuntimeContainer, EngineSystem.Engine.GCModeller).CultivatingMediums
            Dim Metabolism = DirectCast(_CellComponentContainer, SubSystem.MetabolismCompartment)
            Dim DataModel = Metabolism._CellSystem.DataModel.SignalTransductionPathway
            Dim Compartments = New SubSystem.ICompartmentObject() {CultivationMediums, Metabolism}

            ChemotaxisSensing = (From ReactionModel In DataModel.ChemotaxisSensing
                                 Select [Module].PassiveTransportationFlux.CreateObject(ReactionModel, Nothing, Nothing, Compartments, DefaultCompartment:=Metabolism)).ToArray
            CheBMethylesterase = (From ReactionModel In DataModel.CheBMethylesterase
                                  Select SubSystem.DelegateSystem.CreateDelegate(ReactionModel, Metabolism.EnzymeKinetics, Metabolism.Metabolites, CreateEnzymes(ReactionModel.Identifier, ReactionModel.Enzymes, Metabolism.Metabolites), _SystemLogging)).ToArray
            CheBPhosphate = (From ReactionsModel In DataModel.CheBPhosphate Select [Module].MetabolismFlux.CreateObject(Of [Module].MetabolismFlux)(ReactionsModel, Metabolism.Metabolites)).ToArray
            CheRMethyltransferase = (From ReactionModel In DataModel.CheRMethyltransferase Select DelegateSystem.CreateDelegate(ReactionModel, Metabolism.EnzymeKinetics, Metabolism.Metabolites,
                                                                                                                                  CreateEnzymes(ReactionModel.Identifier, ReactionModel.Enzymes, Metabolism.Metabolites), _SystemLogging)).ToArray
            HkAutoPhosphorus = (From ReactionModel In DataModel.HkAutoPhosphorus Select DelegateSystem.CreateDelegate(ReactionModel, Metabolism.EnzymeKinetics, Metabolism.Metabolites,
                                                                                                                          CreateEnzymes(ReactionModel.Identifier, ReactionModel.Enzymes, Metabolism.Metabolites), _SystemLogging)).ToArray
            CrossTalk = (From ReactionModel In DataModel.CrossTalk Select [Module].MetabolismFlux.CreateObject(Of [Module].MetabolismFlux)(ReactionModel, Metabolism.Metabolites)).ToArray
            OCSSensing = (From ReactionModel In DataModel.OCSSensing Select [Module].MetabolismFlux.CreateObject(Of [Module].MetabolismFlux)(ReactionModel, Metabolism.Metabolites)).ToArray
            TFActive = (From ReactionModel In DataModel.TFActive Select [Module].MetabolismFlux.CreateObject(Of [Module].MetabolismFlux)(ReactionModel, Metabolism.Metabolites)).ToArray

            Dim InitializeLQuery = (From FluxObject In CheBMethylesterase Select FluxObject.Initialize(Metabolism.Metabolites, SystemLogging)).Count
            InitializeLQuery = (From FluxObject In Me.CheBPhosphate Select FluxObject.Initialize(Metabolism.Metabolites, SystemLogging)).Count
            InitializeLQuery = (From FluxObject In Me.ChemotaxisSensing Select FluxObject.Initialize(Metabolism.Metabolites, SystemLogging)).Count
            InitializeLQuery = (From FluxObject In Me.CheRMethyltransferase Select FluxObject.Initialize(Metabolism.Metabolites, SystemLogging)).Count
            InitializeLQuery = (From FluxObject In Me.CrossTalk Select FluxObject.Initialize(Metabolism.Metabolites, SystemLogging)).Count
            InitializeLQuery = (From FluxObject In Me.HkAutoPhosphorus Select FluxObject.Initialize(Metabolism.Metabolites, SystemLogging)).Count
            InitializeLQuery = (From FluxObject In Me.OCSSensing Select FluxObject.Initialize(Metabolism.Metabolites, SystemLogging)).Count
            InitializeLQuery = (From FluxObject In Me.TFActive Select FluxObject.Initialize(Metabolism.Metabolites, SystemLogging)).Count

            Call CellSystem._InternalEventDriver.JoinEvents(CheBMethylesterase)
            Call CellSystem._InternalEventDriver.JoinEvents(CheBPhosphate)
            Call CellSystem._InternalEventDriver.JoinEvents(ChemotaxisSensing)
            Call CellSystem._InternalEventDriver.JoinEvents(CheRMethyltransferase)
            Call CellSystem._InternalEventDriver.JoinEvents(CrossTalk)
            Call CellSystem._InternalEventDriver.JoinEvents(HkAutoPhosphorus)
            Call CellSystem._InternalEventDriver.JoinEvents(OCSSensing)
            Call CellSystem._InternalEventDriver.JoinEvents(TFActive)

            Return 0
        End Function

        Private Function CreateEnzymes(ReactionId As String, EnzymeIdList As String(), Metabolites As Entity.Compound()) As Feature.MetabolismEnzyme()
            Dim LQuery = (From EnzymeId As String
                          In EnzymeIdList
                          Let params = New EnzymeCatalystKineticLaw With {
                              .Km = 0.5,
                              .KineticRecord = ReactionId,
                              .PH = 7,
                              .Temperature = 28,
                              .Enzyme = EnzymeId
                          }
                          Select New Feature.MetabolismEnzyme With {
                              .Identifier = EnzymeId,
                              .EnzymeMetabolite = Metabolites.GetItem(EnzymeId),
                              .EnzymeKineticLaw = MathematicsModels.EnzymeKinetics.EnzymeCatalystKineticLaw.[New](params)}).ToArray
            Return LQuery
        End Function

        Public Overrides Function CreateServiceSerials() As IDataAcquisitionService()
            Return New IDataAcquisitionService() {
                New DataAcquisitionService(
                    Adapter:=New EngineSystem.Services.DataAcquisition.DataAdapters.SignalTransductionSystem(System:=Me),
                    RuntimeContainer:=Me.get_RuntimeContainer)}
        End Function

        Public Overrides ReadOnly Property DataSource As DataSource()
            Get
                Return (From item In TFActive Let value = item.DataSource Select value).ToArray
            End Get
        End Property

        Public Overrides Function get_DataSerializerHandles() As HandleF()
            Return (From item In TFActive Select item.SerialsHandle).ToArray
        End Function

        Public Overrides Sub MemoryDump(Dir As String)
            Call Me.I_CreateDump.SaveTo(Dir & "/SignalTransductionNetwork.log")
        End Sub

        Protected Overrides Function __innerTicks(KernelCycle As Integer) As Integer
            Return 0
        End Function
    End Class
End Namespace