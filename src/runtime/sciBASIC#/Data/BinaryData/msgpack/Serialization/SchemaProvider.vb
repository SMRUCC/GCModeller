Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace Serialization

    Public MustInherit Class SchemaProvider(Of T As Class)

        Shared ReadOnly slotList As Dictionary(Of String, PropertyInfo) = DataFramework.Schema(Of T)(
            flag:=PropertyAccess.ReadWrite,
            nonIndex:=True,
            primitive:=False,
            binds:=PublicProperty
        )

        ''' <summary>
        ''' provides a schema table for base object for generates 
        ''' a sequence of <see cref="MessagePackMemberAttribute"/>
        ''' </summary>
        ''' <returns></returns>
        Protected MustOverride Function GetObjectSchema() As Dictionary(Of String, Integer)

        Public Iterator Function GetMembers() As IEnumerable(Of BindProperty(Of MessagePackMemberAttribute))
            For Each item In GetObjectSchema()
                If Not slotList.ContainsKey(item.Key) Then
                    Throw New NotImplementedException($"invalid member name: {item.Key}!")
                End If

                Dim attr As New MessagePackMemberAttribute(item.Value)
                Dim bind As New BindProperty(Of MessagePackMemberAttribute)(attr, slotList(item.Key))

                Yield bind
            Next
        End Function

    End Class
End Namespace