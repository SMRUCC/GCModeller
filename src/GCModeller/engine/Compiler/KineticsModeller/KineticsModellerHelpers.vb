#Region "Microsoft.VisualBasic::04ec1c21023e6e41b62971a5a91986aa, GCModeller\engine\Compiler\KineticsModeller\KineticsModellerHelpers.vb"

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

    '   Total Lines: 89
    '    Code Lines: 68
    ' Comment Lines: 10
    '   Blank Lines: 11
    '     File Size: 3.34 KB


    ' Module KineticsModellerHelpers
    ' 
    '     Function: compoundIdNameIndex, parseKineticsParameters
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Data.SABIORK.SBML
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.Model.SBML.Level3

Module KineticsModellerHelpers

    <Extension>
    Friend Function compoundIdNameIndex(vcell As VirtualCell) As Dictionary(Of String, String)
        Dim index As New Dictionary(Of String, String)
        Dim name As String

        For Each cpd As Compound In vcell.metabolismStructure.compounds
            name = cpd.name

            If Not index.ContainsKey(name) Then
                Call index.Add(name, cpd.ID)
            End If
        Next

        Return index
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    <Extension>
    Friend Iterator Function parseKineticsParameters(reaction As SBMLReaction,
                                                     index As SBMLInternalIndexer,
                                                     KO$,
                                                     compoundId As Dictionary(Of String, String)) As IEnumerable(Of KineticsParameter)

        Dim locals As Dictionary(Of String, localParameter) = reaction.kineticLaw.listOfLocalParameters.ToDictionary(Function(a) a.id)
        Dim local As localParameter
        Dim id As String
        Dim enzyme As species

        For Each require As String In reaction.kineticLaw.math.apply.ci _
            .Skip(1) _
            .Select(AddressOf Strings.Trim)

            If locals.ContainsKey(require) Then
                local = locals(require)
                ' 常数值
                Yield New KineticsParameter With {
                    .name = local.id,
                    .target = local.name,
                    .value = local.value
                }
            Else
                ' 代谢物浓度变量对象
                id = index.getKEGGCompoundId(require)

                If id.StringEmpty Then
                    ' 可能是酶分子
                    ' 也包含有kegg代谢物id
                    enzyme = index.getSpecies(require)

                    If compoundId.ContainsKey(enzyme.name) Then
                        Yield New KineticsParameter With {
                            .name = require,
                            .target = compoundId(enzyme.name),
                            .value = Double.NaN,
                            .isModifier = True
                        }
                    Else
                        ' enzyme?
                        Yield New KineticsParameter With {
                            .name = require,
                            .target = KO,
                            .value = Double.NaN,
                            .isModifier = True
                        }
                    End If
                Else
                    ' 是代谢物反应底物或者产物
                    Yield New KineticsParameter With {
                        .name = require,
                        .target = id,
                        .value = Double.NaN
                    }
                End If
            End If
        Next
    End Function
End Module
