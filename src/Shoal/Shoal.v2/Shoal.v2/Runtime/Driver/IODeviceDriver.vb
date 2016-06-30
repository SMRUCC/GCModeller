Imports System.Drawing
Imports System.Reflection
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.DeviceDriver.DriverHandles
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Exceptions

Namespace Runtime.DeviceDriver

    Public Class IODeviceDriver : Inherits DriverBase(Of IO_DeviceHandle, Func(Of Object, String, Boolean), Type)

        Delegate Function IWriteData(data As Object, path As String) As Boolean

        Private Shared Function WriteDat(data As Object, path As String) As Boolean
            Return CastArray(Of Byte)(data).FlushStream(path)
        End Function

        Private Shared Function WriteBooleanBytes(data As Object, path As String) As Boolean
            Dim value = (From b As Boolean
                             In InputHandler.CastArray(Of Boolean)(data)
                         Select If(b, CByte(1), CByte(0))).ToArray
            Return value.FlushStream(path)
        End Function

        ReadOnly GenericCollection As New Dictionary(Of Type, IWriteData) From {
            {GetType(Integer), AddressOf WriteTextCollection},
            {GetType(Double), AddressOf WriteTextCollection},
            {GetType(Byte), AddressOf WriteDat},
            {GetType(Long), AddressOf WriteTextCollection},
            {GetType(Boolean), AddressOf WriteBooleanBytes},
            {GetType(Date), AddressOf WriteTextCollection},
            {GetType(Object), AddressOf WriteTextCollection}
        }

        Sub New(ScriptEngine As Runtime.ScriptEngine)
            MyBase.New(ScriptEngine)
            MyBase._innerHwnds = New Dictionary(Of Type, Func(Of Object, String, Boolean)) From {
                {GetType(String), AddressOf IODeviceDriver.WriteTextFile},
                {GetType(Char()), AddressOf IODeviceDriver.WriteCharCollectionAsText},
                {GetType(String()), AddressOf IODeviceDriver.WriteTextCollection},
                {GetType(Image), AddressOf IODeviceDriver.WriteImageFile},
                {GetType(Bitmap), AddressOf IODeviceDriver.WriteBitmapFile},
                {GetType(Microsoft.VisualBasic.Logging.LogFile), AddressOf IODeviceDriver.SaveLogFile},
                {GetType(Byte), Function(byt As Object, path As String) {DirectCast(byt, Byte)}.FlushStream(path)}}
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
            Dim Methods As __TYPEHwnd() = GetMethods([Module])

            For Each hwnd In Methods
                If _innerHwnds.ContainsKey(hwnd.Type) Then
                    Call $"[UPDATE] {hwnd.Type.FullName} ==> {[Module].FullName}::{hwnd.MethodInfo.Name}".__DEBUG_ECHO
                    Call _innerHwnds.Remove(hwnd.Type)  '如果已经存在该类型的写入方法，则进行更新
                End If
                Call _innerHwnds.Add(hwnd.Type, value:=Function(data As Object, path As String) As Boolean
                                                           Dim rtvl As Object = hwnd.MethodInfo.Invoke(Nothing, New Object() {data, path})
                                                           Return CType(rtvl, Boolean)
                                                       End Function)
                Call __recordHandleTrace(hwnd.Type, hwnd.MethodInfo)
            Next

            Return Methods.Length
        End Function

        Private Shared Function WriteImageFile(data As Image, path As String) As Boolean
            Call data.Save(path, Drawing.Imaging.ImageFormat.Png)
            Return True
        End Function

        Private Shared Function WriteBitmapFile(data As Bitmap, path As String) As Boolean
            Call data.Save(path)
            Return True
        End Function

        Private Shared Function WriteTextFile(data As Object, path As String) As Boolean
            Dim sValue As String = InputHandler.ToString(data)
            Return sValue.SaveTo(path)
        End Function

        Private Shared Function WriteTextCollection(data As Object, path As String) As Boolean
            Dim textBuffer As String() = (From line In DirectCast(data, IEnumerable)
                                          Let sValue As String = Scripting.ToString(line)
                                          Select sValue).ToArray
            Call IO.File.WriteAllLines(path, textBuffer)
            Return True
        End Function

        Private Shared Function WriteCharCollectionAsText(data As Object, path As String) As Boolean
            Dim sValue As String = If(data Is Nothing, "", New String((From c In DirectCast(data, IEnumerable) Let ch As Char = DirectCast(c, Char) Select ch).ToArray))
            Call VisualBasic.FileIO.FileSystem.WriteAllText(path, sValue, False)
            Return True
        End Function

        Const EXCEPTION_MESSAGE_IO_HANDLER_MISSING As String =
            "IO method is missing for type: {0}, please try using ""imports <namespace>"" command for imports the io method."

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
        ''' <param name="value"></param>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function WriteData(value As Object, path As String) As Boolean
            Dim type As Type = value.GetType

            path = VisualBasic.FileIO.FileSystem.GetFileInfo(MyBase.ScriptEngine.Strings.Format(path)).FullName

            Call $"Flush data to handle *""{path.ToFileURL}""...".__DEBUG_ECHO
            Call VisualBasic.FileIO.FileSystem.CreateDirectory(VisualBasic.FileIO.FileSystem.GetParentPath(path))

            If Not _innerHwnds.ContainsKey(type) Then Return __writeCollection(type, value, path)

            Try
                Return _innerHwnds(type)(value, path)
            Catch ex As Exception
                Throw New ShoalShell.Runtime.Exceptions.RuntimeException(
                    $"An unexpected exception occurs while trying flush the data to the handle *""{path.ToFileURL}""...", ex, ScriptEngine)
            End Try
        End Function

        Private Function __writeCollection(type As Type, value As Object, path As String) As Boolean
            Dim GenericCollection As Type = __genericCollection(type)

            If Not Me.GenericCollection.ContainsKey(GenericCollection) Then _
                Throw New MethodNotFoundException(String.Format(EXCEPTION_MESSAGE_IO_HANDLER_MISSING, type.FullName), ScriptEngine)

            Try
                Dim Hwnd = Me.GenericCollection(GenericCollection) '???字典查找有问题？？
                Return Hwnd(value, path)
            Catch ex As Exception
                Throw New ShoalShell.Runtime.Exceptions.RuntimeException(
                    $"An unexpected exception occurs while trying flush the data to the handle *""{path.ToFileURL}""...", ex, ScriptEngine)
            End Try
        End Function

        Private Function __genericCollection(type As Type) As Type
            Dim GenericCollection As Type = Nothing

            Try
                GenericCollection = type.Collection2GenericIEnumerable
                GenericCollection = GenericCollection.GenericTypeArguments.First
            Catch ex As Exception
                Dim Message As String = String.Format(EXCEPTION_ON_HANDLE_GENERIC_TYPE, type.FullName)
                Throw New ShoalShell.Runtime.Exceptions.RuntimeException(Message, ex, ScriptEngine)
            End Try

            Return GenericCollection
        End Function

        Public Const EXCEPTION_ON_HANDLE_GENERIC_TYPE As String =
            "Unable to handle the type to generic: ""{0}"". The reason for this error maybe is you haven't imports the io device handler for target type."

        Protected Overrides Function HandleEntryToString(item As Type) As String
            Return item.FullName
        End Function
    End Class
End Namespace