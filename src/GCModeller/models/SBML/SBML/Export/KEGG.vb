#Region "Microsoft.VisualBasic::e1abe94f8b11311c64e443e79383f128, GCModeller\models\SBML\SBML\Export\KEGG.vb"

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

    '   Total Lines: 82
    '    Code Lines: 67
    ' Comment Lines: 7
    '   Blank Lines: 8
    '     File Size: 3.71 KB


    '     Module KEGG
    ' 
    '         Function: __getModel, Exists, GetReactions
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Model.SBML.Level2.Elements
Imports SMRUCC.genomics.Model.SBML.Specifics.MetaCyc

Namespace ExportServices

    Public Module KEGG

        <Extension> Public Function GetReactions(model As Level2.XmlFile, Optional nonEnzymes As Boolean = False) As bGetObject.Reaction()
            Dim allCompounds = (From sp As Specie
                                In model.Model.listOfSpecies.AsParallel
                                Let cp As String = New SpeciesPropReader(sp.Notes).KEGG
                                Where Not String.IsNullOrEmpty(cp)
                                Select cp, sp
                                Group By cp Into Group) _
                                        .ToDictionary(Function(x) x.cp,
                                                      Function(x) x.Group.Select(Function(xx) xx.sp).ToArray)
            Dim Meta2KEGG As Dictionary(Of String, String) =
                (From x In allCompounds.AsParallel
                 Select (From xx As Specie In x.Value
                         Select xx,
                             x.Key)).IteratesALL.ToDictionary(Function(x) x.xx.ID,
                                                                   Function(x) x.Key)
            Dim source = (From x As Level2.Elements.Reaction In model.Model.listOfReactions
                          Where Meta2KEGG.Exists(x)
                          Select x)
            Dim models = (From x As Level2.Elements.Reaction In source.AsParallel
                          Let created = __getModel(x, Meta2KEGG, nonEnzymes)
                          Where Not created Is Nothing
                          Select created).ToArray
            Return models
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="rxn"></param>
        ''' <param name="allCompounds"></param>
        ''' <param name="filterNonEnzyme">只会生成非酶促反应的数据</param>
        ''' <returns></returns>
        Private Function __getModel(rxn As Level2.Elements.Reaction, allCompounds As Dictionary(Of String, String), Optional filterNonEnzyme As Boolean = False) As bGetObject.Reaction
            Dim props As New FluxPropReader(rxn.Notes)

            If filterNonEnzyme Then
                If Not props.ECNumber.IsNullOrEmpty Then
                    Return Nothing
                End If
            End If

            Dim equation As New Equation(rxn.Reactants, rxn.Products, allCompounds, rxn.reversible)
            Dim def As New Equation(rxn.Reactants, rxn.Products, rxn.reversible)
            Dim model As New bGetObject.Reaction With {
                .ID = rxn.id,
                .CommonNames = {rxn.name},
                .Comments = rxn.Notes.Text,
                .Equation = equation.ToString,
                .Enzyme = props.ECNumber,
                .Definition = def.ToString
            }
            Return model
        End Function

        <Extension> Public Function Exists(allCompunds As Dictionary(Of String, String), rxn As Level2.Elements.Reaction) As Boolean
            For Each x In rxn.Reactants
                If Not allCompunds.ContainsKey(x.species) Then
                    Return False
                End If
            Next
            For Each x In rxn.Products
                If Not allCompunds.ContainsKey(x.species) Then
                    Return False
                End If
            Next

            Return True
        End Function
    End Module
End Namespace
