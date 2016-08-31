Imports System.Reflection
Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq.Framework.Provider
Imports Microsoft.VisualBasic.Serialization

Namespace Framework.Provider

    ''' <summary>
    ''' Type registry table for loading the external LINQ entity assembly module.
    ''' (起始这个模块就是相当于一个类型缓存而已，因为程序可以直接读取dll文件里面的内容，但是直接读取的方法会造成性能下降，所以需要使用这个对象来缓存所需要的类型数据) 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TypeRegistry : Inherits ITextFile
        Implements IDisposable

        <XmlElement> Public Property typeDefs As TypeEntry()
            Get
                Return _typeHash.Values.ToArray
            End Get
            Set(value As TypeEntry())
                If value Is Nothing Then
                    _typeHash = New Dictionary(Of String, TypeEntry)
                Else
                    _typeHash = value.ToDictionary(Function(x) x.name.ToLower)
                End If
            End Set
        End Property

        ''' <summary>
        ''' .NET SDK directory
        ''' </summary>
        ''' <returns></returns>
        Public Property SDK As String

        Dim _typeHash As Dictionary(Of String, TypeEntry)

        ''' <summary>
        ''' 返回包含有该类型的目标模块的文件路径
        ''' </summary>
        ''' <param name="name">LINQ Entity集合中的元素的简称或者别称，即Item中的Name属性</param>
        ''' <returns>If the key is not exists in this object, than the function will return a empty string.</returns>
        ''' <remarks></remarks>
        Public Function LoadAssembly(name As String) As Assembly
            Dim type As TypeEntry = Find(name)
            If type Is Nothing Then
                Return Nothing
            Else
                Return type.TypeId.LoadAssembly
            End If
        End Function

        ''' <summary>
        ''' 查找不成功会返回空值
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Public Function GetHandle(name As String) As GetLinqResource
            Dim entry As TypeEntry = Find(name)
            If entry Is Nothing Then
                Return Nothing
            Else
                Return entry.GetHandle
            End If
        End Function

        ''' <summary>
        ''' Return a registry item in the table using its specific name property.
        ''' (返回注册表中的一个指定名称的项目)
        ''' </summary>
        ''' <param name="name">大小写不敏感的</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Find(name As String) As TypeEntry
            Dim key As String = name.ToLower

            If _typeHash.ContainsKey(key) Then
                Return _typeHash(key)
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' Registry the external LINQ entity assembly module in the LINQFramework
        ''' </summary>
        ''' <param name="assmPath">DLL file path</param>
        ''' <returns></returns>
        ''' <remarks>查询出目标元素的类型定义并获取信息</remarks>
        Public Function Register(assmPath As String) As Boolean
            Dim assm As Assembly = Assembly.LoadFrom(IO.Path.GetFullPath(assmPath)) 'Load external module
            Dim typeDefs As Type() = assm.GetTypes  'Get type define informations of LINQ entity

            If typeDefs.IsNullOrEmpty Then Return False

            Dim LQuery = From type As Type In typeDefs
                         Let entries As TypeEntry() = __parsingEntry(type, assm)
                         Where Not entries.IsNullOrEmpty
                         Select entries

            For Each x As TypeEntry In LQuery.MatrixAsIterator       'Update exists registry item or insrt new item into the table
                Call _typeHash.Remove(x.name)
                Call _typeHash.Add(x.name, x)  'Insert new record.(添加数据)
            Next
            Return True
        End Function

        Private Shared Function __parsingEntry(type As Type, assm As Assembly) As TypeEntry()
            Dim methods As MethodInfo() = type.GetMethods(bindingAttr:=BindingFlags.Static Or BindingFlags.Public Or BindingFlags.NonPublic)
            Dim LQuery = (From x As MethodInfo In methods
                          Let attrs As Object() = x.GetCustomAttributes(LinqEntity.ILinqEntity, inherit:=True)
                          Where Not attrs.IsNullOrEmpty
                          Select x,
                              attr = DirectCast(attrs(Scan0), LinqEntity))
            Dim path As String = FileIO.FileSystem.GetFileInfo(assm.Location).Name  ' 方法的路径，类型的路径可能是变化的
            Dim result As TypeEntry() = LQuery.ToArray(
                Function(x) New TypeEntry With {
                    .Func = x.x.Name,
                    .name = x.attr.Type,
                    .TypeId = New Scripting.MetaData.TypeInfo With {
                        .assm = FileIO.FileSystem.GetFileInfo(x.attr.RefType.Assembly.Location).Name,
                        .FullIdentity = x.attr.RefType.FullName
                    },   ' 实体类型的信息
                    .Repository = New Scripting.MetaData.TypeInfo With {
                        .assm = path,
                        .FullIdentity = x.x.DeclaringType.FullName
                    }   ' 数据源的方法信息
                })
            Return result
        End Function

        Public Shared Function ParsingEntry(m As MethodInfo) As TypeEntry
            Dim attrs As LinqEntity = m.GetAttribute(Of LinqEntity)
            If attrs Is Nothing Then
                Return Nothing
            End If
            Return New TypeEntry With {
                .Func = m.Name,
                .name = attrs.Type,
                .Repository = New Scripting.MetaData.TypeInfo(m.DeclaringType),
                .TypeId = New Scripting.MetaData.TypeInfo(attrs.RefType)
            }
        End Function

        ''' <summary>
        ''' 扫描安装应用程序文件夹之中的所有插件
        ''' </summary>
        Public Sub InstallCurrent()
            Dim dlls = FileIO.FileSystem.GetFiles(App.HOME, FileIO.SearchOption.SearchTopLevelOnly, "*.dll", "*.exe")
            For Each assm As String In dlls
                Try
                    Call Register(assm)
                Catch ex As Exception
                    ex = New Exception(assm, ex)
                    Call App.LogException(ex)
                    Call ex.PrintException
#If DEBUG Then
                    Call Register(assm)
#End If
                End Try
            Next
        End Sub

        Public Shared Function Load(Path As String) As TypeRegistry
            If FileIO.FileSystem.FileExists(Path) Then
                Try
                    Dim registry As TypeRegistry = Path.LoadTextDoc(Of TypeRegistry)()
                    Return registry
                Catch ex As Exception
                    Call App.LogException(New Exception(Path.ToFileURL, ex))
                    GoTo NEWLY
                End Try
            Else
NEWLY:          Dim nlibs As New TypeRegistry With {
                    .FilePath = Path,
                    .typeDefs = Nothing
                }
                Call nlibs.Register(GetType(TypeRegistry).Assembly.Location)
                Return nlibs
            End If
        End Function

        Public Shared ReadOnly Property DefaultFile As String = App.ProductSharedDIR & "/Linq.Entity.Xml"

        Public Shared Function LoadDefault() As TypeRegistry
            Return TypeRegistry.Load(DefaultFile)
        End Function

        Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean
            Return Me.SaveAsXml(getPath(FilePath), False, Encoding)
        End Function
    End Class
End Namespace