#Region "Microsoft.VisualBasic::0b35c1ffbb62fdf665ba35a8baa3c142, sub-system\FBA\FBA_DP\FBA\Models\SBML.vb"

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

    '     Class SBML
    ' 
    '         Properties: allCompounds, fluxColumns
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: __getLowerbound, __getStoichiometry, __getUpbound, getConstraint
    ' 
    '         Sub: SetObjectiveFunc
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.ObjectModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Model.SBML
Imports SMRUCC.genomics.Model.SBML.Level2.Elements
Imports SMRUCC.genomics.Model.SBML.Specifics.MetaCyc

Namespace Models

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SBML : Inherits lpSolveRModel

        Protected Friend SBMLData As Level2.XmlFile
        Protected Friend Overrides ReadOnly Property fluxColumns As ReadOnlyCollection(Of String)
        Protected Friend Overrides ReadOnly Property allCompounds As ReadOnlyCollection(Of String)

        Protected Friend ReadOnly _fluxs As IReadOnlyDictionary(Of String, Reaction)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="SBMl"></param>
        ''' <param name="objectiveFunction">UniqueId list for the target metabolism reactions.(代谢反应对象的UniqueId列表)</param>
        ''' <param name="forceEnzymeRev">强制所有的酶促反应过程为可逆类型的</param>
        ''' <remarks></remarks>
        Sub New(SBMl As Level2.XmlFile, objectiveFunction As String(), forceEnzymeRev As Boolean)
            Me.SBMLData = SBMl
            Me._fluxs = SBMl.Model.listOfReactions.ToDictionary(Function(x) x.id)
            Me.__fluxObjective = New ReadOnlyCollection(Of String)(objectiveFunction)
            Me.fluxColumns = New ReadOnlyCollection(Of String)(_fluxs.Keys.ToArray)
            Me.allCompounds = New ReadOnlyCollection(Of String)((From x As Reaction In _fluxs.Values.AsParallel
                                                                 Select x.GetMetabolites.Select(
                                                                     Function(m) m.species)).IteratesALL.Distinct.ToArray)
            If forceEnzymeRev Then
                For Each x In Me._fluxs.Values
                    Dim props As New FluxPropReader(x.Notes)
                    If Not props.GENE_ASSOCIATION.IsNullOrEmpty Then
                        x.reversible = True
                    End If
                Next
            End If
        End Sub

        Protected Friend Overrides Function getConstraint(idx As String) As KeyValuePair(Of String, Double)
            Return SBML._constraint
        End Function

        Protected Friend Overrides Function __getLowerbound() As Double()
            Return (From rxn In _fluxs Select rxn.Value.LowerBound).ToArray
        End Function

        Protected Friend Overrides Function __getUpbound() As Double()
            Return (From rxn In _fluxs Select rxn.Value.UpperBound).ToArray
        End Function

        Public Overrides Sub SetObjectiveFunc(factors() As String)
            __fluxObjective = New ReadOnlyCollection(Of String)(factors)
        End Sub

        Protected Friend Overrides Function __getStoichiometry(metabolite As String, rxn As String) As Double
            Dim flux As Reaction = _fluxs(rxn)
            Return flux.GetCoEfficient(metabolite)
        End Function
    End Class
End Namespace
