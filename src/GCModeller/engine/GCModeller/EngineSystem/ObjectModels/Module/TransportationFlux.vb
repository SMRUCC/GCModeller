#Region "Microsoft.VisualBasic::1a69b1fb240b275d5c4de449f8ab68c0, engine\GCModeller\EngineSystem\ObjectModels\Module\TransportationFlux.vb"

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

    '     Class ActiveTransportationFlux
    ' 
    '         Properties: CatalystActivities, DataSource, Enzymes, FluxValue, KineticsModel
    '                     TypeId
    ' 
    '         Function: Invoke
    ' 
    '     Class PassiveTransportationFlux
    ' 
    '         Properties: FluxValue, TypeId
    ' 
    '         Function: __createRefernece, __creates, CreateObject, Initialize
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.IEnumerations
Imports Microsoft.VisualBasic.Serialization
Imports SMRUCC.genomics.GCModeller.Assembly
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.ObjectModels.Module

    Public Class ActiveTransportationFlux : Inherits PassiveTransportationFlux
        Implements EnzymaticFlux.IEnzymaticFlux

        <DumpNode> Friend _Enzymes As EngineSystem.ObjectModels.Feature.MetabolismEnzyme()
        <DumpNode> Friend _EnzymeKinetics As MathematicsModels.EnzymeKinetics.MichaelisMenten

        Dim _CatalystActivity As List(Of EnzymaticFlux.IEnzymaticFlux.EnzymeCatalystActivity) =
            New List(Of EnzymaticFlux.IEnzymaticFlux.EnzymeCatalystActivity)

        Public Overrides Function Invoke() As Double
            Dim FluxValue As Double = 0

            Call _CatalystActivity.Clear()

            For Each Enzyme In _Enzymes.Shuffles
DEBUG:          FluxValue = KineticsModel.GetValue
                FluxValue = _EnzymeKinetics.GetFluxValue(FluxValue, Enzyme)

                Call InvokeFluxConstraintsAndFlowing(FluxValue)

                Dim log As New EnzymaticFlux.IEnzymaticFlux.EnzymeCatalystActivity With {
                    .EnzymeCatalystId = Enzyme.Identifier,
                    .Value = FluxValue
                }
                Call _CatalystActivity.Add(log)
            Next

            Return MyBase.FluxValue
        End Function

        Public Overrides ReadOnly Property DataSource As DataSource
            Get
                Return New DataSource(Handle, FluxValue)
            End Get
        End Property

        Public Overrides ReadOnly Property FluxValue As Double
            Get
                Return (From item In _CatalystActivity Select item.Value).ToArray.Sum
            End Get
        End Property

        Public ReadOnly Property CatalystActivities As EnzymaticFlux.IEnzymaticFlux.EnzymeCatalystActivity() Implements EnzymaticFlux.IEnzymaticFlux.CatalystActivities
            Get
                Return _CatalystActivity.ToArray
            End Get
        End Property

        Public ReadOnly Property Enzymes As Feature.MetabolismEnzyme() Implements EnzymaticFlux.IEnzymaticFlux.Enzymes
            Get
                Return _Enzymes
            End Get
        End Property

        Public Overrides Property KineticsModel As MathematicsModels.GenericKinetic Implements EnzymaticFlux.IEnzymaticFlux.KineticsModel
            Get
                Return MyBase.KineticsModel
            End Get
            Set(value As MathematicsModels.GenericKinetic)
                MyBase.KineticsModel = value
            End Set
        End Property

        Public Overrides ReadOnly Property TypeId As ObjectModel.TypeIds
            Get
                Return TypeIds.ActiveTransportationFlux
            End Get
        End Property
    End Class

    Public Class PassiveTransportationFlux : Inherits [Module].MetabolismFlux

        Public Overrides Function Initialize(Metabolites() As Entity.Compound, SystemLogging As LogFile) As Integer
            Call MyBase.FillMetabolites(SystemLogging)
            Call MyBase.Initialize(Metabolites, SystemLogging)

            Return 0
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="FluxModel">在UniqueId编号之中具有附加属性</param>
        ''' <param name="Enzymes">这个列表是来自于目标待连接的细胞对象的<see cref="subSystem.DelegateSystem.MetabolismEnzymes">代谢组中的酶分子对象列表</see></param>
        ''' <param name="Compartments"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function CreateObject(FluxModel As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.TransportationReaction,
                                                      Enzymes As Feature.MetabolismEnzyme(),
                                                      EnzymeKinetics As MathematicsModels.EnzymeKinetics.MichaelisMenten,
                                                      Compartments As ObjectModels.SubSystem.ICompartmentObject(),
                                                      DefaultCompartment As SubSystem.ICompartmentObject) As PassiveTransportationFlux

            If FluxModel.Products.IsNullOrEmpty OrElse FluxModel.Reactants.IsNullOrEmpty Then
                Dim warnMsg As String = $"FluxModel {FluxModel.Identifier}:  [{FluxModel.Equation}] have gap in its expression! skip this object!"
                Call DefaultCompartment.SystemLogging.WriteLine(warnMsg)
                Return Nothing
            End If

            If FluxModel.Enzymes.IsNullOrEmpty Then
                Dim TransportationFlux As PassiveTransportationFlux = CreateBasicalObject(Of PassiveTransportationFlux)(FluxModel)

                'TransportationFlux和一般的MetabolismFlux对象类型的不同之处在于TransportationFlux中的Reactant和Product是来自不同的Compartment区间的
                '故而在这里需要先解析出Compartment附加属性然后在从Compartments列表之中选择出所需要的Compound类型实例对象
                TransportationFlux._Reactants = __createRefernece(FluxModel.Reactants, Compartments, DefaultCompartment)
                TransportationFlux._Products = __createRefernece(FluxModel.Products, Compartments, DefaultCompartment)

                Return TransportationFlux
            Else
                Dim TransportationFlux As ActiveTransportationFlux = CreateBasicalObject(Of ActiveTransportationFlux)(FluxModel)
                'TransportationFlux和一般的MetabolismFlux对象类型的不同之处在于TransportationFlux中的Reactant和Product是来自不同的Compartment区间的
                '故而在这里需要先解析出Compartment附加属性然后在从Compartments列表之中选择出所需要的Compound类型实例对象
                TransportationFlux._Reactants = __createRefernece(FluxModel.Reactants, Compartments, DefaultCompartment)
                TransportationFlux._Products = __createRefernece(FluxModel.Products, Compartments, DefaultCompartment)
                TransportationFlux.Identifier = TransportationFlux.Identifier
                TransportationFlux._Enzymes = (From EnzymeId As String In FluxModel.Enzymes
                                               Let Enzyme As ObjectModels.Feature.MetabolismEnzyme = Enzymes.GetItem(EnzymeId)
                                               Where Not Enzyme Is Nothing
                                               Select Enzyme).ToArray
                TransportationFlux._EnzymeKinetics = EnzymeKinetics

                Return TransportationFlux
            End If
        End Function

        Protected Friend Shared Function __createRefernece(ModelBase As Generic.IEnumerable(Of GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference),
                                                           Compartments As ObjectModels.SubSystem.ICompartmentObject(),
                                                           DefaultCompartment As SubSystem.ICompartmentObject) As [Module].EquationModel.CompoundSpecieReference()

            Dim LQuery As [Module].EquationModel.CompoundSpecieReference() = (
                From itemObject As GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference
                In ModelBase
                Select __creates(Compartments, itemObject, DefaultCompartment)).ToArray
            Return LQuery
        End Function

        Private Shared Function __creates(Compartments As ObjectModels.SubSystem.ICompartmentObject(),
                                          itemObject As GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference,
                                          DefaultCompartment As SubSystem.ICompartmentObject) As [Module].EquationModel.CompoundSpecieReference
            Dim Compartment = Compartments.GetCompartment(itemObject.CompartmentId)
            If Compartment Is Nothing Then
                Compartment = DefaultCompartment
            End If

            Return [Module].EquationModel.CompoundSpecieReference.CreateObject(itemObject, Compartment.Metabolites)
        End Function

        ''' <summary>
        ''' The molecule interaction event dynamics result.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property FluxValue As Double
            Get
                Return MyBase.FluxValue
            End Get
        End Property

        Public Overrides ReadOnly Property TypeId As ObjectModel.TypeIds
            Get
                Return TypeIds.PassiveTransportationFlux
            End Get
        End Property
    End Class
End Namespace
