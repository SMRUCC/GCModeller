#Region "Microsoft.VisualBasic::88f1b6ed50f183484c007c37d7835764, ..\interops\visualize\Circos\Circos\Karyotype\Adapters\PPTMarks.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Text
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.RpsBLAST
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.Visualize.Circos.Colors

Namespace Karyotype.GeneObjects

    ''' <summary>
    ''' 基因对象
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PTTMarks : Inherits SkeletonInfo

        Public Overrides ReadOnly Property Size As Integer

        Sub New(genome As PTTDbLoader, Optional MyvaCog As MyvaCOG() = Nothing, Optional defaultColor As String = "blue")
            If genome Is Nothing Then
                Throw New Exception("No data was found in the genome information!")
            End If

            If MyvaCog.IsNullOrEmpty Then      ' 绘制基本图型
                __bands = PTTMarks.Generate(genome, defaultColor:=defaultColor).ToList
            Else
                __bands = PTTMarks.Generate(genome, MyvaCog, defaultColor).ToList
            End If

            Call __karyotype()
        End Sub

        Sub New(genes As GeneDumpInfo(), nt As FastaToken, Optional defaultColor As String = "blue")
            Dim MyvaCog = LinqAPI.Exec(Of MyvaCOG) <=
                From gene As GeneDumpInfo
                In genes
                Select New MyvaCOG With {
                    .COG = gene.COG,
                    .QueryName = gene.LocusID,
                    .QueryLength = gene.Length
                }
            Dim genome = PTTDbLoader.CreateObject(genes, nt)
            __bands = PTTMarks.Generate(genome, MyvaCog, defaultColor).ToList
            Call __karyotype()
        End Sub

        Private Shared Iterator Function Generate(GenomeBrief As PTTDbLoader, MyvaCog As MyvaCOG(), Optional defaultColor As String = "blue") As IEnumerable(Of Band)
            Dim GetColorProfile As Func(Of String, String) = GetCogColorProfile(MyvaCog, defaultColor)
            Dim genome As PTT = GenomeBrief.ORF_PTT

            If Not genome Is Nothing AndAlso Not genome.GeneObjects.IsNullOrEmpty Then
                For Each gene As GeneBrief In genome
                    Yield New Band With {
                        .chrName = "chr1",
                        .bandX = gene.Gene,
                        .bandY = gene.Product.Replace(" ", "_"),
                        .start = gene.Location.Left,
                        .end = gene.Location.Right,
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
                        .start = gene.Location.Left,
                        .end = gene.Location.Right,
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
                    .start = gene.Location.Left,
                    .end = gene.Location.Right,
                    .color = defaultColor
                }
            Next
            For Each gene As GeneBrief In GenomeBrief.RNARnt
                Yield New Band With {
                    .chrName = "chr1",
                    .bandX = gene.Gene,
                    .bandY = gene.Product.Replace(" ", "_"),
                    .start = gene.Location.Left,
                    .end = gene.Location.Right,
                    .color = "blue"
                }
            Next
        End Function
    End Class
End Namespace
