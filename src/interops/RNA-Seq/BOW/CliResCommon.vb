Imports System.Reflection

Public Module CliResCommon

    Private ReadOnly bufType As Type = GetType(Byte())
    Private ReadOnly Resource As Dictionary(Of String, Func(Of Byte())) = (
        From p As PropertyInfo
        In GetType(My.Resources.Resources) _
            .GetProperties(bindingAttr:=BindingFlags.NonPublic Or BindingFlags.Static)
        Where p.PropertyType.Equals(bufType)
        Select p) _
              .ToDictionary(Of String, Func(Of Byte()))(
 _
               Function(obj) obj.Name,
               Function(obj) New Func(Of Byte())(Function() DirectCast(obj.GetValue(Nothing, Nothing), Byte())))

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

        Dim bufs = CliResCommon.Resource(Name)()
        Try
            Return If(bufs.FlushStream(Path), Path, "")
        Catch ex As Exception
            ex = New Exception(Name, ex)
            ex = New Exception(Path, ex)
            Call ex.PrintException
            Call App.LogException(ex)
            Return ""
        End Try
    End Function

End Module
