#Region "Microsoft.VisualBasic::122c6c9260b411e8d39bc4447386c540, core\Bio.Annotation\GFF\Feature.vb"

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

'   Total Lines: 300
'    Code Lines: 157 (52.33%)
' Comment Lines: 114 (38.00%)
'    - Xml Docs: 90.35%
' 
'   Blank Lines: 29 (9.67%)
'     File Size: 14.26 KB


'     Class Feature
' 
'         Properties: attributes, COG, comments, ends, feature
'                     frame, ID, left, Length, Location
'                     Product, proteinId, right, score, seqname
'                     source, start, strand, synonym
' 
'         Function: __getMappingLoci, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.Serialization
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Assembly.NCBI.GenBank.TabularFormat.GFF

    ''' <summary>
    ''' A feature is here an interval (i.e., a range of positions) on a chromosome or a union of such intervals.
    '''
    ''' In the case of RNA-Seq, the features are typically genes, where each gene is considered here as the union of all its exons. 
    ''' One may also consider each exon as a feature, e.g., in order to check for alternative splicing. 
    ''' 
    ''' For comparative ChIP-Seq, the features might be binding region from a pre-determined list.
    ''' </summary>
    ''' <remarks>
    ''' (Feature是基因组序列片段之上的一个具备有生物学功能意义的区域，故而这个对象继承自核酸位点对象)
    ''' </remarks>
    Public Class Feature : Inherits Contig
        Implements INamedValue
        Implements IGeneBrief
        Implements ILocationComponent

        ''' <summary>
        ''' The name of the sequence. Having an explicit sequence name allows a feature file to be prepared for a data set 
        ''' of multiple sequences. Normally the seqname will be the identifier of the sequence in an accompanying fasta 
        ''' format file. An alternative is that &lt;seqname> is the identifier for a sequence in a public database, such as 
        ''' an EMBL/Genbank/DDBJ accession number. Which is the case, and which file or database to use, should be explained 
        ''' in accompanying information.
        ''' </summary>
        ''' <returns></returns>
        Public Property seqname As String
        ''' <summary>
        ''' The source of this feature. This field will normally be used to indicate the program making the prediction, 
        ''' or if it comes from public database annotation, or is experimentally verified, etc.
        ''' </summary>
        ''' <returns></returns>
        Public Property source As String
        ''' <summary>
        ''' The feature type name. We hope to suggest a standard set of features, to facilitate import/export, comparison etc.. 
        ''' Of course, people are free to define new ones as needed. For example, Genie splice detectors account for a region 
        ''' of DNA, and multiple detectors may be available for the same site, as shown above. We would like to enforce a 
        ''' standard nomenclature for common GFF features. This does not forbid the use of other features, rather, just that 
        ''' if the feature is obviously described in the standard list, that the standard label should be used. For this standard 
        ''' table we propose to fall back on the international public standards for genomic database feature annotation, 
        ''' specifically, the DDBJ/EMBL/GenBank feature table documentation).
        ''' </summary>
        ''' <returns></returns>
        Public Property feature As String

        ''' <summary>
        ''' Integers. &lt;start> must be less than or equal to &lt;end>. Sequence numbering starts at 1, so these numbers 
        ''' should be between 1 and the length of the relevant sequence, inclusive. 
        ''' 
        ''' (Version 2 change: version 2 condones values of &lt;start> and &lt;end> that extend outside the reference sequence. 
        ''' This is often more natural when dumping from acedb, rather than clipping. It means that some software using the 
        ''' files may need to clip for itself.)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property start As Integer
            Get
                Return MappingLocation.Start
            End Get
        End Property

        ''' <summary>
        ''' Integers. &lt;start> must be less than or equal to &lt;end>. Sequence numbering starts at 1, so these numbers 
        ''' should be between 1 and the length of the relevant sequence, inclusive. 
        ''' 
        ''' (Version 2 change: version 2 condones values of &lt;start> and &lt;end> that extend outside the reference sequence. 
        ''' This is often more natural when dumping from acedb, rather than clipping. It means that some software using the 
        ''' files may need to clip for itself.)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ends As Integer
            Get
                Return MappingLocation.Ends
            End Get
        End Property

        ''' <summary>
        ''' A floating point value. When there is no score (i.e. for a sensor that just records the possible presence of a signal, 
        ''' as for the EMBL features above) you should use '.'. 
        ''' 
        ''' (Version 2 change: in version 1 of GFF you had to write 0 in such circumstances.)
        ''' </summary>
        ''' <returns></returns>
        Public Property score As String

        ''' <summary>
        ''' One of '+', '-' or '.'. '.' should be used when strand is not relevant, e.g. for dinucleotide repeats. 
        ''' 
        ''' Version 2 change: This field is left empty '.' for RNA and protein features.
        ''' </summary>
        ''' <returns></returns>
        Public Property strand As Strands
            Get
                Return _strand
            End Get
            Set(value As Strands)
                _strand = value
            End Set
        End Property

        Dim _strand As Strands

        ''' <summary>
        ''' One of '0', '1', '2' or '.'. '0' indicates that the specified region is in frame, i.e. that its first base corresponds to 
        ''' the first base of a codon. '1' indicates that there is one extra base, i.e. that the second base of the region corresponds 
        ''' to the first base of a codon, and '2' means that the third base of the region is the first base of a codon. 
        ''' 
        ''' If the strand is '-', then the first base of the region is value of &lt;end>, because the corresponding coding region will run 
        ''' from &lt;end> to &lt;start> on the reverse strand. As with &lt;strand>, if the frame is not relevant then set &lt;frame> to '.'. 
        ''' It has been pointed out that "phase" might be a better descriptor than "frame" for this field. 
        ''' 
        ''' Version 2 change: This field is left empty '.' for RNA and protein features.
        ''' </summary>
        ''' <returns></returns>
        Public Property frame As String

#Region "可选的字段域"

        ''' <summary>
        ''' From version 2 onwards, the attribute field must have an tag value structure following the syntax used within objects in 
        ''' a .ace file, flattened onto one line by semicolon separators. Tags must be standard identifiers ([A-Za-z][A-Za-z0-9_]*). 
        ''' Free text values must be quoted with double quotes. Note: all non-printing characters in such free text value strings 
        ''' (e.g. newlines, tabs, control characters, etc) must be explicitly represented by their C (UNIX) style backslash-escaped 
        ''' representation (e.g. newlines as '\n', tabs as '\t'). As in ACEDB, multiple values can follow a specific tag. The aim is 
        ''' to establish consistent use of particular tags, corresponding to an underlying implied ACEDB model if you want to think 
        ''' that way (but acedb is not required). Examples of these would be:
        ''' 
        '''     seq1     BLASTX  similarity   101  235 87.1 + 0  Target "HBA_HUMAN" 11 55 ; E_value 0.0003
        '''     dJ102G20 GD_mRNA coding_exon 7105 7201   .  - 2 Sequence "dJ102G20.C1.1"
        ''' 
        ''' The semantics Of tags In attribute field tag-values pairs has intentionally Not been formalized. Two useful guidelines are 
        ''' To use DDBJ/EMBL/GenBank feature 'qualifiers' (see DDBJ/EMBL/GenBank feature table documentation), or the features that 
        ''' ACEDB generates when it dumps GFF. Version 1 note In version 1 the attribute field was called the group field, with the 
        ''' following specification: An optional string-valued field that can be used as a name to group together a set of records. 
        ''' Typical uses might be to group the introns and exons in one gene prediction (or experimentally verified gene structure), 
        ''' or to group multiple regions of match to another sequence, such as an EST or a protein.
        ''' (请注意，所有的key都已经被转换为小写的形式了)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>gff1, gff2, gff3之间的差异是由于本属性值的列的读取方式的差异而产生的</remarks>
        Public Property attributes As Dictionary(Of String, String)
            Get
                If _attrs Is Nothing Then
                    _attrs = New Dictionary(Of String, String)
                End If
                Return _attrs
            End Get
            Set(value As Dictionary(Of String, String))
                _attrs = value
            End Set
        End Property

        Dim _attrs As Dictionary(Of String, String)

        ''' <summary>
        ''' Comments are allowed, starting with "#" as in Perl, awk etc. Everything following # until the end of the line is ignored. 
        ''' Effectively this can be used in two ways. Either it must be at the beginning of the line (after any whitespace), to make 
        ''' the whole line a comment, or the comment could come after all the required fields on the line.
        ''' </summary>
        ''' <returns></returns>
        Public Property comments As String

        ''' <summary>
        ''' 请注意，这个属性不是基因号
        ''' </summary>
        ''' <returns></returns>
        Public Property ID As String Implements INamedValue.Key
            Get
                Return attributes.TryGetValue("id")
            End Get
            Set(value As String)
                attributes("id") = value
            End Set
        End Property

        Public Property COG As String Implements IFeatureDigest.Feature
            Get
                Dim s As String = attributes.TryGetValue("note")
                If String.IsNullOrEmpty(s) Then
                    Return ""
                Else
                    Return Regex.Match(s, "COG\d+", RegexICSng).Value
                End If
            End Get
            Set(value As String)
                Dim s As String = attributes.TryGetValue("note")

                If String.IsNullOrEmpty(s) Then
                    attributes("note") = value
                Else
                    s = Regex.Replace(s, "COG\d+", value, RegexICSng)
                    attributes("note") = s
                End If
            End Set
        End Property

        Public Property Product As String Implements IGeneBrief.Product
            Get
                Return attributes.TryGetValue("product")
            End Get
            Set(value As String)
                attributes("product") = value
            End Set
        End Property

        Public Property Length As Integer Implements IGeneBrief.Length
            Get
                Return MappingLocation.FragmentSize
            End Get
            Protected Set(value As Integer)
                Throw New NotSupportedException()
            End Set
        End Property

        <IgnoreDataMember>
        Public Property Location As NucleotideLocation Implements IContig.Location
            Get
                Return MappingLocation
            End Get
            Set(value As NucleotideLocation)
                left = value.left
                right = value.right
                strand = value.Strand
            End Set
        End Property

        Public Property right As Integer Implements ILocationComponent.right
            Get
                Return _right
            End Get
            Set(value As Integer)
                _right = value
                _MappingLocation = Nothing
            End Set
        End Property

        Dim _left As Integer, _right As Integer

        Public Property left As Integer Implements ILoci.left
            Get
                Return _left
            End Get
            Set(value As Integer)
                _left = value
                _MappingLocation = Nothing
            End Set
        End Property
#End Region

        Public ReadOnly Property synonym As String
            Get
                Dim s As String = Nothing

                If attributes.TryGetValue("locus_tag", s) Then
                    Return s
                End If
                If attributes.TryGetValue("protein_id", s) Then
                    Return s
                End If
                If attributes.TryGetValue("name", s) Then
                    Return s
                End If

                Return s
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 请注意这个属性里面的标签的读取顺序是和<see cref="synonym"/>属性里面的标签的读取的顺序是不一样的
        ''' </remarks>
        Public ReadOnly Property proteinId As String
            Get
                Dim s As String = Nothing

                If attributes.TryGetValue("name", s) Then
                    Return s
                ElseIf _attrs.TryGetValue("protein_id", s) Then
                    Return s
                Else
                    Return ""
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{MyBase.ToString}   {Me.seqname}::{Me.frame}"
        End Function

        Protected Overrides Function __getMappingLoci() As NucleotideLocation
            Return New NucleotideLocation(left, right, strand)
        End Function
    End Class
End Namespace
