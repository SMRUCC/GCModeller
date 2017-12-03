#Region "Microsoft.VisualBasic::8e28b135f7883e60138b2f09cf278c26, ..\GCModeller\analysis\ProteinTools\Interactions.BioGRID\Identifier.vb"

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

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class Identifier : Inherits BaseClass

    Public Property BIOGRID_ID As String
    Public Property ORGANISM_OFFICIAL_NAME As String

#Region ""

    Public Property ANIMALQTLDB As String
    Public Property APHIDBASE As String
    Public Property BEEBASE As String
    Public Property BGD As String
    Public Property BIOGRID As String
    Public Property CGD As String
    Public Property CGNC As String
    Public Property DICTYBASE As String
    Public Property ECOGENE As String
    Public Property ENSEMBL As String
    <Field("ENSEMBL GENE")> Public Property ENSEMBL_GENE As String
    <Field("ENSEMBL PROTEIN")> Public Property ENSEMBL_PROTEIN As String
    <Field("ENSEMBL RNA")> Public Property ENSEMBL_RNA As String
    Public Property ENTREZ_GENE As String
    Public Property ENTREZ_GENE_ETG As String
    Public Property FLYBASE As String
    <Field("GRID LEGACY")> Public Property GRID_LEGACY As String
    Public Property HGNC As String
    Public Property HPRD As String
    <Field("IMGT/GENE-DB")> Public Property IMGT_GENE_DB As String
    Public Property MAIZEGDB As String
    Public Property MGI As String
    Public Property MIM As String
    Public Property MIRBASE As String
    <Field("OFFICIAL SYMBOL")> Public Property OFFICIAL_SYMBOL As String
    <Field("ORDERED LOCUS")> Public Property ORDERED_LOCUS As String
    Public Property PBR As String
    <Field("REFSEQ-LEGACY")> Public Property REFSEQ_LEGACY As String
    <Field("REFSEQ-PROTEIN-ACCESSION")> Public Property REFSEQ_PROTEIN_ACCESSION As String
    <Field("REFSEQ-PROTEIN-ACCESSION-VERSIONED")> Public Property REFSEQ_PROTEIN_ACCESSION_VERSIONED As String
    <Field("REFSEQ-PROTEIN-GI")> Public Property REFSEQ_PROTEIN_GI As String
    <Field("REFSEQ-RNA-ACCESSION")> Public Property REFSEQ_RNA_ACCESSION As String
    <Field("REFSEQ-RNA-GI")> Public Property REFSEQ_RNA_GI As String
    Public Property RGD As String
    Public Property SGD As String
    <Field("SWISS-PROT")> Public Property SWISS_PROT As String
    Public Property SYNONYM As String
    <Field("SYSTEMATIC NAME")> Public Property SYSTEMATIC_NAME As String
    Public Property TAIR As String
    Public Property TREMBL As String
    Public Property TUBERCULIST As String
    <Field("UNIPROT-ACCESSION")> Public Property UNIPROT_ACCESSION As String
    <Field("UNIPROT-ISOFORM")> Public Property UNIPROT_ISOFORM As String
    Public Property VECTORBASE As String
    Public Property VEGA As String
    Public Property WORMBASE As String
    <Field("WORMBASE-OLD")> Public Property WORMBASE_OLD As String
    Public Property XENBASE As String
    Public Property ZFIN As String

#End Region

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
