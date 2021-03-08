Imports System.IO
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Namespace KEGG.Metabolism

    ''' <summary>
    ''' the schema of <see cref="Compound"/>
    ''' </summary>
    Public Class KEGGCompoundPack ： Inherits SchemaProvider(Of Compound)

        Protected Overrides Function GetObjectSchema() As Dictionary(Of String, NilImplication)
            Return New Dictionary(Of String, NilImplication) From {
                {NameOf(Compound.entry), NilImplication.MemberDefault},
                {NameOf(Compound.commonNames), NilImplication.MemberDefault},
                {NameOf(Compound.formula), NilImplication.MemberDefault},
                {NameOf(Compound.exactMass), NilImplication.MemberDefault},
                {NameOf(Compound.reactionId), NilImplication.MemberDefault},
                {NameOf(Compound.pathway), NilImplication.MemberDefault},
                {NameOf(Compound.Module), NilImplication.MemberDefault},
                {NameOf(Compound.remarks), NilImplication.MemberDefault},
                {NameOf(Compound.enzyme), NilImplication.MemberDefault},
                {NameOf(Compound.category), NilImplication.MemberDefault},
                {NameOf(Compound.DbLinks), NilImplication.MemberDefault},
                {NameOf(Compound.KCF), NilImplication.MemberDefault}
            }
        End Function

        Shared Sub New()
            Call MsgPackSerializer.DefaultContext.RegisterSerializer(New KEGGCompoundPack)
        End Sub

        Public Shared Function ReadKeggDb(file As Stream) As Compound()
            Return MsgPackSerializer.Deserialize(Of Compound())(file)
        End Function

        Public Shared Function WriteKeggDb(db As Compound(), file As Stream) As Boolean
            Try
                Call MsgPackSerializer.SerializeObject(db, file)
            Catch ex As Exception
                Call App.LogException(ex)
                Return False
            End Try

            Return True
        End Function
    End Class
End Namespace