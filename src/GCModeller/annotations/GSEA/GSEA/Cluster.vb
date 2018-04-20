Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Public Class Cluster : Implements INamedValue

    Public Property Name As String Implements IKeyedEntity(Of String).Key
    Public Property Members As String()
        Get
            Return index.Objects
        End Get
        Set(value As String())
            index = value
        End Set
    End Property

    Dim index As Index(Of String)

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Intersect(list As IEnumerable(Of String)) As IEnumerable(Of String)
        Return index.Intersect(collection:=list)
    End Function

    Public Overrides Function ToString() As String
        Return Name
    End Function
End Class

Public Class Genome : Implements INamedValue

    Public Property Name As String Implements IKeyedEntity(Of String).Key
    Public Property Clusters As Cluster()

    Public Overrides Function ToString() As String
        Return Name
    End Function
End Class