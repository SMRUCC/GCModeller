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
    End Class
End Namespace