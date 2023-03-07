#Region "Microsoft.VisualBasic::4b19a6eebe4dd9cd592f0b8a110b2c66, Bio.Repository\KEGG\MessagePack\KEGGReactionPack.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '     Class KEGGReactionPack
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetObjectSchema, KOlink, reactionModel, (+2 Overloads) ReadKeggDb, TermData
    '                   WriteKeggDb
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
        ''' <param name="rxns"></param>
        ''' <param name="file"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' data will be auto flush to <paramref name="file"/>. and the reaction data
        ''' will be make unqiue automatically in this function.
        ''' </remarks>
        Public Shared Function WriteKeggDb(rxns As IEnumerable(Of Reaction), file As Stream) As Boolean
            Try
                Call MsgPackSerializer.SerializeObject(rxns.GroupBy(Function(r) r.ID).Select(Function(r) r.First).ToArray, file)
                Call file.Flush()
            Catch ex As Exception
                Call App.LogException(ex)
                Return False
            End Try

            Return True
        End Function
    End Class
End Namespace
