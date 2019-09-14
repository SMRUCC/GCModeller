#Region "Microsoft.VisualBasic::4ce4288cf199928a727e89428dec26f1, analysis\ProteinTools\ProteinTools.Family\FileSystem\Database.vb"

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

    '     Class Database
    ' 
    '         Properties: lstDb, Repository
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: __init, (+2 Overloads) Add, EntireDb, GetDatabase, ManualAdd
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Data.Xfam

Namespace FileSystem

    ''' <summary>
    ''' 数据库是分模块存放的，每一个模块单独存储于一个Xml文件之中，这个对象就是管理这些模块在文件系统之中的位置的对象
    ''' </summary>
    Public Class Database

        Public ReadOnly Property lstDb As Dictionary(Of String, String)
        Public ReadOnly Property Repository As String

        Sub New()
            Call Settings.Session.Initialize()
            lstDb = __init()
            Repository = GCModeller.FileSystem.Xfam.Pfam.PfamFamily
        End Sub

        Private Function __init() As Dictionary(Of String, String)
            Dim Pfam As String = GCModeller.FileSystem.Xfam.Pfam.PfamFamily
            Dim Files = FileIO.FileSystem.GetFiles(Pfam, FileIO.SearchOption.SearchAllSubDirectories, "*.xml")
            Return Files.ToDictionary(Function(x) basename(x))
        End Function

        ''' <summary>
        ''' 当比对没有指定数据库的时候默认是比对全部的数据库
        ''' </summary>
        ''' <returns></returns>
        Public Function EntireDb() As FamilyPfam
            Dim LQuery = (From entry In lstDb Select entry.Value.LoadXml(Of FamilyPfam)).ToArray
            Dim Merge As FamilyPfam = New FamilyPfam With {
                .Family = LQuery.Select(Function(x) x.Family).ToVector
            }
            Return Merge
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Name">数据库的名称或者文件路径</param>
        ''' <returns></returns>
        Public Function GetDatabase(Name As String) As FamilyPfam
            If Name.FileExists Then
                Call $"Using family database {Name.ToFileURL}...".__DEBUG_ECHO
                Return Name.LoadXml(Of FamilyPfam)
            End If

            If String.IsNullOrEmpty(Name) OrElse Not lstDb.ContainsKey(Name) Then
                Call $"Name is not valid, using the default entired database...".__DEBUG_ECHO
                Return EntireDb()
            End If

            Return lstDb(Name).LoadXml(Of FamilyPfam)
        End Function

        ''' <summary>
        ''' 请注意，这个函数会直接覆盖已经存在的数据库文件而不会给出任何警告的
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <param name="Db"></param>
        ''' <returns></returns>
        Public Function Add(Name As String, Db As FileSystem.Family) As String
            Dim DbFile As New FileSystem.FamilyPfam With {
                .Title = Name,
                .Guid = Guid.NewGuid.ToString,
                .Family = {Db}
            }
            Return Add(Name, DbFile)
        End Function

        Public Function Add(Name As String, DbFile As FamilyPfam) As String
            Dim Path As String = $"{Repository}/{Name}.xml"
            If DbFile.GetXml.SaveTo(Path) Then
                If lstDb.ContainsKey(Name) Then
                    Call lstDb.Remove(Name)
                End If
                Call lstDb.Add(Name, Path)
                Return Path
            Else
                Return ""
            End If
        End Function

        Public Function ManualAdd(Name As String, Cluster As Generic.IEnumerable(Of Pfam.PfamString.PfamString)) As String
            Dim Family As Family = FileSystem.Family.CreateObject(Name, Cluster.ToArray)
            Return Add(Name, Family)
        End Function
    End Class
End Namespace
