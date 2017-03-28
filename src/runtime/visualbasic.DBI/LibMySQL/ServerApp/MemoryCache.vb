Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Oracle.LinuxCompatibility.MySQL.Reflection.Schema

Namespace ServerApp

    ''' <summary>
    ''' 这个缓存对象对于不经常更新数据，即只执行SELECT查询操作的数据表非常有效
    ''' 至少对于生物信息学的数据库而言，由于更新数据很缓慢，大部分时候都只是执行SELECT查询操作，所以非常好用
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Class MemoryCache(Of T As SQLTable)

        ReadOnly mysql As New MySQL
        ReadOnly __cache As Dictionary(Of String, T())

        ''' <summary>
        ''' 这个索引是由用户手动指定的
        ''' </summary>
        ReadOnly __index As PropertyInfo()
        ReadOnly __schema As New Table(GetType(T))
        ''' <summary>
        ''' ``propertyName -> FieldName``
        ''' </summary>
        ReadOnly __fields As Dictionary(Of String, String)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="cnn"></param>
        ''' <param name="index$">属性名列表，请尽量使用``NameOf``操作符来获取</param>
        Sub New(cnn As ConnectionUri, ParamArray index$())
            If (mysql <= cnn) = -1.0R Then
                Throw New Exception("No avalaible mysql connection!")
            End If

            __index = GetType(T) _
                .GetProperties(PublicProperty) _
                .Where(Function([property]) index.IndexOf([property].Name) > -1) _
                .ToArray
        End Sub

        ''' <summary>
        ''' 对从数据库之中读取回来的对象建立缓存之中的索引
        ''' </summary>
        ''' <param name="o"></param>
        ''' <returns></returns>
        Private Function __indexKey(o As T) As String
            Dim keys$() = __index _
                .Select(Function(prop) prop.GetValue(o)) _
                .Select(Function(s) If(s Is Nothing, "null", s.ToString)) _
                .ToArray
            Return String.Join("--", keys)
        End Function

        Private Function __indexKey(o As NamedValue(Of String)()) As String
            Dim keys$() = __index _
                .Select(Function(prop) o.Take(prop.Name).Value) _
                .Select(Function(s) If(s Is Nothing, "null", s)) _
                .ToArray
            Return String.Join("--", keys)
        End Function

        Public Function Query(args As NamedValue(Of String)(), Optional forceUpdate As Boolean = False) As T()
            Dim key$ = __indexKey(args)

            If Not forceUpdate AndAlso __cache.ContainsKey(key) Then
                Return Clone(__cache(key))
            Else
                Dim SQL$ = $"SELECT * FROM `{__schema.TableName}` WHERE {__where(args)};"
                Dim data As T() = Query(SQL, forceUpdate)  ' SQL会被用作为key缓存一次，这样子下次即使直接用SQL查询的话，只要有hit就可以直接从缓存之中读取了
                __cache(key) = Clone(data)    ' 当前的key也会被缓存一次
                Return data
            End If
        End Function

        ''' <summary>
        ''' SQL语句直接用作为key
        ''' </summary>
        ''' <param name="SQL$"></param>
        ''' <returns></returns>
        Public Function Query(SQL$, Optional forceUpdate As Boolean = False) As T()
            If Not forceUpdate AndAlso __cache.ContainsKey(SQL) Then
                Return Clone(__cache(SQL))
            Else
                Dim data As T() = mysql.Query(Of T)(SQL)
                __cache(SQL) = Clone(data)
                Return data
            End If
        End Function

        Private Function __where(args As NamedValue(Of String)()) As String
            Dim out$() = args.Select(Function(x) $"`{__fields(x.Name)}` = '{x.Value}'")
            Return String.Join(" AND ", out)
        End Function

        ''' <summary>
        ''' 当不进行Clone操作的话，会由于Class引用的缘故导致Cache的数据也被修改，所以在这里使用这个函数来避免这种情况的发生
        ''' </summary>
        ''' <param name="array"></param>
        ''' <returns></returns>
        Private Shared Function Clone(array As T()) As T()
            Return array _
                .Select(Function(o) DirectCast(o.Copy, T)) _
                .ToArray
        End Function

        Public Function ExecuteScalar(args As NamedValue(Of String)(), Optional forceUpdate As Boolean = False) As T
            Dim key$ = __indexKey(args)
            Dim o As T

            If Not forceUpdate AndAlso __cache.ContainsKey(key) Then
                ' 当数据库不存在在这条记录的时候会是空集合
                o = __cache(key).FirstOrDefault
            Else
                o = ExecuteScalar(
                    $"SELECT * FROM `{__schema.TableName}` WHERE {__where(args)} LIMIT 1;",
                    forceUpdate)
            End If

            If o Is Nothing Then
                Return Nothing
            Else
                Return o.Copy
            End If
        End Function

        Public Function ExecuteScalar(SQL$, Optional forceUpdate As Boolean = False) As T
            If Not forceUpdate AndAlso __cache.ContainsKey(SQL) Then
                Return Clone(__cache(SQL)).FirstOrDefault
            Else
                Dim data As T = mysql.ExecuteScalar(Of T)(SQL)
                If data Is Nothing Then
                    __cache(SQL) = {}
                Else
                    __cache(SQL) = {
                        DirectCast(data.Copy, T)
                    }
                End If

                Return data
            End If
        End Function
    End Class
End Namespace
