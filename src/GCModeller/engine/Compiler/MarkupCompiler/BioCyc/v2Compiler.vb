﻿#Region "Microsoft.VisualBasic::8b5b694d1cedeec045f40007d3aea205, engine\Compiler\MarkupCompiler\BioCyc\v2Compiler.vb"

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

    '   Total Lines: 211
    '    Code Lines: 180 (85.31%)
    ' Comment Lines: 3 (1.42%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 28 (13.27%)
    '     File Size: 8.62 KB


    '     Class v2Compiler
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CompileImpl, createCompounds, createEnzyme, createKinetics, createReactions
    '                   enzymaticReaction, nonEnzymaticReaction, PreCompile
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Development
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.CompilerServices

Namespace MarkupCompiler.BioCyc

    Public Class v2Compiler : Inherits Compiler(Of VirtualCell)

        ReadOnly biocyc As Workspace
        ReadOnly genbank As GBFF.File

        Sub New(genbank As GBFF.File, biocyc As Workspace)
            Me.biocyc = biocyc
            Me.genbank = genbank
        End Sub

        Protected Overrides Function PreCompile(args As CommandLine) As Integer
            Dim info As New StringBuilder

            Using writer As New StringWriter(info)
                Call CLITools.AppSummary(GetType(v2Compiler).Assembly.FromAssembly, "", "", writer)
            End Using

            m_compiledModel = New VirtualCell With {
                .taxonomy = genbank.Source.GetTaxonomy
            }
            m_logging.WriteLine(info.ToString)

            Return 0
        End Function

        Protected Overrides Function CompileImpl(args As CommandLine) As Integer
            m_compiledModel.metabolismStructure = New MetabolismStructure With {
                .reactions = createReactions(),
                .enzymes = createEnzyme.ToArray,
                .compounds = createCompounds.ToArray
            }

            Dim usedIndex As Index(Of String) = m_compiledModel _
                .metabolismStructure _
                .compounds _
                .Select(Function(c) c.ID) _
                .Distinct _
                .Indexing

            ' join enzyme kinetics compounds
            Dim missing = m_compiledModel.metabolismStructure.enzymes _
                .Select(Function(enz) enz.catalysis) _
                .IteratesALL _
                .Select(Function(cat) cat.parameter) _
                .IteratesALL _
                .Select(Function(par) par.target) _
                .Where(Function(id) Not id.StringEmpty) _
                .Distinct _
                .Where(Function(id) Not id Like usedIndex) _
                .Select(Function(id)
                            Return New Compound With {
                                .ID = id,
                                .formula = "",
                                .name = id
                            }
                        End Function) _
                .ToArray

            m_compiledModel.metabolismStructure.compounds = m_compiledModel _
                .metabolismStructure _
                .compounds _
                .JoinIterates(missing) _
                .ToArray

            Return 0
        End Function

        Private Iterator Function createCompounds() As IEnumerable(Of Compound)
            For Each cpd As compounds In biocyc.compounds.features
                Yield New Compound With {
                    .ID = cpd.uniqueId,
                    .name = cpd.commonName
                }
            Next
        End Function

        Private Iterator Function createEnzyme() As IEnumerable(Of Enzyme)
            Dim enzList = biocyc.enzrxns.features.GroupBy(Function(a) a.enzyme).ToArray

            For Each enz As IGrouping(Of String, enzrxns) In enzList
                Dim ecList = enz.Where(Function(d) Not d.EC_number Is Nothing).ToArray
                Dim ecNumber = ecList.GroupBy(Function(id) id.ToString).OrderByDescending(Function(l) l.Count).FirstOrDefault

                Yield New Enzyme With {
                    .ECNumber = If(ecNumber Is Nothing, Nothing, ecNumber.Key),
                    .proteinID = enz.Key,
                    .KO = Nothing,
                    .catalysis = enz _
                        .Select(Function(a)
                                    Return createKinetics(a)
                                End Function) _
                        .Where(Function(c) Not c Is Nothing) _
                        .ToArray
                }
            Next
        End Function

        Private Function createKinetics(a As enzrxns) As Catalysis
            ' ((kcat * E) * S) / (Km + S)
            ' (Vmax * S) / (Km + S)
            Dim dynamics As FunctionElement
            Dim substrate As String
            Dim params As KineticsParameter()

            If (Not a.Kcat.IsNullOrEmpty) AndAlso (Not a.Km.IsNullOrEmpty) Then
                Dim Kcat = a.Kcat.FirstOrDefault
                Dim Km = a.Km.FirstOrDefault

                substrate = If(Kcat.substrate, Km.substrate)
                params = {
                    New KineticsParameter With {.name = "s", .target = substrate, .value = Double.NaN, .isModifier = False},
                    New KineticsParameter With {.name = "E", .target = a.enzyme, .value = Double.NaN, .isModifier = True}
                }
                dynamics = New FunctionElement With {
                    .name = a.commonName,
                    .parameters = {"E", "s"},
                    .lambda = $"(({Kcat.Km} * E) * s) / ({Km.Km} + s)"
                }
            ElseIf a.Km.IsNullOrEmpty Then
                Return Nothing
            Else
                Dim Km = a.Km.FirstOrDefault
                Dim Vmax = If(a.Vmax = 0.0, 5, a.Vmax)

                substrate = Km.substrate
                params = {New KineticsParameter With {
                    .name = "s",
                    .target = substrate,
                    .value = Double.NaN,
                    .isModifier = False
                }}
                dynamics = New FunctionElement With {
                    .name = a.commonName,
                    .parameters = {"s"},
                    .lambda = $"({Vmax} * s) / ({Km.Km} + s)"
                }
            End If

            Return New Catalysis With {
                .PH = If(a.PH = 0, 7, a.PH),
                .temperature = If(a.temperature = 0, 23, a.temperature),
                .reaction = a.reaction,
                .parameter = params,
                .formula = dynamics
            }
        End Function

        Private Function createReactions() As ReactionGroup
            Dim reactions = biocyc.reactions
            Dim enzymatic = reactions.features _
                .Where(Function(rxn) rxn.ec_number IsNot Nothing OrElse Not rxn.enzymaticReaction.IsNullOrEmpty) _
                .ToArray
            Dim non_enzymatic = reactions.features _
                .Where(Function(rxn)
                           Return rxn.ec_number Is Nothing AndAlso rxn.enzymaticReaction.IsNullOrEmpty
                       End Function) _
                .ToArray

            Return New ReactionGroup With {
                .none_enzymatic = non_enzymatic _
                    .Select(Function(r) nonEnzymaticReaction(r)) _
                    .ToArray,
                .enzymatic = enzymatic _
                    .Select(Function(r) enzymaticReaction(r)) _
                    .ToArray
            }
        End Function

        Private Function enzymaticReaction(rxn As reactions) As Reaction
            Dim left = rxn.equation.Reactants.Select(Function(c) New CompoundFactor(c.Stoichiometry, c.ID)).ToArray
            Dim right = rxn.equation.Products.Select(Function(c) New CompoundFactor(c.Stoichiometry, c.ID)).ToArray

            Return New Reaction With {
                .ID = rxn.uniqueId,
                .is_enzymatic = True,
                .bounds = {5, 5},
                .name = rxn.commonName,
                .substrate = left,
                .product = right
            }
        End Function

        Private Function nonEnzymaticReaction(rxn As reactions) As Reaction
            Dim left = rxn.equation.Reactants.Select(Function(c) New CompoundFactor(c.Stoichiometry, c.ID)).ToArray
            Dim right = rxn.equation.Products.Select(Function(c) New CompoundFactor(c.Stoichiometry, c.ID)).ToArray

            Return New Reaction With {
                .ID = rxn.uniqueId,
                .bounds = {5, 5},
                .is_enzymatic = False,
                .name = rxn.commonName,
                .substrate = left,
                .product = right
            }
        End Function
    End Class
End Namespace
