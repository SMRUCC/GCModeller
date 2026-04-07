#Region "Microsoft.VisualBasic::1919f89f5921b971661ffe4961a55144, engine\Compiler\MarkupCompiler\BioCyc\ReactionNetworkCompiler.vb"

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

    '   Total Lines: 201
    '    Code Lines: 172 (85.57%)
    ' Comment Lines: 2 (1.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 27 (13.43%)
    '     File Size: 8.32 KB


    '     Class ReactionNetworkCompiler
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: BuildModel, CheckEnzymatic, createCompounds, CreateCompounds, createEnzyme
    '                   createKinetics, CreateMaps, createReactions, enzymaticReaction, nonEnzymaticReaction
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2

Namespace MarkupCompiler.BioCyc

    Public Class ReactionNetworkCompiler

        ReadOnly compiler As v2Compiler
        ReadOnly biocyc As Workspace

        Sub New(compiler As v2Compiler)
            Me.compiler = compiler
            Me.biocyc = compiler.biocyc
        End Sub

        Public Function BuildModel() As MetabolismStructure
            Return New MetabolismStructure With {
                .reactions = createReactions(),
                .enzymes = createEnzyme.ToArray,
                .compounds = createCompounds.ToArray,
                .maps = CreateMaps.ToArray
            }
        End Function

        Private Iterator Function CreateMaps() As IEnumerable(Of FunctionalCategory)
            Dim pathways_list = biocyc.pathways.features.GroupBy(Function(p) p.types(0)).ToArray

            For Each category As IGrouping(Of String, pathways) In pathways_list
                Yield New FunctionalCategory With {
                    .category = category.Key,
                    .pathways = category _
                        .Select(Function(pwy)
                                    Return New Pathway With {
                                        .ID = pwy.uniqueId,
                                        .name = pwy.commonName,
                                        .note = pwy.comment,
                                        .reactions = pwy.reactionList
                                    }
                                End Function) _
                        .ToArray
                }
            Next
        End Function

        Private Iterator Function createCompounds() As IEnumerable(Of Compound)
            For Each cpd As compounds In biocyc.compounds.features
                Yield New Compound With {
                    .ID = cpd.uniqueId,
                    .name = cpd.commonName,
                    .db_xrefs = cpd.dbLinks,
                    .referenceIds = { .ID}
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

        Private Shared Function CheckEnzymatic(rxn As reactions) As Boolean
            Return rxn.ec_number IsNot Nothing OrElse
                Not rxn.enzymaticReaction.IsNullOrEmpty
        End Function

        Private Function createReactions() As ReactionGroup
            Dim reactions = biocyc.reactions
            Dim enzymatic = reactions.features _
                .Where(Function(rxn) CheckEnzymatic(rxn)) _
                .Select(Function(r) enzymaticReaction(r)) _
                .Where(Function(r) Not r Is Nothing) _
                .ToArray
            Dim non_enzymatic = reactions.features _
                .Where(Function(rxn) Not CheckEnzymatic(rxn)) _
                .Select(Function(r) nonEnzymaticReaction(r)) _
                .Where(Function(r) Not r Is Nothing) _
                .ToArray

            Return New ReactionGroup With {
                .none_enzymatic = non_enzymatic,
                .enzymatic = enzymatic
            }
        End Function

        Private Function enzymaticReaction(rxn As reactions) As Reaction
            Dim model As Reaction = nonEnzymaticReaction(rxn)

            If Not model Is Nothing Then
                model.is_enzymatic = True
                model.ec_number = rxn.ec_number _
                    .SafeQuery _
                    .Where(Function(e) Not e Is Nothing) _
                    .Select(Function(ec) ec.ECNumberString) _
                    .ToArray
            End If

            Return model
        End Function

        Private Iterator Function CreateCompounds(list As IEnumerable(Of CompoundSpecieReference)) As IEnumerable(Of CompoundFactor)
            If list Is Nothing Then
                Return
            End If

            For Each cpd As CompoundSpecieReference In list
                Yield New CompoundFactor(cpd.Stoichiometry, cpd.ID, compiler.MapCompartId(cpd.Compartment))
            Next
        End Function

        Private Function nonEnzymaticReaction(rxn As reactions) As Reaction
            Dim left = CreateCompounds(rxn.left).ToArray
            Dim right = CreateCompounds(rxn.right).ToArray

            If left.IsNullOrEmpty OrElse right.IsNullOrEmpty Then
                Call $"Missing reactants or products from the reaction {rxn.uniqueId} ({rxn.commonName})!".Warning
                Return Nothing
            End If

            Return New Reaction With {
                .ID = rxn.uniqueId,
                .bounds = {5, 5},
                .is_enzymatic = False,
                .name = rxn.commonName,
                .substrate = left,
                .product = right,
                .compartment = compiler.MapCompartId(rxn.reactionLocations),
                .note = rxn.comment
            }
        End Function
    End Class
End Namespace
