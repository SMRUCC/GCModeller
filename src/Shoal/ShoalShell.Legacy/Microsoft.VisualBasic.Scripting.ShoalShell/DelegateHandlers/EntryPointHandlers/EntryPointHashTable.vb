Namespace DelegateHandlers.EntryPointHandlers

    ''' <summary>
    ''' Imports Command, Delegate and Hybrid Scripting Delegate
    ''' </summary>
    ''' <remarks></remarks>
    Public Class EntryPointHashTable

        ''' <summary>
        ''' Key都为小写的
        ''' </summary>
        ''' <remarks></remarks>
        Dim _InternalHashDictionary As SortedDictionary(Of String, ShoalShell.DelegateHandlers.EntryPointHandlers.CommandMethodEntryPoint) = New SortedDictionary(Of String, CommandMethodEntryPoint)

        ''' <summary>
        ''' Key为小写的，Value为Key的原先的值，即没有经过ToLower方法修剪的值
        ''' </summary>
        ''' <remarks></remarks>
        Dim _InternalKeys As Dictionary(Of String, String) = New Dictionary(Of String, String)

        Public ReadOnly Property InternalKeys As Dictionary(Of String, String)
            Get
                Return _InternalKeys
            End Get
        End Property

        Public ReadOnly Property InternalHashDictionary As SortedDictionary(Of String, ShoalShell.DelegateHandlers.EntryPointHandlers.CommandMethodEntryPoint)
            Get
                Return _InternalHashDictionary
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Name">小写的</param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Default Public ReadOnly Property EntryPoint(Name As String) As DelegateHandlers.EntryPointHandlers.CommandMethodEntryPoint
            Get
                If _InternalHashDictionary.ContainsKey(Name) Then
                    Return _InternalHashDictionary(Name)
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="CommandName">小写的</param>
        ''' <param name="EntryPointInfo"></param>
        ''' <remarks></remarks>
        Public Sub InternalAddEntryPoint(CommandName As String, EntryPointInfo As DelegateHandlers.EntryPointHandlers.CommandMethodEntryPoint)
            Call _InternalHashDictionary.Add(CommandName, EntryPointInfo)        '挂载新的入口点
            Call _InternalKeys.Add(CommandName, EntryPointInfo.Name)
        End Sub
    End Class
End Namespace