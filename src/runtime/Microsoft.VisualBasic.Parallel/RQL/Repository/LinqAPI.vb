Imports System.Collections.Specialized
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComputingServices.TaskHost
Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.SecurityString.MD5Hash
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods
Imports SMRUCC.WebCloud.HTTPInternal.Platform

Namespace Linq

    ''' <summary>
    ''' 对外部提供Linq查询服务的WebApp
    ''' </summary>
    Public Class LinqAPI : Inherits LinqPool

        Public ReadOnly Property Repository As Repository

        ''' <summary>
        ''' {hashCode.tolower, linq_uid}
        ''' </summary>
        ReadOnly __uidMaps As New Dictionary(Of String, String)

        Sub New(repo As Repository)
            Me.Repository = repo
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="url">数据源的引用位置</param>
        ''' <param name="args">查询参数</param>
        ''' <returns></returns>
        Public Overloads Function OpenQuery(url As String, args As String) As LinqEntry
            Dim source As IEnumerable = Repository.GetRepository(url, args) ' expr为空的话，则没有where测试，则返回所有数据
            Dim type As Type = Repository.GetType(url)  ' 得到元素的类型信息
            Dim linq As IPEndPoint = OpenQuery(source, type)
            Dim result As New LinqEntry(type) With {
                .Linq = linq,
                .uid = (linq.ToString & Now.ToString).GetMd5Hash.ToLower
            }
            Call __uidMaps.Add(result.uid, result.Linq.ToString)
            Return result
        End Function

        ''' <summary>
        ''' Linq数据源的MD5哈希值
        ''' </summary>
        Const uid As String = "uid"
        Const n As String = "n"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="args">uid,n</param>
        ''' <returns></returns>
        Public Function MoveNext(args As NameValueCollection) As String
            Dim uid As String = args(LinqAPI.uid)
            Dim n As Integer = Scripting.CastInteger(args(LinqAPI.n))
            Dim linq As LinqProvider = GetLinq(__uidMaps(uid))
            Dim source As Object = linq.Moves(n)
            Dim json As String = JsonContract.GetObjectJson(source, linq.BaseType)
            Return json
        End Function

        ''' <summary>
        ''' 释放掉一个Linq查询的资源
        ''' </summary>
        ''' <param name="args"></param>
        ''' <returns></returns>
        Public Overloads Function Free(args As NameValueCollection) As String
            Dim uid As String = args(LinqAPI.uid)
            Call MyBase.Free(__uidMaps(uid))
            Call __uidMaps.Remove(uid)
            Return True
        End Function
    End Class
End Namespace