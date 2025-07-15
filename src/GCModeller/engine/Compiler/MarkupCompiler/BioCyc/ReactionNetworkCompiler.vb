Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression
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
                .compounds = createCompounds.ToArray
            }
        End Function

        Private Iterator Function createCompounds() As IEnumerable(Of Compound)
            For Each cpd As compounds In BioCyc.compounds.features
                Yield New Compound With {
                    .ID = cpd.uniqueId,
                    .name = cpd.commonName,
                    .formula = compounds.FormulaString(cpd)
                }
            Next
        End Function

        Private Iterator Function createEnzyme() As IEnumerable(Of Enzyme)
            Dim enzList = BioCyc.enzrxns.features.GroupBy(Function(a) a.enzyme).ToArray

            For Each enz As IGrouping(Of String, enzrxns) In enzList
                Dim ecList = enz.Where(Function(d) Not d.EC_number Is Nothing).ToArray
                Dim ecNumber = ecList.GroupBy(Function(id) id.ToString).OrderByDescending(Function(l) l.Count).FirstOrDefault

                Yield New Enzyme With {
                    .ecNumber = If(ecNumber Is Nothing, Nothing, ecNumber.Key),
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
            Dim reactions = BioCyc.reactions
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