#Region "Microsoft.VisualBasic::ea06f360eac5f330e90c1b1930c35579, GCModeller\engine\Compiler\MarkupCompiler\CompileGeneModelWorkflow.vb"

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

    '   Total Lines: 105
    '    Code Lines: 90
    ' Comment Lines: 2
    '   Blank Lines: 13
    '     File Size: 4.58 KB


    '     Class CompileGeneModelWorkflow
    ' 
    '         Properties: locationAsLocus_tag
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: getGenes, getProtVector
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Vector

Namespace MarkupCompiler

    Friend Class CompileGeneModelWorkflow : Inherits CompilerWorkflow

        Public ReadOnly Property locationAsLocus_tag As Boolean
            Get
                'Return compiler.locationAsLocus_tag
            End Get
        End Property

        Sub New(compiler As v2MarkupCompiler)
            Call MyBase.New(compiler)
        End Sub

        Private Function getProtVector(genome As GBFF.File) As Dictionary(Of String, ProteinComposition)
            Return genome.Features _
                .Where(Function(feature)
                           Return feature.KeyName = "CDS"
                       End Function) _
                .Select(Function(feature)
                            Dim id As String

                            If locationAsLocus_tag Then
                                id = feature.Location.ToString
                            Else
                                id = feature.Query("locus_tag")
                            End If

                            Return ProteinComposition.FromRefSeq(feature.Query("translation"), id)
                        End Function) _
                .ToDictionary(Function(prot)
                                  Return prot.proteinID
                              End Function)
        End Function

        Public Iterator Function getGenes(genome As GBFF.File) As IEnumerable(Of gene)
            Dim model As CellularModule ' = compiler.model
            Dim proteinSequnce As Dictionary(Of String, ProteinComposition) = getProtVector(genome)
            Dim genes = genome.Features _
                .Where(Function(feature)
                           Return feature.KeyName = "gene"
                       End Function) _
                .ToDictionary(Function(g)
                                  If locationAsLocus_tag Then
                                      Return g.Location.ToString
                                  Else
                                      Return g.Query("locus_tag")
                                  End If
                              End Function)
            Dim aa As NumericVector
            Dim rna As NumericVector
            Dim locus_tag As String
            Dim proteinId = model.Genotype.centralDogmas _
                .Where(Function(proc) Not proc.IsRNAGene) _
                .ToDictionary(Function(gene)
                                  Return gene.geneID
                              End Function)
            Dim RNAIndex As Index(Of String) = model.Genotype.centralDogmas _
                .Where(Function(proc) proc.IsRNAGene) _
                .Select(Function(proc)
                            Return proc.geneID
                        End Function) _
                .ToArray

            Call compiler.CompileLogging.WriteLine("create gene object models")

            ' RNA基因是没有蛋白序列的
            For Each gene As GeneBrief In genome.GbffToPTT(ORF:=False).GeneObjects
                locus_tag = gene.Synonym

                If proteinSequnce.ContainsKey(gene.Synonym) Then
                    aa = proteinSequnce(gene.Synonym).CreateVector
                ElseIf locus_tag Like RNAIndex Then
                    aa = Nothing
                Else
                    Continue For
                End If

                rna = RNAComposition _
                    .FromNtSequence(genes(locus_tag).SequenceData, locus_tag) _
                    .CreateVector

                Yield New gene With {
                    .left = gene.Location.left,
                    .right = gene.Location.right,
                    .locus_tag = locus_tag,
                    .product = gene.Product,
                    .protein_id = If(aa Is Nothing, "", proteinId(locus_tag).polypeptide),
                    .strand = gene.Location.Strand.GetBriefCode,
                    .amino_acid = aa,
                    .nucleotide_base = rna
                }
            Next
        End Function
    End Class
End Namespace
