#Region "Microsoft.VisualBasic::22682de3fda6d72dab5f2bf0f7ffb5f6, visualize\Circos\Circos\Karyotype\Adapters\PPTMarks.vb"

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

    '     Class PTTMarks
    ' 
    '         Properties: Size
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: (+2 Overloads) Generate
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.NCBI.COG.COGs
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.Visualize.Circos.Colors

Namespace Karyotype.GeneObjects

    ''' <summary>
    ''' 基因对象
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PTTMarks : Inherits SkeletonInfo

        Public Overrides ReadOnly Property Size As Integer

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="genome"></param>
        ''' <param name="MyvaCog">基因的功能分类注释数据</param>
        ''' <param name="defaultColor"></param>
        Sub New(genome As PTTDbLoader, Optional MyvaCog As ICOGCatalog() = Nothing, Optional defaultColor As String = "blue")
            If genome Is Nothing Then
                Throw New Exception("No data was found in the genome information!")
            End If

            If MyvaCog.IsNullOrEmpty Then      ' 绘制基本图型
                bands = PTTMarks.Generate(genome, defaultColor:=defaultColor).AsList
            Else
                bands = PTTMarks.Generate(genome, MyvaCog, defaultColor).AsList
            End If

            Call singleKaryotypeChromosome()
        End Sub

        Sub New(genes As GeneTable(), nt As FastaSeq, Optional defaultColor As String = "blue")
            Dim MyvaCog = LinqAPI.Exec(Of ICOGCatalog) <=
                From gene As GeneTable
                In genes
                Select New COGTable With {
                    .COGId = gene.COG,
                    .ProteinID = gene.locus_id,
                    .ProteinLength = gene.Length
                }
            Dim genome = PTTDbLoader.CreateObject(genes, nt)
            bands = PTTMarks.Generate(genome, MyvaCog, defaultColor).AsList
            Call singleKaryotypeChromosome()
        End Sub

        Private Shared Iterator Function Generate(GenomeBrief As PTTDbLoader, MyvaCog As ICOGCatalog(), Optional defaultColor As String = "blue") As IEnumerable(Of Band)
            Dim GetColorProfile As Func(Of String, String) = COGColors.GetCogColorProfile(MyvaCog, defaultColor)
            Dim genome As PTT = GenomeBrief.ORF_PTT

            If Not genome Is Nothing AndAlso Not genome.GeneObjects.IsNullOrEmpty Then
                For Each gene As GeneBrief In genome
                    Yield New Band With {
                        .chrName = "chr1",
                        .bandX = gene.Gene,
                        .bandY = gene.Product.Replace(" ", "_"),
                        .start = gene.Location.left,
                        .end = gene.Location.right,
                        .color = GetColorProfile(gene.Gene)
                    }
                Next
            End If

            genome = GenomeBrief.RNARnt

            If Not genome Is Nothing AndAlso Not genome.GeneObjects.IsNullOrEmpty Then
                For Each gene As GeneBrief In genome
                    Yield New Band With {
                        .chrName = "chr1",
                        .bandX = gene.Gene,
                        .bandY = gene.Product.Replace(" ", "_"),
                        .start = gene.Location.left,
                        .end = gene.Location.right,
                        .color = "blue"
                    }
                Next
            End If
        End Function

        Private Shared Iterator Function Generate(GenomeBrief As PTTDbLoader, Optional defaultColor As String = "blue") As IEnumerable(Of Band)
            For Each gene As GeneBrief In GenomeBrief.ORF_PTT
                Yield New Band With {
                    .chrName = "chr1",
                    .bandX = gene.Gene,
                    .bandY = gene.Product.Replace(" ", "_"),
                    .start = gene.Location.left,
                    .end = gene.Location.right,
                    .color = defaultColor
                }
            Next
            For Each gene As GeneBrief In GenomeBrief.RNARnt
                Yield New Band With {
                    .chrName = "chr1",
                    .bandX = gene.Gene,
                    .bandY = gene.Product.Replace(" ", "_"),
                    .start = gene.Location.left,
                    .end = gene.Location.right,
                    .color = "blue"
                }
            Next
        End Function
    End Class
End Namespace
