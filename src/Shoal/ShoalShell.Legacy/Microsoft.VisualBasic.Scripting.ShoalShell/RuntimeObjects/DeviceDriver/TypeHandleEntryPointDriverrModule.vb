Imports Microsoft.VisualBasic.Scripting.ShoalShell.DeviceDriver.DriverHandles
Imports System.Reflection

Namespace DeviceDriver

    Public MustInherit Class TypeHandleEntryPointDriverrModule(Of HandlerEntry As TypeHandlerEntryPoint, Handler, T_TYPE_ID) : Inherits ShoalShell.Runtime.Objects.ObjectModels.IScriptEngineComponent

        Protected _ShoalMemoryDevice As Runtime.Objects.I_MemoryManagementDevice
        Protected _InternalHandles As Dictionary(Of T_TYPE_ID, Handler)

        Dim _HandlesTrace As Dictionary(Of T_TYPE_ID, String) = New Dictionary(Of T_TYPE_ID, String)

        Protected MustOverride Function HandleEntryToString(item As T_TYPE_ID) As String

        Public Function GetMountEntries() As String()
            If _HandlesTrace.IsNullOrEmpty Then
                Return {"No external device was mount at here yet..." & vbCrLf}
            End If

            Dim LQuery = (From item In _HandlesTrace Select String.Format("     ----> [{0}] Mount at ""{1}""" & vbCrLf, HandleEntryToString(item.Key), item.Value)).ToList
            Call LQuery.Insert(0, "Device TYPE_ID is " & Me.GetType.FullName & vbCrLf)
            Return LQuery.ToArray
        End Function

        Protected Sub _InternalRecordHandleTrace(Entry As T_TYPE_ID, MountPoint As MethodInfo)
            If _HandlesTrace.ContainsKey(Entry) Then
                Call _HandlesTrace.Remove(Entry)
            End If

            Dim Assembly = MountPoint.DeclaringType
            Dim sPoint As String = String.Format("{0}!{1}::{2}", Assembly.Assembly.Location, Assembly.FullName, MountPoint.ToString)

            Call _HandlesTrace.Add(Entry, sPoint)
        End Sub

        Sub New(ScriptEngine As ShoalShell.Runtime.Objects.ShellScript)
            Call MyBase.New(ScriptEngine)
            _ShoalMemoryDevice = ScriptEngine._EngineMemoryDevice
        End Sub

        Public MustOverride Function ImportsHandler([Module] As System.Type) As Integer

        Protected Shared Function GetMethods([Module] As ShoalShell.DelegateHandlers.TypeLibraryRegistry.RegistryNodes.Module) As TypeHandle()
            Dim Methods = (From Entry As KeyValuePair(Of System.Type, String)
                           In [Module].OriginalAssemblys
                           Select (From MethodInfo As System.Reflection.MethodInfo
                                   In Entry.Key.GetMethods(System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Static)
                                   Let attrs As Object() = MethodInfo.GetCustomAttributes(attributeType:=GetType(HandlerEntry), inherit:=True)
                                   Where Not attrs.IsNullOrEmpty
                                   Let Handle = DirectCast(attrs.First, HandlerEntry)
                                   Select New TypeHandle With
                                          {
                                              .Type = Handle.SupportDatatType, .Handle = Handle, .MethodInfo = MethodInfo}).ToArray).ToArray
            Return Methods.MatrixToVector
        End Function

        Protected Shared Function GetMethods([Module] As System.Type) As TypeHandle()
            Dim Methods = (From MethodInfo As System.Reflection.MethodInfo
                           In [Module].GetMethods(System.Reflection.BindingFlags.Public Or System.Reflection.BindingFlags.Static)
                           Let attrs As Object() = MethodInfo.GetCustomAttributes(attributeType:=GetType(HandlerEntry), inherit:=True)
                           Where Not attrs.IsNullOrEmpty
                           Let Handle = DirectCast(attrs.First, HandlerEntry)
                           Select New TypeHandle With
                                  {
                                       .Handle = Handle, .Type = Handle.SupportDatatType, .MethodInfo = MethodInfo}).ToArray
            Return Methods
        End Function

        Protected Structure TypeHandle
            Dim Type As Type
            Dim MethodInfo As MethodInfo
            Dim Handle As HandlerEntry

            Public Overrides Function ToString() As String
                Return Handle.ToString
            End Function
        End Structure
    End Class
End Namespace