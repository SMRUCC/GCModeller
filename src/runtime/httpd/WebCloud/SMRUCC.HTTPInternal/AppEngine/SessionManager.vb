Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace AppEngine

    Public Module SessionManager

        ReadOnly random As New Random

        Public Function NewSession() As Session
            Dim name$ = RandomASCIIString(32, skipSymbols:=True, seed:=random)
            Dim emptyTable As New Dictionary(Of String, Value)
            Dim session As New Session With {
                .ID = name,
                .Table = emptyTable
            }
            Dim path$ = GetSessionPath(name)

            Call session.GetJson.SaveTo(path)

            Return session
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Sub Save(session As Session)
            Call session.GetJson.SaveTo(GetSessionPath(id:=session.ID))
        End Sub

        ''' <summary>
        ''' 得到session json文件的文件路径
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Public Function GetSessionPath(id As String) As String
            Dim dir$ = App.ProductSharedDIR & "/Sessions"
            Dim path$ = $"{dir}/{id}.json"
            Return path
        End Function

        Public Function GetSession(id As String) As Session
            Dim path$ = GetSessionPath(id)

            If path.FileExists Then
                Return path.LoadObject(Of Session)
            Else
                Return New Session
            End If
        End Function
    End Module

    Public Class Session : Implements INamedValue

        Public Property ID As String Implements INamedValue.Key
        Public Property Table As Dictionary(Of String, Value)

        Default Public Property Item(name As String) As Value
            Get
                Return Table.TryGetValue(name)
            End Get
            Set(value As Value)
                Table(name) = value
            End Set
        End Property

        Public Sub SetValue(key$, value$)
            Item(key) = value
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{ID} => {Table.Keys.ToArray.GetJson}]"
        End Function
    End Class

    Public Class Value

        Public Property Value As String
        Public Property Table As Dictionary(Of String, Value)

        Public Overrides Function ToString() As String
            If Table.IsNullOrEmpty Then
                Return Value
            Else
                Return Table.GetJson
            End If
        End Function

        Public Shared Widening Operator CType(value As String) As Value
            Return New Value With {
                .Value = value,
                .Table = New Dictionary(Of String, Value)
            }
        End Operator
    End Class
End Namespace