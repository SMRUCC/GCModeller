#Region "Microsoft.VisualBasic::97c5c17ee9a0634ef225fcd86a687189, core\test\Feature.vb"

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

    ' 
    ' /********************************************************************************/

#End Region

'Imports System.Text.RegularExpressions
'Imports SMRUCC.genomics.Assembly.NCBI.GenBank.PttGenomeBrief.ComponentModels
'Imports System.Text
'Imports SMRUCC.genomics.ComponentModel.Loci

'Namespace Assembly.NCBI.GenBank.PttGenomeBrief.GenomeFeature

'    ''' <summary>
'    ''' A feature is here an interval (i.e., a range of positions) on a chromosome or a union of such intervals.
'    ''' 
'    ''' In the case of RNA-Seq, the features are typically genes, where each gene is considered here as the union of all its exons. 
'    ''' One may also consider each exon as a feature, e.g., in order to check for alternative splicing. 
'    ''' 
'    ''' For comparative ChIP-Seq, the features might be binding region from a pre-determined list.
'    ''' </summary>
'    Public Class Feature : Inherits LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation

'        ''' <summary>
'        ''' The name of the sequence. Having an explicit sequence name allows a feature file to be prepared for a data set 
'        ''' of multiple sequences. Normally the seqname will be the identifier of the sequence in an accompanying fasta 
'        ''' format file. An alternative is that &lt;seqname> is the identifier for a sequence in a public database, such as 
'        ''' an EMBL/Genbank/DDBJ accession number. Which is the case, and which file or database to use, should be explained 
'        ''' in accompanying information.
'        ''' </summary>
'        ''' <returns></returns>
'        Public Property seqname As String
'        ''' <summary>
'        ''' The source of this feature. This field will normally be used to indicate the program making the prediction, 
'        ''' or if it comes from public database annotation, or is experimentally verified, etc.
'        ''' </summary>
'        ''' <returns></returns>
'        Public Property source As String
'        ''' <summary>
'        ''' The feature type name. We hope to suggest a standard set of features, to facilitate import/export, comparison etc.. 
'        ''' Of course, people are free to define new ones as needed. For example, Genie splice detectors account for a region 
'        ''' of DNA, and multiple detectors may be available for the same site, as shown above. We would like to enforce a 
'        ''' standard nomenclature for common GFF features. This does not forbid the use of other features, rather, just that 
'        ''' if the feature is obviously described in the standard list, that the standard label should be used. For this standard 
'        ''' table we propose to fall back on the international public standards for genomic database feature annotation, 
'        ''' specifically, the DDBJ/EMBL/GenBank feature table documentation).
'        ''' </summary>
'        ''' <returns></returns>
'        Public Property Feature As String
'        ''' <summary>
'        ''' Integers. &lt;start> must be less than or equal to &lt;end>. Sequence numbering starts at 1, so these numbers 
'        ''' should be between 1 and the length of the relevant sequence, inclusive. 
'        ''' 
'        ''' (Version 2 change: version 2 condones values of &lt;start> and &lt;end> that extend outside the reference sequence. 
'        ''' This is often more natural when dumping from acedb, rather than clipping. It means that some software using the 
'        ''' files may need to clip for itself.)
'        ''' </summary>
'        ''' <returns></returns>
'        Public Overrides ReadOnly Property start As Integer
'            Get
'                Return MyBase.Start
'            End Get
'        End Property

'        ''' <summary>
'        ''' Integers. &lt;start> must be less than or equal to &lt;end>. Sequence numbering starts at 1, so these numbers 
'        ''' should be between 1 and the length of the relevant sequence, inclusive. 
'        ''' 
'        ''' (Version 2 change: version 2 condones values of &lt;start> and &lt;end> that extend outside the reference sequence. 
'        ''' This is often more natural when dumping from acedb, rather than clipping. It means that some software using the 
'        ''' files may need to clip for itself.)
'        ''' </summary>
'        ''' <returns></returns>
'        Public Overrides ReadOnly Property [Ends] As Integer
'            Get
'                Return MyBase.Ends
'            End Get
'        End Property

'        ''' <summary>
'        ''' A floating point value. When there is no score (i.e. for a sensor that just records the possible presence of a signal, 
'        ''' as for the EMBL features above) you should use '.'. 
'        ''' 
'        ''' (Version 2 change: in version 1 of GFF you had to write 0 in such circumstances.)
'        ''' </summary>
'        ''' <returns></returns>
'        Public Property score As String
'        ''' <summary>
'        ''' One of '+', '-' or '.'. '.' should be used when strand is not relevant, e.g. for dinucleotide repeats. 
'        ''' 
'        ''' Version 2 change: This field is left empty '.' for RNA and protein features.
'        ''' </summary>
'        ''' <returns></returns>
'        Public Overrides Property Strand As Strands
'            Get
'                Return MyBase.Strand
'            End Get
'            Set(value As Strands)
'                MyBase.Strand = value
'            End Set
'        End Property

'        ''' <summary>
'        ''' One of '0', '1', '2' or '.'. '0' indicates that the specified region is in frame, i.e. that its first base corresponds to 
'        ''' the first base of a codon. '1' indicates that there is one extra base, i.e. that the second base of the region corresponds 
'        ''' to the first base of a codon, and '2' means that the third base of the region is the first base of a codon. 
'        ''' 
'        ''' If the strand is '-', then the first base of the region is value of &lt;end>, because the corresponding coding region will run 
'        ''' from &lt;end> to &lt;start> on the reverse strand. As with &lt;strand>, if the frame is not relevant then set &lt;frame> to '.'. 
'        ''' It has been pointed out that "phase" might be a better descriptor than "frame" for this field. 
'        ''' 
'        ''' Version 2 change: This field is left empty '.' for RNA and protein features.
'        ''' </summary>
'        ''' <returns></returns>
'        Public Property frame As String

'#Region "可选的字段域"
'        ''' <summary>
'        ''' From version 2 onwards, the attribute field must have an tag value structure following the syntax used within objects in 
'        ''' a .ace file, flattened onto one line by semicolon separators. Tags must be standard identifiers ([A-Za-z][A-Za-z0-9_]*). 
'        ''' Free text values must be quoted with double quotes. Note: all non-printing characters in such free text value strings 
'        ''' (e.g. newlines, tabs, control characters, etc) must be explicitly represented by their C (UNIX) style backslash-escaped 
'        ''' representation (e.g. newlines as '\n', tabs as '\t'). As in ACEDB, multiple values can follow a specific tag. The aim is 
'        ''' to establish consistent use of particular tags, corresponding to an underlying implied ACEDB model if you want to think 
'        ''' that way (but acedb is not required). Examples of these would be:
'        ''' 
'        '''     seq1     BLASTX  similarity   101  235 87.1 + 0  Target "HBA_HUMAN" 11 55 ; E_value 0.0003
'        '''     dJ102G20 GD_mRNA coding_exon 7105 7201   .  - 2 Sequence "dJ102G20.C1.1"
'        ''' 
'        ''' The semantics Of tags In attribute field tag-values pairs has intentionally Not been formalized. Two useful guidelines are 
'        ''' To use DDBJ/EMBL/GenBank feature 'qualifiers' (see DDBJ/EMBL/GenBank feature table documentation), or the features that 
'        ''' ACEDB generates when it dumps GFF. Version 1 note In version 1 the attribute field was called the group field, with the 
'        ''' following specification: An optional string-valued field that can be used as a name to group together a set of records. 
'        ''' Typical uses might be to group the introns and exons in one gene prediction (or experimentally verified gene structure), 
'        ''' or to group multiple regions of match to another sequence, such as an EST or a protein.
'        ''' </summary>
'        ''' <returns></returns>
'        ''' <remarks>gff1, gff2, gff3之间的差异是由于本属性值的列的读取方式的差异而产生的</remarks>
'        Public Property attributes As Dictionary(Of String, String)

'        ''' <summary>
'        ''' Comments are allowed, starting with "#" as in Perl, awk etc. Everything following # until the end of the line is ignored. 
'        ''' Effectively this can be used in two ways. Either it must be at the beginning of the line (after any whitespace), to make 
'        ''' the whole line a comment, or the comment could come after all the required fields on the line.
'        ''' </summary>
'        ''' <returns></returns>
'        Public Property comments As String
'#End Region

'        Public Overrides Function ToString() As String
'            Return $"{MyBase.ToString}   {Me.seqname}::{Me.frame}"
'        End Function

'        Public Function GenerateDocumentLine() As String
'            Throw New NotImplementedException
'        End Function

'        ''' <summary>
'        ''' 
'        ''' </summary>
'        ''' <param name="s_Data"></param>
'        ''' <param name="version">gff1, gff2, gff3之间的差异是由于本属性值的列的读取方式的差异而产生的</param>
'        ''' <returns></returns>
'        Public Overloads Shared Function CreateObject(s_Data As String, version As Integer) As Feature
'            Dim Tokens As String() = Split(s_Data, vbTab)
'            Dim Feature As Feature = New Feature
'            Dim p As Integer = 0

'            ' Fields are: <seqname> <source> <feature> <start> <end> <score> <strand> <frame> [attributes] [comments]

'            With Feature
'                .seqname = Tokens(p.MoveNext)
'                .source = Tokens(p.MoveNext)
'                .Feature = Tokens(p.MoveNext)
'                .Left = CLng(Val(Tokens(p.MoveNext)))
'                .Right = CLng(Val(Tokens(p.MoveNext)))
'                .score = Tokens(p.MoveNext)
'                .Strand = LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation.GetStrand(Tokens(p.MoveNext))
'                .frame = Tokens(p)
'            End With

'            '在这里开始读取可选的列数据
'            Dim attrValue As String = If(Tokens.Count - 1 > p, Tokens(p.Increase), "")

'            If Not String.IsNullOrEmpty(attrValue) Then
'                Select Case version
'                    Case 1
'                    Case 2
'                    Case 3 : Feature.attributes = CreateObjectGff3(attrValue)
'                    Case Else
'                        Call Console.WriteLine($"{NameOf(version)}={version} is currently not supported yet, ignored!")
'                End Select
'            End If

'            Return Feature
'        End Function

'        ''' <summary>
'        ''' 
'        ''' </summary>
'        ''' <param name="s_Data"></param>
'        ''' <returns></returns>
'        ''' <remarks>
'        ''' gi|66571684|gb|CP000050.1|	RefSeq	Coding gene	42	1370	.	+	.	name=dnaA;product="chromosome replication initiator DnaA"
'        ''' </remarks>
'        Private Shared Function CreateObjectGff3(s_Data As String) As Dictionary(Of String, String)
'            Dim Tokens As String() = attributeTokens(Line:=s_Data)
'            Dim LQuery = (From Token As String In Tokens Let p As Integer = InStr(Token, "=") Let Name As String = Mid(Token, 1, p - 1), Value As String = Mid(Token, p + 1) Select Name, Value).ToArray
'            Dim attrs = LQuery.ToDictionary(Function(obj) obj.Name, elementSelector:=Function(obj) If(Len(obj.Value) > 2 AndAlso obj.Value.First = """"c AndAlso obj.Value.Last = """"c, Mid(obj.Value, 2, Len(obj.Value) - 2), obj.Value))
'            Return attrs
'        End Function

'        ''' <summary>
'        ''' A regex expression string that use for split the line text.
'        ''' </summary>
'        ''' <remarks></remarks>
'        Protected Const SplitRegxExpression As String = "[" & vbTab & ";](?=(?:[^""]|""[^""]*"")*$)"

'        ''' <summary>
'        ''' Row parsing into column tokens
'        ''' </summary>
'        ''' <param name="Line"></param>
'        ''' <returns></returns>
'        ''' <remarks></remarks>
'        Private Shared Function attributeTokens(Line As String) As String()
'            If String.IsNullOrEmpty(Line) Then
'                Return Nothing
'            End If

'            Dim Row = Regex.Split(Line, SplitRegxExpression)
'            For i As Integer = 0 To Row.Count - 1
'                If Not String.IsNullOrEmpty(Row(i)) Then
'                    If Row(i).First = """"c AndAlso Row(i).Last = """"c Then
'                        Row(i) = Mid(Row(i), 2, Len(Row(i)) - 2)
'                    End If
'                End If

'            Next
'            Return Row
'        End Function

'    End Class
'End Namespace
