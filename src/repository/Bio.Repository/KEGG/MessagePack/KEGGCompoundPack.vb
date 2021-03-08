Imports System.IO
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace KEGG.Metabolism

    ''' <summary>
    ''' the schema of <see cref="Compound"/>
    ''' </summary>
    Public Class KEGGCompoundPack : Inherits SchemaProvider(Of Compound)

        Protected Overrides Iterator Function GetObjectSchema() As IEnumerable(Of (obj As Type, schema As Dictionary(Of String, NilImplication)))
            Yield (GetType(Compound), CompoundObj)
            Yield (GetType(NamedValue), entryObj)
            Yield (GetType(DBLink), dblinkObj)
        End Function

        Protected Function CompoundObj() As Dictionary(Of String, NilImplication)
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
                {NameOf(Compound.DbLinks), NilImplication.MemberDefault},
                {NameOf(Compound.KCF), NilImplication.MemberDefault}
            }
        End Function

        Friend Shared Function entryObj() As Dictionary(Of String, NilImplication)
            Return New Dictionary(Of String, NilImplication) From {
                {NameOf(NamedValue.name), NilImplication.MemberDefault},
                {NameOf(NamedValue.text), NilImplication.MemberDefault}
            }
        End Function

        Protected Function dblinkObj() As Dictionary(Of String, NilImplication)
            Return New Dictionary(Of String, NilImplication) From {
                {NameOf(DBLink.DBName), NilImplication.MemberDefault},
                {NameOf(DBLink.Entry), NilImplication.MemberDefault}
            }
        End Function

        Shared Sub New()
            Call MsgPackSerializer.DefaultContext.RegisterSerializer(New KEGGCompoundPack)
        End Sub

        Public Shared Function ReadKeggDb(file As Stream) As Compound()
            Return MsgPackSerializer.Deserialize(Of Compound())(file)
        End Function

        Public Shared Function WriteKeggDb(cpds As IEnumerable(Of Compound), file As Stream) As Boolean
            Try
                Call MsgPackSerializer.SerializeObject(cpds.ToArray, file)
            Catch ex As Exception
                Call App.LogException(ex)
                Return False
            End Try

            Return True
        End Function
    End Class
End Namespace