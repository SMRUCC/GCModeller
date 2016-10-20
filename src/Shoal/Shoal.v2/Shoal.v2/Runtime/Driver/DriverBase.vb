Imports System.Reflection
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.DeviceDriver.DriverHandles
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.SCOM
Imports Microsoft.VisualBasic.Scripting.ShoalShell.SPM.Nodes

Namespace Runtime.DeviceDriver

    Public MustInherit Class DriverBase(Of
             HwndEntry As HwndEntryPoint,
             Handler,
             __TYPE_ID) : Inherits RuntimeComponent

        Protected _MMUDevice As MMU.MMUDevice
        Protected _innerHwnds As Dictionary(Of __TYPE_ID, Handler)

        Dim _HwndTrace As Dictionary(Of __TYPE_ID, String) = New Dictionary(Of __TYPE_ID, String)

        Protected MustOverride Function HandleEntryToString(hwnd As __TYPE_ID) As String

        Public Function GetMountEntries() As String()
            If _HwndTrace.IsNullOrEmpty Then
                Return {"No external device was mount at here yet..." & vbCrLf}
            End If

            Dim LQuery = (From hwnd In _HwndTrace
                          Select $"     ----> [{HandleEntryToString(hwnd.Key)}] Mount at ""{hwnd.Value}""" & vbCrLf).ToList
            Call LQuery.Insert(0, "Device TYPE_ID is " & Me.GetType.FullName & vbCrLf)
            Return LQuery.ToArray
        End Function

        Protected Sub __recordHandleTrace(Entry As __TYPE_ID, MountPoint As MethodInfo)
            If _HwndTrace.ContainsKey(Entry) Then
                Call _HwndTrace.Remove(Entry)
            End If

            Dim Assembly = MountPoint.DeclaringType
            Dim sPoint As String =
                $"{Assembly.Assembly.Location}!{Assembly.FullName}::{MountPoint.ToString}"

            Call _HwndTrace.Add(Entry, sPoint)
        End Sub

        Sub New(ScriptEngine As ScriptEngine)
            Call MyBase.New(ScriptEngine)
            _MMUDevice = ScriptEngine.MMUDevice
        End Sub

        Public MustOverride Function ImportsHandler([Module] As System.Type) As Integer

        Protected Shared Function GetMethods([module] As PartialModule) As __TYPEHwnd()
            Dim HwndEntry = GetType(HwndEntry)
            Dim Methods = (From Entry As System.Type
                           In [module].Assembly.LoadAssembly.DefinedTypes
                           Select (From MethodInfo As MethodInfo
                                   In Entry.GetMethods(System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Static)
                                   Let attrs As Object() = MethodInfo.GetCustomAttributes(attributeType:=HwndEntry, inherit:=True)
                                   Where Not attrs.IsNullOrEmpty
                                   Let Handle = DirectCast(attrs.First, HwndEntry)
                                   Select New __TYPEHwnd With {
                                       .Type = Handle.SupportType,
                                       .Handle = Handle,
                                       .MethodInfo = MethodInfo}).ToArray).ToArray
            Return Methods.ToVector
        End Function

        Protected Shared Function GetMethods([module] As System.Type) As __TYPEHwnd()
            Dim Methods = (From MethodInfo As System.Reflection.MethodInfo
                           In [module].GetMethods(System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Static)
                           Let attrs As Object() = MethodInfo.GetCustomAttributes(attributeType:=GetType(HwndEntry), inherit:=True)
                           Where Not attrs.IsNullOrEmpty
                           Let Handle = DirectCast(attrs.First, HwndEntry)
                           Select New __TYPEHwnd With {
                               .Handle = Handle,
                               .Type = Handle.SupportType,
                               .MethodInfo = MethodInfo}).ToArray
            Return Methods
        End Function

        Protected Structure __TYPEHwnd
            Dim Type As Type
            Dim MethodInfo As MethodInfo
            Dim Handle As HwndEntry

            Public Overrides Function ToString() As String
                Return Handle.ToString
            End Function
        End Structure
    End Class
End Namespace