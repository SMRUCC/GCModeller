#Region "Microsoft.VisualBasic::e9b01dedc4ff9f901c5d6d3e1091194b, GCModeller\engine\IO\GCMarkupLanguage\GCML_Documents\XmlElements\Metabolism\Reaction\Reaction.vb"

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


    ' Code Statistics:

    '   Total Lines: 282
    '    Code Lines: 178
    ' Comment Lines: 66
    '   Blank Lines: 38
    '     File Size: 13.40 KB


    '     Class Reaction
    ' 
    '         Properties: __ObjectiveCoefficient, DynamicsRegulators, EC, Enzymes, Equation
    '                     Identifier, IFBAC2_LOWER_BOUND, IFBAC2_UPPER_BOUND, IsEnzymaticMetabolismFlux, LOWER_BOUND
    '                     Metabolites, Name, ObjectiveCoefficient, p_Dynamics_K_1, p_Dynamics_K_2
    '                     Products, Reactants, Reversible, Substrates, TCSCrossTalk
    '                     UPPER_BOUND
    ' 
    '         Function: _add_Regulator, CastTo, get_Regulators, GetStoichiometry, ToString
    '         Structure Parameter
    ' 
    '             Function: ToString
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.MetaCyc
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.Assembly.MetaCyc.Schema.Metabolism
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.ComponentModels
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements
Imports SMRUCC.genomics.Model.SBML
Imports SMRUCC.genomics.Model.SBML.FLuxBalanceModel
Imports SMRUCC.genomics.Model.SBML.Level2.Elements

Namespace GCML_Documents.XmlElements.Metabolism

    Public Class Reaction : Inherits T_MetaCycEntity(Of Slots.Reaction)
        Implements FLuxBalanceModel.I_ReactionModel(Of CompoundSpeciesReference)
        Implements I_BiologicalProcess_EventHandle

        <XmlArray>
        Public Property Reactants As CompoundSpeciesReference() Implements FLuxBalanceModel.I_ReactionModel(Of CompoundSpeciesReference).Reactants
        <XmlArray>
        Public Property Products As CompoundSpeciesReference() Implements FLuxBalanceModel.I_ReactionModel(Of CompoundSpeciesReference).Products

        <XmlElement("p_LOWER_BOUND", Namespace:="http://code.google.com/p/genome-in-code/GCModeller/Dynamics")>
        Public Property LOWER_BOUND As Parameter
        <XmlElement("p_UPPER_BOUND", Namespace:="http://code.google.com/p/genome-in-code/GCModeller/Dynamics")>
        Public Property UPPER_BOUND As Parameter

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>为了实现IReaction接口的需要所进行的属性值的复写</remarks>
        <XmlAttribute("UniqueId")>
        Public Overrides Property Identifier As String Implements FLuxBalanceModel.I_ReactionModel(Of CompoundSpeciesReference).Key
            Get
                Return MyBase.Identifier
            End Get
            Set(value As String)
                MyBase.Identifier = value
            End Set
        End Property

        Public Property TCSCrossTalk As Boolean = False

        ''' <summary>
        ''' 本生化反应过程对象是否为可逆的反应对象
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Reversible As Boolean Implements FLuxBalanceModel.I_ReactionModel(Of CompoundSpeciesReference).Reversible

        ''' <summary>
        ''' 获取本反应中所使用的所有的反应代谢物
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Metabolites As speciesReference()
            Get
                Return New List(Of speciesReference)(From x As CompoundSpeciesReference
                                                     In Reactants
                                                     Select New speciesReference(x)) + From x As CompoundSpeciesReference
                                                                                       In Products
                                                                                       Select New speciesReference(x)
            End Get
        End Property

        Public ReadOnly Property Equation As String
            Get
                Dim sBuilder As StringBuilder = New StringBuilder(1024)
                For Each Metabolite In Reactants
                    If Metabolite.StoiChiometry = 1 Then
                        Call sBuilder.AppendFormat("{0} + ", Metabolite.Identifier)
                    Else
                        Call sBuilder.AppendFormat("{0} {1} + ", Metabolite.StoiChiometry, Metabolite.Identifier)
                    End If
                Next
                Call sBuilder.Remove(sBuilder.Length - 3, 3)
                If Reversible Then
                    Call sBuilder.Append(" <-> ")
                Else
                    Call sBuilder.Append(" -> ")
                End If
                For Each Metabolite In Products
                    If Metabolite.StoiChiometry = 1 Then
                        Call sBuilder.AppendFormat("{0} + ", Metabolite.Identifier)
                    Else
                        Call sBuilder.AppendFormat("{0} {1} + ", Metabolite.StoiChiometry, Metabolite.Identifier)
                    End If
                Next
                Call sBuilder.Remove(sBuilder.Length - 3, 3)

                Return sBuilder.ToString
            End Get
        End Property

        ''' <summary>
        ''' Reaction Displaying title.(本生化反应对象的显示标题)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement> Public Property Name As String Implements I_ReactionModel(Of GCML_Documents.ComponentModels.CompoundSpeciesReference).Name

        ''' <summary>
        ''' UniqueId of the Enzymes.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement> Public Property Enzymes As String()
        <XmlAttribute> Public Property EC As String

        <XmlAttribute> Public Property ObjectiveCoefficient As Integer

        ''' <summary>
        ''' 正向反应的反应常数
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement("p_Dynamics_1", Namespace:="http://code.google.com/p/genome-in-code/GCModeller/Dynamics")> Public Property p_Dynamics_K_1 As Double
        ''' <summary>
        ''' 该反应的反方向的常数
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement("p_Dynamics_2", Namespace:="http://code.google.com/p/genome-in-code/GCModeller/Dynamics")> Public Property p_Dynamics_K_2 As Double

        Public ReadOnly Property IsEnzymaticMetabolismFlux As Boolean
            Get
                Return Not (Enzymes Is Nothing OrElse Enzymes.Count = 0)
            End Get
        End Property

        'Public Function BlockDown() As Boolean
        '    LOWER_BOUND = New Parameter With {.Value = 0, .Units = LOWER_BOUND.Units}
        '    UPPER_BOUND = New Parameter With {.Value = 0, .Units = UPPER_BOUND.Units}
        '    Return True
        'End Function

        'Public Function Restore() As Boolean
        '    LOWER_BOUND = New Parameter With {.Value = _LOWER_BOUND, .Units = LOWER_BOUND.Units}
        '    UPPER_BOUND = New Parameter With {.Value = _UPPER_BOUND, .Units = UPPER_BOUND.Units}
        '    Return True
        'End Function

        Public Overrides Function ToString() As String
            Return String.Format("[{0}]  {1}", Identifier, Equation)
        End Function

        Public Shared Widening Operator CType(e As Slots.Reaction) As Reaction
            Dim Reaction As Reaction = New Reaction With {.BaseType = e}
            Dim Direction As ReactionDirections = Directions(e.ReactionDirection)

            Reaction.Identifier = e.Identifier
            Reaction.Name = e.CommonName
            Reaction.Reversible = Direction = ReactionDirections.Reversible
            If Direction = ReactionDirections.RightToLeft Then
                Reaction.Reactants = e.Right.AsMetabolites
                Reaction.Products = e.Left.AsMetabolites
            Else
                Reaction.Reactants = e.Left.AsMetabolites
                Reaction.Products = e.Right.AsMetabolites
            End If
            Reaction.LOWER_BOUND = New Parameter With {.Value = 0, .Units = "mmol_per_gDW_per_hr"}
            Reaction.UPPER_BOUND = New Parameter With {.Value = 10, .Units = "mmol_per_gDW_per_hr"}
            Reaction.ObjectiveCoefficient = 1

            Return Reaction
        End Operator

        Public Shared Function CastTo(e As Level2.Elements.Reaction, Model As BacterialModel) As Reaction
            Dim Reaction As Reaction = New Reaction
            Reaction.Reactants = (From item In e.Reactants Select GCML_Documents.ComponentModels.CompoundSpeciesReference.CreateObject(item)).ToArray
            Reaction.Products = (From item In e.Products Select GCML_Documents.ComponentModels.CompoundSpeciesReference.CreateObject(item)).ToArray
            Reaction.Identifier = e.name
            Reaction.Reversible = e.reversible
            Reaction.LOWER_BOUND = (From p In e.kineticLaw.listOfParameters Where String.Equals(p.id, "LOWER_BOUND") Select p).First  '
            Reaction.UPPER_BOUND = (From p In e.kineticLaw.listOfParameters Where String.Equals(p.id, "UPPER_BOUND") Select p).First  '
            Reaction.ObjectiveCoefficient = e.kineticLaw.ObjectiveCoefficient

            Return Reaction
        End Function

        Public Structure Parameter
            <XmlAttribute> Dim Value As Double
            <XmlAttribute> Dim Units As String

            Public Overrides Function ToString() As String
                Return String.Format("{0}({1})", Value, Units)
            End Function

            Public Shared Widening Operator CType(value As Double) As Parameter
                Return New Parameter With {.Value = value, .Units = "/"}
            End Operator

            Public Shared Widening Operator CType(e As Level2.Elements.parameter) As Parameter
                Return New Parameter With {.Value = e.value, .Units = e.units}
            End Operator
        End Structure

        ''' <summary>
        ''' 返回目标代谢物在本反应对象之中的化学计量数，不存在的时候返回0，
        ''' 当目标代谢物存在于Reactants列表之中的时候，返回的化学计量数小于0，即目标对象在本方程中是被消耗的对象；
        ''' 反之当目标对象存在于Products列表中的时候，返回的化学计量数大于零，即目标对象在本方程之中是合成的目标对象
        ''' </summary>
        ''' <param name="Metabolite">目标代谢物对象的UniqueID属性值</param>
        ''' <returns>目标代谢物在本反应对象之中的化学计量数</returns>
        ''' <remarks></remarks>
        Public Function GetStoichiometry(Metabolite As String) As Double Implements I_ReactionModel(Of GCML_Documents.ComponentModels.CompoundSpeciesReference).GetStoichiometry
            Dim QueryResults As GCML_Documents.ComponentModels.CompoundSpeciesReference()

            If Not Reactants.IsNullOrEmpty Then
                QueryResults = (From e In Reactants Where String.Equals(Metabolite, e.Identifier) Select e).ToArray  '
                If QueryResults.Count > 0 Then
                    Return -1 * QueryResults.First.StoiChiometry
                End If
            End If

            If Not Products.IsNullOrEmpty Then
                QueryResults = (From e In Products Where String.Equals(Metabolite, e.Identifier) Select e).ToArray
                If QueryResults.Count > 0 Then
                    Return QueryResults.First.StoiChiometry '目标代谢物在本方程式对象中是被合成的
                End If
            End If

            Return 0  'not exists in this reaction.
        End Function

        Public ReadOnly Property IFBAC2_LOWER_BOUND As Double Implements I_ReactionModel(Of GCML_Documents.ComponentModels.CompoundSpeciesReference).LOWER_BOUND
            Get
                Return LOWER_BOUND.Value
            End Get
        End Property

        Public ReadOnly Property IFBAC2_UPPER_BOUND As Double Implements I_ReactionModel(Of GCML_Documents.ComponentModels.CompoundSpeciesReference).UPPER_BOUND
            Get
                Return UPPER_BOUND.Value
            End Get
        End Property

        <XmlIgnore>
        Public ReadOnly Property __ObjectiveCoefficient As Integer Implements I_ReactionModel(Of GCML_Documents.ComponentModels.CompoundSpeciesReference).ObjectiveCoefficient
            Get
                Return ObjectiveCoefficient
            End Get
        End Property

        ''' <summary>
        ''' 对反应过程起到调控作用的，而非对酶分子的活性起到调控作用的调控分子
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DynamicsRegulators As List(Of GCML_Documents.XmlElements.SignalTransductions.Regulator)

        Public ReadOnly Property Substrates As String()
            Get
                Return {(From Metabolite In Me.Reactants Select Metabolite.Identifier).ToArray, (From Metabolite In Me.Products Select Metabolite.Identifier).ToArray}.ToVector
            End Get
        End Property

        Public Function get_Regulators() As SignalTransductions.Regulator() Implements I_BiologicalProcess_EventHandle.get_Regulators
            If Me.DynamicsRegulators.IsNullOrEmpty Then
                Return New SignalTransductions.Regulator() {}
            End If
            Return Me.DynamicsRegulators.ToArray
        End Function

        Public Function _add_Regulator(internal_GUID As String, Regulator As SignalTransductions.Regulator) As Boolean Implements I_BiologicalProcess_EventHandle._add_Regulator
            Call Me.DynamicsRegulators.Add(Regulator)
            Return True
        End Function
    End Class
End Namespace
