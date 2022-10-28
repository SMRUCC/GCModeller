#Region "Microsoft.VisualBasic::ef14f29742b6a8284bd799c3c7f8c579, Bio.Repository\NCBI\Genbank.vb"

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

' Module Installer
' 
'     Function: BuildLocusHash, BuildNameHash, GetsiRNATargetSeqs, Install
' 
' Class Genbank
' 
'     Properties: DIR
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: __entryQuery, __query, AddNew, Exists, GetAll
'               GetByKey, GetWhere, (+3 Overloads) Query
' 
'     Sub: AddOrUpdate, Delete
' 
' Class GeneInfo
' 
'     Properties: [function], accId, locus_tag, name
' 
'     Constructor: (+2 Overloads) Sub New
'     Function: NameEquals, ToString
' 
' Class GenbankIndex
' 
'     Properties: AccId, definition, DIR, genome
' 
'     Function: Gbk, ToString
' 
' /********************************************************************************/

#End Region

#If NETCOREAPP Then
Imports System.Data
#End If
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.SequenceModel

Public Module Installer

    ''' <summary>
    ''' 这个函数主要是进行创建数据库的索引文件
    ''' </summary>
    ''' <param name="DIR">Extract location of file: all.gbk.tar.gz from NCBI FTP website.</param>
    ''' <returns></returns>
    Public Function Install(DIR As String, refresh As Boolean) As Boolean
        Dim index As String = $"{DIR}/.genbank/{Genbank.IndexJournal}"

        If refresh Then
            Call "".SaveTo(index)  ' 清除原始文件的所有数据，重新创建索引文件
        End If

        Using DbWriter As New WriteStream(Of GenbankIndex)(index)  ' 打开数据库的文件句柄

            For Each table As String In ls - l - lsDIR - r <= DIR   ' 一个物种的文件夹
                Dim path As String = $"{DIR}/.genbank/meta/{table.BaseName}.csv"

                If path.FileLength > 0 Then
                    If Not refresh Then
                        Continue For
                    End If
                End If

                Dim genes As New List(Of GeneInfo)

                For Each gbk As String In ls - l - r - wildcards("*.gbk", "*.gb") <= table
                    For Each gb As GBFF.File In GBFF.File.LoadDatabase(gbk)  ' 使用迭代器读取数据库文件

                        Dim idx As New GenbankIndex With {
                            .AccId = gb.Locus.AccessionID,
                            .definition = gb.Definition.Value,
                            .DIR = table.BaseName,
                            .genome = gb.Source.SpeciesName
                        }
                        Call DbWriter.Flush(idx)   ' 将对象写入内存缓存，进入队列等待回写入文件系统

                        genes += gb.GbffToPTT(ORF:=True).GeneObjects.Select(Function(g) New GeneInfo(g, idx.AccId))
                    Next
                Next

                Call genes.SaveTo(path)
            Next
        End Using

        Return True
    End Function

    ''' <summary>
    ''' key: <see cref="GeneInfo.name"/>, <see cref="GeneInfo.locus_tag"/>
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="x"></param>
    ''' <returns></returns>
    <Extension>
    Public Function BuildNameHash(source As IEnumerable(Of GeneInfo), x As GenbankIndex) As IReadOnlyDictionary(Of String, GeneInfo)
        Dim hash As New Dictionary(Of String, GeneInfo)

        For Each gene As GeneInfo In source
            If Not String.Equals(gene.accId, x.AccId, StringComparison.OrdinalIgnoreCase) Then
                Continue For
            End If
            Dim key As String = gene.name
            If String.IsNullOrEmpty(key) OrElse String.Equals(key, "-") Then
                key = gene.locus_tag
            End If
            Call hash.Add(key, gene)
        Next

        Return hash
    End Function

    ''' <summary>
    ''' key: <see cref="GeneInfo.locus_tag"/>
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="x"></param>
    ''' <returns></returns>
    <Extension>
    Public Function BuildLocusHash(source As IEnumerable(Of GeneInfo), x As GenbankIndex) As IReadOnlyDictionary(Of String, GeneInfo)
        Dim gHash As Dictionary(Of GeneInfo) =
            (From g As GeneInfo
             In source
             Where String.Equals(g.accId, x.AccId, StringComparison.OrdinalIgnoreCase)
             Select g
             Group g By g.locus_tag Into Group) _
                  .Select(Function(o) o.Group.First).ToDictionary
        Return gHash
    End Function

    ''' <summary>
    ''' Query target nt sequence
    ''' </summary>
    ''' <param name="siRNAtarget">
    ''' 这个应该是通过对<see cref="Bac_sRNA.org.Interaction.Organism"/>Group之后所得到的数据
    ''' </param>
    ''' <param name="repo"></param>
    ''' <returns></returns>
    Public Function GetsiRNATargetSeqs(siRNAtarget As IEnumerable(Of Bac_sRNA.org.Interaction), repo As Genbank) As IEnumerable(Of FASTA.FastaSeq)
        Dim source = siRNAtarget.ToArray
        Dim index As GenbankIndex = repo.Query(source)

        If index Is Nothing Then
            Return Nothing
        Else
            Dim gbkk As GBFF.File = index.Gbk(repo.DIR)
            Dim genes As FASTA.FastaFile = gbkk.ExportGeneNtFasta(geneName:=True)
            Dim hash As Dictionary(Of String, FASTA.FastaSeq) =
                genes.ToDictionary(Function(x) x.Headers.First)

            Return From itr As Bac_sRNA.org.Interaction
                   In source
                   Where hash.ContainsKey(itr.TargetName)
                   Select hash(itr.TargetName)
        End If
    End Function
End Module

''' <summary>
''' NCBI genbank repository system.(请注意，这个对象里面的所有的Repository实体都是使用genbank编号来作为Key的)
''' </summary>
Public Class Genbank
    Implements IRepository(Of String, GenbankIndex)

    ''' <summary>
    ''' The index database file..
    ''' </summary>
    Public Const IndexJournal As String = "ncbi_genbank.csv"

    ''' <summary>
    ''' The genbank repository root directory.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property DIR As String

    ReadOnly __indexHash As Dictionary(Of GenbankIndex)

    Sub New(DIR As String)
        Dim index As String = $"{DIR}/.genbank/{IndexJournal}"

        Me.DIR = DIR
        Me.__indexHash = index.LoadCsv(Of GenbankIndex).ToDictionary
    End Sub

#Region "Implements IRepository(Of String, GenbankIndex)"

    ''' <summary>
    ''' 查询出gbk的文件路径，这个主要是为了RegPrecise查询使用的
    ''' </summary>
    ''' <param name="genome"></param>
    ''' <param name="locus"></param>
    ''' <returns></returns>
    Public Function Query(genome As String, locus As IEnumerable(Of String)) As GenbankIndex
        Return __query(genome, locus, AddressOf BuildLocusHash)
    End Function

    Private Function __query(genome As String,
                             locus As IEnumerable(Of String),
                             hash As Func(Of
                             IEnumerable(Of GeneInfo),
                             GenbankIndex,
                             IReadOnlyDictionary(Of String, GeneInfo))) As GenbankIndex
        For Each x As GenbankIndex In __entryQuery(genome)
            Dim path As String = $"{DIR}/.genbank/meta/{x.DIR}.Csv"
            Dim genes As IEnumerable(Of GeneInfo) = path.LoadCsv(Of GeneInfo)
            Dim ghash As IReadOnlyDictionary(Of String, GeneInfo) = hash(genes, x)

            For Each sid As String In locus
                If ghash.ContainsKey(sid) Then
                    Return x
                End If
            Next
        Next

        Return Nothing
    End Function

    Private Function __entryQuery(genomeName As String) As IEnumerable(Of GenbankIndex)
        Dim LQuery = (From x As GenbankIndex
                      In __indexHash.Values.AsParallel
                      Let edits As DistResult = LevenshteinDistance.ComputeDistance(genomeName, x.genome)
                      Where Not edits Is Nothing
                      Select x,
                          edits
                      Order By edits.MatchSimilarity Descending).Take(10)
        Return LQuery.Select(Function(x) x.x)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="siRNAtarget">
    ''' 这个应该是通过对<see cref="Bac_sRNA.org.Interaction.Organism"/>Group之后所得到的数据
    ''' </param>
    ''' <returns></returns>
    Public Function Query(siRNAtarget As IEnumerable(Of Bac_sRNA.org.Interaction)) As GenbankIndex
        Dim names As IEnumerable(Of String) = siRNAtarget.Select(Function(x) x.TargetName)
        Return __query(siRNAtarget.First.Organism, names, AddressOf BuildNameHash)
    End Function

    Public Function Query(source As WebServices.QuerySource) As GenbankIndex
        Return Query(source.genome, source.locusId)
    End Function

    Public Function Exists(key As String) As Boolean Implements IRepositoryRead(Of String, GenbankIndex).Exists
        Return __indexHash.ContainsKey(key)
    End Function

    Public Function GetByKey(key As String) As GenbankIndex Implements IRepositoryRead(Of String, GenbankIndex).GetByKey
        If __indexHash.ContainsKey(key) Then
            Return __indexHash(key)
        Else
            Return Nothing
        End If
    End Function

    Public Function GetWhere(clause As Func(Of GenbankIndex, Boolean)) As IReadOnlyDictionary(Of String, GenbankIndex) Implements IRepositoryRead(Of String, GenbankIndex).GetWhere
        Dim LQuery As IEnumerable(Of GenbankIndex) =
            From x As GenbankIndex
            In __indexHash.Values
            Where True = clause(x)
            Select x
        Return LQuery.ToDictionary(Function(x) x.AccId)
    End Function

    Public Function GetAll() As IReadOnlyDictionary(Of String, GenbankIndex) Implements IRepositoryRead(Of String, GenbankIndex).GetAll
        Return New Dictionary(Of String, GenbankIndex)(__indexHash)
    End Function

    Public Sub Delete(key As String) Implements IRepositoryWrite(Of String, GenbankIndex).Delete
        Throw New ReadOnlyException(ReadOnlyRepository)
    End Sub

    Public Sub AddOrUpdate(entity As GenbankIndex, key As String) Implements IRepositoryWrite(Of String, GenbankIndex).AddOrUpdate
        Throw New ReadOnlyException(ReadOnlyRepository)
    End Sub

    Const ReadOnlyRepository As String = "This repository is readonly, please updates from method: Installer.Install"

    Public Function AddNew(entity As GenbankIndex) As String Implements IRepositoryWrite(Of String, GenbankIndex).AddNew
        Throw New ReadOnlyException(ReadOnlyRepository)
    End Function
#End Region
End Class

Public Class GeneInfo
    Implements IKeyedEntity(Of String), INamedValue

    ''' <summary>
    ''' 基因组的编号
    ''' </summary>
    ''' <returns></returns>
    Public Property accId As String
    ''' <summary>
    ''' 基因的编号
    ''' </summary>
    ''' <returns></returns>
    Public Property locus_tag As String Implements IKeyedEntity(Of String).Key
    ''' <summary>
    ''' /gene="基因名"
    ''' </summary>
    ''' <returns></returns>
    Public Property name As String
    Public Property [function] As String

    Sub New(g As GeneBrief, acc As String)
        locus_tag = g.Synonym
        [function] = g.Product
        accId = acc
        name = g.Gene
    End Sub

    Sub New()
    End Sub

    Public Function NameEquals(name As String) As Boolean
        Return String.Equals(name, Me.name, StringComparison.OrdinalIgnoreCase) OrElse
            String.Equals(name, locus_tag, StringComparison.OrdinalIgnoreCase)
    End Function

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class

Public Class GenbankIndex : Implements IKeyedEntity(Of String), INamedValue

    ''' <summary>
    ''' DIR name
    ''' </summary>
    ''' <returns></returns>
    Public Property DIR As String
    Public Property genome As String
    ''' <summary>
    ''' locus_tag, 索引文件的表主键
    ''' </summary>
    ''' <returns></returns>
    Public Property AccId As String Implements INamedValue.Key
    Public Property definition As String

    Public Function Gbk(DIR As String) As GBFF.File
        Dim path As String = $"{DIR}/{Me.DIR}/"
        Dim files = (ls - l - r - wildcards("*.gb", "*.gbk") <= path).ToArray

        If files.IsNullOrEmpty Then
            Return Nothing
        Else
            Dim LQuery As String = (From file As String
                                    In files
                                    Let name As String = file.BaseName
                                    Where InStr(name, AccId, CompareMethod.Text) = 1
                                    Select file).FirstOrDefault

            If String.IsNullOrEmpty(LQuery) Then
                Return Nothing
            Else
                Return GBFF.File.Load(path:=LQuery)
            End If
        End If
    End Function

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
