Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Namespace KEGG.Metabolism

    Public Class KEGGMapPack : Inherits SchemaProvider(Of Map)

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
    End Class
End Namespace