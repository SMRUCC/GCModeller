#Region "Microsoft.VisualBasic::a1d039201d5459d4ef2fbb19318bd82a, engine\GCModeller\EngineSystem\ObjectModels\Module\CentralDogma\Transcription.vb"

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

    '     Class Transcription
    ' 
    '         Properties: CoefficientFactor, FluxValue, MotifSites, TypeId
    ' 
    '         Function: CreateConstraintFlux, InitializeConstraints, Invoke, ToString, TranscriptionWeightFitting
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic
Imports SMRUCC.genomics.GCModeller.Assembly
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Language

Namespace EngineSystem.ObjectModels.Module.CentralDogmaInstance

    ''' <summary>
    ''' The transcription event for a <see cref="Feature.Gene"></see> object.(一个基因对象的转录过程)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Transcription : Inherits ExpressionProcedureEvent
        Implements IEventUnit(Of Transcription)

        ''' <summary>
        ''' 1表示正常表达水平，0表示缺失突变，1~0表示低量表达，>1表示过量表达
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DumpNode> <XmlAttribute> Public Property CoefficientFactor As Double = 1

        Dim _FluxValue As Double

        Public Overrides Function Invoke() As Double
            If CoefficientFactor = 0 Then Return -1 '该基因被突变了

            '获取调控的大小 '调控大小在上一级调用中已经被设置好了
            ConstraintFlux._RegulationConstraint.Quantity = MyBase._Regulations * CoefficientFactor / CompositionDelayEffect  '调控大小在上一级调用中已经被设置好了
            Call ConstraintFlux.Invoke()

            Dim V As Double = ConstraintFlux.FluxValue

            If V > UPPER_BOUND OrElse Double.IsPositiveInfinity(V) Then
                V = UPPER_BOUND
            ElseIf V < 0 OrElse Double.IsNegativeInfinity(V) OrElse Double.IsNaN(V) Then
                V = 0
            End If

            Product.Quantity = Product.DataSource.value + V
            _FluxValue = V

            Return FluxValue
        End Function

        Public Shared Function TranscriptionWeightFitting(Regulators As Double(), Weights As Double()) As Double
            Dim LQuery = From Hwnd As Long In Regulators.Count.Sequence Select Regulators(Hwnd) * Weights(Hwnd) '
            Return LQuery.ToArray.Sum
        End Function

        Protected Friend Overloads Overrides Function InitializeConstraints(MetabolismSystem As SubSystem.MetabolismCompartment) As Integer
            Dim ConstraintMapping = MetabolismSystem.ConstraintMetabolite
            Me.ConstraintFlux = CreateConstraintFlux(Identifier, MetabolismSystem, MetabolismSystem._CellSystem.ExpressionRegulationNetwork.RNAPolymerase,
                                                             MetabolismSystem._CellSystem.ExpressionRegulationNetwork.ExpressionKinetics,
                                                             MetabolismSystem._CellSystem.ExpressionRegulationNetwork._TranscriptionK1, CompositionVector)
            UPPER_BOUND = ConstraintFlux._BaseType.UPPER_BOUND.Value
            Return 0
        End Function

        ''' <summary>
        ''' 生成转录模型的约束条件
        ''' </summary>
        ''' <param name="MetabolismSystem"></param>
        ''' <param name="RNAPolymeraseEntity"></param>
        ''' <param name="EnzymeKinetics"></param>
        ''' <param name="K1"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateConstraintFlux(UniqueId As String, MetabolismSystem As SubSystem.MetabolismCompartment,
                                              RNAPolymeraseEntity As Feature.MetabolismEnzyme(),
                                              EnzymeKinetics As MathematicsModels.EnzymeKinetics.ExpressionKinetics, K1 As Double, CompositionVector As Integer()) As ExpressionConstraintFlux
            Dim ConstraintModel As New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction With {
              .Name = UniqueId, .Identifier = UniqueId, .Reversible = False}

            ConstraintModel.UPPER_BOUND = New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction.Parameter With {.Value = 50}
            ConstraintModel.Enzymes = (From Enzyme In RNAPolymeraseEntity Let IdValue As String = Enzyme.Identifier Select IdValue).ToArray
            ConstraintModel.p_Dynamics_K_1 = K1

            Dim ConstraintMapping = MetabolismSystem.ConstraintMetabolite
            Dim p As i32 = 0

            ConstraintModel.Reactants = {
                New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_ATP.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)},
                New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_GTP.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)},
                New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_CTP.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)},
                New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = ConstraintMapping.CONSTRAINT_UTP.Identifier, .StoiChiometry = Global.System.Math.Log(CompositionVector(++p) + 2, 2)}}

            ConstraintModel.Products = {
                    New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With
                    {
                        .Identifier = ConstraintMapping.CONSTRAINT_PI.Identifier, .StoiChiometry = CompositionVector.Sum / 10}}

            Dim ConstraintFlux As ExpressionConstraintFlux = New ExpressionConstraintFlux With {._BaseType = ConstraintModel}
            Dim Reactants = (From item In ConstraintModel.Reactants Select [Module].EquationModel.CompoundSpecieReference.CreateObject(ConstraintMapping(item.Identifier), item.StoiChiometry)).AsList
            UniqueId = String.Format("{0}.transcription_regulation_constraints", UniqueId)
            Dim RegulationConstraint As New EquationModel.CompoundSpecieReference With {
                .Stoichiometry = 1,
                .Identifier = UniqueId,
                .EntityCompound = Entity.Compound.CreateObject(UniqueId, 0, 1)
            }
            Call Reactants.Add(RegulationConstraint)
            ConstraintFlux._Reactants = Reactants.ToArray
            ConstraintFlux._Products = (From item In ConstraintModel.Products Select [Module].EquationModel.CompoundSpecieReference.CreateObject(ConstraintMapping(item.Identifier), item.StoiChiometry)).ToArray

            Call ConstraintFlux.FillMetabolites(MetabolismSystem.SystemLogging)

            ConstraintFlux._EnzymeKinetics = EnzymeKinetics
            ConstraintFlux._Enzymes = RNAPolymeraseEntity
            ConstraintFlux.KineticsModel = New MathematicsModels.GenericKinetic(ConstraintFlux, LoggingClient, 50, 0, 1, 1)
            ConstraintFlux.Identifier = UniqueId
            ConstraintFlux._RegulationConstraint = RegulationConstraint.EntityCompound

            Return ConstraintFlux
        End Function

        Public Overrides Function ToString() As String
            Return Me.Product.ToString
        End Function

        Public Overrides ReadOnly Property TypeId As ObjectModel.TypeIds
            Get
                Return TypeIds.EventTranscription
            End Get
        End Property

        Public Overrides ReadOnly Property FluxValue As Double
            Get
                Return _FluxValue
            End Get
        End Property

        Public Property MotifSites As Feature.MotifSite(Of Transcription)() Implements IEventUnit(Of Transcription).MotifSites
    End Class
End Namespace
