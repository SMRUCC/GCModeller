#Region "Microsoft.VisualBasic::bc6772fcaa450d6b578ce66aa170fb55, engine\GCModeller\EngineSystem\ObjectModels\Module\MetabolismFlux.vb"

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

    '     Class MetabolismFlux
    ' 
    '         Properties: FluxValue, KineticsModel, LOWER_BOUND, MetaboliteCounts, Reversible
    '                     SerialsHandle, TypeId, UPPER_BOUND
    ' 
    '         Function: __createReferences, Constraint, Contains, CreateBasicalObject, CreateObject
    '                   GetCoefficient, GetEnumerator, GetEnumerator1, Initialize, Invoke
    '                   InvokeFluxConstraintsAndFlowing, ToString
    ' 
    '         Sub: FillMetabolites
    ' 
    '     Class ExpressionConstraintFlux
    ' 
    '         Function: Invoke
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.Serialization
Imports SMRUCC.genomics.GCModeller.Assembly
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer

Namespace EngineSystem.ObjectModels.Module

    ''' <summary>
    ''' Metaolism reaction basetype in GCModeller ObjectModels.(GCModeller计算引擎之中的代谢反应对象模型类型的基类)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MetabolismFlux : Inherits SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.FluxObject
        Implements Generic.IEnumerable(Of Entity.Compound)
        Implements Generic.IReadOnlyCollection(Of Entity.Compound)
        Implements Generic.IReadOnlyList(Of Entity.Compound)

        ' <DumpNode> <XmlElement> Protected Friend _Parameters As MetabolismFlux.ParameterF

        <DumpNode> Protected Friend _Modifiers As Entity.ReactionModifier()
        <DumpNode> Protected Friend _Reactants As [Module].EquationModel.CompoundSpecieReference()
        <DumpNode> Protected Friend _Products As [Module].EquationModel.CompoundSpecieReference()

        <DumpNode> Public Overridable Property KineticsModel As MathematicsModels.GenericKinetic

        Protected Friend _BaseType As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction

        ''' <summary>
        ''' 使用Reactants字段域与Products字段域所生成的属性，表示为本代谢流对象中所涉及到的所有的代谢底物的集合
        ''' </summary>
        ''' <remarks></remarks>
        <DumpNode> Protected Friend _Metabolites As Entity.Compound()
        <DumpNode> Public ReadOnly Property UPPER_BOUND As Double
            Get
                Return _BaseType.UPPER_BOUND.Value
            End Get
        End Property

        <DumpNode> Public ReadOnly Property LOWER_BOUND As Double
            Get
                Return _BaseType.LOWER_BOUND.Value
            End Get
        End Property

        Dim _FluxValue As Double

        Protected Friend Overridable ReadOnly Property Reversible As Boolean
            Get
                Return _BaseType.Reversible
            End Get
        End Property

        Public Overrides ReadOnly Property SerialsHandle As HandleF
            Get
                If Me._BaseType.Reversible Then
                    Return New HandleF With {
                        .Handle = Handle,
                        .Identifier = $"[{Identifier}]"
                    }
                Else
                    Return New HandleF With {
                        .Handle = Handle,
                        .Identifier = Identifier
                    }
                End If
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="UniqueId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCoefficient(UniqueId As String) As Double
            Dim LQuery = (From Metabolite In _Reactants Where String.Equals(Metabolite.Identifier, UniqueId) Select Metabolite.Stoichiometry).ToArray
            If LQuery.IsNullOrEmpty Then
                LQuery = (From Metabolite In _Products Where String.Equals(Metabolite.Identifier, UniqueId) Select Metabolite.Stoichiometry).ToArray
                If LQuery.IsNullOrEmpty Then
                    Return 0
                Else
                    Return LQuery.First
                End If
            Else
                Return LQuery.First * -1
            End If
        End Function

        Protected Friend Shared Function Constraint(rFluxValue As Double, FluxObject As MetabolismFlux) As Double
            If rFluxValue > 0 Then  '正向的反映流量，则只需要考虑左端的被消耗的代谢物
                Dim LQuery = (From Metabolite In FluxObject._Reactants Select Metabolite.Stoichiometry * Metabolite.EntityCompound.DataSource.value).ToArray.Min
                If rFluxValue > LQuery Then
                    Return LQuery
                Else
                    Return rFluxValue
                End If

            ElseIf rFluxValue < 0 Then

                Dim LQuery = (From Metabolite In FluxObject._Products Select Metabolite.Stoichiometry * Metabolite.EntityCompound.DataSource.value).ToArray.Min
                rFluxValue = -1 * rFluxValue
                If rFluxValue > LQuery Then
                    Return -1 * LQuery
                Else
                    Return -1 * rFluxValue
                End If
            Else
                Return 0
            End If

            'Dim ConstraintTestLQuery = (From Metabolite In FluxObject._Reactants Select Metabolite.ConstraintTest(-1 * _rFluxValue)).ToArray.Count + (From Metabolite In FluxObject._Products Select Metabolite.ConstraintTest(_rFluxValue)).ToArray.Count
            'Dim _Reactant = (From item In FluxObject._Reactants Select item Order By item._ConstraintTestValue Ascending).First
            'Dim _Product = (From item In FluxObject._Products Select item Order By item._ConstraintTestValue Ascending).First
            'Dim FluxValueSign As Integer = Global.System.Math.Sign(_rFluxValue)

            'If _Reactant._ConstraintTestValue < 0 Then
            '    _rFluxValue = FluxValueSign * _Reactant.EntityCompound.Quantity / _Reactant.Stoichiometry
            '    _rFluxValue *= 0.8

            '    If Not FluxObject.Reversible Then
            '        Return _rFluxValue
            '    End If
            'End If

            'If _Product._ConstraintTestValue < 0 Then
            '    Dim v = FluxValueSign * _Product.EntityCompound.Quantity / _Product.Stoichiometry
            '    v *= 0.8
            '    _rFluxValue = Global.System.Math.Min(Global.System.Math.Abs(_rFluxValue), Global.System.Math.Abs(v)) * FluxValueSign
            'End If

            'Return _rFluxValue
        End Function

        Protected Friend Sub FillMetabolites(Logging As LogFile)
            Dim array As EquationModel.CompoundSpecieReference() = {_Products, _Reactants}.ToVector

            _Metabolites = (From cspref As EquationModel.CompoundSpecieReference
                            In array
                            Select cspref.EntityCompound
                            Distinct).ToArray
            Dim LQuery = (From item As EquationModel.CompoundSpecieReference
                          In array
                          Where item.EntityCompound Is Nothing
                          Select item.Identifier).ToArray
            If Not LQuery.IsNullOrEmpty Then
                Call Logging.WriteLine(String.Format("FluxObject have null reference metabolites! ({0} [[[{1}]]]  ==>  {2}), this can cause the kernel engine crash, please check for the model data consistent!",
                                                     Identifier, Me._BaseType.Equation, String.Join("; ", LQuery)),
                                       "MetabolismFlux -> FillMetabolites(Logging As Logging.LogFile)",
                                       Type:=MSG_TYPES.ERR)
            End If
        End Sub

        Public Shared Function CreateBasicalObject(Of FluxObject As MetabolismFlux) _
            (FluxModel As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction) As FluxObject

            Dim FluxObjectInstance As FluxObject = Activator.CreateInstance(Of FluxObject)()

            With FluxObjectInstance
                .Identifier = FluxModel.Identifier
                ._BaseType = FluxModel
            End With

            Return FluxObjectInstance
        End Function

        Public Iterator Function GetEnumerator() As Global.System.Collections.Generic.IEnumerator(Of Entity.Compound) _
            Implements Global.System.Collections.Generic.IEnumerable(Of Entity.Compound).GetEnumerator

            For Each Metabolite In _Metabolites
                Yield Metabolite
            Next
        End Function

        Public ReadOnly Property MetaboliteCounts As Integer Implements Global.System.Collections.Generic.IReadOnlyCollection(Of Entity.Compound).Count
            Get
                Return _Metabolites.Count
            End Get
        End Property

        Default Public ReadOnly Property Item(index As Integer) As Entity.Compound Implements Global.System.Collections.Generic.IReadOnlyList(Of Entity.Compound).Item
            Get
                Return _Metabolites(index)
            End Get
        End Property

        Public Iterator Function GetEnumerator1() As Global.System.Collections.IEnumerator Implements Global.System.Collections.IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function

        Public Function Contains(Metabolite As String) As Boolean
            For i As Integer = 0 To _Metabolites.Count - 1
                If String.Equals(Metabolite, _Metabolites(i).Identifier) Then
                    Return True
                End If
            Next
            Return False
        End Function

        ''' <summary>
        ''' 执行一次反应
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 
        ''' </remarks>
        Public Overrides Function Invoke() As Double
            Dim v = Me._KineticsModel.GetValue()
            Call InvokeFluxConstraintsAndFlowing(Vmax:=v)
            Return Me.FluxValue
        End Function

        Protected Function InvokeFluxConstraintsAndFlowing(Vmax As Double) As Double
            If Vmax > UPPER_BOUND Then
                Vmax = UPPER_BOUND
            ElseIf Vmax <= LOWER_BOUND Then
                Vmax = LOWER_BOUND
            ElseIf Global.System.Math.Abs(Vmax - 0.0R) < TOLERANCE Then
                Return 0
            End If

            If Not Global.System.Math.Abs(Vmax - 0.0R) < TOLERANCE Then
                _FluxValue = Constraint(Vmax, Me) * 0.75
                Dim LQuery = (From Metabolite As EquationModel.CompoundSpecieReference
                              In _Reactants
                              Select Metabolite.Flowing(-1 * _FluxValue)).ToArray.Count + (From Metabolite In _Products Select Metabolite.Flowing(_FluxValue)).ToArray.Length
            End If

            'If (From Metabolite In _Metabolites Where Metabolite.Quantity < 0 Select 1).ToArray.Count > 0 Then
            '    Dim sBuilder As StringBuilder = New StringBuilder(1024)
            '    Call sBuilder.AppendLine("DEBUG_INFO: ")
            '    Call sBuilder.AppendLine(String.Format("UNIQUE-ID:  {0}", UniqueId))
            '    Call sBuilder.AppendLine(String.Format("UPPER_BOUND:={0}; LOWER_BOUND:={1}; K1:={2}; K2:={3}; Reversible:={4}", _UPPER_BOUND, _LOWER_BOUND, _BaseType.Keq_1, _BaseType.Keq_2, Reversible))
            '    Call sBuilder.AppendLine("Reactions:")
            '    For Each Metabolite In _Reactants
            '        Call sBuilder.AppendLine(String.Format("{0} = {1},  [{2}]", Metabolite.UniqueId, Metabolite.EntityCompound.Quantity, Metabolite._ConstraintTestValue))
            '    Next
            '    Call sBuilder.AppendLine("Products:")
            '    For Each Metabolite In _Products
            '        Call sBuilder.AppendLine(String.Format("{0} = {1},  [{2}]", Metabolite.UniqueId, Metabolite.EntityCompound.Quantity, Metabolite._ConstraintTestValue))
            '    Next

            '    Call sBuilder.AppendLine(String.Format("FluxValue:= {0}", FluxValue))

            '    Call LoggingClient.WriteLine("Metabolite quantity negative exception occur!" & vbCrLf & vbCrLf & sBuilder.ToString, "", Type:=Logging.MSG_TYPES.ERR)

            '    Call Microsoft.VisualBasic.MemoryDump.CreateDump(Me).SaveTo(String.Format("{0}/{1}.cpp", Global.Settings.TEMP, UniqueId))
            'End If

            Return _FluxValue
        End Function

        Public Overrides Function ToString() As String
            Return $"{Identifier}, Flux:={FluxValue}mmol/(l*s)^-1"
        End Function

        Public Shared Function CreateObject(Of TMetabolismFlux As MetabolismFlux) _
            (obj As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction,
             Metabolites As EngineSystem.ObjectModels.Entity.Compound()) As TMetabolismFlux

            Dim Reaction As TMetabolismFlux = CreateBasicalObject(Of TMetabolismFlux)(obj)

            If Not obj.Reactants Is Nothing Then Reaction._Reactants = __createReferences(obj.Reactants, Metabolites)
            If Not obj.Products Is Nothing Then Reaction._Products = __createReferences(obj.Products, Metabolites)

            Return Reaction
        End Function

        Private Shared Function __createReferences(refData As Generic.IEnumerable(Of GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference),
                                                           Metabolites As EngineSystem.ObjectModels.Entity.Compound()) As EquationModel.CompoundSpecieReference()

            Dim LQuery = (From Compound In refData Select ObjectModels.Module.EquationModel.CompoundSpecieReference.CreateObject(Model:=Compound, Metabolites:=Metabolites)).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' 将SpeciesReference转换为相应的代谢物的对象模型，其实最主要的部分是对动力学模型的初始化操作
        ''' </summary>
        ''' <param name="Metabolites"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function Initialize(Metabolites As ObjectModels.Entity.Compound(), SystemLogging As LogFile) As Integer
            Call FillMetabolites(SystemLogging)

            If Not Me._BaseType.get_Regulators.IsNullOrEmpty Then
                '初始化反应对象的调控因子
                Dim LinkRegulatorQuery = From Regulator In Me._BaseType.get_Regulators Select New Entity.ReactionModifier(Regulator, Metabolites.GetItem(Regulator.Identifier)) '
                Me._Modifiers = LinkRegulatorQuery.ToArray
            End If
            _KineticsModel = New EngineSystem.MathematicsModels.GenericKinetic(FluxObject:=Me, SystemLogging:=SystemLogging)

            Return 0
        End Function

        Public Overrides ReadOnly Property TypeId As ObjectModel.TypeIds
            Get
                Return TypeIds.MetabolismFlux
            End Get
        End Property

        Public Overrides ReadOnly Property FluxValue As Double
            Get
                Return _FluxValue
            End Get
        End Property
    End Class

    Public Class ExpressionConstraintFlux : Inherits [Module].EnzymaticFlux
        Implements EnzymaticFlux.IEnzymaticFlux

        <DumpNode> Protected Friend _RegulationConstraint As Entity.Compound
        <DumpNode> Protected Friend CompositionDelayEffect As Double
        <DumpNode> Friend Shadows _EnzymeKinetics As MathematicsModels.EnzymeKinetics.ExpressionKinetics

        Public Overrides Function Invoke() As Double
            Dim FluxValue As Double

            Call _CatalystActivity.Clear()

            For Each Enzyme In Enzymes.Shuffles
                FluxValue = KineticsModel.GetValue
                FluxValue = _EnzymeKinetics.GetFluxValue(FluxValue, _RegulationConstraint.Quantity, Enzyme)
                Call _CatalystActivity.Add(New IEnzymaticFlux.EnzymeCatalystActivity With {.EnzymeCatalystId = Enzyme.Identifier, .Value = FluxValue})
                Call InvokeFluxConstraintsAndFlowing(FluxValue)
            Next

            Return Me.FluxValue
        End Function
    End Class
End Namespace
