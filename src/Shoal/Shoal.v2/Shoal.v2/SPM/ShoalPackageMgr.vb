Imports System.Reflection
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Scripting.ShoalShell.SPM.Nodes
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace SPM

    ''' <summary>
    ''' Shoal模块管理器
    ''' </summary>
    Public Class ShoalPackageMgr

        Implements IDictionary(Of String, SPM.Nodes.Namespace)
        Implements System.IDisposable

        Dim _NamespaceTable As Dictionary(Of String, SPM.Nodes.Namespace)
        Dim _LibraryDb As SPM.PackageModuleDb

        Sub New(ByRef Library As SPM.PackageModuleDb)
            If Library.NamespaceCollection Is Nothing Then
                Library.NamespaceCollection = New [Namespace]() {}
            End If

            _LibraryDb = Library
            _NamespaceTable = Library.NamespaceCollection.ToDictionary(Function(ns) ns.Namespace.ToLower)
        End Sub

        ''' <summary>
        ''' 将所得到的模块合并到现有的模块之中
        ''' </summary>
        ''' <param name="NsCollection"></param>
        ''' <returns></returns>
        Public Function MergeNamespace(NsCollection As Nodes.PartialModule()) As Integer
            Dim LQuery = (From ns As Nodes.PartialModule
                          In NsCollection  '不需要并行，因为给出的集合之中可能含有同名的命名空间，合并的时候使用并行化会产生冲突
                          Where Me.__mergeNamespace(ns) Select 1).ToArray.Sum
            Return LQuery
        End Function

        ''' <summary>
        ''' 向脚本环境之中安装注册一个外部的模块
        ''' </summary>
        ''' <param name="assemPath">文件路径不需要特殊处理，函数会自动转换为全路径</param>
        ''' <returns></returns>
        Public Function [Imports](assemPath As String) As PartialModule()
            Dim nsModules = AssemblyParser.LoadAssembly(assemPath)
            Call MergeNamespace(nsModules)
            Call __safelyImports(System.Reflection.Assembly.LoadFile(FileIO.FileSystem.GetFileInfo(assemPath).FullName))

            Return nsModules
        End Function

        Private Sub __safelyImports(assm As System.Reflection.Assembly)
            Try
                Call __importsEnvir(assm)
            Catch ex As Exception
                Call Console.WriteLine(vbCrLf)
                Call $"{MethodBase.GetCurrentMethod.GetFullName}{vbCrLf}   >> {assm.Location.ToFileURL}::{assm.FullName}".__DEBUG_ECHO
                Call Console.WriteLine(ex.ToString)
            End Try
        End Sub

        Private Sub __importsEnvir(Assembly As System.Reflection.Assembly)
            Dim LQuery = (From typeRef In Assembly.GetTypes.AsParallel
                          Let envir = Runtime.HybridsScripting.EnvironmentParser.Imports(typeRef)
                          Where Not envir.IsNull
                          Select envir).ToArray
            For Each environment In LQuery
                Call Me._LibraryDb.Update(environment)
            Next
        End Sub

        Public Sub UpdateDb()
            _LibraryDb.NamespaceCollection = _NamespaceTable.Values.ToArray
            _LibraryDb.Save()

            Call HTML.DocRenderer.Indexing(_LibraryDb)
            Call $"[Job DONE!] {NameOf(UpdateDb)}".__DEBUG_ECHO
        End Sub

        Private Function __mergeNamespace(ns As Nodes.PartialModule) As Boolean
            Dim [Namespace] As SPM.Nodes.Namespace
            Dim Name As String = ns.Namespace.ToLower

            If _NamespaceTable.ContainsKey(Name) Then
                [Namespace] = _NamespaceTable(Name)

                Dim EqualsModule = (From [module] In [Namespace].PartialModules
                                    Where SPM.Nodes.PartialModule.Equals([module], ns)
                                    Select [module]).FirstOrDefault
                If EqualsModule Is Nothing Then '没有相同的模块，则直接添加
                    [Namespace].PartialModules = [Namespace].PartialModules.Join(ns).ToArray
                Else
                    Call EqualsModule.Copy(ns)
                End If

                [Namespace].Cites = __mergeCites([Namespace].Cites, ns.Cites)
                [Namespace].Publisher = __mergeAuthors([Namespace].Publisher, ns.Publisher)
            Else
                [Namespace] = New [Namespace] With {  '不存在则新建添加
                    .Description = ns.Description,
                    .Publisher = ns.Publisher,
                    .Revision = ns.Revision,
                    .Cites = {ns.Cites},
                    .Url = ns.Url,
                    .PartialModules = {ns},
                    .Namespace = ns.Namespace
                }
                Call _NamespaceTable.Add(Name, [Namespace])
            End If

            Call HTML.DocRenderer.GenerateHtmlDoc([Namespace])

            Return True
        End Function

        ''' <summary>
        ''' 这个是参照EndNote使用逗号以及分号分割的
        ''' </summary>
        ''' <param name="DbValue"></param>
        ''' <param name="Merge"></param>
        ''' <returns></returns>
        Private Shared Function __mergeAuthors(DbValue As String, Merge As String) As String
            If String.IsNullOrEmpty(DbValue) Then
                Return Merge
            End If
            If String.IsNullOrEmpty(Merge) Then
                Return DbValue
            End If

            Dim Tokens = DbValue.Split(";"c).ToArray(Function(name) name.Split(","c)).Unlist
            Dim mToken = Merge.Split(";"c).ToArray(Function(name) name.Split(","c)).Unlist
            Dim Values As List(Of String) = Tokens.Join(mToken)
            Dim GroupName = (From name As String
                             In Values
                             Let tName As String = Trim(name)
                             Where Not String.IsNullOrEmpty(tName)
                             Select tName.ToUpper, tName
                             Group By ToUpper Into Group).ToArray
            Dim Names As String() = GroupName.ToArray(Function(ng) ng.Group.First.tName)
            Return String.Join(", ", Names)
        End Function

        Private Shared Function __mergeCites(cites As String(), imported As String) As String()
            If cites.IsNullOrEmpty Then
                If String.IsNullOrEmpty(imported) Then
                    Return New String() {}
                Else
                    Return Strings.Split(imported, "///")
                End If

            ElseIf String.IsNullOrEmpty(imported) Then
                Return cites

            Else
                For Each cite As String In Strings.Split(imported, "///")
                    cites = ___mergeCites2(cites, cite)
                Next
                Return cites
            End If
        End Function

        Private Shared Function ___mergeCites2(cites As String(), imported As String) As String()
            Dim ImportedTitle As String = Regex.Match(imported, """[^""]+", RegexOptions.Singleline).Value

            If String.IsNullOrEmpty(ImportedTitle) Then
                ImportedTitle = imported
            End If

            For i As Integer = 0 To cites.Length - 1
                If InStr(cites(i), ImportedTitle, CompareMethod.Text) > 0 Then
                    cites(i) = imported
                    Return cites
                End If
            Next

            cites = cites.Join(imported).ToArray

            Return cites
        End Function

#Region "Implements System.Collections.Generic.IDictionary(Of String, SPM.Nodes.Namespace)"

        ''' <summary>
        ''' 大小写不敏感
        ''' </summary>
        ''' <param name="key"></param>
        ''' <returns></returns>
        Default Public Property Item(key As String) As [Namespace] Implements IDictionary(Of String, [Namespace]).Item
            Get
                key = key.ToLower

                If Me._NamespaceTable.ContainsKey(key) Then
                    Return Me._NamespaceTable(key)
                Else
                    Return Nothing
                End If
            End Get
            Set(value As [Namespace])
                key = key.ToLower

                If Me._NamespaceTable.ContainsKey(key) Then
                    Call Me._NamespaceTable.Remove(key)
                End If

                Call Me._NamespaceTable.Add(key, value)
            End Set
        End Property

        Public ReadOnly Property Keys As ICollection(Of String) Implements IDictionary(Of String, [Namespace]).Keys
            Get
                Return Me._NamespaceTable.Keys
            End Get
        End Property

        Public ReadOnly Property Values As ICollection(Of [Namespace]) Implements IDictionary(Of String, [Namespace]).Values
            Get
                Return Me._NamespaceTable.Values
            End Get
        End Property

        Public ReadOnly Property Count As Integer Implements ICollection(Of KeyValuePair(Of String, [Namespace])).Count
            Get
                Return Me._NamespaceTable.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of KeyValuePair(Of String, [Namespace])).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Public Function ContainsKey(key As String) As Boolean Implements IDictionary(Of String, [Namespace]).ContainsKey
            Return Me._NamespaceTable.ContainsKey(key.ToLower)
        End Function

        Public Sub Add(key As String, value As [Namespace]) Implements IDictionary(Of String, [Namespace]).Add
            Call Me._NamespaceTable.Add(key.ToLower, value)
        End Sub

        Public Function Remove(key As String) As Boolean Implements IDictionary(Of String, [Namespace]).Remove
            Return Me._NamespaceTable.Remove(key.ToLower)
        End Function

        Public Function TryGetValue(key As String, ByRef value As [Namespace]) As Boolean Implements IDictionary(Of String, [Namespace]).TryGetValue
            Return Me._NamespaceTable.TryGetValue(key.ToLower, value)
        End Function

        Public Sub Add(item As KeyValuePair(Of String, [Namespace])) Implements ICollection(Of KeyValuePair(Of String, [Namespace])).Add
            Call Me._NamespaceTable.Add(item.Key.ToLower, item.Value)
        End Sub

        Public Sub Clear() Implements ICollection(Of KeyValuePair(Of String, [Namespace])).Clear
            Call Me._NamespaceTable.Clear()
        End Sub

        Public Function Contains(item As KeyValuePair(Of String, [Namespace])) As Boolean Implements ICollection(Of KeyValuePair(Of String, [Namespace])).Contains
            Return Me._NamespaceTable.ContainsKey(item.Key.ToLower)
        End Function

        Public Sub CopyTo(array() As KeyValuePair(Of String, [Namespace]), arrayIndex As Integer) Implements ICollection(Of KeyValuePair(Of String, [Namespace])).CopyTo
            Call Me._NamespaceTable.ToArray.CopyTo(array, arrayIndex)
        End Sub

        Public Function Remove(item As KeyValuePair(Of String, [Namespace])) As Boolean Implements ICollection(Of KeyValuePair(Of String, [Namespace])).Remove
            Return Me._NamespaceTable.Remove(item.Key.ToLower)
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, [Namespace])) _
            Implements IEnumerable(Of KeyValuePair(Of String, [Namespace])).GetEnumerator

            For Each obj In Me._NamespaceTable
                Yield obj
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
#End Region

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call UpdateDb()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace