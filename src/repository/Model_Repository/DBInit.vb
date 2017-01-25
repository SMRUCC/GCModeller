#Region "Microsoft.VisualBasic::e1005c577ae365343bef9a36ac059647, ..\workbench\Model_Repository\DBInit.vb"

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

Imports System.Data.SQLite.Linq.Reflector
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Data.Model_Repository.SQLEngines

''' <summary>
''' 这个模块之中的命令当且仅当第一次创建数据库的时候使用，假若在已经创建了数据库的情况之下使用，则会重置整个数据库
''' </summary>
''' <remarks></remarks>
Public Module DBInit

    Public ReadOnly Property RepositoryEngine As SQLiteIndex

    Sub New()
        Call Settings.Initialize()
        DBInit.RepositoryEngine = New SQLiteIndex(Settings.SettingsFile.RepositoryRoot)
    End Sub

    Public Function InvokeOperation(Connection As SQLEngines.SQLiteIndex) As Boolean
        If Connection Is Nothing OrElse Connection.SQLiteEngine Is Nothing Then
            Return False
        Else
            Call Connection.Close()
            Call FileIO.FileSystem.DeleteFile(Connection.SQLiteEngine.URL, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)
            Connection = New SQLEngines.SQLiteIndex(Connection.RepositoryRoot)  '重新连接数据并初始化数据库
        End If

        '导入现有的数据
        Dim GenbankDir As String = Connection.RepositoryRoot & "/Genbank/"
        Call DBInit.UpdateGenbankEntryInfo(GenbankDir, Cnn:=Connection)

        Return True
    End Function

    ''' <summary>
    ''' 由于文件已经存放于数据库的文件夹之中，故而不会再进行文件的复制操作，函数只需要生成数据库数据就可以了
    ''' </summary>
    ''' <param name="dir"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdateGenbankEntryInfo(dir As String, Cnn As SQLEngines.SQLiteIndex) As Boolean
        Dim FileList = FileIO.FileSystem.GetFiles(dir, FileIO.SearchOption.SearchAllSubDirectories, "*.gb", "*.gbk")
        Dim EntryList = (From Path As String In FileList
                         Let Genbank As GBFF.File = GBFF.File.Load(Path)
                         Let Entry As Tables.GenbankEntryInfo =
                             New Tables.GenbankEntryInfo With {
                                .MD5Hash = SecurityString.GetFileHashString(Path),
                                .LocusID = Genbank.Locus.AccessionID,
                                .plasmid = Genbank.SourceFeature.Query("plasmid"),
                                .GI = Genbank.Version.GI,
                                .Definition = Genbank.Definition.Value.Replace("'", ""),
                                .Species = Genbank.Source.OrganismHierarchy.Categorys.Last
                            }
                         Select Cnn.SQLiteEngine.Insert(obj:=Entry)).ToArray
        Return True
    End Function
End Module

