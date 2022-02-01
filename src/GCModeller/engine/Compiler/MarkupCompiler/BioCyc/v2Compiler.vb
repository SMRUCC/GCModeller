Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Development
Imports Microsoft.VisualBasic.CommandLine
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
                .reactions = createReactions()
            }

            Return 0
        End Function

        Private Function createReactions() As ReactionGroup
            Dim reactions = biocyc.reactions
            Dim enzymatic = reactions.features.Where(Function(rxn) Not rxn.ec_number Is Nothing).ToArray
            Dim non_enzymatic = reactions.features _
                .Where(Function(rxn)
                           Return rxn.ec_number Is Nothing AndAlso rxn.enzymaticReaction.IsNullOrEmpty
                       End Function) _
                .ToArray

            Return New ReactionGroup With {
                .etc = non_enzymatic _
                    .Select(Function(r) nonEnzymaticReaction(r)) _
                    .ToArray,
                .enzymatic = enzymatic _
                    .Select(Function(r) enzymaticReaction(r)) _
                    .ToArray
            }
        End Function

        Private Function enzymaticReaction(rxn As reactions) As Reaction
            Return New Reaction With {
                .ID = rxn.uniqueId,
                .is_enzymatic = True,
                .bounds = {5, 5},
                .Equation = rxn.equation.ToString,
                .name = rxn.commonName
            }
        End Function

        Private Function nonEnzymaticReaction(rxn As reactions) As Reaction
            Return New Reaction With {
                .ID = rxn.uniqueId,
                .bounds = {5, 5},
                .is_enzymatic = False,
                .name = rxn.commonName,
                .Equation = rxn.equation.ToString
            }
        End Function
    End Class
End Namespace