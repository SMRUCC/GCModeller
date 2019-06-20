#Region "Microsoft.VisualBasic::cc69eea4f116643035d4b55cc8d43816, Bio.Assembly\Assembly\NCBI\Database\GenBank\GBK\File.vb"

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

    '     Class File
    ' 
    '         Properties: Accession, Comment, DbLinks, Definition, Features
    '                     HasSequenceData, IsPlasmidSource, IsWGS, Keywords, Locus
    '                     Origin, Reference, Source, SourceFeature, Taxon
    '                     Version
    ' 
    '         Function: __trims, IsValidGenbankFormat, Load, LoadDatabase, Read
    '                   (+2 Overloads) Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports r = System.Text.RegularExpressions.Regex

Namespace Assembly.NCBI.GenBank.GBFF

    ''' <summary>
    ''' NCBI GenBank database file.(NCBI GenBank数据库文件)
    ''' </summary>
    ''' <remarks></remarks>
    '''
    <Package("NCBI.Genbank.GBFF")>
    Public Class File : Implements ISaveHandle

        Public Property Comment As Keywords.COMMENT
        ''' <summary>
        ''' This GenBank keyword section stores the sequence data for this database.
        ''' </summary>
        ''' <returns></returns>
        Public Property Origin As Keywords.ORIGIN
        Public Property Features As Keywords.FEATURES.FEATURES
        ''' <summary>
        ''' LocusID, GI or AccessionID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Accession As Keywords.ACCESSION
        Public Property Reference As Keywords.REFERENCE
        ''' <summary>
        ''' The definition value for this organism's GenBank data.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Definition As Keywords.DEFINITION
        Public Property Version As Keywords.VERSION

        ''' <summary>
        ''' 物种信息
        ''' </summary>
        ''' <returns></returns>
        Public Property Source As Keywords.SOURCE
        ''' <summary>
        ''' The brief entry information of this genbank data.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Locus As Keywords.LOCUS
        Public Property Keywords As Keywords.KEYWORDS
        Public Property DbLinks As DBLINK

        ''' <summary>
        ''' 这个Genbank对象是否为一个质粒的基因组数据
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsPlasmidSource As Boolean
            Get
                Return Not String.IsNullOrEmpty(Features.source.Query("plasmid"))
            End Get
        End Property

        ''' <summary>
        ''' 物种数据
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Taxon As String
            Get
                Dim db_xref As String() = Features.source.QueryDuplicated("db_xref")
                Dim LQuery = LinqAPI.DefaultFirst(Of String) <=
                    From s As String
                    In db_xref
                    Let tokens As String() = s.Split(CChar(":"))
                    Where String.Equals(tokens.First, "taxon", StringComparison.OrdinalIgnoreCase)
                    Select tokens.Last

                Return LQuery
            End Get
        End Property

        ''' <summary>
        ''' 这个Genbank对象是否具有序列数据
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property HasSequenceData As Boolean
            Get
                If Origin Is Nothing Then
                    Return False
                Else
                    Return Not String.IsNullOrEmpty(Origin.SequenceData)
                End If
            End Get
        End Property

        ''' <summary>
        ''' This GenBank data is the WGS(Whole genome shotgun) type data.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsWGS As Boolean
            Get
                For Each KeyWord As String In GenBank.GBFF.Keywords.DEFINITION.WGSKeywords
                    If InStr(Definition.Value, KeyWord, CompareMethod.Text) > 0 Then
                        Return True
                    End If

                    For Each s As String In Keywords
                        If InStr(s, KeyWord) = 1 Then
                            Return True
                        End If
                    Next
                Next

                Return False
            End Get
        End Property

        ''' <summary>
        ''' Gets the original source brief entry information of this genome.(获取这个基因组的摘要信息)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property SourceFeature As Feature
            Get
                Return Features.source
            End Get
        End Property

        ''' <summary>
        ''' Read the gene nucleic acid sequence of a gene feature and then returns a fasta sequence object.
        ''' (读取一个基因特性的核酸序列，该Feature对象可以为任意形式的Qualifier的值，但是必需要具有Location属性)
        ''' </summary>
        ''' <param name="feature">The target feature site on the genome DNA sequence.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Read(feature As Feature) As FASTA.FastaSeq
            Dim left As Long = feature.Location.Locations.First.Left
            Dim right As Long = feature.Location.Locations.Last.Right
            Dim sequence As String = Mid(Origin, left, Math.Abs(left - right))

            If feature.Location.Complement Then
                sequence = (NucleicAcid.Complement(sequence))
            End If

            Dim fa As New FASTA.FastaSeq With {
                .SequenceData = sequence,
                .Headers = New String() {
                    "Feature",
                    feature.Location.ToString,
                    feature.KeyName
                }
            }

            Return fa
        End Function

        ''' <summary>
        ''' Read a specific GenBank database text file.
        ''' (读取一个特定的GenBank数据库文件)
        ''' </summary>
        ''' <param name="Path">The target database text file to read.(所要读取的目标数据库文件)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Shadows Widening Operator CType(Path As String) As NCBI.GenBank.GBFF.File
            Return GbkParser.Read(Path)
        End Operator

        ''' <summary>
        ''' 当发生错误的时候，会返回空值
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <ExportAPI("Load")>
        Public Shared Function Load(path As String) As NCBI.GenBank.GBFF.File
            Try
                Return GbkParser.Read(path)
            Catch ex As Exception
                ex = New Exception(path.ToFileURL, ex)
                Call ex.PrintException
                Return App.LogException(ex)
            End Try
        End Function

        ''' <summary>
        ''' 检查目标文件是否为Genbank文件格式
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Shared Function IsValidGenbankFormat(path As String) As Boolean
            Static samplingHeaders As Index(Of String) = {
                "LOCUS", "DEFINITION", "ACCESSION", "VERSION", "DBLINK"
            }

            Dim hits%

            For Each line As String In path.IterateAllLines.Take(samplingHeaders.Count)
                If line.Split.First Like samplingHeaders Then
                    hits += 1
                End If
            Next

            Return hits / samplingHeaders.Count >= 0.5
        End Function

        Public Const GenbankMultipleRecordDelimiterRegexp$ = "^//$"
        Public Const GenbankMultipleRecordDelimiter$ = "//"

        ''' <summary>
        ''' 假若一个gbk文件之中包含有多个记录的话，可以使用这个函数进行数据的加载，多个genebank记录在一个文件之中
        ''' 一般出现在细菌具有染色体基因组和质粒基因组这种多个复制子的情况
        ''' </summary>
        ''' <param name="filePath">The file path of the genbank database file, this gb file may contains sevral gb sections</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <ExportAPI("Load.DbList", Info:="Using this function to load the ncbi genbank database file if the database file contains more than one genome.")>
        Public Shared Iterator Function LoadDatabase(filePath As String) As IEnumerable(Of File)
            Dim data As String = FileIO.FileSystem.ReadAllText(filePath)
            Dim parts$() = r.Split(data, GenbankMultipleRecordDelimiterRegexp, RegexOptions.Multiline)
            Dim sBuf As IEnumerable(Of String()) =
 _
                From s As String
                In parts.AsParallel
                Let ss As String = s & vbCrLf & GenbankMultipleRecordDelimiter
                Select ss.LineTokens

            Try
                For Each buf As String() In sBuf
                    Dim sDat As String() = __trims(buf)

                    If sDat.IsNullOrEmpty Then
                        Continue For
                    Else
                        Yield __loadData(sDat, filePath)
                    End If
                Next
            Catch ex As Exception
                ex = New Exception(filePath, ex)
                Throw ex
            End Try
        End Function

        Private Shared Function __trims(buf As String()) As String()
            Dim i As Integer = 0

            If buf.Length < 5 Then
                Return Nothing
            End If

            Do While String.IsNullOrEmpty(buf.Read(i))
            Loop

            If i = 1 Then
                Return buf
            Else
                i -= 1
            End If

            buf = buf.Skip(i).ToArray
            Return buf
        End Function

        Public Function Save(FilePath As String, Encoding As Encoding) As Boolean Implements ISaveHandle.Save
            Return GbkWriter.WriteGenbank(Me, FilePath, Encoding)
        End Function

        Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(path, encoding.CodePage)
        End Function
    End Class
End Namespace
