#Region "Microsoft.VisualBasic::858fc6f9731b608ef9d7e2acbaed4097, Bio.Repository\KEGG\MessagePack\ReactionClassPack.vb"

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

    '     Class ReactionClassPack
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetObjectSchema, ReactionObj, (+2 Overloads) ReadKeggDb, transformObj, WriteKeggDb
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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

        Public Shared Function ReadKeggDb(file As String) As ReactionClass()
            Using buffer As Stream = file.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                Return ReadKeggDb(buffer)
            End Using
        End Function

        Public Shared Function ReadKeggDb(file As Stream) As ReactionClass()
            Return MsgPackSerializer.Deserialize(Of ReactionClass())(file)
        End Function

        Public Shared Function WriteKeggDb(reactions As IEnumerable(Of ReactionClass), file As Stream) As Boolean
            Try
                Call MsgPackSerializer.SerializeObject(reactions.ToArray, file)
                Call file.Flush()
            Catch ex As Exception
                Call App.LogException(ex)
                Return False
            End Try

            Return True
        End Function
    End Class
End Namespace
