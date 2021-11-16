#Region "Microsoft.VisualBasic::37820aeb3814124625e71d902323e94f, meme_suite\MEME.DocParser\XmlOutput\MAST.vb"

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

    '     Class ValueBase
    ' 
    '         Properties: value
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    '         Class RemoveCorrelated
    ' 
    ' 
    ' 
    '         Class StrandHandling
    ' 
    ' 
    ' 
    '         Class TranslateDNA
    ' 
    ' 
    ' 
    '         Class adj_hit_pvalue
    ' 
    ' 
    ' 
    ' 
    ' 
    '     Class Model
    ' 
    '         Properties: [When], adj_hit_pvalue, max_correlation, max_hit_pvalue, max_seq_evalue
    '                     max_weak_pvalue, remove_correlated, strand_handling, translate_dna
    ' 
    '     Class Motif
    ' 
    '         Properties: BriefName, Correlations, Directory, last_mod_date, memePWM
    '                     Motifs, name, source
    ' 
    '         Function: ToString
    ' 
    '     Class MotifSite
    ' 
    '         Properties: bad, best_f, best_r, id, name
    '                     num, width
    ' 
    '         Function: ToString
    ' 
    '     Class Correlation
    ' 
    '         Properties: motif_a, motif_b, value
    ' 
    '     Class DbProperty
    ' 
    '         Properties: id, last_mod_date, name, num, residue_count
    '                     seq_count, source, type
    ' 
    '         Function: ToString
    ' 
    '     Class SequenceList
    ' 
    '         Properties: Databases, ListCount, SequenceList
    ' 
    '         Function: ToString
    ' 
    '     Class SequenceDescript
    ' 
    '         Properties: comment, db, id, length, name
    '                     num, Score, Segments, title
    ' 
    '         Function: ToString
    ' 
    '     Class Score
    ' 
    '         Properties: combined_pvalue, evalue, strand
    ' 
    '     Class Segment
    ' 
    '         Properties: Hits, SegmentData, SequenceData, start
    ' 
    '         Function: ToString
    ' 
    '     Class HitResult
    ' 
    '         Properties: gap, idx, match, motif, pos
    '                     pvalue, rc, strand
    ' 
    '         Function: GetId, GetStrand, ToString
    ' 
    '     Class RuntimeEnvironment
    ' 
    '         Properties: cycles, seconds
    ' 
    '         Function: ToString
    ' 
    '     Class MAST
    ' 
    '         Properties: Model, Motifs, Runtime, Sequences
    ' 
    '         Function: Convert, ExportMotifs, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace DocumentFormat.XmlOutput.MAST

    Public MustInherit Class ValueBase

        <XmlAttribute("value")> Public Property value As String

        Protected Friend Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Return $"{Me.GetType.Name}  = {value}"
        End Function

        <XmlType("remove_correlated")>
        Public Class RemoveCorrelated : Inherits ValueBase
        End Class

        <XmlType("strand_handling")>
        Public Class StrandHandling : Inherits ValueBase
        End Class

        <XmlType("translate_dna")>
        Public Class TranslateDNA : Inherits ValueBase
        End Class

        <XmlType("adj_hit_pvalue")>
        Public Class adj_hit_pvalue : Inherits ValueBase
        End Class
    End Class

    Public Class Model : Inherits XmlOutput.ModelBase
        <XmlElement("max_correlation")> Public Property max_correlation As String
        Public Property remove_correlated As ValueBase.RemoveCorrelated
        Public Property strand_handling As ValueBase.StrandHandling
        Public Property translate_dna As ValueBase.TranslateDNA
        <XmlElement("max_seq_evalue")> Public Property max_seq_evalue As String
        Public Property adj_hit_pvalue As ValueBase.adj_hit_pvalue
        <XmlElement("max_hit_pvalue")> Public Property max_hit_pvalue As String
        <XmlElement("max_weak_pvalue")> Public Property max_weak_pvalue As String
        <XmlElement("when")> Public Property [When] As String
    End Class

    Public Class Motif

        ''' <summary>
        ''' MEME来源的文件名
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("source")> Public Property source As String
        <XmlAttribute("name")> Public Property name As String
        <XmlAttribute("last_mod_date")> Public Property last_mod_date As String
        <XmlElement("motif")> Public Property Motifs As MotifSite()
        <XmlElement("correlation")> Public Property Correlations As Correlation()

        Public Overrides Function ToString() As String
            Return $"[{BriefName}, {Motifs.Length}]   ""{source}"""
        End Function

        Public ReadOnly Property Directory As String
            Get
                If source Is Nothing Then
                    Return Nothing
                End If
                Dim DIR As String = FileIO.FileSystem.GetFileInfo(source).Directory.Name
                Dim Name As String = basename(DIR)
                Return Name
            End Get
        End Property

        Public ReadOnly Property memePWM As String
            Get
                Dim Name As String = basename(source)
                If String.Equals(Name, "meme", StringComparison.OrdinalIgnoreCase) Then
                    Return Me.Directory
                Else
                    Return Name
                End If
            End Get
        End Property

        Public ReadOnly Property BriefName As String
            Get
                Return source.Replace("\", "/").Split("/"c).Last.Split("."c).First
            End Get
        End Property
    End Class

    Public Class MotifSite
        <XmlAttribute("id")> Public Property id As String
        ''' <summary>
        ''' num is simply the loading order of the motif, it's superfluous but makes things easier for XSLT
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("num")> Public Property num As String
        <XmlAttribute("name")> Public Property name As String
        <XmlAttribute("width")> Public Property width As String
        <XmlAttribute("best_f")> Public Property best_f As String
        <XmlAttribute("best_r")> Public Property best_r As String
        <XmlAttribute("bad")> Public Property bad As String

        Public Overrides Function ToString() As String
            Return $"[{id}]    {best_f} | {best_r}"
        End Function
    End Class

    ''' <summary>
    ''' for n > 1 motifs there should be (n * (n - 1)) / 2 correlations, obviously there are none for only 1 motif
    ''' </summary>
    <XmlType("correlation")>
    Public Class Correlation
        <XmlAttribute("motif_a")> Public Property motif_a As String
        <XmlAttribute("motif_b")> Public Property motif_b As String
        <XmlAttribute("value")> Public Property value As Double
    End Class

    ''' <summary>
    ''' the database tags are expected to be ordered in file specification order
    ''' </summary>
    <XmlType("database")> Public Class DbProperty
        <XmlAttribute("id")> Public Property id As String
        <XmlAttribute("num")> Public Property num As String
        <XmlAttribute("source")> Public Property source As String
        <XmlAttribute("name")> Public Property name As String
        <XmlAttribute("last_mod_date")> Public Property last_mod_date As String
        <XmlAttribute("seq_count")> Public Property seq_count As String
        <XmlAttribute("residue_count")> Public Property residue_count As String
        <XmlAttribute("type")> Public Property type As String

        Public Overrides Function ToString() As String
            Return $"[{type}]  {name}"
        End Function
    End Class

    <XmlType("sequences")> Public Class SequenceList
        ''' <summary>
        ''' the database tags are expected to be ordered in file specification order
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("database")> Public Property Databases As DbProperty()
        <XmlElement("sequence")> Public Property SequenceList As SequenceDescript()

        Public ReadOnly Property ListCount As Integer
            Get
                Return SequenceList.Length
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Databases.ToString
        End Function
    End Class

    ''' <summary>
    ''' the sequence tags are expected to be ordered by best combined p-value (of contained score tags) ascending
    ''' </summary>
    Public Class SequenceDescript
        <XmlAttribute("id")> Public Property id As String
        <XmlAttribute("db")> Public Property db As String
        <XmlAttribute("num")> Public Property num As String
        <XmlAttribute("name")> Public Property name As String
        <XmlAttribute("comment")> Public Property comment As String
        ''' <summary>
        ''' length is in the same unit as the motifs, which is not always the same unit as the sequence
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("length")> Public Property length As String

        <XmlElement("score")> Public Property Score As Score
        ''' <summary>
        ''' within each sequence the seg tags are expected to be ordered by start ascending
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("seg")> Public Property Segments As Segment()

        Public ReadOnly Property title As String
            Get
                Return $"{name} {comment}"
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return title
        End Function
    End Class

    <XmlType("score")> Public Class Score
        ''' <summary>
        ''' frame is the starting offset for translation of dna sequences which gives the lowest pvalues for the provided protein motifs
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("strand")> Public Property strand As String
        <XmlAttribute("combined_pvalue")> Public Property combined_pvalue As String
        ''' <summary>
        ''' the expect tags are expected to be ordered by pos ascending
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("evalue")> Public Property evalue As String
    End Class

    ''' <summary>
    ''' within each seg the hit tags are expected to be ordered by pos ascending and then forward strand first
    ''' </summary>
    <XmlType("seg")> Public Class Segment

        ''' <summary>
        ''' <see cref="SegmentData"/>在基因组上面的左端的起始位置
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("start")> Public Property start As String
        ''' <summary>
        ''' 里面是有回车符的，使用前请先使用<see cref="TrimNewLine(String, String)"/>进行修剪
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("data")> Public Property SegmentData As String
        ''' <summary>
        ''' within each seg the hit tags are expected to be ordered by pos ascending and then forward strand first
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("hit")> Public Property Hits As HitResult()

        Public ReadOnly Property SequenceData As String
            Get
                Return SegmentData.TrimNewLine("").Trim
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return SequenceData
        End Function
    End Class

    ''' <summary>
    ''' gap, while superfluous, makes creating motif diagrams for the text version much easier when using XSLT
    ''' </summary>
    <XmlType("hit")> Public Class HitResult

        <XmlAttribute("pos")> Public Property pos As Integer
        ''' <summary>
        ''' gap, while superfluous, makes creating motif diagrams for the text version much easier when using XSLT
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("gap")> Public Property gap As String
        <XmlAttribute("motif")> Public Property motif As String
        <XmlAttribute("pvalue")> Public Property pvalue As String
        <XmlAttribute("strand")> Public Property strand As String
        <XmlAttribute("match")> Public Property match As String
        <XmlAttribute> Public Property idx As Integer
        <XmlAttribute> Public Property rc As String

        Public Function GetStrand() As String
            If strand Is Nothing Then
                If rc Is Nothing Then
                    Return "?"
                Else
                    Return If(rc = "n", "+", "-")
                End If
            Else
                Return strand.GetBriefStrandCode
            End If
        End Function

        Public Function GetId() As String
            If motif Is Nothing Then
                Return idx + 1
            Else
                Return motif
            End If
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class RuntimeEnvironment

        <XmlAttribute("cycles")> Public Property cycles As String
        <XmlAttribute("seconds")> Public Property seconds As String

        Public Overrides Function ToString() As String
            Return $"<runtime cycles=""{cycles}"" seconds=""{seconds}""/>"
        End Function
    End Class

    ''' <summary>
    ''' mast.xml
    ''' </summary>
    <XmlType("mast")> Public Class MAST : Inherits XmlOutput.MEMEXmlBase

        <XmlElement("model")> Public Property Model As Model
        <XmlElement("sequences")> Public Property Sequences As SequenceList
        <XmlElement("runtime")> Public Property Runtime As RuntimeEnvironment
        <XmlElement("motifs")> Public Property Motifs As Motif

        Public Overrides Function ToString() As String
            Return $"<mast version=""{Version}"" release=""{Release}"">"
        End Function

        Public Shared Widening Operator CType(XmlPath As String) As XmlOutput.MAST.MAST
            Return XmlPath.LoadXml(Of XmlOutput.MAST.MAST)()
        End Operator

        ''' <summary>
        ''' Regtransbase object parser
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Convert() As RowObject()
            If Sequences.SequenceList.IsNullOrEmpty Then
                Return Nothing
            End If

            Dim rowCollection As RowObject() = New RowObject(Me.Sequences.SequenceList.Count - 1) {}

            For i As Integer = 0 To Me.Sequences.ListCount - 1
                Dim Sequence As SequenceDescript = Sequences.SequenceList(i)
                Dim row As RowObject = New RowObject
                Dim Tokens As String() = Sequence.name.Split(CChar("|"))
                Dim Descripts As String() = Sequence.comment.Split(CChar("|"))

                If InStr(Sequence.comment, "* Fixed!") Then
                    row.Add("*")
                Else
                    row.Add("")
                End If

                Dim objId As String = Motifs.name
                Dim siteId As String = Tokens(1)
                Dim RegulatorId As String = Tokens(3)
                Dim Description As String = Tokens.Last & " " & Descripts.First.Trim(vbCrLf, vbCr, vbLf, " ")
                Dim EValue As String = Sequence.Score.evalue

                Call row.AddRange(New String() {objId, siteId, RegulatorId, EValue, Description})

                rowCollection(i) = row
            Next

            Return rowCollection
        End Function

        Public Shared Function ExportMotifs(MastOutputList As String()) As File
            Dim XmlFiles = (From FilePath As String In MastOutputList Select FilePath.LoadXml(Of MAST)()).ToArray
            Dim LQuery = (From File In XmlFiles Let rows = File.Convert Where Not rows Is Nothing Select rows).ToArray
            Dim out As File = New File
            Call out.AppendLine(New String() {"", "AccessionId", "SiteGuid", "RegulatorGuid", "E-Value", "Description", "SiteSequenceData", "RegulatorSequence"})
            For Each rowCollection As RowObject() In LQuery
                Call out.AppendRange(rowCollection)
            Next

            Return out
        End Function
    End Class
End Namespace
