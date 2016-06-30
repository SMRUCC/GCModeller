Public Module CliResCommon

    Private ReadOnly CHUNK_BUFFER As Type = GetType(Byte())
    Private ReadOnly Resource As Dictionary(Of String, Func(Of Byte())) = (From p As System.Reflection.PropertyInfo
                                                                           In GetType(My.Resources.Resources).GetProperties(bindingAttr:=Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Static)
                                                                           Where p.PropertyType.Equals(CHUNK_BUFFER)
                                                                           Select p).ToArray.ToDictionary(Of String, Func(Of Byte()))(
 _
                                                                                Function(obj) obj.Name,
                                                                                elementSelector:=Function(obj) New Func(Of Byte())(Function() DirectCast(obj.GetValue(Nothing, Nothing), Byte())))

    Sub New()
        Call Settings.Session.Initialize()
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Name">使用 NameOf 操作符来获取资源</param>
    ''' <returns></returns>
    Public Function TryRelease(Name As String) As String
        Dim Path As String = $"{Settings.Session.DataCache}/{Name}.exe"

        If Path.FileExists Then
            Return Path
        End If

        If Not CliResCommon.Resource.ContainsKey(Name) Then
            Return ""
        End If

        Dim ChunkBuffer = CliResCommon.Resource(Name)()
        Try
            Return If(ChunkBuffer.FlushStream(Path), Path, "")
        Catch ex As Exception
            Call Console.WriteLine(ex.ToString)
            Return ""
        End Try
    End Function

End Module
