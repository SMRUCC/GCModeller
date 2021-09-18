Imports System.IO
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports XmlProperty = Microsoft.VisualBasic.Text.Xml.Models.Property

Namespace KEGG.Metabolism

    Public Class KEGGReactionPack : Inherits SchemaProvider(Of Reaction)

        Shared Sub New()
            Call MsgPackSerializer.DefaultContext.RegisterSerializer(New KEGGReactionPack)
        End Sub

        Protected Overrides Iterator Function GetObjectSchema() As IEnumerable(Of (obj As Type, schema As Dictionary(Of String, NilImplication)))
            Yield (GetType(NamedValue), KEGGCompoundPack.entryObj)
            Yield (GetType(DBLink), KEGGCompoundPack.dblinkObj)
            Yield (GetType(Reaction), reactionModel)
            Yield (GetType(OrthologyTerms), KOlink)
            Yield (GetType(XmlProperty), TermData)
        End Function

        Private Shared Function KOlink() As Dictionary(Of String, NilImplication)
            Return New Dictionary(Of String, NilImplication) From {
                 {NameOf(OrthologyTerms.Terms), NilImplication.MemberDefault}
             }
        End Function

        Private Shared Function TermData() As Dictionary(Of String, NilImplication)
            Return New Dictionary(Of String, NilImplication) From {
                {NameOf(XmlProperty.name), NilImplication.MemberDefault},
                {NameOf(XmlProperty.value), NilImplication.MemberDefault},
                {NameOf(XmlProperty.comment), NilImplication.MemberDefault}
            }
        End Function

        Private Shared Function reactionModel() As Dictionary(Of String, NilImplication)
            Return New Dictionary(Of String, NilImplication) From {
                {NameOf(Reaction.ID), NilImplication.MemberDefault},
                {NameOf(Reaction.CommonNames), NilImplication.MemberDefault},
                {NameOf(Reaction.Definition), NilImplication.MemberDefault},
                {NameOf(Reaction.Equation), NilImplication.MemberDefault},
                {NameOf(Reaction.Enzyme), NilImplication.MemberDefault},
                {NameOf(Reaction.Comments), NilImplication.MemberDefault},
                {NameOf(Reaction.Pathway), NilImplication.MemberDefault},
                {NameOf(Reaction.Module), NilImplication.MemberDefault},
                {NameOf(Reaction.Class), NilImplication.MemberDefault},
                {NameOf(Reaction.DBLink), NilImplication.MemberDefault},
                {NameOf(Reaction.Orthology), NilImplication.MemberDefault}
            }
        End Function

        Public Shared Function ReadKeggDb(file As String) As Reaction()
            Using buffer As Stream = file.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                Return ReadKeggDb(buffer)
            End Using
        End Function

        Public Shared Function ReadKeggDb(file As Stream) As Reaction()
            Return MsgPackSerializer.Deserialize(Of Reaction())(file)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="cpds"></param>
        ''' <param name="file"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' data will be auto flush to <paramref name="file"/>.
        ''' </remarks>
        Public Shared Function WriteKeggDb(cpds As IEnumerable(Of Reaction), file As Stream) As Boolean
            Try
                Call MsgPackSerializer.SerializeObject(cpds.ToArray, file)
                Call file.Flush()
            Catch ex As Exception
                Call App.LogException(ex)
                Return False
            End Try

            Return True
        End Function
    End Class
End Namespace