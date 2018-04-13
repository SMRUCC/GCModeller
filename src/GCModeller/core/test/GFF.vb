#Region "Microsoft.VisualBasic::bc37200a13b1103c5d473fe9af6c9515, core\test\GFF.vb"

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

'Namespace Assembly.NCBI.GenBank.PttGenomeBrief.GenomeFeature

'    'http://www.sanger.ac.uk/resources/software/gff/spec.html

'    ''' <summary>
'    ''' GFF (General Feature Format) specifications document
'    ''' </summary>
'    Public Class GFF : Inherits LANS.SystemsBiology.ComponentModel.File

'#Region "Meta Data"

'        ''' <summary>
'        ''' gff-version   (##gff-version 2)
'        ''' 
'        ''' GFF version - In Case it Is a real success And we want To change it. The current Default version Is 2, 
'        ''' so If this line Is Not present version 2 Is assumed. 
'        ''' </summary>
'        ''' <returns></returns>
'        Public Property GffVersion As Integer

'        ''' <summary>
'        ''' source-version   (##source-version &lt;source> &lt;version text>)
'        ''' 
'        ''' So that people can record what version Of a program Or package was used To make the data In this file. 
'        ''' I suggest the version Is text without whitespace. That allows things Like 1.3, 4a etc. There should be 
'        ''' at most one source-version line per source.
'        ''' </summary>
'        ''' <returns></returns>
'        Public Property SrcVersion As String

'        ''' <summary>
'        ''' date    (##date &lt;date>)
'        ''' 
'        ''' The date the file was made, Or perhaps that the prediction programs were run. We suggest to use 
'        ''' astronomical format 1997-11-08 for 8th November 1997, first because these sort properly, And 
'        ''' second to avoid any US/European bias. 
'        ''' </summary>
'        ''' <returns></returns>
'        Public Property [Date] As String

'        ''' <summary>
'        ''' type   (##Type &lt;type> [&lt;seqname>])
'        ''' 
'        ''' The type Of host sequence described by the features. Standard types are 'DNA', 'Protein' and 'RNA'. 
'        ''' The optional &lt;seqname> allows multiple ##Type definitions describing multiple GFF sets in one file, 
'        ''' each of which have a distinct type. If the name is not provided, then all the features in the file 
'        ''' are of the given type. Thus, with this meta-comment, a single file could contain DNA, RNA and 
'        ''' Protein features, for example, representing a single genomic locus or 'gene', alongside type-specific 
'        ''' features of its transcribed mRNA and translated protein sequences. If no ##Type meta-comment is 
'        ''' provided for a given GFF file, then the type is assumed to be DNA. 
'        ''' </summary>
'        ''' <returns></returns>
'        Public Property Type As String

'        ''' <summary>
'        ''' DNA 
'        ''' 
'        ''' (##DNA &lt;seqname>
'        '''  ##acggctcggattggcgctggatgatagatcagacgac
'        '''  ##...
'        '''  ##End-DNA)
'        ''' 
'        ''' To give a DNA sequence. Several people have pointed out that it may be convenient to include the sequence in the file. It should Not become mandatory to do so, And in our experience this has been very little used. Often the seqname will be a well-known identifier, And the sequence can easily be retrieved from a database, Or an accompanying file.
'        ''' </summary>
'        ''' <returns></returns>
'        Public Property DNA As String

'        ''' <summary>
'        ''' RNA 
'        ''' 
'        ''' (##RNA &lt;seqname>
'        '''  ##acggcucggauuggcgcuggaugauagaucagacgac
'        '''  ##...
'        '''  ##End-RNA)
'        ''' 
'        ''' Similar to DNA. Creates an implicit ##Type RNA &lt;seqname> directive.
'        ''' </summary>
'        ''' <returns></returns>
'        Public Property RNA As String

'        ''' <summary>
'        ''' Protein
'        ''' 
'        ''' (##Protein &lt;seqname>
'        '''
'        '''  ##MVLSPADKTNVKAAWGKVGAHAGEYGAEALERMFLSF
'        '''  ##...
'        '''  ##End-Protein)
'        ''' 
'        ''' Similar to DNA. Creates an implicit ##Type Protein &lt;seqname> directive.
'        ''' </summary>
'        ''' <returns></returns>
'        Public Property Protein As String

'        ''' <summary>
'        ''' sequence-region  (##sequence-region &lt;seqname> &lt;start> &lt;end>)
'        ''' 
'        ''' To indicate that this file only contains entries for the specified subregion of a sequence.
'        ''' </summary>
'        ''' <returns></returns>
'        Public Property SequenceRegion As String
'#End Region

'        Public Property GenomeFeatures As Feature()

'        ''' <summary>
'        ''' Load a GFF (General Feature Format) specifications document file from a plant text file.
'        ''' (从一个指定的文本文件之中加载基因组特性片段的数据)
'        ''' </summary>
'        ''' <param name="Path"></param>
'        ''' <returns></returns>
'        Public Shared Function LoadDocument(Path As String) As GFF
'            Dim Text As String() = IO.File.ReadAllLines(Path)
'            Dim GFF As GFF = New GFF With {._FilePath = Path, .InternalFileData = Text}
'            Dim MetaValue As String() = TryGetMetaData(Text)

'            Call TrySetMetaData(MetaValue, GFF)

'            Dim Features = TryGetFreaturesData(Text, GFF.GffVersion)
'            GFF.GenomeFeatures = Features

'            Return GFF
'        End Function

'        Private Shared Sub TrySetMetaData(s_Data As String(), ByRef GFF As GFF)
'            Dim LQuery = (From Token As String In s_Data Let p As Integer = InStr(Token, " ") Let Name As String = Mid(Token, 1, p - 1) Let Value As String = Mid(Token, p + 1) Select Name, Value).ToArray
'            Dim hash = LQuery.ToDictionary(Function(obj) obj.Name.ToLower, elementSelector:=Function(obj) obj.Value)

'            GFF.GffVersion = CInt(Val(TryGetValue(hash, "##gff-version")))
'            GFF.Date = TryGetValue(hash, "##date")
'            GFF.SrcVersion = TryGetValue(hash, "##source-version")
'            GFF.Type = TryGetValue(hash, "##type")
'            GFF.SequenceRegion = TryGetValue(hash, "##sequence-region")

'        End Sub

'        ''' <summary>
'        ''' 
'        ''' </summary>
'        ''' <param name="hash"></param>
'        ''' <param name="Key">全部是小写字符</param>
'        ''' <returns></returns>
'        Private Shared Function TryGetValue(hash As Dictionary(Of String, String), Key As String) As String
'            If hash.ContainsKey(Key) Then
'                Return hash(Key)
'            Else
'                Return ""
'            End If
'        End Function

'        Private Shared Function TryGetFreaturesData(s_Data As String(), version As Integer) As Feature()
'            Dim p As Integer = 0
'            '  Dim sTemp As String = ""

'            Do While Not Mid(s_Data(p), 1, 2).Equals("##")
'                p += 1
'            Loop
'            Do While Mid(s_Data(p), 1, 2).Equals("##")
'                p += 1
'            Loop

'            Dim ChunkBuffer = s_Data.Skip(p).ToArray
'            Dim Features = (From sLine As String In ChunkBuffer Select Feature.CreateObject(sLine, version)).ToArray
'            Return Features
'        End Function

'        Private Shared Function TryGetMetaData(s_Data As String()) As String()
'            Dim LQuery = (From sLine As String In s_Data.AsParallel
'                          Where Len(sLine) > 2 AndAlso String.Equals(Mid(sLine, 1, 2), "##")
'                          Select sLine).ToArray
'            Return LQuery
'        End Function

'    End Class
'End Namespace
