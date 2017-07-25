Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Scripting.Runtime

Namespace ComponentModel.DBLinkBuilder

    <AttributeUsage(AttributeTargets.Property)>
    Public Class Xref : Inherits Attribute
        Implements INamedValue

        Public Property Name As String Implements IKeyedEntity(Of String).Key

        Public Overrides Function ToString() As String
            Return "Xref Link"
        End Function

        Public Shared Function GetProperties(type As Type) As PropertyInfo()
            Return type _
                .Schema(PropertyAccess.ReadWrite, PublicProperty, True) _
                .Values _
                .Where(Function([property])
                           Return Not [property].GetCustomAttribute(Of Xref) Is Nothing
                       End Function) _
                .ToArray
        End Function

        Public Shared Function CreateDictionary(Of T)() As Func(Of T, Dictionary(Of String, String))
            Dim properties As PropertyInfo() = Xref.GetProperties(GetType(T))
            Dim reader As Dictionary(Of String, PropertyInfo) = properties _
                .ToDictionary(Function(name)
                                  Dim xref As Xref = name.GetCustomAttribute(Of Xref)

                                  If xref.Name.StringEmpty Then
                                      Return name.Name
                                  Else
                                      Return xref.Name
                                  End If
                              End Function)
            Return Function(x)
                       Return reader _
                           .ToDictionary(Function(key) key.Key,
                                         Function(read)
                                             Return CStrSafe(read.Value.GetValue(x))
                                         End Function)
                   End Function
        End Function
    End Class
End Namespace