﻿#Region "Microsoft.VisualBasic::bcb310f6929884f5b8fc80825068a20f, engine\IO\GCMarkupLanguage\v2\ModelExtensions.vb"

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

    '   Total Lines: 315
    '    Code Lines: 268 (85.08%)
    ' Comment Lines: 18 (5.71%)
    '    - Xml Docs: 33.33%
    ' 
    '   Blank Lines: 29 (9.21%)
    '     File Size: 14.19 KB


    '     Module ModelExtensions
    ' 
    '         Function: BuildEquation, createFluxes, createGenotype, CreateModel, createPhenotype
    '                   exportRegulations, loadKinetics
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Scripting
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression.Impl
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Molecule
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process
Imports FluxModel = SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process.Reaction

Namespace v2

    <HideModuleName>
    Public Module ModelExtensions

        ''' <summary>
        ''' Load model file as the unify data model for run the downstream simulation analysis.
        ''' (将所加载的XML模型文件转换为统一的数据模型)
        ''' </summary>
        ''' <param name="model"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CreateModel(model As VirtualCell) As CellularModule
            Dim hasGenotype As Boolean = (Not model.genome Is Nothing) AndAlso Not model.genome.replicons.IsNullOrEmpty
            Dim genotype As New Genotype With {
                .centralDogmas = model _
                    .createGenotype _
                    .OrderByDescending(Function(gene) gene.RNA.Value) _
                    .ToArray,
                .ProteinMatrix = {},
                .RNAMatrix = {}
            }

            If hasGenotype Then
                genotype.ProteinMatrix = model.genome.replicons _
                    .Select(Function(rep) rep.GetGeneList) _
                    .IteratesALL _
                    .Where(Function(gene) Not gene.amino_acid Is Nothing) _
                    .Select(Function(gene)
                                Return gene.amino_acid.DoCall(AddressOf ProteinFromVector)
                            End Function) _
                    .ToArray
                genotype.RNAMatrix = model.genome.replicons _
                    .Select(Function(rep) rep.GetGeneList) _
                    .IteratesALL _
                    .Select(Function(rna)
                                Return rna.nucleotide_base.DoCall(AddressOf RNAFromVector)
                            End Function) _
                    .ToArray
            End If

            Return New CellularModule With {
                .Taxonomy = model.taxonomy,
                .Genotype = genotype,
                .Phenotype = model.createPhenotype,
                .Regulations = model.exportRegulations.ToArray
            }
        End Function

        <Extension>
        Private Iterator Function createGenotype(model As VirtualCell) As IEnumerable(Of CentralDogma)
            Dim genomeName$
            Dim enzymes As Dictionary(Of String, Enzyme) = model.metabolismStructure.enzymes.ToDictionary(Function(enzyme) enzyme.geneID)
            Dim rnaTable As Dictionary(Of String, NamedValue(Of RNATypes))
            Dim RNA As NamedValue(Of RNATypes)
            Dim proteinId$

            ' just contains the metabolism network
            ' for run simulator
            If model.genome Is Nothing OrElse model.genome.replicons.IsNullOrEmpty Then
                Return
            End If

            For Each replicon As replicon In model.genome.replicons
                genomeName = replicon.genomeName
                replicon.RNAs = replicon.RNAs.OrderBy(Function(r) r.gene).ToArray
                rnaTable = replicon.RNAs _
                    .AsEnumerable _
                    .GroupBy(Function(r) r.gene) _
                    .Select(Function(r) r.First) _
                    .ToDictionary(Function(r) r.gene,
                                  Function(r)
                                      Return New NamedValue(Of RNATypes) With {
                                          .Name = r.gene,
                                          .Value = r.type,
                                          .Description = r.val
                                      }
                                  End Function)

                For Each gene As gene In replicon.GetGeneList
                    If rnaTable.ContainsKey(gene.locus_tag) Then
                        RNA = rnaTable(gene.locus_tag)
                        proteinId = Nothing
                    Else
                        ' 枚举的默认值为mRNA
                        RNA = New NamedValue(Of RNATypes) With {
                            .Name = gene.locus_tag
                        }
                        proteinId = gene.protein_id ' Or $"{gene.locus_tag}::peptide".AsDefault

                        If proteinId.StringEmpty Then
                            Dim warn = $"broken central dogma of '{gene.locus_tag}' was found. this gene should be a mRNA but missing polypeptide data."

                            Call warn.Warning
                            Call VBDebugger.EchoLine("[warn] " & warn)
                        End If
                    End If

                    Yield New CentralDogma With {
                        .replicon = genomeName,
                        .geneID = gene.locus_tag,
                        .polypeptide = proteinId,
                        .orthology = enzymes.TryGetValue(.geneID)?.KO,
                        .RNA = RNA,
                        .transcript = gene.nucleotide_base?.name,
                        .translation = gene.amino_acid?.name
                    }
                Next
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Private Function createPhenotype(model As VirtualCell) As Phenotype
            Dim hasGenotype As Boolean = (Not model.genome Is Nothing) AndAlso
                Not model.genome.replicons.IsNullOrEmpty
            Dim fluxChannels = model.createFluxes _
                .OrderByDescending(Function(r) r.enzyme.TryCount) _
                .ToArray
            Dim enzymes = model.metabolismStructure.enzymes _
                .Select(Function(enz) enz.geneID) _
                .ToArray
            Dim proteins As Protein() = {}

            If hasGenotype Then
                proteins = model.genome.replicons _
                    .Select(Function(genome)
                                Return genome.GetGeneList
                            End Function) _
                    .IteratesALL _
                    .Where(Function(gene) Not gene.amino_acid Is Nothing) _
                    .Select(Function(orf)
                                Return New Protein With {
                                    .compounds = {},
                                    .polypeptides = {orf.protein_id},
                                    .ProteinID = orf.protein_id
                                }
                            End Function) _
                    .ToArray
            End If

            Return New Phenotype With {
                .fluxes = fluxChannels,
                .enzymes = enzymes,
                .proteins = proteins
            }
        End Function

        <Extension>
        Private Function BuildEquation(reaction As Reaction) As Equation
            Return New Equation With {
                .Id = reaction.ID,
                .reversible = True,
                .Reactants = reaction.substrate _
                    .Select(Function(c) New CompoundSpecieReference(c.factor, c.compound)) _
                    .ToArray,
                .Products = reaction.product _
                    .Select(Function(c) New CompoundSpecieReference(c.factor, c.compound)) _
                    .ToArray
            }
        End Function

        <Extension>
        Private Iterator Function loadKinetics(reaction As Reaction, ko As IEnumerable(Of NamedValue(Of Catalysis))) As IEnumerable(Of Kinetics)
            Dim expr As Expression
            Dim refVals As Object()
            Dim pars As String()

            For Each k As NamedValue(Of Catalysis) In ko
                If k.Value.reaction <> reaction.ID Then
                    Continue For
                Else
                    If k.Value.formula Is Nothing Then
                        ' apply of the default kinetics
                        ' Michaelis-Menten equation
                        ' V= (Vmax⋅[S]) / (km [S])
                        ' Vmax = kcat [E]
                        expr = ScriptEngine.ParseExpression("(2 * E *S)/(2+S)")
                        refVals = New Object() {k.Name & ".complex", reaction.substrate.First.compound}
                        pars = {"E", "S"}
                    Else
                        expr = ScriptEngine.ParseExpression(k.Value.formula.lambda)
                        refVals = k.Value.parameter _
                            .Select(Function(a) As Object
                                        Dim useReferenceId As String = (a.value = 0.0 OrElse a.value.IsNaNImaginary) AndAlso
                                            Not a.target.StringEmpty

                                        If useReferenceId Then
                                            Return a.target
                                        Else
                                            Return a.value
                                        End If
                                    End Function) _
                            .ToArray
                        pars = k.Value.formula.parameters
                    End If
                End If

                Yield New Kinetics With {
                    .enzyme = k.Name,
                    .formula = expr,
                    .parameters = pars,
                    .paramVals = refVals,
                    .target = reaction.ID,
                    .PH = k.Value.PH,
                    .temperature = k.Value.temperature
                }
            Next
        End Function

        <Extension>
        Private Iterator Function createFluxes(model As VirtualCell) As IEnumerable(Of FluxModel)
            Dim equation As Equation
            ' {reactionID => KO()}
            Dim enzymes = model.metabolismStructure _
                .enzymes _
                .Select(Function(enz)
                            Return enz.catalysis _
                                .SafeQuery _
                                .Select(Function(ec)
                                            Return (rID:=ec.reaction, enz:=New NamedValue(Of Catalysis)(If(enz.KO, enz.geneID), ec))
                                        End Function)
                        End Function) _
                .IteratesALL _
                .GroupBy(Function(r) r.rID) _
                .ToDictionary(Function(r) r.Key,
                              Function(g)
                                  Return g.Select(Function(r) r.enz) _
                                          .Distinct _
                                          .ToArray
                              End Function)
            Dim KO As NamedValue(Of Catalysis)()
            Dim bounds As Double()
            Dim kinetics As Kinetics()

            For Each reaction As Reaction In model.metabolismStructure.reactions.AsEnumerable
                equation = reaction.BuildEquation

                If reaction.is_enzymatic Then
                    KO = enzymes.TryGetValue(reaction.ID, [default]:={}, mute:=True)

                    If KO.IsNullOrEmpty Then
                        ' 当前的基因组内没有对应的酶来催化这个反应过程
                        ' 则限制一个很小的range
                        bounds = If(reaction.bounds, {10, 10.0})
                        ' 标准的米氏方程？
                        kinetics = {}
                    Else
                        bounds = If(reaction.bounds, {500, 1000.0})
                        kinetics = reaction.loadKinetics(KO).ToArray
                    End If
                Else
                    KO = {}
                    bounds = If(reaction.bounds, {200, 200.0})
                    kinetics = {}
                End If

                If Not equation.reversible Then
                    ' only forward flux direction
                    bounds(Scan0) = 0
                End If

                Yield New FluxModel With {
                    .ID = reaction.ID,
                    .name = reaction.name,
                    .substrates = equation.Reactants _
                        .Select(Function(c) c.AsFactor) _
                        .ToArray,
                    .products = equation.Products _
                        .Select(Function(c) c.AsFactor) _
                        .ToArray,
                    .enzyme = KO.Keys.Distinct.ToArray,
                    .bounds = bounds,
                    .kinetics = kinetics
                }
            Next
        End Function

        <Extension>
        Private Iterator Function exportRegulations(model As VirtualCell) As IEnumerable(Of Regulation)
            Dim hasGenotype As Boolean = (Not model.genome Is Nothing) AndAlso
                Not model.genome.replicons.IsNullOrEmpty

            If Not hasGenotype Then
                Return
            End If
            If model.genome.regulations.IsNullOrEmpty Then
                Return
            End If

            For Each reg As transcription In model.genome.regulations
                Yield New Regulation With {
                    .effects = reg.mode.EvalEffects,
                    .name = reg.biological_process,
                    .process = reg.centralDogma,
                    .regulator = reg.regulator,
                    .type = Processes.Transcription
                }
            Next
        End Function
    End Module
End Namespace
