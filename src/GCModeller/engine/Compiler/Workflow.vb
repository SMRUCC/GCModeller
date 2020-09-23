#Region "Microsoft.VisualBasic::145d7907d156e2d9884a0ad6726f0d27, engine\Compiler\Workflow.vb"

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
    '     Function: AssemblingGenomeInformation, AssemblingMetabolicNetwork, AssemblingRegulationNetwork, BuildReactions, converts
    '               createMetabolicProcess, GetCentralDogmas, getTaxonomy, glycan2Cpd
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
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model
Imports SMRUCC.genomics.Metagenomics

''' <summary>
''' 编译器的作用就是进行数据模型和其他的基础模块之间的解耦和作用
''' 通过这个编译器模块，可以将模型文件模块与其他的模块之间的依赖程度减少
''' 这样子在模拟计算分析引擎模块之中引用模型文件模块就可以不需要引用额外的其他
''' 的模块代码的，方便进行数据计算模拟引擎的调试工作
''' </summary>
Public Module Workflow

    <Extension>
    Public Function AssemblingRegulationNetwork(model As CellularModule, regulations As RegulationFootprint()) As CellularModule
        Dim genes = model.Genotype.centralDogmas.ToDictionary

        model.Regulations = model.Regulations _
            .AsList + regulations _
            .Select(Function(reg)
                        ' 调控的过程为中心法则的转录过程
                        Return New Regulation With {
                            .effects = reg.mode.EvalEffects,
                            .regulator = reg.regulator,
                            .type = Processes.Transcription,
                            .name = reg.biological_process,
                            .process = genes(reg.regulated).ToString
                        }
                    End Function)

        Return model
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Friend Function getTaxonomy(replicons As Dictionary(Of String, GBFF.File)) As Taxonomy
        Return replicons.Values _
           .First(Function(gb) Not gb.isPlasmid) _
           .Source _
           .GetTaxonomy
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="replicons"></param>
    ''' <param name="locationAsLocustag"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function AssemblingGenomeInformation(replicons As Dictionary(Of String, GBFF.File), KOfunction As Dictionary(Of String, String), locationAsLocustag As Boolean) As CellularModule
        Dim taxonomy As Taxonomy = replicons.getTaxonomy
        Dim genotype As New Genotype With {
            .centralDogmas = replicons _
                .GetCentralDogmas(KOfunction, locationAsLocustag) _
                .ToArray
        }

        Return New CellularModule With {
            .Taxonomy = taxonomy,
            .Genotype = genotype
        }
    End Function

    ''' <summary>
    ''' 输出Model，然后再从Model写出模型文件
    ''' </summary>
    ''' <param name="KOfunction">``[geneID => KO]`` maps</param>
    ''' <param name="repo"></param>
    ''' <returns></returns>
    <Extension>
    Public Function AssemblingMetabolicNetwork(cell As CellularModule, KOfunction As Dictionary(Of String, String), repo As RepositoryArguments) As CellularModule
        Dim phenotype As New Phenotype With {
            .fluxes = repo _
                .GetReactions _
                .BuildReactions（repo.Glycan2Cpd） _
                .ToArray
        }

        cell.Phenotype = phenotype
        cell.Regulations = KOfunction _
            .createMetabolicProcess(repo.GetReactions) _
            .ToArray

        Return cell
    End Function

    <Extension>
    Friend Iterator Function createMetabolicProcess(KOfunction As Dictionary(Of String, String), reactions As ReactionRepository) As IEnumerable(Of Regulation)
        Dim KOreactions = reactions _
            .metabolicNetwork _
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
                Dim fluxName$ = flux _
                    .flux.CommonNames.ElementAtOrDefault(0) Or flux.flux.Definition.AsDefault

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
    Private Iterator Function BuildReactions(repo As ReactionRepository, glycan2Cpd As Dictionary(Of String, String)) As IEnumerable(Of Reaction)
        For Each reaction In repo.metabolicNetwork
            Dim model As Equation = reaction.ReactionModel
            Dim enzymes$() = {}

            If Not reaction.Orthology.Terms Is Nothing Then
                enzymes = reaction _
                    .Orthology _
                    .Terms _
                    .Select(Function(t) t.name) _
                    .ToArray
            End If

            ' 如果enzyme属性是空的，说明不是酶促反应过程
            Yield New Reaction With {
                .enzyme = enzymes,
                .ID = reaction.ID,
                .substrates = model.Reactants.converts.glycan2Cpd(glycan2Cpd),
                .products = model.Products.converts.glycan2Cpd(glycan2Cpd),
                .name = reaction _
                    .CommonNames _
                    .ElementAtOrDefault(0, reaction.Definition)
            }
        Next
    End Function

    <Extension>
    Private Function glycan2Cpd(factors As IEnumerable(Of FactorString(Of Double)), glycan2CpdMaps As Dictionary(Of String, String)) As FactorString(Of Double)()
        Return factors _
            .Select(Function(factor)
                        If glycan2CpdMaps.ContainsKey(factor.text) Then
                            Return New FactorString(Of Double) With {
                                .factor = factor.factor,
                                .text = glycan2CpdMaps(factor.text)
                            }
                        Else
                            Return factor
                        End If
                    End Function) _
            .ToArray
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Private Function converts(compounds As CompoundSpecieReference()) As IEnumerable(Of FactorString(Of Double))
        Return compounds.Select(Function(r) r.AsFactor)
    End Function

    ReadOnly centralDogmaComponents As Index(Of String) = {"gene", "CDS", "tRNA", "rRNA", "RNA"}

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="genomes"></param>
    ''' <param name="KOfunction"></param>
    ''' <param name="locationAsLocustag">
    ''' 对于刚注释完的基因组，由于还没有提交至NCBI服务器，所以一般都没有基因编号
    ''' 则使用GCModeller进行KEGG代谢途径注释的时候，一般是使用location来唯一标记基因的
    ''' 则可以设置这个参数为true，使用localtion作为基因的locus_tag
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Friend Iterator Function GetCentralDogmas(genomes As Dictionary(Of String, GBFF.File), KOfunction As Dictionary(Of String, String), locationAsLocustag As Boolean) As IEnumerable(Of CentralDogma)
        Dim centralDogmaFeatures = genomes.Values _
            .Select(Function(genome)
                        Dim repliconId$ = genome.Locus.AccessionID

                        Return genome.Features _
                            .Where(Function(feature)
                                       Return feature.KeyName Like centralDogmaComponents
                                   End Function) _
                            .GroupBy(Function(feature)
                                         If locationAsLocustag Then
                                             Return feature.Location.ToString
                                         Else
                                             Return feature.Query("locus_tag")
                                         End If
                                     End Function) _
                            .Select(Function(feature)
                                        Return New NamedCollection(Of Feature)(feature.Key, feature.ToArray, repliconId)
                                    End Function)
                    End Function) _
            .IteratesALL

        For Each feature As NamedCollection(Of Feature) In centralDogmaFeatures
            Dim gene As Feature = feature.FirstOrDefault(Function(component) component.KeyName = "gene")

            If gene Is Nothing Then
                gene = feature.FirstOrDefault(Function(component) component.KeyName = "CDS")
            End If

            Dim RNA As Feature = feature _
                .FirstOrDefault(Function(component)
                                    Return InStr(component.KeyName, "RNA", CompareMethod.Text) > 0
                                End Function)
            Dim CDS As Feature = feature _
                .FirstOrDefault(Function(component)
                                    Return component.KeyName = "CDS"
                                End Function)

            ' 在注释不规范的原始数据文件中
            ' 是可能不存在gene feature的
            ' 在这里使用RNA feature来进行替代
            If gene Is Nothing Then
                gene = RNA
            End If

            Dim locus_tag$ = feature.name Or gene.Location.ToString.When(locationAsLocustag)
            Dim rnaType As RNATypes = RNATypes.mRNA
            Dim rnaData As String = ""
            Dim proteinId As String = Nothing

            If Not RNA Is Nothing Then
                If RNA.KeyName = "tRNA" Then
                    rnaType = RNATypes.tRNA
                    rnaData = RNA.Query("anticodon")

                    If rnaData.StringEmpty Then
                        ' 有些genbank注释里面没有/anticodon=数据
                        ' 则只能从/product里面拿出对应的氨基酸信息了
                        rnaData = RNA.Query("product").GetTagValue("-").Value
                    Else
                        rnaData = tRNAAnticodon.Parse(rnaData).aa
                    End If
                ElseIf RNA.KeyName = "rRNA" Then
                    rnaType = RNATypes.ribosomalRNA
                    rnaData = RNA.Query("product").Trim.Split().First
                Else
                    rnaType = RNATypes.micsRNA
                    rnaData = RNA.Query("product").Trim
                End If
            ElseIf Not CDS Is Nothing Then
                proteinId = CDS.Query("protein_id")

                If proteinId.StringEmpty Then
                    proteinId = CDS.Query("db_xref")
                End If
                If proteinId.StringEmpty Then
                    proteinId = $"{locus_tag}::peptide"
                End If

                ' 蛋白功能描述
                rnaData = CDS.Query("product").Trim
            Else
                ' 既没有RNA也没有CDS，这个可能是其他的类型的feature
                ' 例如移动原件之类的
                ' 跳过这些
                Call $"Skip invalid locus_tag: {feature.name}".Warning

                Continue For
            End If

            Yield New CentralDogma With {
                .geneID = locus_tag,
                .RNA = New NamedValue(Of RNATypes) With {
                    .Name = locus_tag,
                    .Value = rnaType,
                    .Description = rnaData
                },
                .polypeptide = proteinId,
                .orthology = KOfunction.TryGetValue(.geneID),
                .replicon = feature.description
            }
        Next
    End Function
End Module
