#Region "Microsoft.VisualBasic::e19495cd45423e3052c038936e522c85, engine\GCModeller\EngineSystem\ObjectModels\Module\CentralDogma\BasalExpression.vb"

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

    '     Class BasalExpression
    ' 
    '         Properties: FluxValue, TypeId
    ' 
    '         Function: InitializeConstraints, Invoke, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization

Namespace EngineSystem.ObjectModels.Module.CentralDogmaInstance

    ''' <summary>
    ''' 这个主要是由于没有被计算出来调控关系的基因不能够正常的进行表达的原因，在这里使用一个随机数来产生本底表达
    ''' </summary>
    ''' <remarks></remarks>
    Public Class BasalExpression : Inherits ExpressionProcedureEvent

        <DumpNode> Protected Friend DeletedMutation As Boolean = False
        <DumpNode> Protected Friend BasalLevel As Double
        <DumpNode> Protected _FluxValue As Double
        ''' <summary>
        ''' 用于调试的时候观察所使用的
        ''' </summary>
        ''' <remarks></remarks>
        <DumpNode> Protected Friend ExpressedTranscript As Entity.Transcript

        Protected Friend Overloads Overrides Function InitializeConstraints(MetabolismSystem As SubSystem.MetabolismCompartment) As Integer
            Dim ConstraintMapping = MetabolismSystem.ConstraintMetabolite

            Me.Identifier = String.Format("{0}.BasalExpression", Template.locusId)
            MyBase.CompositionDelayEffect = Global.System.Math.Log10(Me.CompositionDelayEffect + 10)
            Me.ConstraintFlux = [Module].CentralDogmaInstance.Transcription.CreateConstraintFlux(Identifier,
                                                                                                 MetabolismSystem, MetabolismSystem._CellSystem.ExpressionRegulationNetwork.RNAPolymerase,
                                                                                                 MetabolismSystem._CellSystem.ExpressionRegulationNetwork.ExpressionKinetics,
                                                                                                 MetabolismSystem._CellSystem.ExpressionRegulationNetwork._TranscriptionK1,
                                                                                                 CompositionVector)
            UPPER_BOUND = ConstraintFlux._BaseType.UPPER_BOUND.Value
            Return 0
        End Function

        Public Overrides Function Invoke() As Double
            If DeletedMutation = True Then
                Return 0
            End If

            ConstraintFlux._RegulationConstraint.Quantity = Rnd() * BasalLevel / Me.CompositionDelayEffect
            ConstraintFlux.Invoke()

            Dim V As Double = ConstraintFlux.FluxValue

            If V > UPPER_BOUND OrElse Double.IsPositiveInfinity(V) Then
                V = UPPER_BOUND
            ElseIf V < 0 OrElse Double.IsNegativeInfinity(V) OrElse Double.IsNaN(V) Then
                V = 0
            End If

            Product.Quantity = Product.DataSource.value + V
            _FluxValue = V

            Return Me.FluxValue
        End Function

        Public Overrides Function ToString() As String
            Return Product.ToString
        End Function

        Public Overrides ReadOnly Property TypeId As ObjectModel.TypeIds
            Get
                Return TypeIds.BasalExpression
            End Get
        End Property

        Public Overrides ReadOnly Property FluxValue As Double
            Get
                Return _FluxValue
            End Get
        End Property
    End Class
End Namespace
