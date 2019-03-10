Imports System.Drawing
Imports System.Reflection
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Objects.ObjectModels.Exceptions
Imports Microsoft.VisualBasic.Scripting.ShoalShell.DeviceDriver.DriverHandles

Namespace DeviceDriver

    Public Class IODeviceDriver : Inherits TypeHandleEntryPointDriverrModule(Of IO_DeviceHandle, Func(Of Object, String, Boolean), Type)

        Sub New(Memory As Runtime.Objects.I_MemoryManagementDevice)
            MyBase.New(Memory.ScriptEngine)
            MyBase._InternalHandles = New Dictionary(Of Type, Func(Of Object, String, Boolean)) From {
                {GetType(String), AddressOf IODeviceDriver.WriteTextFile},
                {GetType(Char()), AddressOf IODeviceDriver.WriteCharCollectionAsText},
                {GetType(String()), AddressOf IODeviceDriver.WriteTextCollection},
                {GetType(Image), AddressOf IODeviceDriver.WriteImageFile},
                {GetType(Bitmap), AddressOf IODeviceDriver.WriteBitmapFile},
                {GetType(Microsoft.VisualBasic.Logging.LogFile), AddressOf IODeviceDriver.SaveLogFile}}
        End Sub

        Private Shared Function SaveLogFile(log As Microsoft.VisualBasic.Logging.LogFile, path As String) As Boolean
            Return log.Save(path)
        End Function

        ''' <summary>
        ''' 返回成功导入的IO方法
        ''' </summary>
        ''' <param name="Module"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ImportsHandler([Module] As System.Type) As Integer
            Dim Methods As TypeHandle() = GetMethods([Module])

            For Each item In Methods
                If _InternalHandles.ContainsKey(item.Type) Then
                    Call Console.WriteLine("[UPDATE] {0} ==> {1}::{2}", item.Type.FullName, [Module].FullName, item.MethodInfo.Name)
                    Call _InternalHandles.Remove(item.Type)  '如果已经存在该类型的写入方法，则进行更新
                End If
                Call _InternalHandles.Add(item.Type, value:=Function(data As Object, path As String) As Boolean
                                                                Dim rtvl As Object = item.MethodInfo.Invoke(Nothing, New Object() {data, path})
                                                                Return CType(rtvl, Boolean)
                                                            End Function)
                Call _InternalRecordHandleTrace(item.Type, item.MethodInfo)
            Next

            Return Methods.Count
        End Function

        Private Shared Function WriteImageFile(data As Image, path As String) As Boolean
            Call data.Save(path)
            Return True
        End Function

        Private Shared Function WriteBitmapFile(data As Bitmap, path As String) As Boolean
            Call data.Save(path)
            Return True
        End Function

        Private Shared Function WriteTextFile(data As Object, path As String) As Boolean
            Dim strValue As String = If(data Is Nothing, "", data.ToString)
            Call FileIO.FileSystem.WriteAllText(path, strValue, False)
            Return True
        End Function

        Private Shared Function WriteTextCollection(data As Object, path As String) As Boolean
            Dim ChunkBuffer As String() = (From item In DirectCast(data, IEnumerable) Let strValue As String = item.ToString Select strValue).ToArray
            Call IO.File.WriteAllLines(path, ChunkBuffer)
            Return True
        End Function

        Private Shared Function WriteCharCollectionAsText(data As Object, path As String) As Boolean
            Dim strValue As String = If(data Is Nothing, "", New String((From c In DirectCast(data, IEnumerable) Let ch As Char = DirectCast(c, Char) Select ch).ToArray))
            Call FileIO.FileSystem.WriteAllText(path, strValue, False)
            Return True
        End Function

        Const EXCEPTION_MESSAGE_IO_HANDLER_MISSING As String = "IO method is missing for type: {0}, please try using ""imports <namespace>"" command for imports the io method."

        ''' <summary>
        ''' This function will trying to save the data in a properly method from the data type:
        ''' Function will trying save the string as a text file;
        ''' Basic data type collection will be save as a csv data table;
        ''' All of the other data type will be saved based on the registry data;
        ''' If the method is not found in the registry for the data type, then function will throw a method missing exception!
        ''' (函数会尝试根据文件的类型来选择合适的保存格式:
        ''' 字符串类型会保存为文本文件
        ''' 数组会保存为Csv
        ''' 其他的复杂类型会尝试根据注册的句柄来执行相应的数据保存操作)
        ''' </summary>
        ''' <param name="variable"></param>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function WriteData(variable As String, path As String) As Boolean
            Dim var As Object = _ShoalMemoryDevice.TryGetValue(variable)
            Dim type As Type = var.GetType

            path = FileIO.FileSystem.GetFileInfo(_ShoalMemoryDevice.FormatString(path)).FullName

            Call Console.WriteLine("Flush data to handle *""file:///{0}""...", path)
            Call FileIO.FileSystem.CreateDirectory(FileIO.FileSystem.GetParentPath(path))

            If _InternalHandles.ContainsKey(type) Then

                Try
                    Return _InternalHandles(type)(var, path)
                Catch ex As Exception
                    Throw New ShoalShell.Runtime.Objects.ObjectModels.Exceptions.ScriptRunTimeException(
                        String.Format("An unexpected exception occurs while trying flush the data to the handle *""{0}""...", path) & vbCrLf & vbCrLf & ex.ToString, _ShoalMemoryDevice)
                End Try
            Else
                Dim GenericCollection As Type = Nothing

                Try
                    GenericCollection = type.Collection2GenericIEnumerable
                Catch ex As Exception
                    Dim Message As String = String.Format(EXCEPTION_ON_HANDLE_GENERIC_TYPE, type.FullName) & vbCrLf & vbCrLf & ex.ToString
                    Throw New ShoalShell.Runtime.Objects.ObjectModels.Exceptions.ScriptRunTimeException(Message, _ShoalMemoryDevice)
                End Try

                If _InternalHandles.ContainsKey(GenericCollection) Then

                    Try
                        Return _InternalHandles(GenericCollection)(var, path)
                    Catch ex As Exception
                        Throw New ShoalShell.Runtime.Objects.ObjectModels.Exceptions.ScriptRunTimeException(
                            String.Format("An unexpected exception occurs while trying flush the data to the handle *""{0}""...", path) & vbCrLf & vbCrLf & ex.ToString, _ShoalMemoryDevice)
                    End Try
                Else
                    Throw New MethodNotFoundException(String.Format(EXCEPTION_MESSAGE_IO_HANDLER_MISSING, type.FullName), "")
                End If
            End If
        End Function

        Public Const EXCEPTION_ON_HANDLE_GENERIC_TYPE As String = "Unable to handle the type to generic: ""{0}"". The reason for this error maybe is you haven't imports the io device handler for target type."

        Protected Overrides Function HandleEntryToString(item As Type) As String
            Return item.FullName
        End Function
    End Class
End Namespace