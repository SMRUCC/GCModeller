#Region "Microsoft.VisualBasic::be14490d298acd25da70c7434043c330, CLI_tools\S.M.A.R.T\DomainArchitecture\CompileDomains.vb"

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

    ' Class CompileDomains
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Compile, CreateProteinDescription, ExportDb, GetLastProcessData, Performance
    ' 
    ' Class SMARTDB
    ' 
    '     Properties: Proteins
    ' 
    '     Function: Export, Generate, Takes
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Scripting
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.NCBI.CDD
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.XmlFile
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.InteropService
Imports SMRUCC.genomics.ProteinModel
Imports SMRUCC.genomics.SequenceModel

Public Class CompileDomains

    Dim LocalBLAST As InteropService
    Dim CDD As CDDLoader
    Dim TempWorkspace As String
    Dim _LastData As SMARTDB

    Sub New(LocalBLAST As InteropService, CDD As CDDLoader, TEMP As String)
        Me.LocalBLAST = LocalBLAST
        Me.CDD = CDD
        Me.TempWorkspace = TEMP
    End Sub

    Public Function GetLastProcessData() As SMARTDB
        Return _LastData
    End Function

    ''' <summary>
    ''' 函数会返回缓存的目标蛋白质序列数据库中的蛋白质对象的结构域列表数据文件
    ''' </summary>
    ''' <param name="QueryInput">将要进行编译的目标蛋白质数据库</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Performance(QueryInput As String, GrepScript As String, Cache As String, Optional DbName As String = "Pfam") As String
        Dim SubjectDbPath As String = CDD.GetFastaUrl(DbName)

        Call LocalBLAST.FormatDb(QueryInput, LocalBLAST.MolTypeProtein).Start(waitForExit:=True)
        Call LocalBLAST.FormatDb(SubjectDbPath, LocalBLAST.MolTypeProtein).Start(waitForExit:=True)
        Call LocalBLAST.Blastp(TargetSubjectDb:=QueryInput,
                               InputQuery:=SubjectDbPath,
                               Output:=String.Format("{0}/{1}-${2}.xml", TempWorkspace, FileIO.FileSystem.GetName(QueryInput), DbName),
                               e:="1e-3").Start(waitForExit:=True)

        Dim SubjectDb = CDD.LoadFASTA(DbName)
        Dim CddDb = CDD.Load(DbName)
        Dim BlastLogOutput = BlastOutput.LoadFromFile(LocalBLAST.LastBLASTOutputFilePath)

        If Not String.IsNullOrEmpty(GrepScript) Then
            Dim Script = TextGrepScriptEngine.Compile(GrepScript)
            Call BlastLogOutput.Grep(AddressOf Script.Grep, Nothing)
        End If

        Dim QueryDb As FASTA.FastaFile = FASTA.FastaFile.Read(QueryInput)
        Dim LQuery = From Iteraction In BlastLogOutput.Iterations Select CreateProteinDescription(Iteraction, QueryDb, SubjectDb, CddDb) '
        Dim CachePath As String = Cache & "/" & FileIO.FileSystem.GetName(QueryInput) & ".xml"

        _LastData = New SMARTDB() With {.Proteins = LQuery.ToArray}
        Call _LastData.GetXml.SaveTo(CachePath)

        Return CachePath
    End Function

    Public Function ExportDb(DbName As String) As File
        Dim Db = CDD.Load(DbName)
        Dim LQuery = From item In Db.SmpData
                     Select New RowObject From {
                         item.Name,
                         item.ID,
                         item.CommonName,
                         item.Title,
                         item.Describes,
                         item.SequenceData} '
        Dim File As File = New File
        Call File.AppendLine(New String() {"AccessionId", "TagId", "CommonName", "Title", "Describes", "Sequence"})
        Call File.AppendRange(LQuery.ToArray)

        Return File
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="BlastLogOutput"></param>
    ''' <param name="GrepScript"></param>
    ''' <param name="SubjectDb">Cdd数据库中导出来的序列数据</param>
    ''' <param name="CddDb">与SubjectDb一致的Cdd数据库中的一个子库</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Compile(BlastLogOutput As BlastOutput, GrepScript As String, SubjectDb As FASTA.FastaFile, CddDb As CDD.DbFile) As SMARTDB
        Dim Script = TextGrepScriptEngine.Compile(GrepScript)
        Call BlastLogOutput.Grep(AddressOf Script.Grep, Nothing)
        Dim LQuery = From Iteraction In BlastLogOutput.Iterations Select CreateProteinDescription(Iteraction, Nothing, SubjectDb, CddDb) '

        Return New SMARTDB() With {.Proteins = LQuery.ToArray}
    End Function

    Public Shared Function CreateProteinDescription(QueryIteration As Iteration, QueryInput As FASTA.FastaFile, SubjectDb As FASTA.FastaFile, CddDb As CDD.DbFile) As Protein
        Dim SeqData As String = (From fsa In QueryInput Where InStr(fsa.Title.First, QueryIteration.QueryDef) Select fsa.SequenceData).First

        If QueryIteration.Hits.IsNullOrEmpty Then
            Return New Protein With {
                .ID = QueryIteration.QueryDef.Split.First,
                .Domains = New DomainObject() {},
                .SequenceData = SeqData,
                .Description = QueryIteration.QueryDef.Split(CChar("|")).Last}
        Else
            Dim LQuery = From Hit As Hits.Hit
                         In QueryIteration.Hits
                         Where Hit.HitLength / Val(Hit.Len) > 0.85 AndAlso Math.Abs(Hit.HitLength - Hit.QueryLength) < 20
                         Let Idx As Long = Val(Regex.Match(Hit.Id, "\d+").Value)
                         Let SmpFile = DomainInfo.Query(SubjectDb(Idx - 1).Headers(1), CddDb)
                         Select New DomainObject(SmpFile) With {
                             .Position = New Location() With {
                                    .Left = Val(Hit.Hsps.First.HitFrom),
                                    .Right = Val(Hit.Hsps.Last.HitTo)
                             }
                         } '
            Dim Protein As New Protein With {
                .ID = QueryIteration.QueryDef.Split.First,
                .Domains = LQuery.ToArray,
                .SequenceData = SeqData,
                .Description = QueryIteration.QueryDef.Split(CChar("|")).Last
            }
            Return Protein
        End If
    End Function
End Class

<XmlRoot("SMART.DB", Namespace:="http://code.google.com/p/genome-in-code/protein-domains/smart_db")>
Public Class SMARTDB
    '请注意： 实现这个接口会让XML序列化比较困难
    ' Implements Generic.IEnumerable(Of SMRUCC.genomics.Assembly.ProteinDomainArchitecture)

    <XmlElement> Public Property Proteins As Protein()

    Public Function Takes(IdList As String()) As SMARTDB
        Dim Result As Protein() = IEnumerations.Takes(IdList, Me.Proteins)
        Return New SMARTDB With {.Proteins = Result}
    End Function

    Public Function Export() As File
        Dim LQuery = (From p As Protein
                      In Proteins.AsParallel
                      Let row As RowObject = SMARTDB.Generate(p)
                      Where Not row Is Nothing
                      Select row).ToArray

        Dim File As File = New File
        File.AppendLine({"Accession_Id", "Common_Name", "Description", "Domain_IdList", "Sequence"})
        File.AppendRange(LQuery)
        Return File
    End Function

    Private Shared ReadOnly FilledEmptys As String() = New String() {"", "", ""}

    Private Shared Function Generate(Protein As Protein) As RowObject
        If Protein.ID.StringEmpty Then
            Return Nothing
        End If

        Dim Row As New RowObject
        Dim Tokens As List(Of String) = Protein.Id.Split(CChar("|")).AsList

        Call Tokens.AddRange(FilledEmptys)
        Call Row.Add(Tokens(0))
        Call Row.Add(Tokens(1))
        Call Row.Add(Tokens(2))
        Call Row.Add(String.Join("; ", (From pd In Protein.Domains Select pd.Name).ToArray))
        Call Row.Add(Protein.SequenceData)

        Return Row
    End Function
End Class
