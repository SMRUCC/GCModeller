#Region "Microsoft.VisualBasic::037bdbb4c629830db39f248e7cb59e57, engine\GCModeller\EngineSystem\ObjectModels\SubSystem\CultivationMediums.vb"

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

    '     Class CultivationMediums
    ' 
    '         Properties: CellObjects, CompartmentId, ConfigurationData, Count, CultivationMediums
    '                     EnvironmentFactors, EnzymeKinetics, EventId, IsLiquidsBrothMedium, RuntimeTicks
    '                     SystemLogging, SystemVariables, Temperature, WATER_FREE_DIFFUSION
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: __innerTicks, CreateServiceSerials, Get_currentPH, Get_currentTemperature, Get_runtimeContainer
    '                   GetArguments, GetEnumerator, GetEnumerator1, Initialize, ToString
    ' 
    '         Sub: Add, InitializeDelegatesBetweenTheCell, MemoryDump
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.Serialization
Imports SMRUCC.genomics.GCModeller.Assembly
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.ComponentModels
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.RuntimeObjects
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL

Namespace EngineSystem.ObjectModels.SubSystem

    Public Class CultivationMediums : Inherits SubSystem.ReactorMachine(Of [Module].PassiveTransportationFlux)
        Implements IDrivenable
        Implements EngineSystem.Engine.IContainerSystemRuntimeEnvironment
        Implements IReadOnlyList(Of CellSystem)
        Implements IDisposable
        Implements ICompartmentObject

        Protected _CompartmentId As String

        ''' <summary>
        ''' 外部程序是通过本属性来改变整个体系的环境温度的
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DumpNode> Public Property Temperature As Double
        <DumpNode> Protected _InternalCellList As List(Of EngineSystem.ObjectModels.SubSystem.CellSystem)
        <DumpNode> Protected _CultivationMediumsModel As GCMarkupLanguage.CultivationMediums
        ''' <summary>
        ''' 用于表示培养基的生化反应网络的对象类型
        ''' </summary>
        ''' <remarks></remarks>
        <DumpNode> Public Property CultivationMediums As Entity.Compound() Implements ICompartmentObject.Metabolites
        <DumpNode> Protected Friend _TransmembraneFluxes As List(Of [Module].PassiveTransportationFlux)

        Dim _EnzymeKinetics As MathematicsModels.EnzymeKinetics.MichaelisMenten
        Dim _Environment_DynamicsFactor As MathematicsModels.EnzymeKinetics.pH_Tempratures
        ''' <summary>
        ''' <see cref="Tick"></see>过程之中会被重新设置水平的代谢物，这个列表和<see cref="_CultivationMediumsModel">培养基的数据模型</see>之中的顺序是一致的
        ''' </summary>
        ''' <remarks></remarks>
        Dim _LevelResetList As EngineSystem.ObjectModels.Entity.Compound()
        ''' <summary>
        ''' 是否为液体培养基，是的话，则虚拟细胞之中的水充足，不会被改变
        ''' </summary>
        ''' <remarks></remarks>
        Dim _IsLiquidBrothType As Boolean

        Public ReadOnly Property IsLiquidsBrothMedium As Boolean
            Get
                Return _IsLiquidBrothType
            End Get
        End Property

        Public ReadOnly Property CellObjects As EngineSystem.ObjectModels.SubSystem.CellSystem()
            Get
                Return _InternalCellList.ToArray
            End Get
        End Property

        Sub New(obj_Model As GCMarkupLanguage.CultivationMediums, RuntimeContainer As Engine.GCModeller)
            Me.IRuntimeContainer = RuntimeContainer
            Me._InternalCellList = New List(Of CellSystem)
            Me._CultivationMediumsModel = obj_Model
            Me._SystemLogging = RuntimeContainer.SystemLogging
            Call SystemLogging.WriteLine(String.Format("Found '{0}' medium substrates define in the csv data.", _CultivationMediumsModel.Uptake_Substrates.Count))
            Me.Temperature = RuntimeContainer.ConfigurationData.Initial_Temperature

            Dim s As String =
                RuntimeContainer.SystemVariable(GCMarkupLanguage.GCML_Documents.ComponentModels.SystemVariables.PARA_CULTIVATION_MEDIUM_TYPE)
            If String.IsNullOrEmpty(s) OrElse String.Equals("Broth", s, StringComparison.OrdinalIgnoreCase) Then
                Me._IsLiquidBrothType = True
            Else
                Me._IsLiquidBrothType = False
            End If

            Call RuntimeContainer.SystemLogging.WriteLine("Cultivation medium type is " & If(_IsLiquidBrothType, "Liquid broth", "Solid mediums"))
        End Sub

        ''' <summary>
        ''' 假设培养基中的物质很多，所以浓度基本不会被改变，故而在这里每一次循环都会将培养基之中指定的成分的浓度设置为原来的水平
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overrides Function __innerTicks(KernelCycle As Integer) As Integer Implements IDrivenable.__innerTicks

            For i As Integer = 0 To _LevelResetList.Count - 1
                Dim ItemMetabolite As EngineSystem.ObjectModels.Entity.Compound = _LevelResetList(i)

                If Not ItemMetabolite.DataSource.value > ItemMetabolite.ModelBaseElement.InitialAmount Then
                    ItemMetabolite.Quantity = ItemMetabolite.ModelBaseElement.InitialAmount
                End If
            Next

            Call Me._TransmembraneFluxes.Invoke()

            Dim DeathCell As List(Of SubSystem.CellSystem) = New List(Of CellSystem)
#Const DEBUG = 0
#If DEBUG Then
            For Each CellObject In Me._InnerCellList.RandomizeElements
                Call CellObject.Tick()

                If True = CellObject.CellDeathDetection Then
                    Call DeathCell.Add(CellObject)
                End If
            Next
#Else
            DeathCell = (From CellObject As ModellingEngine.EngineSystem.ObjectModels.SubSystem.CellSystem
                         In _InternalCellList.Shuffles.AsParallel
                         Let Invoke As Integer = CellObject.Tick(KernelCycle)
                         Where CellObject.CellDeathDetection = True
                         Select CellObject).AsList
#End If

            For Each CellObject In DeathCell
                Call _SystemLogging.WriteLine(String.Format("Cell GUID:={0} has detected death!", CellObject.Guid))
                Call _InternalCellList.Remove(CellObject)
                Call CellObject.Dispose()

                If _InternalCellList.Count = 0 Then
                    Return -1
                End If
            Next

            Return 0
        End Function

        Public Sub Add(CellObject As EngineSystem.ObjectModels.SubSystem.CellSystem)
            Call SystemLogging.WriteLine("A cell object was placed into the cultivation medium.")
            Call SystemLogging.WriteLine("   ------>  " & CellObject.ToString)
            Call _InternalCellList.Add(CellObject)
            Call InitializeDelegatesBetweenTheCell(NewCellObject:=CellObject)
        End Sub

        Public Overrides Function Initialize() As Integer Implements ICompartmentObject.Initialize
            If Me._CultivationMediumsModel Is Nothing OrElse Me._CultivationMediumsModel.Uptake_Substrates.IsNullOrEmpty Then
                Call SystemLogging.WriteLine("It seems like that you haven't define any cultivation medium substrates....", "", Type:=MSG_TYPES.WRN)
                Me._CultivationMediumsModel = New GCMarkupLanguage.CultivationMediums
                Me._CultivationMediumsModel.Uptake_Substrates = New I_SubstrateRefx() {}
            End If

            Me._CultivationMediums = MetabolismCompartment.CreateMetaboliteObjects(DirectCast(Me.IRuntimeContainer, EngineSystem.Engine.GCModeller).get_DataModel.Metabolism.Metabolites, Me.Guid & "CCO-OUT")
            Dim TempList = New List(Of EngineSystem.ObjectModels.Entity.Compound)
            For Each Item As Entity.Compound In _CultivationMediums
                Item.Quantity = 0
                Dim Find = Me._CultivationMediumsModel.Uptake_Substrates.GetItem(Item.Identifier)
                If Not Find Is Nothing Then
                    Item.Quantity = Find.InitialQuantity '仅有在培养基模型中指定存在的代谢物可以具有初始浓度，其他的代谢物都设置为0，以此形式来表示培养基的组成成分
                    Item.ModelBaseElement.InitialAmount = Find.InitialQuantity
                    Call TempList.Add(Item)
                End If
            Next

            Me.CultivationMediums.GetItem("WATER").Quantity = 10000

            Me._LevelResetList = TempList.ToArray
            Me._TransmembraneFluxes = New List(Of [Module].PassiveTransportationFlux)
            Me._CompartmentId = Me.IRuntimeContainer.SystemVariable(ID_COMPARTMENT_CULTIVATION_MEDIUMS)

            If String.IsNullOrEmpty(Me._CompartmentId) Then
                Call _SystemLogging.WriteLine("[ERROR] cultivation medium compartment id is not defined in the system variables! Try to ignore this error and resume running using the defaul value!", "", Type:=MSG_TYPES.ERR)
                _CompartmentId = "CCO-OUT"
            End If

            Me._Environment_DynamicsFactor = New MathematicsModels.EnzymeKinetics.pH_Tempratures(Me)
            Me._EnzymeKinetics = New MathematicsModels.EnzymeKinetics.MichaelisMenten(Me)

            Return 0
        End Function

        Private Shared ReadOnly Property WATER_FREE_DIFFUSION As TransportationReaction =
            New TransportationReaction With {
                .Identifier = "WATER-FREE-DIFFUSION",
                .UPPER_BOUND = 1000,
                .LOWER_BOUND = 1000,
                .p_Dynamics_K_1 = 1,
                .p_Dynamics_K_2 = 1,
                .Reversible = True,
                .Reactants = {
                    New CompoundSpeciesReference With {
                        .Identifier = "WATER",
                        .StoiChiometry = 1,
                        .CompartmentId = "CCO-IN"
                    }
                },
                .Products = {
                    New CompoundSpeciesReference With {
                        .Identifier = "WATER",
                        .StoiChiometry = 1,
                        .CompartmentId = "CCO-OUT"
                    }
                }
            }

        Private Sub InitializeDelegatesBetweenTheCell(NewCellObject As CellSystem)
            Dim CellEnzymes As ObjectModels.Feature.MetabolismEnzyme() = NewCellObject.Metabolism.DelegateSystem.MetabolismEnzymes
            Dim Compartments = New EngineSystem.ObjectModels.SubSystem.ICompartmentObject() {NewCellObject.Metabolism, Me}
            Dim DefaultCompartment As SubSystem.ICompartmentObject = Nothing

            If String.IsNullOrEmpty(Me.IRuntimeContainer.ConfigurationData.DefaultCompartmentId) Then
                Call _SystemLogging.WriteLine("Default compartment id is empty, this may be case the system crash later in the transportation flux initalize...",
                                              "CultivationMediums --> InitializeDelegatesBetweenTheCell()", Type:=MSG_TYPES.WRN)
            Else
                DefaultCompartment = (From item As ObjectModels.SubSystem.ICompartmentObject
                                      In Compartments
                                      Where String.Equals(IRuntimeContainer.ConfigurationData.DefaultCompartmentId, item.CompartmentId, StringComparison.OrdinalIgnoreCase)
                                      Select item).First
            End If

            Call _SystemLogging.WriteLine("Start to create the transmembrane flux object models between the cultivation mediums and cell...")

            Dim LQuery = (From model As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.TransportationReaction
                          In NewCellObject.DataModel.TransmembraneTransportation
                          Let TrRxnModel As ObjectModels.Module.PassiveTransportationFlux =
                              [Module].PassiveTransportationFlux.CreateObject(model, CellEnzymes, NewCellObject.Metabolism.EnzymeKinetics, Compartments, DefaultCompartment)
                          Select TrRxnModel).AsList

            Call LQuery.Add([Module].PassiveTransportationFlux.CreateObject(WATER_FREE_DIFFUSION, CellEnzymes, NewCellObject.Metabolism.EnzymeKinetics, Compartments, DefaultCompartment))

            Call _SystemLogging.WriteLine("transmembrane flux object model create job done!")
            Call _SystemLogging.WriteLine("Start to initialize them!")

            Dim InitializeLQuery = (From TrFluxModel As EngineSystem.ObjectModels.Module.PassiveTransportationFlux
                                    In LQuery
                                    Select TrFluxModel.Initialize(NewCellObject.Metabolism.Metabolites, SystemLogging)).ToArray
            Call _TransmembraneFluxes.AddRange(LQuery)
            NewCellObject.Metabolism.TransmembraneFluxs = LQuery.ToArray
            Call NewCellObject.Metabolism.Pathways.Initialize()
            _DynamicsExprs = _TransmembraneFluxes.WriteAddress.ToArray

            If IRuntimeContainer.ConfigurationData.ATP_Compensate Then
                Call _SystemLogging.WriteLine("[DEBUG!!!!!] Setup ATP compensation...")
                Call NewCellObject.Metabolism.ConstraintMetabolite.setup_ATP_Compensate(NewCellObject.Metabolism)
            End If

            Call NewCellObject.set_CultivationMedium(Me)
            Call _SystemLogging.WriteLine("Delegate between the cultivating mediums and the cell object initialzie done!")
        End Sub

        Public Overrides Sub MemoryDump(DumpFile As String) Implements IContainerSystemRuntimeEnvironment.MemoryDump
            Throw New NotImplementedException
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0} cell in the cultivation mediums!", _InternalCellList.Count)
        End Function

#Region "IReadOnlyList(Of EngineSystem.ObjectModels.System.CellSystem)"
        Default Public Overloads ReadOnly Property Item(index As Integer) As CellSystem Implements Global.System.Collections.Generic.IReadOnlyList(Of CellSystem).Item
            Get
                Return _InternalCellList(index)
            End Get
        End Property

        Public Overloads Iterator Function GetEnumerator() As Global.System.Collections.Generic.IEnumerator(Of CellSystem) Implements Global.System.Collections.Generic.IEnumerable(Of CellSystem).GetEnumerator
            For i As Integer = 0 To _InternalCellList.Count - 1
                Yield _InternalCellList(i)
            Next
        End Function

        Public Overloads ReadOnly Property Count As Integer Implements Global.System.Collections.Generic.IReadOnlyCollection(Of CellSystem).Count
            Get
                Return _InternalCellList.Count
            End Get
        End Property

        Public Overloads Iterator Function GetEnumerator1() As Global.System.Collections.IEnumerator
            Yield GetEnumerator()
        End Function
#End Region

        Public Function GetArguments(Name As String) As String Implements IContainerSystemRuntimeEnvironment.GetArguments
            Return Me.IRuntimeContainer.GetArguments(Name)
        End Function

        Public Overrides Function CreateServiceSerials() As IDataAcquisitionService()
            Return New IDataAcquisitionService() {
                New DataAcquisitionService(New EngineSystem.Services.DataAcquisition.DataAdapters.CultivationMediumsMetabolites(Me), RuntimeContainer:=IRuntimeContainer),
                New DataAcquisitionService(New EngineSystem.Services.DataAcquisition.DataAdapters.CultivationMediumsReactor(Me), RuntimeContainer:=IRuntimeContainer),
                New DataAcquisitionService(New EngineSystem.Services.DataAcquisition.DataAdapters.TransmembraneTransportation(Me), IRuntimeContainer)}
        End Function

        Public ReadOnly Property CompartmentId As String Implements ICompartmentObject.CompartmentId
            Get
                Return _CompartmentId
            End Get
        End Property

        Public Function Get_currentPH() As Double Implements ICompartmentObject.Get_currentPH
            Return Me._Environment_DynamicsFactor.Get_currentPH
        End Function

        Public Function Get_currentTemperature() As Double Implements ICompartmentObject.Get_currentTemperature
            Return Me.Temperature
        End Function

        Public ReadOnly Property SystemVariables(var As String) As String Implements IContainerSystemRuntimeEnvironment.SystemVariable
            Get
                Return Me.IRuntimeContainer.SystemVariable(var)
            End Get
        End Property

        Public ReadOnly Property EnvironmentFactors As MathematicsModels.EnzymeKinetics.pH_Tempratures Implements ICompartmentObject.EnvironmentFactors
            Get
                Return Me._Environment_DynamicsFactor
            End Get
        End Property

        Public ReadOnly Property EnzymeKinetics As MathematicsModels.EnzymeKinetics.MichaelisMenten Implements ICompartmentObject.EnzymeKinetics
            Get
                Return Me._EnzymeKinetics
            End Get
        End Property

        Protected Friend Overrides ReadOnly Property SystemLogging As LogFile Implements ICompartmentObject.SystemLogging, IContainerSystemRuntimeEnvironment.SystemLogging
            Get
                Return Me._SystemLogging
            End Get
        End Property

        Public Overrides Function Get_runtimeContainer() As IContainerSystemRuntimeEnvironment Implements ICellComponentContainer.get_runtimeContainer
            Return Me.IRuntimeContainer
        End Function

        Public ReadOnly Property ConfigurationData As Engine.Configuration.ConfigReader Implements IContainerSystemRuntimeEnvironment.ConfigurationData
            Get
                Return IRuntimeContainer.ConfigurationData
            End Get
        End Property

        Public ReadOnly Property EventId As String Implements IDrivenable.EventId
            Get
                Return "Cell Driver"
            End Get
        End Property

        Public Overrides ReadOnly Property RuntimeTicks As Long Implements IContainerSystemRuntimeEnvironment.RuntimeTicks
            Get
                Return IRuntimeContainer.RuntimeTicks
            End Get
        End Property
    End Class
End Namespace
