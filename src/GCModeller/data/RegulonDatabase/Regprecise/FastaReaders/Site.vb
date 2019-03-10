#Region "Microsoft.VisualBasic::f098f1b498c439c0c51b02292ae23ea2, data\RegulonDatabase\Regprecise\FastaReaders\Site.vb"

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

    '     Class Site
    ' 
    '         Properties: Bacteria, geneLocusTag, geneVIMSSId, Headers, position
    '                     regulonId, score, SequenceData, uid
    ' 
    '         Function: BacteriaNameFromRegulog, CreateFrom, CreateObject, Load, ToFastaObject
    '                   ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Data.Regprecise.WebServices
Imports SMRUCC.genomics.SequenceModel

Namespace Regprecise.FastaReaders

    ''' <summary>
    ''' 调控位点的数据
    ''' > geneLocusTag:position|geneVIMSSId|regulonId|score|Bacteria
    ''' </summary>
    Public Class Site : Inherits FASTA.FastaSeq

#Region "Public Property"

        ''' <summary>
        ''' locus tag of a downstream gene in GeneBank
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property geneLocusTag As String
        ''' <summary>
        ''' identifier of a downstream gene in MicrobesOnline database.
        ''' (请注意这个是基因的编号，而非这个调控位点的编号，假若需要唯一确定一个调控位点，请使用locus_tag:position的组合) 
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property geneVIMSSId As Integer
        ''' <summary>
        ''' position of a regulatory site relative to the start of a downstream gene 
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property position As Integer
        ''' <summary>
        '''  identifier of regulon
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property regulonId As Integer
        ''' <summary>
        ''' score of a regualtory site
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property score As Double

        Public Property Bacteria As String

        ''' <summary>
        ''' Motif sequence
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Column("motif")> Public Overrides Property SequenceData As String

        Public Overrides Property Headers As String()
            Get
                Return {$"{geneLocusTag}:{position}", CStr(geneVIMSSId), CStr(regulonId), CStr(score), Bacteria}
            End Get
            Set(value As String())
                MyBase.Headers = value
            End Set
        End Property
#End Region

        Public ReadOnly Property uid As String
            Get
                Return $"{geneLocusTag}:{position}"
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{geneLocusTag}:{position}|{geneVIMSSId}|{regulonId}|{score}|{Bacteria}"
        End Function

        Public Shared Function CreateObject(FASTA As FASTA.FastaFile) As Site()
            Dim LQuery As Site() =
                LinqAPI.Exec(Of Site) <= From fa As FASTA.FastaSeq
                                         In FASTA
                                         Let attrs = fa.Headers
                                         Let loci As String() = attrs(Scan0).Split(":"c)
                                         Let site As Site = New Site With {
                                             .geneLocusTag = loci(Scan0),
                                             .position = CInt(Val(loci(1))),
                                             .geneVIMSSId = CInt(Val(attrs(1))),
                                             .regulonId = attrs(2),
                                             .score = Val(attrs(3)),
                                             .Bacteria = attrs(4),
                                             .SequenceData = fa.SequenceData
                                         }
                                         Select site
            Return LQuery
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="fasta">位点Fasta文件的文件路径</param>
        ''' <returns></returns>
        Public Overloads Shared Function Load(fasta As String) As Site()
            Dim FastaFile As FASTA.FastaFile = SequenceModel.FASTA.FastaFile.LoadNucleotideData(fasta)
            Return Site.CreateObject(FastaFile)
        End Function

        Public Shared Function CreateFrom(site As JSON.site, Bacteria As String) As Site
            Return New Site With {
                .Bacteria = Bacteria,
                .geneLocusTag = site.geneLocusTag,
                .geneVIMSSId = site.geneVIMSSId,
                .position = site.position,
                .regulonId = site.regulonId,
                .score = site.score,
                .SequenceData = Regtransbase.WebServices.Regulator.SequenceTrimming(site.sequence)
            }
        End Function

        Public Function ToFastaObject() As Regtransbase.WebServices.MotifFasta
            Return New Regtransbase.WebServices.MotifFasta With {
                .SequenceData = SequenceData,
                .position = position,
                .name = Me.geneVIMSSId,
                .locus_tag = Me.geneLocusTag,
                .Bacteria = Bacteria
            }
        End Function

        Public Shared Function BacteriaNameFromRegulog(Regulog As String) As String
            Return Strings.Split(Regulog.Split(CChar("=")).Last, " - ").Last.Trim
        End Function
    End Class
End Namespace
