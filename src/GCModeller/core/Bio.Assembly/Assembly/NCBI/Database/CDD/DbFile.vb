#Region "Microsoft.VisualBasic::6461a11b2fc739651fe5a727c76dee24, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\CDD\DbFile.vb"

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

    '   Total Lines: 267
    '    Code Lines: 206
    ' Comment Lines: 36
    '   Blank Lines: 25
    '     File Size: 11.32 KB


    '     Class DbFile
    ' 
    '         Properties: BuildTime, FastaUrl, FilePath, Id, MimeType
    '                     SmpData
    ' 
    '         Function: ContainsId, ContainsId_p, (+2 Overloads) ExportFASTA, FindByTabId, PreLoad
    '                   (+2 Overloads) Save, Takes, ToString
    ' 
    '         Sub: (+2 Overloads) __buildDb, BuildDb
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.Utility
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.SequenceModel
Imports DIR = System.String

Namespace Assembly.NCBI.CDD

    ''' <summary>
    ''' CDD database builder.(CDD数据库构建工具)
    ''' </summary>
    ''' <remarks>
    ''' ftp://ftp.ncbi.nlm.nih.gov/pub/mmdb/cdd/cdd.tar.gz
    ''' </remarks>
    ''' 
    <XmlRoot("CDD.DB_File", Namespace:="http://code.google.com/p/genome-in-code/ncbi/cdd")>
    <Package("NCBI.CDD", Url:="ftp://ftp.ncbi.nlm.nih.gov/pub/mmdb/cdd/cdd.tar.gz",
                      Category:=APICategories.ResearchTools,
                      Description:="CDD database builder.",
                      Publisher:="xie.guigang@gmail.com")>
    <Cite(Title:="CDD: a Conserved Domain Database for protein classification", Volume:=33, Year:=2005,
          Journal:="Nucleic Acids Res",
          Keywords:="Amino Acid Sequence
Conserved Sequence
*Databases, Protein
Phylogeny
*Protein Structure, Tertiary
Proteins/*classification
Sequence Alignment
Sequence Analysis, Protein
User-Computer Interface",
          Authors:="Marchler-Bauer, A.
Anderson, J. B.
Cherukuri, P. F.
DeWeese-Scott, C.
Geer, L. Y.
Gwadz, M.
He, S.
Hurwitz, D. I.
Jackson, J. D.
Ke, Z.
Lanczycki, C. J.
Liebert, C. A.
Liu, C.
Lu, F.
Marchler, G. H.
Mullokandov, M.
Shoemaker, B. A.
Simonyan, V.
Song, J. S.
Thiessen, P. A.
Yamashita, R. A.
Yin, J. J.
Zhang, D.
Bryant, S. H.",
          DOI:="10.1093/nar/gki069",
          ISSN:="1362-4962 (Electronic)
0305-1048 (Linking)",
          Abstract:="The Conserved Domain Database (CDD) is the protein classification component of NCBI's Entrez query and retrieval system. CDD is linked to other Entrez databases such as Proteins, Taxonomy and PubMed, and can be accessed at http://www.ncbi.nlm.nih.gov/entrez/query.fcgi?db=cdd. 
CD-Search, which is available at http://www.ncbi.nlm.nih.gov/Structure/cdd/wrpsb.cgi, is a fast, interactive tool to identify conserved domains in new protein sequences. 
CD-Search results for protein sequences in Entrez are pre-computed to provide links between proteins and domain models, and computational annotation visible upon request. 
Protein-protein queries submitted to NCBI's BLAST search service at http://www.ncbi.nlm.nih.gov/BLAST are scanned for the presence of conserved domains by default. 
While CDD started out as essentially a mirror of publicly available domain alignment collections, such as SMART, Pfam and COG, we have continued an effort to update, and in some cases replace these models with domain hierarchies curated at the NCBI. 
Here, we report on the progress of the curation effort and associated improvements in the functionality of the CDD information retrieval system.",
          AuthorAddress:="National Center for Biotechnology Information, National Library of Medicine, National Institutes of Health, Building 38 A, Room 8N805, 8600 Rockville Pike, Bethesda, MD 20894, USA. bauer@ncbi.nlm.nih.gov",
          Issue:="Database issue",
          Pages:="D192-6",
          PubMed:=15608175)>
    Public Class DbFile : Implements ISaveHandle, IFileReference

        Dim _innerDict As Dictionary(Of String, CDD.SmpFile)

        <XmlElement> Public Property SmpData As CDD.SmpFile()
            Get
                If Not _innerDict Is Nothing Then
                    Return _innerDict.Values.ToArray
                Else
                    Return New SmpFile() {}
                End If
            End Get
            Set(value As CDD.SmpFile())
                If Not value.IsNullOrEmpty Then
                    _innerDict = value.ToDictionary(Function(o As CDD.SmpFile) o.Name)
                Else
                    Call $"Null database entries!".__DEBUG_ECHO
                    _innerDict = New Dictionary(Of DIR, SmpFile)
                End If
            End Set
        End Property

        <XmlAttribute> Public Property Id As String
        <XmlAttribute> Public Property BuildTime As String

        Public Function FindByTabId(strTagId As String) As CDD.SmpFile
            Dim TagId As Integer = Val(strTagId)
            Dim LQuery = (From smp As SmpFile
                          In SmpData
                          Where TagId = smp.ID
                          Select smp).FirstOrDefault
            Return LQuery
        End Function

        ''' <summary>
        ''' 不存在则返回空值
        ''' </summary>
        ''' <param name="Id"></param>
        ''' <returns></returns>
        Default Public ReadOnly Property Smp(Id As String) As CDD.SmpFile
            Get
                Return _innerDict.TryGetValue(Id)
            End Get
        End Property

        Public ReadOnly Property FastaUrl As String
            Get
                Return FilePath.Replace(".xml", ".fasta")
            End Get
        End Property

        Public Property FilePath As String Implements IFileReference.FilePath

        Public ReadOnly Property MimeType As ContentType() Implements IFileReference.MimeType
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        ''' <summary>
        ''' 非并行版本的<see cref="CDD.SmpFile.Name">AccessionId</see>, <see cref="CDD.SmpFile.Id">TagId</see>, <see cref="CDD.SmpFile.CommonName">CommonName</see>
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ContainsId(id As String) As CDD.SmpFile
            If _innerDict.ContainsKey(id) Then
                Return _innerDict(id)
            End If

            Return (From item In _innerDict.Values
                    Where String.Equals(item.CommonName, id) OrElse
                        String.Equals(item.ID.ToString, id)
                    Select item).FirstOrDefault
        End Function

        ''' <summary>
        ''' 并行版本的
        ''' </summary>
        ''' <param name="Id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ContainsId_p(Id As String) As CDD.SmpFile
            If _innerDict.ContainsKey(Id) Then
                Return _innerDict(Id)
            End If

            Return (From smp As SmpFile
                    In _innerDict.Values.AsParallel
                    Where String.Equals(smp.CommonName, Id) OrElse
                        String.Equals(smp.ID.ToString, Id)
                    Select smp).FirstOrDefault
        End Function

        Private Shared Sub __buildDb(DIR As String, EXPORT As String)
            For Each Pn As Pn In PreLoad(DIR)
                Call $"> Build {Pn.FilePath.ToFileURL}".__DEBUG_ECHO
                Call __buildDb(Pn, EXPORT)
            Next
        End Sub

        Private Shared Sub __buildDb(pn As Pn, EXPORT As String)
            Dim File As String = String.Format("{0}/{1}", EXPORT, pn.ToString.Split(CChar("/")).Last.Replace(".pn", String.Empty))
            Dim FASTA As String = File & ".fasta"
            Dim LQuery = From FilePath As String
                         In pn.AsParallel
                         Where FileIO.FileSystem.FileExists(FilePath)
                         Let SmpFile As CDD.SmpFile = CDD.SmpFile.Load(FilePath)
                         Select SmpFile Order By SmpFile.Name
                         Ascending  '..AsParallel
            Dim DbFile As CDD.DbFile = New DbFile With {
                .FilePath = File & ".xml",
                .Id = pn.FilePath.Split(CChar("/")).Last,
                .SmpData = LQuery.ToArray,
                .BuildTime = Now.ToString
            }

            Call $" EXPORT fasta sequence data {FASTA}".__DEBUG_ECHO
            Call CType((From Smp As SmpFile
                        In DbFile.SmpData.AsParallel
                        Let Fsa As FASTA.FastaSeq = Smp.EXPORT
                        Select Fsa).ToArray, FASTA.FastaFile).Save(FASTA, Encodings.UTF8)
            Call DbFile.GetXml.SaveTo(DbFile.FilePath)
        End Sub

        <ExportAPI("Db.Build")>
        Public Shared Sub BuildDb(DIR As String, EXPORT As DIR)
            Using busy As New CBusyIndicator
                Call FileIO.FileSystem.CreateDirectory(EXPORT)
                Call busy.Start()
                Call __buildDb(DIR, EXPORT)
            End Using
        End Sub

        Public Overloads Function ExportFASTA() As FASTA.FastaFile
            Dim Fasta As FASTA.FastaFile = ExportFASTA(Me)
            Call Fasta.Save(Me.FastaUrl, Encodings.UTF8)
            Return Fasta
        End Function

        <ExportAPI("Fasta.Export")>
        Public Overloads Shared Function ExportFASTA(Db As DbFile) As FASTA.FastaFile
            Dim LQuery = From smp As CDD.SmpFile
                         In Db.SmpData
                         Select smp.EXPORT '
            Dim Fasta As New FASTA.FastaFile(LQuery)
            Return Fasta
        End Function

        Public Overrides Function ToString() As String
            Return FilePath
        End Function

        ''' <summary>
        ''' 根据唯一标识符的集合来获取数据库记录数据
        ''' </summary>
        ''' <param name="lstAccId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Takes(lstAccId As IEnumerable(Of String)) As DbFile
            Dim Ids As String() = lstAccId.ToArray
            Dim LQuery = From smp As SmpFile
                         In Me.SmpData.AsParallel
                         Where Array.IndexOf(Ids, smp.Name) > -1
                         Select smp '
            Return New DbFile With {
                .SmpData = LQuery.ToArray
            }
        End Function

        ''' <summary>
        ''' 在编译整个CDD数据库之前进行预加载
        ''' </summary>
        ''' <param name="DIR"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Db.PreLoad")>
        Public Shared Function PreLoad(DIR As DIR) As Pn()
            Dim LQuery As IEnumerable(Of Pn) = From File As String
                                               In FileIO.FileSystem.GetFiles(DIR, FileIO.SearchOption.SearchTopLevelOnly, "*.pn")
                                               Select CType(File, Pn) '
            Return LQuery.ToArray
        End Function

        Public Function Save(FilePath As String, Encoding As Encoding) As Boolean Implements ISaveHandle.Save
            Return Me.GetXml.SaveTo(FilePath Or Me.FilePath.When(FilePath.StringEmpty), Encoding)
        End Function

        Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(path, encoding.CodePage)
        End Function
    End Class
End Namespace
