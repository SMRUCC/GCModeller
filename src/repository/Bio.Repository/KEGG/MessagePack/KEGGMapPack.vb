Imports System.IO
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Namespace KEGG.Metabolism

    Public Class KEGGMapPack : Inherits SchemaProvider(Of Map)

        Shared Sub New()
            Call MsgPackSerializer.DefaultContext.RegisterSerializer(New KEGGMapPack)
        End Sub

        Protected Overrides Iterator Function GetObjectSchema() As IEnumerable(Of (obj As Type, schema As Dictionary(Of String, NilImplication)))
            Yield (GetType(Map), GetMapSchema)
            Yield (GetType(Area), GetShapeSchema)
        End Function

        Private Shared Function GetMapSchema() As Dictionary(Of String, NilImplication)
            Return New Dictionary(Of String, NilImplication) From {
                {NameOf(Map.id), NilImplication.MemberDefault},
                {NameOf(Map.Name), NilImplication.MemberDefault},
                {NameOf(Map.URL), NilImplication.MemberDefault},
                {NameOf(Map.description), NilImplication.MemberDefault},
                {NameOf(Map.PathwayImage), NilImplication.MemberDefault},
                {NameOf(Map.shapes), NilImplication.MemberDefault}
            }
        End Function

        Private Shared Function GetShapeSchema() As Dictionary(Of String, NilImplication)
            Return New Dictionary(Of String, NilImplication) From {
                {NameOf(Area.class), NilImplication.MemberDefault},
                {NameOf(Area.coords), NilImplication.MemberDefault},
                {NameOf(Area.data_coords), NilImplication.MemberDefault},
                {NameOf(Area.data_id), NilImplication.MemberDefault},
                {NameOf(Area.entry), NilImplication.MemberDefault},
                {NameOf(Area.href), NilImplication.MemberDefault},
                {NameOf(Area.moduleId), NilImplication.MemberDefault},
                {NameOf(Area.refid), NilImplication.MemberDefault},
                {NameOf(Area.shape), NilImplication.MemberDefault},
                {NameOf(Area.title), NilImplication.MemberDefault}
            }
        End Function

        Public Shared Function ReadKeggDb(file As Stream) As Map()
            Return MsgPackSerializer.Deserialize(Of Map())(file)
        End Function

        Public Shared Function WriteKeggDb(maps As IEnumerable(Of Map), file As Stream) As Boolean
            Try
                Call MsgPackSerializer.SerializeObject(maps.ToArray, file)
                Call file.Flush()
            Catch ex As Exception
                Call App.LogException(ex)
                Return False
            End Try

            Return True
        End Function
    End Class
End Namespace