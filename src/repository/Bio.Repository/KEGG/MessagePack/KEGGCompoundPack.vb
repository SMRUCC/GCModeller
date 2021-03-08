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
    End Class
End Namespace