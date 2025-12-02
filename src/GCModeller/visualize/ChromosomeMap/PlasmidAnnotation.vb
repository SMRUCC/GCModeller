#Region "Microsoft.VisualBasic::0b8035192b557909dd0c5f7196280a91, visualize\ChromosomeMap\PlasmidAnnotation.vb"

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

    '   Total Lines: 138
    '    Code Lines: 95 (68.84%)
    ' Comment Lines: 35 (25.36%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 8 (5.80%)
    '     File Size: 5.81 KB


    ' Class PlasmidAnnotation
    ' 
    '     Properties: COG_annotation, COG_cat, COG_NO, discription, Evalue
    '                 Family, Gene_name, GeneNA, GO_discription, GO_ID
    '                 GO_name, GOI, Identity, Length, Location
    '                 ORF_ID, product, Protein, Protein_len, qsp
    '                 qst, SP, ssp, sst, ST
    '                 Strand, subject_length
    ' 
    '     Function: ExportAnnotations, ExportProteinFasta, READ_PlasmidData
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.Framework.Extensions
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

<[Namespace]("PlasmidAnnotations")>
Public Class PlasmidAnnotation : Implements IGeneBrief

    ''' <summary>
    ''' 基因号
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ORF_ID As String Implements INamedValue.Key
    Public Property Strand As String
    <Column("Gene-Length")> Public Property Length As Integer Implements IGeneBrief.Length
    ''' <summary>
    ''' 基因核酸序列
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Column("Gene")> Public Property GeneNA As String
    ''' <summary>
    ''' 蛋白质序列
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Protein As String
    Public Property ST As Integer
    Public Property SP As Integer
    ''' <summary>
    ''' 简写的基因名
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Gene_name As String
    ''' <summary>
    ''' 蛋白质功能注释
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property product As String Implements IGeneBrief.Product
    <Column("GO-I")> Public Property GOI As String
    <Column("GO-ID")> Public Property GO_ID As String
    Public Property GO_name As String
    Public Property GO_discription As String
    <Column("Protein family membership")> Public Property Family As String
    <Column("Protein family membership discription")> Public Property discription As String
    <Column("COG-NO.")> Public Property COG_NO As String Implements IFeatureDigest.Feature
    <Column("COG-cat")> Public Property COG_cat As String
    <Column("COG-annotation")> Public Property COG_annotation As String
    Public Property Identity As String
    <Column("qst.")> Public Property qst As String
    <Column("qsp.")> Public Property qsp As String
    <Column("sst.")> Public Property sst As String
    <Column("ssp.")> Public Property ssp As String
    Public Property subject_length As String
    <Column("E-value")> Public Property Evalue As String
    Public Property Protein_len As String

    Public Property Location As NucleotideLocation Implements IGeneBrief.Location
        Get
            Return New NucleotideLocation(ST, SP, Strand)
        End Get
        Set(value As NucleotideLocation)
            If Not value Is Nothing Then
                ST = value.left
                SP = value.right
            End If
        End Set
    End Property

    Shared ReadOnly fullAnnotation As New [Default](Of Func(Of PlasmidAnnotation, String()))(Function(gene As PlasmidAnnotation) New String() {gene.ORF_ID, gene.Gene_name, gene.product})
    Shared ReadOnly ORFidAnnotation As New Func(Of PlasmidAnnotation, String())(Function(gene As PlasmidAnnotation) {gene.ORF_ID})

    ''' <summary>
    ''' 导出蛋白质的序列信息
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("Export.Orf")>
    Public Shared Function ExportProteinFasta(data As IEnumerable(Of PlasmidAnnotation), Optional justId As Boolean = False) As FastaFile
        Dim getTitle As Func(Of PlasmidAnnotation, String()) = ORFidAnnotation Or fullAnnotation.When(Not justId)
        Dim LQuery = LinqAPI.Exec(Of FastaSeq) _
 _
            () <= From PlasmidGene As PlasmidAnnotation
                  In data
                  Where Not String.IsNullOrEmpty(PlasmidGene.Protein)
                  Select New FastaSeq With {
                      .Headers = getTitle(PlasmidGene),
                      .SequenceData = PlasmidGene.Protein
                  }

        Return New FastaFile(LQuery)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("read.csv.plasmid_data")>
    Public Shared Function READ_PlasmidData(path As String) As PlasmidAnnotation()
        Return path.LoadCsv(Of PlasmidAnnotation)(False).ToArray
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("export.as_ncbi_annotation")>
    Public Shared Function ExportAnnotations(data As IEnumerable(Of PlasmidAnnotation)) As GeneTable()
        Return LinqAPI.Exec(Of GeneTable) _
 _
            () <= From item As PlasmidAnnotation
                  In data
                  Where Not String.IsNullOrEmpty(item.Protein)
                  Let gc As Double = GC_Content(New NucleicAcid(item.GeneNA).ToArray)
                  Select New GeneTable With {
                      .CDS = item.GeneNA,
                      .GC_Content = gc,
                      .COG = item.COG_NO,
                      .Strand = item.Strand,
                      .commonName = item.product,
                      .geneName = item.Gene_name,
                      .locus_id = item.ORF_ID,
                      .translation = item.Protein,
                      .left = item.Location.left,
                      .right = item.Location.right
                  }
    End Function
End Class
