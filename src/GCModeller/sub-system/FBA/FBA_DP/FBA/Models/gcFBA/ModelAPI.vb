#Region "Microsoft.VisualBasic::5090de6980826cdfef418fb399631d24, sub-system\FBA\FBA_DP\FBA\Models\gcFBA\ModelAPI.vb"

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

    '     Module ModelAPI
    ' 
    '         Function: __creates, __getEquation, DumpModel
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.Model.SBML.Level2.Elements
Imports SMRUCC.genomics.Model.SBML.Specifics.MetaCyc

Namespace Models.rFBA

    <Package("rFBA.ModelAPI")>
    Public Module ModelAPI

        ''' <summary>
        ''' 只是将反应过程给Dump出来，没有涉及到调控信息
        ''' </summary>
        ''' <param name="model"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("Model.Dump")>
        <Extension> Public Function DumpModel(model As rFBAMetabolism) As Simpheny.RXN()
            Dim upBounds As Double() = model.__getUpbound
            Dim lowBound As Double() = model.__getLowerbound
            Dim rxns As Simpheny.RXN() = model.fluxColumns _
                .Select(Function(sId, i)
                            Return __creates(model._fluxs(sId),
                                           upBounds(i),
                                           lowBound(i),
                                           If(model.__fluxObjective.IndexOf(sId) = -1, 0, 1))
                        End Function)
            Return rxns
        End Function

        Private Function __creates(rxn As Reaction, up As Double, lower As Double, objective As Double) As Simpheny.RXN
            Dim props As New FluxPropReader(rxn.Notes)
            Dim model As New Simpheny.RXN With {
                .UPPER_BOUND = up,
                .LOWER_BOUND = lower,
                .Reversible = rxn.reversible,
                .ReactionId = rxn.id,
                .OfficialName = rxn.name,
                .Abbreviation = rxn.__getEquation,
                .Enzyme = props.GENE_ASSOCIATION,
                .Objective = objective
            }
            Return model
        End Function

        <Extension> Private Function __getEquation(rxn As Reaction) As String
            Return EquationBuilder.ToString(rxn)
        End Function
    End Module
End Namespace
