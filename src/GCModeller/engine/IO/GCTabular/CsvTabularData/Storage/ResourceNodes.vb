#Region "Microsoft.VisualBasic::d9cc2379cf03499d60e696f8f0e07298, engine\IO\GCTabular\CsvTabularData\Storage\ResourceNodes.vb"

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

    '     Class HrefLink
    ' 
    '         Properties: Md5_Hash, TypeId
    ' 
    '         Function: get_Path, InternalWriteEmptyFile, (+5 Overloads) LoadResource, (+3 Overloads) SaveResource, set_Category
    ' 
    '         Sub: InternalGetMd5
    ' 
    '     Class ResourceNode
    ' 
    '         Properties: Comment, InternalHrefLinks, ResourceCategory, TYPE_ID
    ' 
    '         Function: CopyFile, get_Resource, get_ResourceEntry, ToString, (+3 Overloads) WriteResource
    ' 
    '         Sub: Add
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace FileStream.XmlFormat

    ''' <summary>
    ''' Resource link to the table data source.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class HrefLink : Inherits Href

        ''' <summary>
        ''' Type full name of the target csv table.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TypeId As String
        Public Property Md5_Hash As String

        Dim _ResourceCategoryNode As ResourceNode

        Public Function set_Category(node As ResourceNode) As HrefLink
            Me._ResourceCategoryNode = node
            Return Me
        End Function

        Public Function LoadResource(Of T As Class)() As T()
            If Not String.Equals(TypeId, GetType(T).FullName) Then
                Throw New TypeAccessException(String.Format("{0} could not matched type {1}", GetType(T).FullName, TypeId))
            End If

            Return LoadResource(Of T)(_ResourceCategoryNode.ResourceCategory)
        End Function

        Public Function LoadResource(Of T As Class)(ByRef List As List(Of T)) As T()
            Dim ChunkBuffer As T() = LoadResource(Of T)()
            List = ChunkBuffer.AsList
            Return ChunkBuffer
        End Function

        Public Function LoadResource(Of T As Class)(ByRef array As T()) As T()
            Dim ChunkBuffer As T() = LoadResource(Of T)()
            array = ChunkBuffer
            Return ChunkBuffer
        End Function

        Public Function LoadResource(Of T As Class)(category As String) As T()
            Dim url As String = Me.GetFullPath(category)
            Dim ChunkBuffer As T() = url.LoadCsv(Of T)(False).ToArray
            Return ChunkBuffer
        End Function

        Public Function LoadResource(Of T As Class)(ByRef obj As T) As T
            Dim url As String = Me.GetFullPath(_ResourceCategoryNode.ResourceCategory)
            obj = url.LoadXml(Of T)()
            Return obj
        End Function

        ''' <summary>
        ''' Generate the path value for the <see cref="FileStream.XmlFormat.CellSystemXmlModel.Save"></see> method.
        ''' </summary>
        ''' <param name="root"></param>
        ''' <param name="category"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function get_Path(root As String, category As String) As String
            Return String.Format("{0}/{1}/{2}", root, category, Value)
        End Function

        ''' <summary>
        ''' 请注意设置工作目录，因为这里是以相对路径来保存的
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="data"></param>
        ''' <param name="category"></param>
        ''' <param name="name">相对路径</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SaveResource(Of T As Class)(data As IEnumerable(Of T), category As String, name As String) As Boolean
            Dim Path As String = String.Format("./{0}/{1}", category, name)

            Me.TypeId = GetType(T).FullName
            Me.Value = name
            Dim f As Boolean = data.SaveTo(Path, False)
            Call InternalGetMd5(Path)
            Return f
        End Function

        Private Sub InternalGetMd5(path As String)
            Call FlushMemory()
            Dim data As Byte() = System.IO.File.ReadAllBytes(path)
            Me.Md5_Hash = SecurityString.GetMd5Hash(data)
        End Sub

        ''' <summary>
        ''' 请注意设置工作目录，因为这里是以相对路径来保存的
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="data"></param>
        ''' <param name="Category"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SaveResource(Of T As Class)(data As IEnumerable(Of T), Category As String) As Boolean
            Dim Path As String = String.Format("./{0}/{1}", Category, Me.Value)
            Me.TypeId = GetType(T).FullName
            Dim f As Boolean

            If data.Empty Then
                f = InternalWriteEmptyFile(Path, TypeId)
            Else
                f = data.SaveTo(Path, False)
            End If

            Call InternalGetMd5(Path)

            Return f
        End Function

        Public Function SaveResource(Of T As ISaveHandle)(data As T, category As String) As Boolean
            Dim Path As String = String.Format("./{0}/{1}", category, Me.Value)
            Me.TypeId = GetType(T).FullName
            Dim f As Boolean

            If data Is Nothing Then
                f = InternalWriteEmptyFile(Path, TypeId)
            Else
                f = data.Save(Path, Encodings.UTF8)
            End If

            Call InternalGetMd5(Path)

            Return f
        End Function

        Private Shared Function InternalWriteEmptyFile(path As String, typeid As String) As Boolean
            Call Console.WriteLine("Target data ""{0}"" is empty!!!", typeid)
            Call "".SaveTo(path, )
            Return False
        End Function
    End Class

    Public Class ResourceNode : Implements INamedValue

#Region "GCML data storage location"

        ''' <summary>
        ''' The file reader required this property to located the resources
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property TYPE_ID As String Implements INamedValue.Key

        ''' <summary>
        '''  The data storage directory name.(数据文件的存储目录)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ResourceCategory As String

        <XmlArray(IsNullable:=True, Namespace:="http://code.google.com/p/genome-in-code/gcml/href-link-text")>
        Public Property InternalHrefLinks As HrefLink()
            Get
                If _hrefLinks.IsNullOrEmpty Then
                    Return New HrefLink() {}
                End If
                Return _hrefLinks.Values.ToArray
            End Get
            Set(value As HrefLink())
                _hrefLinks = value.ToDictionary
            End Set
        End Property

        <XmlText> Public Property Comment As String
#End Region

        Dim _hrefLinks As New Dictionary(Of HrefLink)

        Public Overrides Function ToString() As String
            Return Me.TYPE_ID
        End Function

        Public Sub Add(Of T As Class)(resourceId As String)
            If _hrefLinks.ContainsKey(resourceId) Then
                Call _hrefLinks.Remove(resourceId)
            End If

            Call _hrefLinks.Add(resourceId, New HrefLink With {.ResourceId = resourceId})
        End Sub

        Public Function get_Resource(Of T As Class)(resourceId As String) As T()
            If _hrefLinks.ContainsKey(resourceId) Then
                Dim Entry = _hrefLinks(resourceId)
                Dim ChunkBuffer As T() = Entry.LoadResource(Of T)(My.Computer.FileSystem.CurrentDirectory)
                Return ChunkBuffer
            Else
                Return Nothing
            End If
        End Function

        Public Function WriteResource(Of T As Class)(data As IEnumerable(Of T), ByRef hrefLink As HrefLink) As Boolean
            Return hrefLink.SaveResource(data, Me.ResourceCategory)
        End Function

        Public Function WriteResource(Of T As Class)(data As Dictionary(Of String, T), ByRef hreflink As HrefLink) As Boolean
            Dim Collection As T() = If(data.IsNullOrEmpty, Nothing, data.Values.ToArray)
            Return hreflink.SaveResource(Of T)(data:=Collection, Category:=Me.ResourceCategory)
        End Function

        ''' <summary>
        ''' 在保存数据之前，请设置好工作目录
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="data"></param>
        ''' <param name="hreflink"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function WriteResource(Of T As ISaveHandle)(data As T, ByRef hreflink As HrefLink) As Boolean
            Return hreflink.SaveResource(data, Me.ResourceCategory)
        End Function

        ''' <summary>
        ''' 将目标资源文件<paramref name="hreflink"></paramref>复制到目标目录<paramref name="subject"></paramref>之中
        ''' </summary>
        ''' <param name="subject">这个参数是模型的主Xml文件所在的文件夹，即root文件夹</param>
        ''' <param name="hreflink"></param>
        ''' <param name="source">模型文件的复制原的root文件夹</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CopyFile(subject As String, source As String, hreflink As HrefLink) As Boolean
            Dim src As String = hreflink.GetFullPath(source)      '得到全路径
            Dim sbj As String = hreflink.get_Path(root:=subject, category:=Me.ResourceCategory)

            If FileIO.FileSystem.FileExists(src) Then
                Call FileIO.FileSystem.CopyFile(src, sbj)
            Else
                Call $"The original resource file ""{src}"" is not exists!".__DEBUG_ECHO
            End If

            Return True
        End Function

        Public Function get_ResourceEntry(resourceId As String) As HrefLink
            If _hrefLinks.ContainsKey(resourceId) Then
                Dim Entry = _hrefLinks(resourceId)
                Return Entry
            Else
                Return Nothing
            End If
        End Function

        Public Const TYPE_ID_BACTERIA_ANNOTATION_DATA_CHUNK As String = "TYPE_ID_BACTERIA_ANNOTATION_DATA_CHUNK/RES-TEXT-Guid('4CA66D8C-4D5E-4B55-A83C-8D24D05C4435')"
        Public Const TYPE_ID_BACTERIA_GENOME_PROGRAMMING_INFORMATION As String = "TYPE_ID_BACTERIA_GENOME_PROGRAMMING_INFORMATION/RES-TEXT-Guid('B0D07F26-3F9B-4F60-B8E9-3091AFB5B56A')"
        Public Const TYPE_ID_BACTERIA_CELL_PHENOTYPE_DESCRIPTION As String = "TYPE_ID_BACTERIA_CELL_PHENOTYPE_DESCRIPTION/RES-TEXT-Guid('BF81B6E1-1227-4D96-8610-B74DE0DF3A4B')"
        Public Const TYPE_ID_BACTERIA_SIGNAL_TRANSDUCTION_NETWORK As String = "TYPE_ID_BACTERIA_SIGNAL_TRANSDUCTION_NETWORK/RES-TEXT-Guid('3F3C3E06-8C3B-4D75-89DC-687EC3DC2F6C')"
        Public Const TYPE_ID_BACTERIA_EXPERINMENT_ENVIRONMENT As String = "TYPE_ID_BACTERIA_EXPERINMENT_ENVIRONMENT/RES-TEXT-Guid('ED7DF092-E8FE-4610-A48A-C99307BC0EC8')"
    End Class
End Namespace
