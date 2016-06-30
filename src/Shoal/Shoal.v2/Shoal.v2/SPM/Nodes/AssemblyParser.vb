Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.ShoalShell.HTML
Imports Microsoft.VisualBasic.CommandLine.Interpreter
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Linker.APIHandler
Imports Microsoft.VisualBasic.ComponentModel

Namespace SPM.Nodes

    Public Module AssemblyParser

        ''' <summary>
        ''' 解析出错或返回空集合并在终端上面打印出错误信息
        ''' </summary>
        ''' <param name="Path">不需要特殊处理，函数会自动转换为全路径</param>
        ''' <returns></returns>
        Public Function LoadAssembly(Path As String) As PartialModule()
            Dim Assembly As System.Reflection.Assembly

            Try
                Assembly = System.Reflection.Assembly.LoadFile(VisualBasic.FileIO.FileSystem.GetFileInfo(Path).FullName)
            Catch ex As Exception
                Return __exHandler(ex, Path)
            End Try

            Try
                Dim assemblyValue = Nodes.Assembly.CreateObject(Of Assembly)(Assembly)
                Dim Namespaces = (From Type In Assembly.DefinedTypes '.AsParallel
                                  Let [Namespace] = __getNamespaceEntry(Type, assemblyValue)  '得到原始的部分的模块定义
                                  Where Not [Namespace] Is Nothing
                                  Select [Namespace]).ToArray
                Return Namespaces
            Catch ex As Exception
                Return __exHandler(ex, Path)
            End Try
        End Function

        Private Function __exHandler(ex As Exception, path As String) As PartialModule()
            ex = New Exception($"Assembly Parsing Error: {path.ToFileURL}", ex)
            Call App.LogException(ex, $"{NameOf(AssemblyParser)}::{NameOf(LoadAssembly)}")
            Call ex.PrintException()

            Return New PartialModule() {}
        End Function

        Private Function __getNamespaceEntry(Type As Type, Assembly As Assembly) As SPM.Nodes.PartialModule
            If Not Type.IsClass Then
                Return Nothing
            End If

            Dim attrs As Object() =
                Type.GetCustomAttributes(
                    attributeType:=PackageNamespace.TypeInfo,
                    inherit:=False)

            If attrs.IsNullOrEmpty Then
                attrs = (From ns As Object
                         In Type.GetCustomAttributes(
                             attributeType:=Microsoft.VisualBasic.CommandLine.Reflection.Namespace.TypeInfo,
                             inherit:=False)
                         Let nsEntry = DirectCast(ns, Microsoft.VisualBasic.CommandLine.Reflection.Namespace)
                         Select New PackageNamespace(nsEntry)).ToArray
                If attrs.IsNullOrEmpty Then
                    Return Nothing
                End If
            End If

            Dim nsAttr As PackageNamespace = DirectCast(attrs(Scan0), PackageNamespace)
            Return __nsParser(Type, nsAttr, Assembly)
        End Function

        Private Function __nsParser(type As Type,
                                    nsEntry As PackageNamespace,
                                    assembly As Assembly) As PartialModule
            Dim Functions = GetAllCommands(type, False)
            Dim EntryPoints = (From Func In Functions Select __entryPointParser(Func)).ToArray
            Dim assm As Assembly = Serialization.ShadowsCopy.ShadowsCopy(assembly)

            assm.TypeId = type.FullName

            Return New PartialModule(nsEntry) With {
                .Assembly = assm,
                .EntryPoints = EntryPoints
            }
        End Function

        ''' <summary>
        ''' 直接导入静态方法
        ''' </summary>
        ''' <param name="[module]"></param>
        ''' <returns></returns>
        Public Function [Imports]([module] As Type) As APIEntryPoint()
            Dim Functions = GetAllCommands([module], False)
            Return APIParser(Functions.ToArray)
        End Function

        Public Function APIParser(EntryPoints As IEnumerable(Of EntryPoints.APIEntryPoint)) As APIEntryPoint()
            Dim OverloadsGroup = (From api As EntryPoints.APIEntryPoint
                                  In EntryPoints
                                  Select api
                                  Group api By api.Name.ToLower Into Group).ToArray
            Dim __LoadedEntryPoints = (From apiGroup
                                       In OverloadsGroup
                                       Select New APIEntryPoint(
                                           apiGroup.Group.First.Name,
                                           apiGroup.Group.ToArray)).ToArray
            Return __LoadedEntryPoints
        End Function

        Private Function __entryPointParser(Command As EntryPoints.APIEntryPoint) As EntryPointMeta
            Return New EntryPointMeta() With {
                .Description = Command.Info,
                .Name = Command.Name,
                .ReturnedType = Command.EntryPoint.ReturnType.FullName,
                .Parameters = __getParameters(Command.EntryPoint)
            }
        End Function

        Private Function __getParameters(Method As System.Reflection.MethodInfo) As TripleKeyValuesPair()
            Dim parameters = Method.GetParameters
            Dim LQuery = (From p As System.Reflection.ParameterInfo
                              In parameters
                          Let attrs = p.GetCustomAttributes(Parameter.TypeInfo, True)
                          Let attr = If(attrs.IsNullOrEmpty, New Parameter(p.Name), DirectCast(attrs(Scan0), Parameter))
                          Select New TripleKeyValuesPair With {
                              .Key = attr.Alias,
                              .Value1 = attr.Description,
                              .Value2 = p.ParameterType.FullName}).ToArray
            Return LQuery
        End Function
    End Module
End Namespace