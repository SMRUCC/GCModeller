#Region "Microsoft.VisualBasic::d6450ea0ee552e8f4cf1194539c83a41, Compiler\Compiler.vb"

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

' Module Workflow
' 
'     Function: AssemblingModel, BuildReactions, converts, GetCentralDogmas
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model
Imports SMRUCC.genomics.Metagenomics

Public Module Workflow

    ''' <summary>
    ''' 输出Model，然后再从Model写出模型文件
    ''' </summary>
    ''' <param name="genome"></param>
    ''' <param name="KOfunction"></param>
    ''' <param name="repo"></param>
    ''' <returns></returns>
    <Extension>
    Public Function AssemblingModel(genome As GBFF.File, KOfunction As Dictionary(Of String, String), repo As RepositoryArguments) As CellularModule
        Dim taxonomy As Taxonomy = genome.Source.GetTaxonomy
        Dim genotype As New Genotype With {
            .CentralDogmas = genome _
                .GetCentralDogmas(KOfunction) _
                .ToArray
        }
        Dim phenotype As New Phenotype With {
            .fluxes = repo _
                .GetReactions _
                .BuildReactions _
                .ToArray
        }

        Return New CellularModule With {
            .Taxonomy = taxonomy,
            .Genotype = genotype,
            .Phenotype = phenotype,
            .Regulations = KOfunction _
                .createMetabolicProcess(repo.GetReactions) _
                .ToArray
        }
    End Function

    <Extension>
    Private Iterator Function createMetabolicProcess(KOfunction As Dictionary(Of String, String), reactions As ReactionRepository) As IEnumerable(Of Regulation)
        Dim KOreactions = reactions _
            .MetabolicNetwork _
            .Where(Function(r)
                       Return Not r.Orthology.Terms.IsNullOrEmpty
                   End Function) _
            .Select(Function(r)
                        Return r.Orthology _
                                .Terms _
                                .Select(Function(KO)
                                            Return (name:=KO.name, KOterm:=KO, flux:=r)
                                        End Function)
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(KO) KO.name) _
            .ToDictionary(Function(KO) KO.Key,
                          Function(g)
                              Return g.ToArray
                          End Function)

        For Each enzyme In KOfunction
            Dim catalysis = KOreactions.TryGetValue(enzyme.Value)

            For Each flux In catalysis.SafeQuery
                Dim fluxName$ = flux.flux.CommonNames.ElementAtOrDefault(0) Or flux.flux.Definition.AsDefault

                Yield New Regulation With {
                    .effects = 2,
                    .regulator = enzyme.Key,
                    .type = Processes.MetabolicProcess,
                    .name = fluxName,
                    .process = flux.flux.ID
                }
            Next
        Next
    End Function

    <Extension>
    Private Iterator Function BuildReactions(repo As ReactionRepository) As IEnumerable(Of Reaction)
        For Each reaction In repo.MetabolicNetwork
            Dim model As Equation = reaction.ReactionModel
            Dim enzymes$() = {}

            If Not reaction.Orthology.Terms Is Nothing Then
                enzymes = reaction _
                    .Orthology _
                    .Terms _
                    .Select(Function(t) t.name) _
                    .ToArray
            End If

            Yield New Reaction With {
                .enzyme = enzymes,
                .ID = reaction.ID,
                .substrates = model.Reactants.converts,
                .products = model.Products.converts,
                .name = reaction _
                    .CommonNames _
                    .ElementAtOrDefault(0, reaction.Definition)
            }
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Private Function converts(compounds As CompoundSpecieReference()) As FactorString(Of Double)()
        Return compounds _
            .Select(Function(r)
                        Return New FactorString(Of Double) With {
                            .Factor = r.StoiChiometry,
                            .text = r.ID
                        }
                    End Function) _
            .ToArray
    End Function

    ReadOnly centralDogmaComponents As Index(Of String) = {"gene", "CDS", "tRNA", "rRNA"}

    <Extension>
    Private Iterator Function GetCentralDogmas(genome As GBFF.File, KOfunction As Dictionary(Of String, String)) As IEnumerable(Of CentralDogma)
        Dim centralDogmaFeatures = genome _
            .Features _
            .Where(Function(feature)
                       Return feature _
                           .KeyName _
                           .IsOneOfA(centralDogmaComponents)
                   End Function) _
            .GroupBy(Function(feature)
                         Return feature.Query("locus_tag")
                     End Function)

        For Each feature As IGrouping(Of String, Feature) In centralDogmaFeatures
            Dim gene As Feature = feature.First(Function(component) component.KeyName = "gene")
            Dim RNA As Feature = feature _
                .FirstOrDefault(Function(component)
                                    Return component.KeyName = "tRNA" OrElse component.KeyName = "rRNA"
                                End Function)
            Dim CDS As Feature = feature _
                .FirstOrDefault(Function(component)
                                    Return component.KeyName = "CDS"
                                End Function)
            Dim locus_tag$ = feature.Key
            Dim rnaType As RNATypes = RNATypes.mRNA
            Dim proteinId As String = Nothing

            If Not RNA Is Nothing Then
                If RNA.KeyName = "tRNA" Then
                    rnaType = RNATypes.tRNA
                Else
                    rnaType = RNATypes.ribosomalRNA
                End If
            ElseIf Not CDS Is Nothing Then
                proteinId = CDS.Query("protein_id")

                If proteinId.StringEmpty Then
                    proteinId = CDS.Query("db_xref")
                End If
                If proteinId.StringEmpty Then
                    proteinId = $"{locus_tag}::peptide"
                End If
            Else
                ' 既没有RNA也没有CDS，这个可能是其他的类型的feature
                ' 例如移动原件之类的
                ' 跳过这些
                Call $"Skip invalid locus_tag: {feature.Key}".Warning

                Continue For
            End If

            Yield New CentralDogma With {
                .geneID = locus_tag,
                .RNA = New NamedValue(Of RNATypes) With {
                    .Name = locus_tag,
                    .Value = rnaType
                },
                .polypeptide = proteinId,
                .orthology = KOfunction.TryGetValue(.geneID)
            }
        Next
    End Function
End Module

