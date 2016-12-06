Imports System.Dynamic
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.LINQ.Framework

Namespace Script

    Public Class Variable : Implements INamedValue
        Implements IEnumerable

        Public Property Name As String Implements INamedValue.Key
        Public Property Data As IEnumerable

        Public Overrides Function ToString() As String
            Return Name
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield Data.GetEnumerator
        End Function

        Public Shared Operator +(hash As Dictionary(Of String, Variable), x As Variable) As Dictionary(Of String, Variable)
            x.Name = x.Name.ToLower
            If hash.ContainsKey(x.Name) Then
                Call hash.Remove(x.Name)
            End If
            Call hash.Add(x.Name, x)
            Return hash
        End Operator
    End Class
End Namespace