Imports Microsoft.VisualBasic.Serialization.JSON.JsonContract
Imports Microsoft.VisualBasic

Namespace StorageTek

    ''' <summary>
    ''' 系统自有的存储API
    ''' </summary>
    Public Module API

        ''' <summary>
        ''' 系统自带的存储API
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property InternalAPIs As Dictionary(Of StorageTeks, IRepository)

        Sub New()
            InternalAPIs = New Dictionary(Of StorageTeks, IRepository) From {
 _
                {StorageTeks.Json, AddressOf JsonList},
                {StorageTeks.Xml, AddressOf XmlList},
                {StorageTeks.DIR Or StorageTeks.Json, AddressOf DIRJson},
                {StorageTeks.DIR Or StorageTeks.Xml, AddressOf DIRXml}
            }
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="url"></param>
        ''' <param name="type">单个元素的类型</param>
        ''' <returns></returns>
        Public Function XmlList(url As String, type As Type) As IEnumerable
            Dim listType As Type = GetType(List(Of )).MakeGenericType(type)
            Dim obj As Object = url.LoadXml(type)
            Dim source = DirectCast(obj, IEnumerable)
            Return source
        End Function

        Public Function JsonList(url As String, type As Type) As IEnumerable
            Dim listType As Type = GetType(List(Of )).MakeGenericType(type)
            Dim json As String = IO.File.ReadAllText(url)
            Dim source As Object = LoadObject(json, type)
            Return DirectCast(source, IEnumerable)
        End Function

        ''' <summary>
        ''' 实体对象单独保存在一个json文件之中
        ''' </summary>
        ''' <param name="url"></param>
        ''' <param name="type"></param>
        ''' <returns></returns>
        Public Function DIRJson(url As String, type As Type) As IEnumerable
            Return DIRRepository(url, type, AddressOf __loadJSON, "json")
        End Function

        Private Function __loadJSON(url As String, type As Type) As Object
            Try
                Dim json As String = IO.File.ReadAllText(url)
                Return LoadObject(json, type)
            Catch ex As Exception
                ex = New Exception(url, ex)
                Call App.LogException(ex)
                Return Nothing
            End Try
        End Function

        Private Function DIRRepository(url As String, type As Type, repo As IRepository, ext As String) As IEnumerable
            Dim DIR = FileIO.FileSystem.GetDirectoryInfo(url)
            Dim LQuery = (From f As IO.FileInfo
                          In DIR.EnumerateFiles
                          Let iExt As String = f.FullName.Split("."c).Last
                          Where String.Equals(iExt, ext, StringComparison.OrdinalIgnoreCase)
                          Select repo(f.FullName, type))
            Return LQuery
        End Function

        Public Function DIRXml(url As String, type As Type) As IEnumerable
            Return DIRRepository(url, type, AddressOf LoadXml, "xml")
        End Function
    End Module

    Public Delegate Function IRepository(url As String, type As Type) As IEnumerable
End Namespace