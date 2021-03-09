Imports System.IO
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Namespace KEGG.Metabolism

    Public Class ReactionClassPack : Inherits SchemaProvider(Of ReactionClass)

        Protected Overrides Iterator Function GetObjectSchema() As IEnumerable(Of (obj As Type, schema As Dictionary(Of String, NilImplication)))
            Yield (GetType(NamedValue), KEGGCompoundPack.entryObj)
            Yield (GetType(ReactionClass), ReactionObj)
            Yield (GetType(ReactionCompoundTransform), transformObj)
        End Function

        Private Function ReactionObj() As Dictionary(Of String, NilImplication)
            Return New Dictionary(Of String, NilImplication) From {
                {NameOf(ReactionClass.entryId), NilImplication.MemberDefault},
                {NameOf(ReactionClass.definition), NilImplication.MemberDefault},
                {NameOf(ReactionClass.reactantPairs), NilImplication.MemberDefault},
                {NameOf(ReactionClass.reactions), NilImplication.MemberDefault},
                {NameOf(ReactionClass.enzymes), NilImplication.MemberDefault},
                {NameOf(ReactionClass.pathways), NilImplication.MemberDefault},
                {NameOf(ReactionClass.orthology), NilImplication.MemberDefault},
                {NameOf(ReactionClass.category), NilImplication.MemberDefault}
            }
        End Function

        Private Function transformObj() As Dictionary(Of String, NilImplication)
            Return New Dictionary(Of String, NilImplication) From {
                {NameOf(ReactionCompoundTransform.from), NilImplication.MemberDefault},
                {NameOf(ReactionCompoundTransform.to), NilImplication.MemberDefault}
            }
        End Function

        Shared Sub New()
            Call MsgPackSerializer.DefaultContext.RegisterSerializer(New ReactionClassPack)
        End Sub

        Public Shared Function ReadKeggDb(file As Stream) As ReactionClass()
            Return MsgPackSerializer.Deserialize(Of ReactionClass())(file)
        End Function

        Public Shared Function WriteKeggDb(reactions As IEnumerable(Of ReactionClass), file As Stream) As Boolean
            Try
                Call MsgPackSerializer.SerializeObject(reactions.ToArray, file)
            Catch ex As Exception
                Call App.LogException(ex)
                Return False
            End Try

            Return True
        End Function
    End Class
End Namespace