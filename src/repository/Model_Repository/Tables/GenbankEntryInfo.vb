#Region "Microsoft.VisualBasic::581c693f1f7b212a370f86c4e88be30d, Model_Repository\Tables\GenbankEntryInfo.vb"

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

    '     Class GenbankEntryInfo
    ' 
    '         Properties: GI, plasmid, Species
    ' 
    '         Function: GetPath, InternalUpdateInfo, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Data.Linq.Mapping
' Imports System.Data.SQLite.Linq
' Imports System.Data.SQLite.Linq.Reflector
Imports SMRUCC.genomics.Assembly.NCBI.GenBank

Namespace Tables

    <Table(Name:="genbank_entry_info")>
    Public Class GenbankEntryInfo : Inherits DbFileSystemObject

        <Column(DbType:="varchar(128)", Name:="gi", IsPrimaryKey:=True)> Public Property GI As String
        <Column(DbType:="varchar(2048)", Name:="species")> Public Property Species As String
        <Column(DbType:="varchar(1024)", Name:="plasmid")> Public Property plasmid As String

        Public Overrides Function ToString() As String
            Return String.Format("{0}/Genbank/{1}/{2}/{3}.gbk", "RepositoryRoot://", Species, LocusID.First.ToString, LocusID)
        End Function

        ''' <summary>
        ''' Genbank文件的文件相对路径
        ''' </summary>
        ''' <returns></returns>
        ''' <param name="RepositoryRoot">数据源的根目录</param>
        ''' <remarks></remarks>
        Public Overrides Function GetPath(RepositoryRoot As String) As String
            Dim Path As String = String.Format("{0}/Genbank/{1}/{2}/{3}.gbk", RepositoryRoot, Species, LocusID.First.ToString, LocusID)
            Return FileIO.FileSystem.GetFileInfo(Path).FullName
        End Function

        Private Function InternalUpdateInfo(RepositoryRoot As String) As GenbankEntryInfo
            Dim File As String = GetPath(RepositoryRoot)

            If Not File.FileExists() Then
                Return Nothing  '数据入口点已经失效了
            End If

            Dim Genbank As GBFF.File = GBFF.File.Load(path:=File)

            Me.LocusID = Genbank.Locus.AccessionID
            Me.GI = Genbank.Version.GI
            Me.Definition = Genbank.Definition.Value.Replace("'", "")
            Me.Species = Genbank.Source.OrganismHierarchy.Lineage.Last
            Me.plasmid = Genbank.SourceFeature.Query("plasmid")
            Me.MD5Hash = SecurityString.GetFileMd5(File)

            Return Me
        End Function

        '''' <summary>
        '''' Only using for the development when you have change the definition of this table, then using this function to update the database.(在更新了数据表的结构之后，请使用本方法更新数据库)
        '''' </summary>
        '''' <param name="DB"></param>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Public Shared Function UpdateTableSchema(DB As SQLEngines.SQLiteIndex) As Boolean
        '    Dim TableSchema = GetType(GenbankEntryInfo)
        '    Dim Data = (From item In Reflector.Load(DB.SQLiteEngine, TableSchema) Select DirectCast(item, GenbankEntryInfo)).ToArray
        '    Data = (From item As GenbankEntryInfo In Data.AsParallel Let obj = item.InternalUpdateInfo(DB.RepositoryRoot) Where Not obj Is Nothing Select obj).ToArray '更新数据
        '    '删除原来的表
        '    Call DB.SQLiteEngine.DeleteTable(TableSchema)
        '    Call DB.SQLiteEngine.CreateTableFor(TableSchema)
        '    '插入新的数据
        '    Return DB.SQLiteEngine.Insert(data:=Data)
        'End Function
    End Class
End Namespace
