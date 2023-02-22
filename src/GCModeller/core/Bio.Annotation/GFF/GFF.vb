#Region "Microsoft.VisualBasic::79e658fa400e9b34643bb7f6eaaee3ea, GCModeller\core\Bio.Annotation\GFF\GFF.vb"

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

'   Total Lines: 287
'    Code Lines: 137
' Comment Lines: 112
'   Blank Lines: 38
'     File Size: 12.18 KB


'     Class GFFTable
' 
'         Properties: [date], DNA, features, GffVersion, processor
'                     Protein, RNA, SeqRegion, Size, species
'                     SrcVersion, type
' 
'         Constructor: (+2 Overloads) Sub New
'         Function: __getStrandFeatures, GenerateDocument, GetByName, GetRelatedGenes, GetStrandFeatures
'                   LoadDocument, (+2 Overloads) Save, ToString
' 
' 
' /********************************************************************************/

#End Region

#If NET48 Then
Imports System.Data.Linq.Mapping
#End If
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ContextModel

Namespace Assembly.NCBI.GenBank.TabularFormat.GFF

    'http://www.sanger.ac.uk/resources/software/gff/spec.html

    ''' <summary>
    ''' GFF (General Feature Format) specifications document
    ''' </summary>
    Public Class GFFTable
        Implements ISaveHandle
        Implements IGenomicsContextProvider(Of Feature)

#Region "Meta Data"

        ''' <summary>
        ''' gff-version   (##gff-version 2)
        ''' 
        ''' GFF version - In Case it Is a real success And we want To change it. The current Default version Is 2, 
        ''' so If this line Is Not present version 2 Is assumed. 
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="##gff-version")> Public Property GffVersion As Integer

        ''' <summary>
        ''' source-version   (##source-version &lt;source> &lt;version text>)
        ''' 
        ''' So that people can record what version Of a program Or package was used To make the data In this file. 
        ''' I suggest the version Is text without whitespace. That allows things Like 1.3, 4a etc. There should be 
        ''' at most one source-version line per source.
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="##source-version")> Public Property SrcVersion As String

        ''' <summary>
        ''' date    (##date &lt;date>)
        ''' 
        ''' The date the file was made, Or perhaps that the prediction programs were run. We suggest to use 
        ''' astronomical format 1997-11-08 for 8th November 1997, first because these sort properly, And 
        ''' second to avoid any US/European bias. 
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="##date")> Public Property [date] As String

        ''' <summary>
        ''' type   (##Type &lt;type> [&lt;seqname>])
        ''' 
        ''' The type Of host sequence described by the features. Standard types are 'DNA', 'Protein' and 'RNA'. 
        ''' The optional &lt;seqname> allows multiple ##Type definitions describing multiple GFF sets in one file, 
        ''' each of which have a distinct type. If the name is not provided, then all the features in the file 
        ''' are of the given type. Thus, with this meta-comment, a single file could contain DNA, RNA and 
        ''' Protein features, for example, representing a single genomic locus or 'gene', alongside type-specific 
        ''' features of its transcribed mRNA and translated protein sequences. If no ##Type meta-comment is 
        ''' provided for a given GFF file, then the type is assumed to be DNA. 
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="##type")> Public Property type As String

        <Column(Name:="##species")> Public Property species As String
        ''' <summary>
        ''' 生成这个文件的应用程序
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="#!processor")> Public Property processor As String

        ''' <summary>
        ''' DNA 
        ''' 
        ''' (##DNA &lt;seqname>
        '''  ##acggctcggattggcgctggatgatagatcagacgac
        '''  ##...
        '''  ##End-DNA)
        ''' 
        ''' To give a DNA sequence. Several people have pointed out that it may be convenient to include the sequence in the file. 
        ''' It should Not become mandatory to do so, And in our experience this has been very little used. Often the seqname will 
        ''' be a well-known identifier, And the sequence can easily be retrieved from a database, Or an accompanying file.
        ''' </summary>
        ''' <returns></returns>
        Public Property DNA As String

        ''' <summary>
        ''' RNA 
        ''' 
        ''' (##RNA &lt;seqname>
        '''  ##acggcucggauuggcgcuggaugauagaucagacgac
        '''  ##...
        '''  ##End-RNA)
        ''' 
        ''' Similar to DNA. Creates an implicit ##Type RNA &lt;seqname> directive.
        ''' </summary>
        ''' <returns></returns>
        Public Property RNA As String

        ''' <summary>
        ''' Protein
        ''' 
        ''' (##Protein &lt;seqname>
        '''
        '''  ##MVLSPADKTNVKAAWGKVGAHAGEYGAEALERMFLSF
        '''  ##...
        '''  ##End-Protein)
        ''' 
        ''' Similar to DNA. Creates an implicit ##Type Protein &lt;seqname> directive.
        ''' </summary>
        ''' <returns></returns>
        Public Property Protein As String

        ''' <summary>
        ''' sequence-region  (##sequence-region &lt;seqname> &lt;start> &lt;end>)
        ''' 
        ''' To indicate that this file only contains entries for the specified subregion of a sequence.
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="##sequence-region")> Public Property SeqRegion As SeqRegion
#End Region

        ''' <summary>
        ''' Genome size
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Size As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return SeqRegion.ends
            End Get
        End Property

        ''' <summary>
        ''' 基因组上面的特性位点
        ''' </summary>
        ''' <returns></returns>
        Public Property features As Feature() Implements IGenomicsContextProvider(Of Feature).AllFeatures
            Get
                Return _features
            End Get
            Set(value As Feature())
                _features = value
                _forwards = __getStrandFeatures(Strands.Forward)
                _reversed = __getStrandFeatures(Strands.Reverse)
                _contextModel = New GenomeContextProvider(Of Feature)(Me)
            End Set
        End Property

        Default Public ReadOnly Property Feature(locus_tag As String) As Feature Implements IGenomicsContextProvider(Of Feature).Feature
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return GetByName(locus_tag)
            End Get
        End Property

        Dim _features As Feature()
        Dim _forwards As Feature()
        Dim _reversed As Feature()
        Dim _contextModel As GenomeContextProvider(Of Feature)

        Sub New()
        End Sub

        ''' <summary>
        ''' Copy specific type
        ''' </summary>
        ''' <param name="gff"></param>
        ''' <param name="type"></param>
        Sub New(gff As GFFTable, type As Features)
            Me.date = gff.date
            Me.DNA = gff.DNA
            Me.Features = gff.GetsAllFeatures(type)
            Me.GffVersion = gff.GffVersion
            Me.Protein = gff.Protein
            Me.RNA = gff.RNA
            Me.SeqRegion = gff.SeqRegion
            Me.SrcVersion = gff.SrcVersion
            Me.type = gff.type
        End Sub

        Public Overrides Function ToString() As String
            Return SeqRegion.GetJson
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Name"><see cref="feature.attributes"/> -> name</param>
        ''' <returns></returns>
        Public Function GetByName(Name As String) As Feature Implements IGenomicsContextProvider(Of Feature).GetByName
            Dim LQuery = From Feature As Feature
                         In Me.Features
                         Where Feature.attributes.ContainsKey("name") AndAlso
                             String.Equals(Feature.attributes("name"), Name, StringComparison.OrdinalIgnoreCase)
                         Select Feature
            Return LQuery.FirstOrDefault
        End Function

        Shared ReadOnly metaAttrs As BindProperty(Of ColumnAttribute)() = (
            From p As PropertyInfo
            In GetType(GFFTable).GetProperties(BindingFlags.Public Or BindingFlags.Instance)
            Let attrs As Object() = p.GetCustomAttributes(attributeType:=GetType(ColumnAttribute), inherit:=True)
            Where Not attrs.IsNullOrEmpty
            Let name As ColumnAttribute = DirectCast(attrs.First, ColumnAttribute)
            Select New BindProperty(Of ColumnAttribute)(name, p, Function(a) a.Name)
        ).ToArray

        Public Function GenerateDocument() As String
            Dim sb As New StringBuilder()
            Dim features$() = Me.Features _
                .Select(AddressOf FeatureParser.ToString) _
                .ToArray

            Me.processor = "SMRUCC\GCModeller"

            For Each [property] In metaAttrs
                Dim value As Object = [property].GetValue(Me)
                Dim str As String = Scripting.ToString(value)

                If String.IsNullOrEmpty(str) Then
                    Continue For
                End If

                Call sb.AppendLine($"{[property].GetColumnName} {str}")
            Next

            Call sb.AppendLine(features.JoinBy(ASCII.LF))
            Call sb.AppendLine("###")

            Return sb.ToString
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Save(Path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
            Return GenerateDocument.SaveTo(Path, encoding)
        End Function

        ''' <summary>
        ''' Load a GFF (General Feature Format) specifications document file from a plant text file.
        ''' (从一个指定的文本文件之中加载基因组特性片段的数据)
        ''' </summary>
        ''' <param name="path"></param>
        ''' <param name="defaultVersion">
        ''' 当GFF的文件头部之中没有包含有版本字样的时候，所使用的的默认版本号，默认是版本3
        ''' </param>
        ''' <returns></returns>
        Public Shared Function LoadDocument(path$, Optional defaultVersion% = 3) As GFFTable
            Dim text As String() = path.ReadAllLines
            Dim gff As New GFFTable

            Call Document.TrySetMetaData(text, gff, defaultVer:=defaultVersion)
            Call Linq.SetValue(Of GFFTable).InvokeSet(gff, NameOf(gff.features), Document.TryGetFreaturesData(text, gff.GffVersion))
            Call $"There are {gff.Features.Length} genome features exists in the gff file: {path.ToFileURL}".__DEBUG_ECHO

            Return gff
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetRelatedGenes(loci As NucleotideLocation,
                                        Optional unstrand As Boolean = False,
                                        Optional ATGDist As Integer = 500) As Relationship(Of Feature)() Implements IGenomicsContextProvider(Of Feature).GetRelatedGenes

            Dim relates As Relationship(Of Feature)() = _contextModel.GetAroundRelated(loci, Not unstrand, ATGDist)
            Return relates
        End Function

        Private Function __getStrandFeatures(strand As Strands) As Feature()
            Return (From x As Feature In Features Where x.strand = strand Select x).ToArray
        End Function

        Public Function GetStrandFeatures(strand As Strands) As Feature() Implements IGenomicsContextProvider(Of Feature).GetStrandFeatures
            Return If(strand = Strands.Forward, _forwards, _reversed)
        End Function

        Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(path, encoding.CodePage)
        End Function
    End Class
End Namespace
