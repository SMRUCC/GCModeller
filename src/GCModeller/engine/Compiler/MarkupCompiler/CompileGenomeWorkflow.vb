#Region "Microsoft.VisualBasic::f2a5f07b076a3771cb7a93f25f31d716, GCModeller\engine\Compiler\MarkupCompiler\CompileGenomeWorkflow.vb"

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

    '   Total Lines: 68
    '    Code Lines: 51
    ' Comment Lines: 8
    '   Blank Lines: 9
    '     File Size: 2.87 KB


    '     Class CompileGenomeWorkflow
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: getRNAs, populateReplicons
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process

Namespace MarkupCompiler

    Public Class CompileGenomeWorkflow : Inherits CompilerWorkflow

        Sub New(compiler As v2MarkupCompiler)
            Call MyBase.New(compiler)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="genomes">染色体基因组+质粒基因组</param>
        ''' <returns></returns>
        Friend Iterator Function populateReplicons(genomes As Dictionary(Of String, GBFF.File)) As IEnumerable(Of replicon)
            Dim genePopulator As New CompileGeneModelWorkflow(compiler)
            Dim replicon As replicon

            For Each genome In genomes
                replicon = New replicon With {
                    .genomeName = genome.Value.Locus.AccessionID,
                    .operons = genePopulator _
                        .getGenes(genome.Value) _
                        .Select(Function(gene)
                                    ' no transcript unit information
                                    Return New TranscriptUnit With {
                                        .id = gene.locus_tag,
                                        .genes = New XmlList(Of gene) With {
                                            .items = {gene}
                                        }
                                    }
                                End Function) _
                        .ToArray,
                    .RNAs = getRNAs(.genomeName).ToArray,
                    .isPlasmid = genome.Value.isPlasmid
                }

                Call compiler.CompileLogging.WriteLine("create replicon model: " & replicon.ToString)

                Yield replicon
            Next
        End Function

        Private Function getRNAs(repliconName$) As IEnumerable(Of RNA)
            Dim cdProcess As CentralDogma() '= compiler.model _
            '.Genotype _
            '.centralDogmas

            Return cdProcess _
                .Where(Function(proc)
                           Return proc.RNA.Value <> RNATypes.mRNA AndAlso repliconName = proc.replicon
                       End Function) _
                .Select(Function(proc)
                            Return New RNA With {
                                .type = proc.RNA.Value,
                                .val = proc.RNA.Description,
                                .gene = proc.geneID
                            }
                        End Function)
        End Function
    End Class
End Namespace
