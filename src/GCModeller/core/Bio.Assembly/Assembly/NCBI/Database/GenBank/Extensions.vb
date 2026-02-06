#Region "Microsoft.VisualBasic::e117727487179ec4ed6a25ab597ec9e7, core\Bio.Assembly\Assembly\NCBI\Database\GenBank\Extensions.vb"

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

'   Total Lines: 278
'    Code Lines: 215 (77.34%)
' Comment Lines: 37 (13.31%)
'    - Xml Docs: 91.89%
' 
'   Blank Lines: 26 (9.35%)
'     File Size: 12.01 KB


'     Module Extensions
' 
'         Function: __lociUid, __protShort, _16SribosomalRNA, CreateGenbankObject, ExportProteins
'                   ExportProteins_Short, GeneList, GetObjects, LoadPTT, loadRepliconTable
'                   (+2 Overloads) LocusMaps
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.ELIXIR.EBI.ChEBI.Database.IO.StreamProviders.Tsv.Tables
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports featureLocation = SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.Location
Imports gbffFeature = SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.Feature

Namespace Assembly.NCBI.GenBank

    <Package("NCBI.Genbank.Extensions", Publisher:="amethyst.asuka@gcmodeller.org")>
    Public Module Extensions

        <Extension>
        Public Function loadRepliconTable(genome As String) As Dictionary(Of String, GBFF.File)
            Return GBFF.File _
               .LoadDatabase(filePath:=genome) _
               .ToDictionary(Function(gb)
                                 Return gb.Locus.AccessionID
                             End Function)
        End Function

        <Extension>
        Public Function CreateGenbankObject(table As PTT) As GBFF.File
            Dim feature, CDS As gbffFeature
            Dim gb As New GBFF.File With {
                .Features = New FEATURES,
                .Source = New SOURCE With {
                    .SpeciesName = table.Title,
                    .OrganismHierarchy = New ORGANISM With {
                        .SpeciesName = table.Title,
                        .Lineage = {"n/a"}
                    }
                },
                .Locus = New LOCUS With {
                    .AccessionID = "n/a",
                    .Length = table.Size,
                    .Molecular = "unknown",
                    .Type = "nucl",
                    .UpdateTime = Now.ToString
                },
                .Definition = New DEFINITION With {.Value = table.Title}
            }
            Dim loci As NucleotideLocation

            For Each gene As GeneBrief In table.GeneObjects
                loci = gene.Location
                feature = New gbffFeature With {
                    .gb = gb,
                    .KeyName = "gene",
                    .Location = New featureLocation With {
                        .Complement = loci.Strand = Strands.Reverse,
                        .Locations = {
                            New RegionSegment With {.Left = loci.left, .Right = loci.right}
                        }
                    }
                }
                CDS = New gbffFeature With {
                    .gb = gb,
                    .KeyName = "CDS",
                    .Location = New featureLocation With {
                        .Complement = loci.Strand = Strands.Reverse,
                        .Locations = {
                            New RegionSegment With {.Left = loci.left, .Right = loci.right}
                        }
                    }
                }

                Call feature.SetValue(FeatureQualifiers.gene, gene.Gene)
                Call feature.SetValue(FeatureQualifiers.locus_tag, gene.Synonym)

                Call CDS.SetValue(FeatureQualifiers.gene, gene.Gene)
                Call CDS.SetValue(FeatureQualifiers.locus_tag, gene.Synonym)
                Call CDS.SetValue(FeatureQualifiers.product, gene.Product)

                Call gb.Features.Add(feature)
                Call gb.Features.Add(CDS)
            Next

            Return gb
        End Function

        <ExportAPI("Locus.Maps"), Extension>
        Public Function LocusMaps(gb As GBFF.File) As Dictionary(Of String, String)
            Dim LQuery = (From x As gbffFeature
                          In gb.Features._innerList
                          Let locus As String = x.Query(FeatureQualifiers.locus_tag)
                          Where Not String.IsNullOrEmpty(locus)
                          Select locus,
                              loci = x.Location.ToString
                          Group By loci Into Group).ToArray
            Dim hash = LQuery.ToDictionary(Function(x) x.loci, Function(x) x.Group.First.locus)
            Return hash
        End Function

        ''' <summary>
        ''' ``[location => locus_tag]``
        ''' </summary>
        ''' <param name="PTT"></param>
        ''' <returns></returns>
        <ExportAPI("Locus.Maps"), Extension>
        Public Function LocusMaps(PTT As PTT) As Dictionary(Of String, String)
            Dim LQuery = (From gene As GeneBrief
                          In PTT.GeneObjects
                          Let locus As String = gene.Synonym
                          Where Not String.IsNullOrEmpty(locus)
                          Select locus,
                              loci = gene.Location.toUniqueId
                          Group By loci Into Group).ToArray
            Dim hash = LQuery.ToDictionary(Function(x) x.loci, Function(x) x.Group.First.locus)
            Return hash
        End Function

        <Extension>
        Private Function toUniqueId(loci As NucleotideLocation) As String
            Call loci.Normalization()

            If loci.Strand = Strands.Forward Then
                Return $"{loci.left}..{loci.right}"
            Else
                Return $"complement({loci.left()}..{loci.right()})"
            End If
        End Function

        ''' <summary>
        ''' a shortcut of the <see cref="PTT.Load(String, Boolean)"/> function.
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        <ExportAPI("Read.PTT")>
        Public Function LoadPTT(path As String) As PTT
            Return PTT.Load(path)
        End Function

        <Extension>
        Public Function GetObjects(Of TGene As IGeneBrief)(source As IEnumerable(Of TGene), site As Integer, direction As Strands) As TGene()
            Dim genes As TGene()

            If direction = Strands.Reverse Then
                genes = From gene As TGene
                        In source
                        Where gene.Location.Strand = Strands.Reverse
                        Select gene
            ElseIf direction = Strands.Forward Then
                genes = From gene As TGene
                        In source
                        Where gene.Location.Strand = Strands.Forward
                        Select gene
            Else
                genes = source
            End If

            Dim LQuery = (From gene As TGene
                          In genes
                          Where gene.Location.IsInside(site)
                          Select gene).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' Export protein sequence with full annotation.
        ''' </summary>
        ''' <param name="gbk"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ExportProteins(gbk As NCBI.GenBank.GBFF.File, Optional filterEmpty As Boolean = False) As FastaFile
            Dim template As New StringTemplate("gi|<gb_asm_id>|gb|<protein_id>|<locus_tag>|<gene>|<nucl_loc>|<product>")
            Dim fasta As New FastaFile(gbk.ExportProteins(template, filterEmpty:=filterEmpty))
            Return fasta
        End Function

        ''' <summary>
        ''' Export protein sequence with custom annotation title with given template.
        ''' </summary>
        ''' <param name="gb"></param>
        ''' <param name="headers">the fasta sequence header template</param>
        ''' <param name="filterEmpty"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function ExportProteins(gb As GBFF.File,
                                                headers As StringTemplate,
                                                Optional filterEmpty As Boolean = False) As IEnumerable(Of FastaSeq)
            Dim i As i32 = 1
            Dim accessionId As String = gb.Accession.AccessionId
            Dim lineage As String = gb.Source.BiomString

            Call headers.SetDefaultKey("ncbi_taxid", gb.Taxon)
            Call headers.SetDefaultKey("lineage", lineage)
            Call headers.SetDefaultKey("gb_asm_id", accessionId)

            For Each feature As gbffFeature In gb.Features.AsEnumerable
                If feature.KeyName <> "CDS" Then
                    Continue For
                Else
                    Call headers.SetDefaultKey("locus_tag", $"locus_{++i}")
                    Call headers.SetDefaultKey("nucl_loc", feature.Location.ContiguousRegion.ToString)
                End If

                Dim prot_seq As String = feature.Query(FeatureQualifiers.translation)
                Dim title_str As String = headers.CreateString(feature)

                If filterEmpty AndAlso prot_seq.StringEmpty Then
                    Continue For
                End If

                Yield New FastaSeq(prot_seq, title:=title_str)
            Next
        End Function

        ''' <summary>
        ''' fasta sequence title header in format: locus_tag product_description
        ''' </summary>
        ''' <param name="gb"></param>
        ''' <param name="duplicateOldLocusTag">
        ''' andalso populate an duplicated sequence data if ``old_locus_tag`` is existed inside thse CDS feature.
        ''' </param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <Extension>
        Public Function ExportProteins_Short(gb As NCBI.GenBank.GBFF.File,
                                             Optional onlyLocusTag As Boolean = False,
                                             Optional duplicateOldLocusTag As Boolean = False) As FastaFile

            Dim LQuery As IEnumerable(Of FastaSeq)

            If duplicateOldLocusTag Then
                Dim pull As IEnumerable(Of FastaSeq()) = From feature As gbffFeature
                                                         In gb.Features
                                                         Where String.Equals(feature.KeyName, "CDS")
                                                         Let seq As FastaSeq = feature.makeProtSeqShort(onlyLocusTag)
                                                         Let old As FastaSeq = feature.makeProtSeqShort(onlyLocusTag, oldLocusTag:=True)
                                                         Select New FastaSeq() {seq, old}
                LQuery = From seq As FastaSeq
                         In pull.IteratesALL
                         Where Not seq Is Nothing
            Else
                LQuery = From feature As gbffFeature
                         In gb.Features
                         Where String.Equals(feature.KeyName, "CDS")
                         Select feature.makeProtSeqShort(onlyLocusTag)
            End If

            Dim unique As IEnumerable(Of FastaSeq) = LQuery _
                .GroupBy(Function(seq) seq.Headers(Scan0)) _
                .OrderBy(Function(seq) seq.Key) _
                .Select(Function(seq)
                            Return seq.First
                        End Function)
            Dim fasta As New FastaFile(unique)

            Return fasta
        End Function

        ''' <summary>
        ''' 假若是新注释的基因组还没有基因号，则默认使用位置来做唯一标示
        ''' </summary>
        ''' <param name="feature"></param>
        ''' <param name="onlyLocusTag"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' > locus_id info
        ''' </remarks>
        <Extension>
        Private Function makeProtSeqShort(feature As gbffFeature, onlyLocusTag As Boolean, Optional oldLocusTag As Boolean = False) As FastaSeq
            Dim product As String = feature.Query("product")
            Dim locusId As String

            If product Is Nothing Then
                product = ""
            End If

            If oldLocusTag Then
                locusId = feature.Query("old_locus_tag")

                If locusId.StringEmpty Then
                    Return Nothing
                End If
            Else
                locusId = feature.Query("locus_tag")
            End If

            If locusId.StringEmpty Then
                locusId = feature.QueryDuplicated("db_xref").FirstOrDefault
            End If
            If locusId.StringEmpty Then
                locusId = feature.Location.UniqueId
            End If

            Dim ORF_transl As String = feature.Query("translation")
            Dim attrs As String() = If(Not onlyLocusTag, {locusId & " " & product}, {locusId})
            Dim fa As New FastaSeq With {
                .Headers = attrs,
                .SequenceData = ORF_transl
            }

            Return fa
        End Function

        ''' <summary>
        ''' export all gene features: RNA + CDS
        ''' </summary>
        ''' <returns>{locus_tag, gene}</returns>
        ''' <remarks></remarks>
        '''
        <ExportAPI("Export.GeneList")>
        <Extension>
        Public Function GeneList(gbk_asm As NCBI.GenBank.GBFF.File) As NamedValue(Of String)()
            Dim GQuery As IEnumerable(Of gbffFeature) =
                From feature
                In gbk_asm.Features
                Where String.Equals(feature.KeyName, "gene")
                Select feature 'Gene list query
            Dim AQuery = LinqAPI.Exec(Of NamedValue(Of String)) <=
                                                                  _
                From locusTag
                In GQuery.ToArray
                Select New NamedValue(Of String)(locusTag.Query("locus_tag"), locusTag.Query("gene")) '

            Return AQuery
        End Function

        ''' <summary>
        ''' make export of the annotated 16s RNA from the given genbank assembly
        ''' </summary>
        ''' <param name="Gbk"></param>
        ''' <returns></returns>
        <Extension>
        Public Function _16SribosomalRNA(Gbk As NCBI.GenBank.GBFF.File) As gbffFeature
            Dim LQuery = From feature As gbffFeature
                         In Gbk.Features.AsParallel
                         Where String.Equals(feature.KeyName, "rRNA") AndAlso InStr(feature.Query("product"), "16S ribosomal RNA")
                         Select feature '
            Return LQuery.First
        End Function
    End Module
End Namespace
