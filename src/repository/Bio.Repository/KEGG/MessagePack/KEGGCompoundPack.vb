Imports System.IO
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Namespace KEGG.Metabolism

    ''' <summary>
    ''' the schema of <see cref="Compound"/>
    ''' </summary>
    Public Class KEGGCompoundPack ： Inherits SchemaProvider(Of Compound)

        Protected Overrides Function GetObjectSchema() As Dictionary(Of String, Integer)
            Return New Dictionary(Of String, Integer) From {
                {NameOf(Compound.entry), 0},
                {NameOf(Compound.commonNames), 1},
                {NameOf(Compound.formula), 2},
                {NameOf(Compound.exactMass), 3},
                {NameOf(Compound.reactionId), 4},
                {NameOf(Compound.pathway), 5},
                {NameOf(Compound.Module), 6},
                {NameOf(Compound.remarks), 7},
                {NameOf(Compound.enzyme), 8},
                {NameOf(Compound.category), 9},
                {NameOf(Compound.DbLinks), 10},
                {NameOf(Compound.KCF), 11}
            }
        End Function

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