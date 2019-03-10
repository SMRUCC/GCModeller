Imports System.Text
Imports Microsoft.VisualBasic.Scripting.ShoalShell.DelegateHandlers.TypeLibraryRegistry.RegistryNodes

Namespace DelegateHandlers.TypeLibraryRegistry

    Public Class RegistryModuleLoader

        Protected Friend _RegistryFile As DelegateRegistry

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="RegistryFile">注册表的文件路径</param>
        ''' <param name="PreLoadAssembly">预加载的模块名称</param>
        ''' <remarks></remarks>
        Public Sub New(RegistryFile As String, PreLoadAssembly As String())
            If FileIO.FileSystem.FileExists(RegistryFile) Then
                _RegistryFile = FileIO.FileSystem.ReadAllText(RegistryFile).CreateObjectFromXml(Of DelegateRegistry)(ThrowEx:=False)
                If _RegistryFile Is Nothing Then _RegistryFile = InternalCreateDefaultRegistry(RegistryFile)
            Else
                _RegistryFile = InternalCreateDefaultRegistry(RegistryFile)
            End If

            _RegistryFile._RegisteredModuleLoader = Me
            _RegistryFile._FilePath = RegistryFile

            If _RegistryFile.IsEmpty Then
                '安装Shoal的核心模块的内建的命名空间以及外部空间
                _RegistryFile.RegisterAssemblyModule(GetType(ShoalShell.Runtime.Objects.ShellScript).Assembly.Location, "")
                _RegistryFile.RegisterAssemblyModule(My.Application.Info.DirectoryPath & "/" & My.Application.Info.AssemblyName & ".exe", "")
            End If

            For Each AssemblyName As String In PreLoadAssembly
                Dim AssemblyPaths As String() = _RegistryFile.GetAssemblyPaths(AssemblyName)
                Dim Modules = (From path As String In AssemblyPaths Select Me.get_ModuleFromAssembly(path)).ToArray.MatrixToVector

                For Each ModuleItem In Modules
                    Call _RegistryFile._LoadedModules.Add(ModuleItem.Key, ModuleItem.Value)
                Next
            Next

            Call LoadSystemModules()
        End Sub

        Private Sub LoadSystemModules()
            Dim AssemblyPath As String = String.Format("{0}/{1}.exe", My.Application.Info.DirectoryPath, My.Application.Info.AssemblyName)
            Dim Modules = get_ModuleFromAssembly(AssemblyPath)
            For Each ModuleItem In Modules
                Call _RegistryFile._LoadedModules.Add(ModuleItem.Key, ModuleItem.Value)
            Next
        End Sub

        Private Shared Function InternalCreateDefaultRegistry(RegistryFile As String) As DelegateRegistry
            Dim _RegistryFile = New DelegateRegistry With {.RegisteredModules = New [Namespace]() {}}
            _RegistryFile._FilePath = RegistryFile

            Call _RegistryFile.Save()
            Return _RegistryFile
        End Function

        ''' <summary>
        ''' 空的Namespace属性值的Namespace对象将会被忽略
        ''' </summary>
        ''' <param name="AssemblyPath"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function get_ModuleFromAssembly(AssemblyPath As String) As KeyValuePair(Of Microsoft.VisualBasic.CommandLine.Reflection.[Namespace], System.Type)()
            Dim Assembly = System.Reflection.Assembly.LoadFrom(AssemblyPath)
            Dim Types = (From Type As System.Reflection.TypeInfo In Assembly.DefinedTypes
                         Let attrs As Object() = Type.GetCustomAttributes(attributeType:=Microsoft.VisualBasic.CommandLine.Reflection.[Namespace].TypeInfo, inherit:=True)
                         Where Not attrs.IsNullOrEmpty
                         Let EntryInfo = DirectCast(attrs.First, Microsoft.VisualBasic.CommandLine.Reflection.[Namespace])
                         Select New KeyValuePair(Of Microsoft.VisualBasic.CommandLine.Reflection.[Namespace], System.Type)(EntryInfo, Type)).ToArray
            Dim LQuery = (From item In Types Where Not String.IsNullOrEmpty(item.Key.Namespace) Select item).ToArray
            Return LQuery
        End Function

        Public Shared Function GetModule(Type As Type) As KeyValuePair(Of Type, Microsoft.VisualBasic.CommandLine.Reflection.[Namespace])()
            Dim attrs As Object() = Type.GetCustomAttributes(attributeType:=Microsoft.VisualBasic.CommandLine.Reflection.[Namespace].TypeInfo, inherit:=True)

            If attrs.IsNullOrEmpty Then
                Return Nothing
            End If

            Dim EntryInfo = DirectCast(attrs.First, Microsoft.VisualBasic.CommandLine.Reflection.[Namespace])
            Dim [Module] = New KeyValuePair(Of Type, Microsoft.VisualBasic.CommandLine.Reflection.[Namespace])(Type, EntryInfo)

            If String.IsNullOrEmpty([Module].Key.Namespace) Then
                Return Nothing
            Else
                Return New KeyValuePair(Of Type, Microsoft.VisualBasic.CommandLine.Reflection.[Namespace])() {[Module]}
            End If
        End Function

        ' ''' <summary>
        ' ''' 
        ' ''' </summary>
        ' ''' <param name="AssemblyPath">必须是完整的路径字符串</param>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        'Public Function LoadModules(AssemblyPath As String) As Microsoft.VisualBasic.ComponentModel.Collection.Generic.KeyValuePairObject(Of CommandLine.Reflection.Namespace, Type)()
        '    Dim Types = get_ModuleFromAssembly(AssemblyPath)
        '    Dim LQuery = (From item As KeyValuePair(Of Microsoft.VisualBasic.CommandLine.Reflection.[Namespace], System.Type)
        '                  In Types
        '                  Select New Microsoft.VisualBasic.ComponentModel.Collection.Generic.KeyValuePairObject(Of String, [Module]) With {
        '                      .Key = item.Key.Namespace,
        '                      .Value = New [Module](item.Key, item.Value)}).ToArray
        '    Return LQuery
        'End Function
    End Class
End Namespace