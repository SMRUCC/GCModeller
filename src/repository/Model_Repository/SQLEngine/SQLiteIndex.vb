#Region "Microsoft.VisualBasic::c4017d317e64aefd0c310714aa696c50, ..\repository\Model_Repository\SQLEngine\SQLiteIndex.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

'#Region "Microsoft.VisualBasic::3a4741468d04ba3a59d688e8785610a5, ..\workbench\Model_Repository\SQLEngine\SQLiteIndex.vb"

'' Author:
'' 
''       asuka (amethyst.asuka@gcmodeller.org)
''       xieguigang (xie.guigang@live.com)
''       xie (genetics@smrucc.org)
'' 
'' Copyright (c) 2016 GPL3 Licensed
'' 
'' 
'' GNU GENERAL PUBLIC LICENSE (GPL3)
'' 
'' This program is free software: you can redistribute it and/or modify
'' it under the terms of the GNU General Public License as published by
'' the Free Software Foundation, either version 3 of the License, or
'' (at your option) any later version.
'' 
'' This program is distributed in the hope that it will be useful,
'' but WITHOUT ANY WARRANTY; without even the implied warranty of
'' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'' GNU General Public License for more details.
'' 
'' You should have received a copy of the GNU General Public License
'' along with this program. If not, see <http://www.gnu.org/licenses/>.

'#End Region

'Imports System.Data.Linq.Mapping
'Imports System.Data.SQLite.Linq
'Imports System.Data.SQLite.Linq.Reflector
'Imports SMRUCC.genomics.Assembly.NCBI.GenBank

'Namespace SQLEngines

'    Public Class SQLiteIndex : Inherits SQLEngine

'        Dim Engine ' As SQLProcedure
'        Dim _RepositoryRoot As String

'        Public Const MAIN_DB_URL As String = "/GCModeller.db"

'        ''' <summary>
'        ''' 数据源的根目录
'        ''' </summary>
'        ''' <value></value>
'        ''' <returns></returns>
'        ''' <remarks></remarks>
'        Public ReadOnly Property RepositoryRoot As String
'            Get
'                Return Me._RepositoryRoot
'            End Get
'        End Property

'        Public ReadOnly Property SQLiteEngine As SQLProcedure
'            Get
'                Return Me.Engine
'            End Get
'        End Property

'        Sub New(RepositoryRoot As String)
'            Me._RepositoryRoot = RepositoryRoot
'            Me.Engine = SQLProcedure.CreateSQLTransaction(RepositoryRoot & "/" & SQLiteIndex.MAIN_DB_URL)
'            If Not Me.Engine.ExistsTableForType(Of Tables.GenbankEntryInfo)() Then
'                Call Me.Engine.CreateTableFor(Of Tables.GenbankEntryInfo)()
'            End If
'        End Sub

'        ''' <summary>
'        ''' 断开与SQLite数据库的连接
'        ''' </summary>
'        ''' <remarks></remarks>
'        Public Sub Close()
'            Call Engine.CloseTransaction()
'        End Sub

'        Public Overrides Function ToString() As String
'            Return Engine.ToString
'        End Function

'        ''' <summary>
'        ''' 通过查询匹配定义之中的关键词来进行查询
'        ''' </summary>
'        ''' <param name="keyword"></param>
'        ''' <returns></returns>
'        ''' <remarks></remarks>
'        Public Function QueryGenbankEntryByDefinition(keyword As String) As Tables.GenbankEntryInfo()
'            Dim ChunkBuffer = Engine.Load(Of Tables.GenbankEntryInfo)(String.Format("SELECT * FROM 'genbank_entry_info' WHERE lower( definition ) LIKE '%'||lower( '{0}' )||'%';", keyword))
'            Return ChunkBuffer
'        End Function

'        Public Function [Imports](Genbank As GBFF.File) As Boolean
'            Dim Entry As New Tables.GenbankEntryInfo With {
'                .LocusID = Genbank.Locus.AccessionID,
'                .plasmid = Genbank.SourceFeature.Query("plasmid"),
'                .GI = Genbank.Version.GI,
'                .Definition = Genbank.Definition.Value.Replace("'", ""),
'                .Species = Genbank.Source.OrganismHierarchy.Categorys.Last
'            }
'            Dim Path As String = Entry.GetPath(Me._RepositoryRoot)

'            If FileIO.FileSystem.FileExists(Path) Then
'                Entry.MD5Hash = SecurityString.GetFileHashString(Path)
'            End If

'            If Not Entry.VerifyData(Me._RepositoryRoot, Genbank.FilePath) Then  '当文件不存在的时候，肯定不能够校验通过，因为新生成的Entry的哈希值为空的

'                On Error Resume Next

'                If FileIO.FileSystem.FileExists(Path) Then
'                    Call FileIO.FileSystem.DeleteFile(Path)     '更新数据文件
'                    Call FileIO.FileSystem.CopyFile(Genbank.FilePath, Path)
'                    Entry.MD5Hash = SecurityString.GetFileHashString(Genbank.FilePath)     '更新哈希值
'                    Return Engine.Update(Entry)  '更新数据库
'                Else '数据文件不存在则插入新的数据
'                    Call FileIO.FileSystem.CopyFile(Genbank.FilePath, Path)
'                    Entry.MD5Hash = SecurityString.GetFileHashString(Genbank.FilePath)     '更新哈希值
'                    Return Engine.Insert(obj:=Entry)  '更新数据库
'                End If
'            Else '能够通过校验，则不再进行任何操作
'                Return True
'            End If
'        End Function
'    End Class
'End Namespace

