#Region "Microsoft.VisualBasic::74c2100226347e98e08bb2889b96227b, sub-system\FBA\FBA_DP\FBA\Models\KEGG\KEGGXml.vb"

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

    '     Class KEGGXml
    ' 
    '         Properties: allCompounds, fluxColumns
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: __countsMapping, __getLowerbound, __getStoichiometry, __getUpbound, getConstraint
    '                   GetEquation, GetName
    ' 
    '         Sub: SetGeneObjectives, SetObjectiveFunc
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.ObjectModel
Imports SMRUCC.genomics.Assembly.KEGG.Archives.Xml
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports Microsoft.VisualBasic.Linq

Namespace Models

    Public Class KEGGXml : Inherits lpSolveRModel

        ReadOnly _model As XmlModel
        ReadOnly _reactions As IReadOnlyDictionary(Of String, Equation)

        Dim _baseFactor As Double = 2.5

        Protected Friend Overrides ReadOnly Property fluxColumns As ReadOnlyCollection(Of String)
        Protected Friend Overrides ReadOnly Property allCompounds As ReadOnlyCollection(Of String)

        Public Overrides Function GetName(rxn As String) As String
            Dim flux = _model.GetReaction(rxn)

            If flux Is Nothing Then
                Return ""
            Else
                Return flux.CommonNames.FirstOrDefault
            End If
        End Function

        Public Overrides Function GetEquation(rxn As String) As String
            If _reactions.ContainsKey(rxn) Then
                Return _reactions(rxn).ToString
            Else
                Return ""
            End If
        End Function

        Sub New(model As XmlModel)
            _model = model
            _reactions = (From x As bGetObject.Reaction In model.Metabolome
                          Select x
                          Group x By x.ID Into Group) _
                                .ToDictionary(Function(x) x.ID,
                                              Function(x) x.Group.First.ReactionModel)

            Dim array As String() = _reactions.Keys.ToArray
            fluxColumns = New ReadOnlyCollection(Of String)(array)  ' 直接赋值为什么会有BUG？？
            array = (From x As Equation
                     In _reactions.Values.AsParallel
                     Select x.GetMetabolites.Select(
                         Function(m) m.ID)).IteratesALL.Distinct.ToArray
            allCompounds = New ReadOnlyCollection(Of String)(array)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="factors">代谢反应编号</param>
        Public Overrides Sub SetObjectiveFunc(factors() As String)
            __fluxObjective = New ReadOnlyCollection(Of String)(factors)
        End Sub

        ''' <summary>
        ''' 从基因号映射到代谢过程
        ''' </summary>
        ''' <param name="locus"></param>
        Public Sub SetGeneObjectives(locus As IEnumerable(Of String))
            Dim LQuery = (From x As String
                          In locus
                          Let map As Nodes.EC_Mapping = _model.GetMaps(x)
                          Where Not map Is Nothing
                          Select map.ECMaps.Select(Function(m) m.Reactions).IteratesALL).IteratesALL
            Dim rxns As String() = LQuery.Distinct.ToArray
            Call SetObjectiveFunc(factors:=rxns)
        End Sub

        Protected Friend Overrides Function __getStoichiometry(metabolite As String, rxn As String) As Double
            Dim rxnFlux As Equation = _reactions(rxn)
            Return rxnFlux.GetCoEfficient(metabolite)
        End Function

        Protected Friend Overrides Function __getLowerbound() As Double()
            Dim LQuery = (From x As String In fluxColumns
                          Let dir As Boolean = _reactions(x).Reversible
                          Select If(dir = True, -1 * __countsMapping(x) * _baseFactor, 0R)).ToArray
            Return LQuery
        End Function

        Protected Friend Overrides Function __getUpbound() As Double()
            Dim LQuery = (From x As String In fluxColumns
                          Let n As Integer = __countsMapping(rxn:=x)
                          Select n * _baseFactor).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' 1 个是基本反应
        ''' </summary>
        ''' <param name="rxn"></param>
        ''' <returns></returns>
        Private Function __countsMapping(rxn As String) As Integer
            Dim LQuery = (From x As Nodes.EC_Mapping
                          In _model.EC_Mappings.AsParallel
                          Where x.ContainsRxn(rxn)
                          Select 1).Sum + 1
            Return LQuery
        End Function

        Protected Friend Overrides Function getConstraint(metabolite As String) As KeyValuePair(Of String, Double)
            Return MyBase._constraint
        End Function
    End Class
End Namespace
