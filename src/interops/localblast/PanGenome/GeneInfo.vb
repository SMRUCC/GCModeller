#Region "Microsoft.VisualBasic::1594219b2dc66660b7946191cdc0f769, localblast\PanGenome\GeneInfo.vb"

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

    '   Total Lines: 78
    '    Code Lines: 60 (76.92%)
    ' Comment Lines: 7 (8.97%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 11 (14.10%)
    '     File Size: 2.72 KB


    ' Class GeneInfo
    ' 
    '     Properties: [End], Chromosome, GeneID, GenomeName, Length
    '                 Start
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: CastTable, CreateGeneModel, GenomeSet, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.ComponentModel.Annotation

''' <summary>
''' 基因详细信息，增加位置信息以支持共线性分析
''' </summary>
Public Class GeneInfo : Implements INamedValue

    Public Property GeneID As String Implements INamedValue.Key
    Public Property GenomeName As String
    Public Property Chromosome As String
    Public Property Start As Integer
    Public Property [End] As Integer

    ''' <summary>
    ''' 用于排序和距离计算
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Length As Integer
        Get
            Return [End] - Start + 1
        End Get
    End Property

    Sub New()
    End Sub

    Sub New(gene As GeneTable)
        GeneID = gene.locus_id
        GenomeName = gene.species
        Chromosome = gene.replicon_accessionID
        Start = gene.left
        [End] = gene.right
    End Sub

    Public Overrides Function ToString() As String
        Return GeneID
    End Function

    Public Shared Iterator Function CreateGeneModel(genome As GFFTable) As IEnumerable(Of GeneInfo)
        For Each gene As Feature In genome.features
            If gene.feature <> "gene" Then
                Continue For
            End If

            Yield New GeneInfo With {
                .Chromosome = gene.seqname,
                .GeneID = gene.ID,
                .GenomeName = If(genome.species, gene.source),
                .Start = gene.left,
                .[End] = gene.right
            }
        Next
    End Function

    Public Shared Function GenomeSet(genomes As IEnumerable(Of GFFTable)) As Dictionary(Of String, GeneInfo())
        Return genomes _
            .ToDictionary(Function(gn) gn.species,
                          Function(gn)
                              Return GeneInfo _
                                  .CreateGeneModel(gn) _
                                  .ToArray
                          End Function)
    End Function

    Public Shared Function CastTable(genomes As Dictionary(Of String, GeneTable())) As Dictionary(Of String, GeneInfo())
        Return genomes _
            .ToDictionary(Function(g) g.Key,
                          Function(g)
                              Return (From gene As GeneTable
                                      In g.Value
                                      Group By gene.locus_id Into Group
                                      Select New GeneInfo(Group.First)).ToArray
                          End Function)
    End Function

End Class
