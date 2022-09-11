#Region "Microsoft.VisualBasic::847b1e58494296e19d659d3f83d0049a, GCModeller\core\Bio.Annotation\GFF\Extensions.vb"

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

    '   Total Lines: 146
    '    Code Lines: 122
    ' Comment Lines: 7
    '   Blank Lines: 17
    '     File Size: 6.56 KB


    '     Module Extensions
    ' 
    '         Function: AsGenes, GPFF2Feature, Load, ProtId2Locus, ToGeneBrief
    '                   (+2 Overloads) ToGff
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports gbffFeature = SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.Feature

Namespace Assembly.NCBI.GenBank.TabularFormat.GFF

    <HideModuleName>
    Public Module Extensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Load(path As String) As GFFTable
            Return GFFTable.LoadDocument(path)
        End Function

        ''' <summary>
        ''' 将NCBI genbank数据库文件转换为GFF3文件
        ''' </summary>
        ''' <param name="gb"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function ToGff(gb As GBFF.File) As GFFTable
            Dim Gff As New GFFTable With {
                .date = gb.Locus.UpdateTime,
                .Features = gb.Features.Select(AddressOf gb.ToGff).ToArray,
                .GffVersion = 3,
                .SeqRegion = New SeqRegion With {
                      .accessId = gb.Accession.AccessionId,
                      .start = 1,
                      .ends = gb.Origin.SequenceData.Length
                },
                .type = "DNA"
            }
            Return Gff
        End Function

        <Extension>
        Friend Function ToGff(gb As GBFF.File, feature As gbffFeature) As GFF.Feature
            Dim gff As New GFF.Feature

            ' Fields are: <seqname> <source> <feature> <start> <end> <score> <strand> <frame> [attributes] [comments]
            gff.seqname = gb.Accession.AccessionId
            gff.source = "Genebank"
            gff.feature = feature.KeyName
            gff.Left = feature.Location.Location.left
            gff.Right = feature.Location.Location.right
            gff.score = "."
            gff.strand = feature.Location.ContiguousRegion.Strand
            gff.frame = "."
            gff.attributes = feature.attributes()
            gff.comments = feature.Query(FeatureQualifiers.note)

            Return gff
        End Function

        <Extension>
        Public Function GPFF2Feature(gb As GBFF.File, gff As Dictionary(Of String, GFF.Feature)) As GeneBrief
            Dim prot As gbffFeature = gb.Features.ListFeatures("Protein").FirstOrDefault
            If prot Is Nothing Then
                Return Nothing
            End If

            Dim CDS As gbffFeature = gb.Features.ListFeatures("CDS").FirstOrDefault
            Dim locus_tag As String = ""

            If CDS Is Nothing Then
                locus_tag = "-"
            Else
                locus_tag = CDS.Query("locus_tag")
            End If

            Dim uid As String = gb.Version.VersionString

            If Not gff.ContainsKey(uid) Then
                Throw New KeyNotFoundException(uid & " is not exists in the genomics feature table!")
            End If

            Dim ntLoci As NucleotideLocation = gff(uid).MappingLocation
            Dim gene As New GeneBrief With {
                .Code = gb.Version.GI,
                .COG = "-",
                .Gene = locus_tag,
                .Location = ntLoci,
                .PID = gb.Accession.AccessionId,
                .Length = ntLoci.FragmentSize,
                .Product = prot.Query("product"),
                .Synonym = uid
            }

            Return gene
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function AsGenes(features As IEnumerable(Of GFF.Feature)) As IEnumerable(Of GeneBrief)
            Return features _
                .SafeQuery _
                .Select(Function(feature) feature.ToGeneBrief)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function ToGeneBrief(feature As GFF.Feature) As GeneBrief
            Return New GeneBrief With {
                .Code = feature.ID,
                .COG = feature.COG,
                .Gene = feature.attributes.TryGetValue("locus_tag") Or feature.proteinId.AsDefault,
                .IsORF = True,
                .Length = feature.Length,
                .Location = feature.Location,
                .PID = feature.proteinId,
                .Product = feature.Product,
                .Synonym = feature.synonym
            }
        End Function

        <Extension>
        Public Function ProtId2Locus(gff As GFFTable) As Dictionary(Of String, String)
            Dim CDS As Feature() = LinqAPI.Exec(Of Feature) _
                () <= From x As Feature
                      In gff.features
                      Where String.Equals(x.feature, "CDS", StringComparison.OrdinalIgnoreCase)
                      Select x
            Dim gene As Dictionary(Of String, Feature) = (From x In gff.features
                                                          Where String.Equals(x.feature, "gene", StringComparison.OrdinalIgnoreCase)
                                                          Select x) _
                                                                .ToDictionary(Function(x) x.attributes("id"))
            Dim transformHash As Dictionary(Of String, String) = (From x As Feature
                                                                  In CDS
                                                                  Let parent As String = x.attributes("parent")
                                                                  Where gene.ContainsKey(parent)
                                                                  Select x,
                                                                      locus_tag = gene(parent).attributes("locus_tag")) _
                                                                        .ToDictionary(Function(x) x.x.attributes("name"),
                                                                                      Function(x)
                                                                                          Return x.locus_tag
                                                                                      End Function)
            Return transformHash
        End Function
    End Module
End Namespace
