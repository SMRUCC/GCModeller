Imports System.Text

Namespace DelegateHandlers.TypeLibraryRegistry.RegistryNodes

    ''' <summary>
    ''' Namespace object for the imports operations
    ''' </summary>
    ''' <remarks></remarks>
    Public Class [Module] : Inherits Microsoft.VisualBasic.CommandLine.Reflection.Namespace
        Implements IReadOnlyDictionary(Of String, System.Reflection.MethodInfo())

        Dim _InternalHashList As Dictionary(Of String, Microsoft.VisualBasic.CommandLine.Reflection.EntryPoints.CommandEntryPointInfo()) =
            New Dictionary(Of String, Microsoft.VisualBasic.CommandLine.Reflection.EntryPoints.CommandEntryPointInfo())
        Protected Friend _InternalOriginalAssemblys As Dictionary(Of System.Type, CommandLine.Reflection.Namespace)

        Public Sub New(EntryInfo As Microsoft.VisualBasic.CommandLine.Reflection.[Namespace], Type As System.Type)
            Call MyBase.New(EntryInfo.Namespace)

            Dim InternalGroupedCommands = (From cmdl As Microsoft.VisualBasic.CommandLine.Reflection.EntryPoints.CommandEntryPointInfo
                                           In Microsoft.VisualBasic.CommandLine.Interpreter.GetAllCommands(Type)
                                           Select cmdl
                                           Group By cmdl.Name.ToLower Into Group).ToArray
            For Each CommandInfo In InternalGroupedCommands
                Call _InternalHashList.Add(CommandInfo.ToLower, CommandInfo.Group.ToArray)
            Next

            _InternalOriginalAssemblys = New Dictionary(Of Type, CommandLine.Reflection.Namespace) From {{Type, EntryInfo}}
            Description = EntryInfo.Description
        End Sub

        Public ReadOnly Property OriginalAssemblys() As KeyValuePair(Of System.Type, String)()
            Get
                Return (From item In _InternalOriginalAssemblys Select New KeyValuePair(Of System.Type, String)(item.Key, item.Value.Description)).ToArray
            End Get
        End Property

        Public Function MergeNamespace(EntryInfo As Microsoft.VisualBasic.CommandLine.Reflection.[Namespace], Type As System.Type) As [Module]
            Dim groupedCommands = (From cmdl In Microsoft.VisualBasic.CommandLine.Interpreter.GetAllCommands(Type) Select cmdl Group By cmdl.Name Into Group).ToArray
            For Each CommandInfo In groupedCommands
                Dim Name As String = CommandInfo.Name.ToLower
                Dim TempBuffer = CommandInfo.Group.ToArray

                If _InternalHashList.ContainsKey(Name) Then
                    TempBuffer = {TempBuffer, _InternalHashList(Name)}.MatrixToVector
                    Call _InternalHashList.Remove(Name)
                End If
                Call _InternalHashList.Add(CommandInfo.Name.ToLower, TempBuffer)
            Next

            Call _InternalOriginalAssemblys.Add(Type, EntryInfo)
            Description &= vbCrLf & vbCrLf & EntryInfo.Description

            Return Me
        End Function

        Public Overrides Function ToString() As String
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            For Each Item As String In _InternalHashList.Keys
                Call sBuilder.AppendFormat("{0}; ", Item)
            Next
            Return String.Format("[{0}]  {1}", [Namespace], sBuilder.ToString)
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, System.Reflection.MethodInfo())) Implements IEnumerable(Of KeyValuePair(Of String, System.Reflection.MethodInfo())).GetEnumerator
            For Each Key As String In _InternalHashList.Keys
                Dim EntryPoints = (From entry In _InternalHashList(Key) Select entry.MethodEntryPoint).ToArray
                Yield New KeyValuePair(Of String, System.Reflection.MethodInfo())(Key, EntryPoints)
            Next
        End Function

        Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of KeyValuePair(Of String, System.Reflection.MethodInfo())).Count
            Get
                Return _InternalHashList.Count
            End Get
        End Property

        Public Function ContainsKey(key As String) As Boolean Implements IReadOnlyDictionary(Of String, System.Reflection.MethodInfo()).ContainsKey
            Return _InternalHashList.ContainsKey(key.ToLower)
        End Function

        ''' <summary>
        ''' Gets a command entry point from the registry table using its <paramref name="key">name</paramref> property. 
        ''' </summary>
        ''' <param name="key"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Default Public ReadOnly Property Item(key As String) As System.Reflection.MethodInfo() Implements IReadOnlyDictionary(Of String, System.Reflection.MethodInfo()).Item
            Get
                Dim keyName As String = key.ToLower

                If Not _InternalHashList.ContainsKey(keyName) Then
                    Throw New ShoalShell.Runtime.Objects.ObjectModels.Exceptions.MethodNotFoundException(key, "")
                End If

                Return (From entry As CommandLine.Reflection.EntryPoints.CommandEntryPointInfo
                        In _InternalHashList(keyName)
                        Select entry.MethodEntryPoint).ToArray
            End Get
        End Property

        Public ReadOnly Property Keys As IEnumerable(Of String) Implements IReadOnlyDictionary(Of String, System.Reflection.MethodInfo()).Keys
            Get
                Return _InternalHashList.Keys
            End Get
        End Property

        Public Function TryGetValue(key As String, ByRef value As System.Reflection.MethodInfo()) As Boolean Implements IReadOnlyDictionary(Of String, System.Reflection.MethodInfo()).TryGetValue
            Dim CommandInfo As Microsoft.VisualBasic.CommandLine.Reflection.EntryPoints.CommandEntryPointInfo() = Nothing
            Dim f As Boolean = _InternalHashList.TryGetValue(key, CommandInfo)
            value = (From entry In CommandInfo Select entry.MethodEntryPoint).ToArray

            Return f
        End Function

        Public ReadOnly Property Values As IEnumerable(Of System.Reflection.MethodInfo()) Implements IReadOnlyDictionary(Of String, System.Reflection.MethodInfo()).Values
            Get
                Return (From item In _InternalHashList.Values Select (From entry In item Select entry.MethodEntryPoint).ToArray).ToArray
            End Get
        End Property

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace