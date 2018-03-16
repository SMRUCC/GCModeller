#Region "Microsoft.VisualBasic::1056bcecd1f789ebde0c6f4bcdc74567, engine\GCModeller\EngineSystem\ObjectModels\SubSystem\MetabolismSystem\Metabolism.vb"

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

    '     Class MetabolismCompartment
    ' 
    '         Properties: CompartmentId, ConstraintMetabolite, DataSource, DelegateSystem, EnvironmentFactors
    '                     EnzymeActivitiesDataSource, EnzymeActivitySerialHandles, EnzymeKinetics, EventId, Metabolites
    '                     NetworkComponents, Pathways, ProteinCPLXAssemblies, SystemActivityLoad, SystemLogging
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: __innerTicks, CreateMetaboliteObjects, CreateServiceSerials, Get_currentPH, Get_currentTemperature
    '                   get_DataSerializerHandles, GetMetabolite, Initialize, ToString
    ' 
    '         Sub: InitalizeTrimedHandles, MemoryDump
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.Serialization
Imports SMRUCC.genomics.GCModeller.Assembly
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.RuntimeObjects
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.ObjectModels.SubSystem

    ''' <summary>
    ''' 代谢反应所构成的网络系统对象，同时也是一个Compartment对象
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MetabolismCompartment : Inherits SubSystemContainer(Of [Module].MetabolismFlux)
        Implements ModellingEngine.PlugIns.ISystemFrameworkEntry.ISystemFramework
        Implements SubSystem.ICompartmentObject
        Implements IDrivenable

        <XmlElement> Public Property DelegateSystem As ObjectModels.SubSystem.DelegateSystem
        Public Property Pathways As ObjectModels.SubSystem.PathwayCollection
        ''' <summary>
        ''' 蛋白质复合物形成的容器：信号转导网络
        ''' </summary>
        ''' <remarks></remarks>
        <XmlElement> Public Property ProteinCPLXAssemblies As ObjectModels.SubSystem.ProteinAssembly
        Dim _EnzymeKinetics As MathematicsModels.EnzymeKinetics.MichaelisMenten
        Dim _EnvironmentFactors As MathematicsModels.EnzymeKinetics.pH_Tempratures

        <DumpNode> Public Property ConstraintMetabolite As ConstraintMetabolites

        Public Property Metabolites As Entity.Compound() Implements ICompartmentObject.Metabolites

        Public Overrides ReadOnly Property NetworkComponents As [Module].MetabolismFlux()
            Get
                Return DelegateSystem.NetworkComponents
            End Get
        End Property

        ''' <summary>
        ''' 是否从代谢组的数据中去除RNA和蛋白质的浓度
        ''' </summary>
        ''' <remarks></remarks>
        <DumpNode> Protected TrimMetabolism As Boolean
        <DumpNode> Protected TrimedHandles As Long()
        Protected _CompartmentId As String
        Protected _MetabolismEnzymes As Feature.MetabolismEnzyme()

        Public Overrides ReadOnly Property SystemActivityLoad As Double
            Get
                Return 0
            End Get
        End Property

        Sub New(CellSystem As CellSystem)
            Call MyBase.New(CellSystem)
        End Sub

        Public Overrides Sub MemoryDump(Dir As String)
            Call Me.I_CreateDump.SaveTo(String.Format("{0}/{1}.log", Dir, Me.GetType.Name))
            Call DelegateSystem.MemoryDump(Dir)
            Call Pathways.MemoryDump(Dir)
            Call ProteinCPLXAssemblies.MemoryDump(Dir)
        End Sub

        Public Function GetMetabolite(UniqueId As String) As Entity.Compound
            Return Metabolites.GetByKey(UniqueId, StringComparison.OrdinalIgnoreCase)
        End Function

        ''' <summary>
        ''' 返回代谢物的浓度
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property DataSource As DataSource()
            Get
                If TrimMetabolism Then
                    Dim LQuery = (From Metabolit As ObjectModels.Entity.Compound
                                  In Metabolites.AsParallel
                                  Where Array.IndexOf(TrimedHandles, Metabolit.Handle) = -1
                                  Let value = Metabolit.DataSource
                                  Select value).ToArray
                    Return LQuery
                Else
                    Dim LQuery = From Metabolit As ObjectModels.Entity.Compound
                                 In Metabolites.AsParallel
                                 Let value = Metabolit.DataSource
                                 Select value '
                    Return LQuery.ToArray
                End If
            End Get
        End Property

        Public ReadOnly Property EnzymeActivitiesDataSource As DataSource()
            Get
                Return (From Enz In _MetabolismEnzymes Select Enz.DataSource).ToArray
            End Get
        End Property

        Public ReadOnly Property EnzymeActivitySerialHandles As HandleF()
            Get
                Return (From Enz In _MetabolismEnzymes Select Enz.SerialsHandle).ToArray
            End Get
        End Property

        Public Overrides Function Initialize() As Integer Implements ICompartmentObject.Initialize
            Call SystemLogging.WriteLine("   Created the metabolites object model.")

            Me._CompartmentId = Me.IRuntimeContainer.SystemVariable(GCMarkupLanguage.GCML_Documents.ComponentModels.SystemVariables.ID_COMPARTMENT_METABOLISM)
            If String.IsNullOrEmpty(Me._CompartmentId) Then
                Call _SystemLogging.WriteLine("[ERROR] Metabolism compartment id is not defined in the system variables! Try to ignore this error and resume running using the defaul value!", "", Type:=MSG_TYPES.ERR)
                _CompartmentId = "CCO-IN"
            End If

            Me.Metabolites = CreateMetaboliteObjects(_CellSystem.DataModel.Metabolism.Metabolites, Me.Guid & "..CCO-IN")
            Me._EnvironmentFactors = New MathematicsModels.EnzymeKinetics.pH_Tempratures(Me)
            Me._EnzymeKinetics = New MathematicsModels.EnzymeKinetics.MichaelisMenten(Me)
            Me.DelegateSystem = New DelegateSystem(Me)
            Me.ProteinCPLXAssemblies = New ProteinAssembly(Me)
            Me.Pathways = New PathwayCollection(Me)

            Call MetabolismCompartment.ConstraintMetabolites.Initialize(Me)
            Call SystemLogging.WriteLine("Initialize the protein complexity assembly delegates...")
            Call Me.ProteinCPLXAssemblies.Initialize()
            Call SystemLogging.WriteLine("Initialize the metabolism network...")
            Call DelegateSystem.Initialize()
            Call SystemLogging.WriteLine("Metabolism system initialize job done!")

            Dim ChunkList As List(Of Feature.MetabolismEnzyme) = New List(Of Feature.MetabolismEnzyme)
            For Each Enz In (From item In DelegateSystem.NetworkComponents
                             Where Not item._BaseType.Enzymes.IsNullOrEmpty
                             Let EnzymaticFlux = DirectCast(item, [Module].EnzymaticFlux)
                             Select EnzymaticFlux.Enzymes).ToArray
                Call ChunkList.AddRange(Enz)
            Next

            Me._MetabolismEnzymes = (From Enz In ChunkList Select Enz Order By Enz.ReferenceTag Ascending).ToArray.WriteAddress.ToArray

            Call _CellSystem._InternalEventDriver.JoinEvents(DelegateSystem.NetworkComponents)

            Return 0
        End Function

        Public Shared Function CreateMetaboliteObjects(DataModels As Generic.IEnumerable(Of GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite), TagId As String) As Entity.Compound()
            Dim LQuery = (From Compound As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite In DataModels.AsParallel
                          Let Metabolite As ObjectModels.Entity.Compound = ObjectModels.Entity.Compound.CreateObject(Metabolite:=Compound, TagId:=TagId)
                          Select Metabolite).ToArray
            Return LQuery.WriteAddress
        End Function

        ''' <summary>
        ''' 本方法是在初始化完毕基因表达调控系统之后进行的，因为代谢组是第一个被初始化的系统，而此时基因表达调控网络系统还是Null空值，所以本方法需要在调控网络初始化完毕之后由外部的其他对象所调用
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend Sub InitalizeTrimedHandles()
            Me.TrimMetabolism = IRuntimeContainer.ConfigurationData.TrimMetabolism
            Me.Metabolites = Me.Metabolites.WriteAddress.ToArray
            If Me.TrimMetabolism Then
                Call SystemLogging.WriteLine("   Trim the protein & transcripts metabolite handle from the metabolism system...")

                Dim HandleList As List(Of Long) = New List(Of Long)
                Call HandleList.AddRange((From Transcript In _CellSystem.ExpressionRegulationNetwork._InternalTranscriptsPool Select Transcript.EntityBaseType.Handle).ToArray)
                Call HandleList.AddRange((From Compound In Metabolites Where Compound.ModelBaseElement.MetaboliteType <> GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite.MetaboliteTypes.Compound
                                          Select Compound.Handle).ToArray)

                Me.TrimedHandles = HandleList.Distinct.ToArray
            End If
        End Sub

        Protected Overrides Function __innerTicks(KernelCycle As Integer) As Integer Implements IDrivenable.__innerTicks
            Return Pathways.TICK
        End Function

        Public Overrides Function ToString() As String
            Return Metabolites.Count & " metabolites in the metabolism system."
        End Function

        Public Overrides Function CreateServiceSerials() As Services.MySQL.IDataAcquisitionService()
            Dim ServiceList As New List(Of Services.MySQL.IDataAcquisitionService)
            Call ServiceList.Add(New DataAcquisitionService(New EngineSystem.Services.DataAcquisition.DataAdapters.MetabolismSystem(System:=Me), RuntimeContainer:=IRuntimeContainer))
            Call ServiceList.AddRange(ProteinCPLXAssemblies.CreateServiceSerials)
            Call ServiceList.Add(New DataAcquisitionService(New EngineSystem.Services.DataAcquisition.DataAdapters.DelegateSystem(Me.DelegateSystem), RuntimeContainer:=IRuntimeContainer))
            Call ServiceList.AddRange(Pathways.CreateServiceSerials)
            Call ServiceList.Add(New DataAcquisitionService(New EngineSystem.Services.DataAcquisition.DataAdapters.EnzymeActivity(Me), IRuntimeContainer))
            Call ServiceList.Add(New DataAcquisitionService(New EngineSystem.Services.DataAcquisition.DataAdapters.ConstraintsMetabolite(Me), IRuntimeContainer))
            Return ServiceList.ToArray
        End Function

        Public ReadOnly Property CompartmentId As String Implements ICompartmentObject.CompartmentId
            Get
                Return _CompartmentId
            End Get
        End Property

        Public Function Get_currentPH() As Double Implements ICompartmentObject.Get_currentPH
            Return EnvironmentFactors.Get_currentPH
        End Function

        Public Function Get_currentTemperature() As Double Implements ICompartmentObject.Get_currentTemperature
            Return Me.EnvironmentFactors.get_CultivationMediumsTemperature()
        End Function

        ''' <summary>
        ''' 返回代谢物UniqueId列表，对于反应过程的数据则定义于DelegateSystem处
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function get_DataSerializerHandles() As HandleF()
            If TrimMetabolism Then
                Return (From Metabolite In Metabolites Where Array.IndexOf(Me.TrimedHandles, Metabolite.Handle) = -1 Select Metabolite.SerialsHandle).ToArray
            Else
                Return (From item In Metabolites Select item.SerialsHandle).ToArray
            End If
        End Function

        Public ReadOnly Property EnvironmentFactors As MathematicsModels.EnzymeKinetics.pH_Tempratures Implements ICompartmentObject.EnvironmentFactors
            Get
                Return Me._EnvironmentFactors
            End Get
        End Property

        Public ReadOnly Property EnzymeKinetics As MathematicsModels.EnzymeKinetics.MichaelisMenten Implements ICompartmentObject.EnzymeKinetics
            Get
                Return Me._EnzymeKinetics
            End Get
        End Property

        Protected Friend Overrides ReadOnly Property SystemLogging As LogFile
            Get
                Return Me._SystemLogging
            End Get
        End Property

        Public ReadOnly Property EventId As String Implements IDrivenable.EventId
            Get
                Throw New NotImplementedException
            End Get
        End Property
    End Class
End Namespace
