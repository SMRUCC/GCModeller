#Region "Microsoft.VisualBasic::f95660fdb172b579a97c63b71bdaf8a2, engine\GCModeller\EngineSystem\ObjectModels\SubSystem\MetabolismSystem\DelegateSystem.vb"

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

    '     Class DelegateSystem
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: CreateDelegate, CreateEnzymeObjects, CreateServiceSerials, GetNetworkComponents, Initialize
    ' 
    '         Sub: MemoryDump
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.Serialization
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.MathematicsModels.EnzymeKinetics

Namespace EngineSystem.ObjectModels.SubSystem

    Public Class DelegateSystem : Inherits CellComponentSystemFramework(Of ObjectModels.Module.MetabolismFlux)
        Implements PlugIns.ISystemFrameworkEntry.ISystemFramework

        Protected Friend MetabolismEnzymes As EngineSystem.ObjectModels.Feature.MetabolismEnzyme()

        Sub New(Metabolism As SubSystem.MetabolismCompartment)
            Call MyBase.New(Metabolism)
        End Sub

        Public Overrides Function Initialize() As Integer
            Dim CellSystem = DirectCast(Me._CellComponentContainer, SubSystem.MetabolismCompartment)._CellSystem
            Dim Metabolites = CellSystem.Metabolism.Metabolites

            '  Me.MetabolismEnzymes = CreateEnzymeObjects(CellSystem.Metabolism.Metabolites, CellSystem.DataModel.Metabolism.MetabolismEnzymes)
            Dim LQuery = (From model In CellSystem.DataModel.Metabolism.MetabolismNetwork
                          Let rxn As ObjectModels.Module.MetabolismFlux = CreateDelegate(model, CellSystem.Metabolism.EnzymeKinetics, Metabolites, MetabolismEnzymes, SystemLogging)
                          Select rxn).ToArray  'generate the flux object

            MyBase._DynamicsExprs = LQuery.WriteAddress.ToArray
#If DEBUG Then
            Dim InitQuery = (From idx As Integer In _DynamicsExprs.Sequence Let FluxObject = _DynamicsExprs(idx)
                             Select FluxObject.Initialize(CellSystem.Metabolism.Metabolites, SystemLogging)).ToArray 'link the flux object with the delegate system
#Else
            Dim InitQuery = (From idx As Integer In _DynamicsExprs.Sequence.AsParallel
                             Let FluxObject = _DynamicsExprs(idx)
                             Select FluxObject.Initialize(CellSystem.Metabolism.Metabolites, SystemLogging)).ToArray 'link the flux object with the delegate system
#End If
            Return 0
        End Function

        ''' <summary>
        ''' 对于酶促反映而言，会产生与酶分子数量相同的反映数目
        ''' </summary>
        ''' <param name="UniqueId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetNetworkComponents(UniqueId As String) As [Module].MetabolismFlux()
            Dim LQuery = (From item In _DynamicsExprs.AsParallel Where String.Equals(UniqueId, item._BaseType.Identifier) Select item).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' 从反应过程中的定义创建酶分子对象，由于一种酶分子可以对多种反应进行催化，并且可能存在对不同的反应表现出不同的动力学性质，故而在初始化后得到的酶分子，其标识符和数据模型基类可能会存在重合，但是却拥有者各自独立的动力学参数
        ''' </summary>
        ''' <param name="Metabolites"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function CreateEnzymeObjects(Metabolites As ObjectModels.Entity.Compound(), Enzymes As EnzymeCatalystKineticLaw()) _
            As ObjectModels.Feature.MetabolismEnzyme()

            Dim LQuery = From EnzymeKinetic In Enzymes Select ObjectModels.Feature.MetabolismEnzyme.CreateObject(EnzymeKinetic.Enzyme, EnzymeKinetic, Metabolites) '
            Return LQuery.ToArray
        End Function

        ''' <summary>
        ''' 创建代谢流对象
        ''' 1. 数据模型中的反应对象无任何标记的时候，创建普通的代谢流
        ''' 2. 数据模型中的反应对象有酶分子标记的时候，创建酶促反应代谢流
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Function CreateDelegate(Model As XmlElements.Metabolism.Reaction,
                                              EnzymeKinetics As MathematicsModels.EnzymeKinetics.MichaelisMenten,
                                              Metabolites As Entity.Compound(),
                                              MetabolismEnzymes As EngineSystem.ObjectModels.Feature.MetabolismEnzyme(),
                                              _LoggingClient As LogFile) As ObjectModels.Module.MetabolismFlux

            If Model.IsEnzymaticMetabolismFlux Then
                Dim LQuery = (From EnzymeId In Model.Enzymes
                              Let Enzymes = (From item In MetabolismEnzymes Where String.Equals(EnzymeId, item.Identifier) AndAlso String.Equals(Model.Identifier, item.EnzymeKineticLaw.KineticRecord) Select item).ToArray
                              Where Not Enzymes.IsNullOrEmpty
                              Select Enzymes.First).ToArray

                If LQuery.Count <> Model.Enzymes.Count OrElse LQuery.Count = 0 Then  '说明数据有问题
                    Call _LoggingClient.WriteLine("[ENZYME/FLUX_OBJECT_NOT_FOUND]  Generated enzymatic reaction count is not match with the enzyme counts in the datamodel, the data maybe broken!" & vbCrLf &
                                                  "Check for the data of metabolism flux object:  " & Model.Identifier, "" & vbCrLf & "Try to ignore this error!", Type:=MSG_TYPES.ERR)
                End If
                Return ObjectModels.Module.EnzymaticFlux.CreateObject(Model, LQuery, EnzymeKinetics, Metabolites)
            Else
                Return ObjectModels.Module.MetabolismFlux.CreateObject(Of ObjectModels.Module.MetabolismFlux)(Model, Metabolites)
            End If
        End Function

        Public Overrides Function CreateServiceSerials() As Services.MySQL.IDataAcquisitionService()
            Return New Services.MySQL.IDataAcquisitionService() {New EngineSystem.Services.DataAcquisition.DataAdapters.DelegateSystem(DataSource:=Me)}
        End Function

        Public Overrides Sub MemoryDump(Dir As String)
            Call Me.I_CreateDump.SaveTo(String.Format("{0}/{1}.log", Dir, Me.GetType.Name))
        End Sub
    End Class
End Namespace
