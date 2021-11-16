#Region "Microsoft.VisualBasic::4835e13704d4b872964bbba3b075af3c, Bio.Repository\KEGG\MessagePack\KEGGPathwayPack.vb"

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

    '     Class KEGGPathwayPack
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: GetObjectSchema, GetPathwayModel, (+2 Overloads) ReadKeggDb, ReferenceModel, TupleModel
    '                   WriteKeggDb
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports System.IO
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace KEGG.Metabolism

    Public Class KEGGPathwayPack : Inherits SchemaProvider(Of Pathway)

        Public Sub New()
        End Sub

        Protected Overrides Iterator Function GetObjectSchema() As IEnumerable(Of (obj As Type, schema As Dictionary(Of String, NilImplication)))
            Yield (GetType(Pathway), GetPathwayModel)
            Yield (GetType(NamedValue), KEGGCompoundPack.entryObj)
            Yield (GetType(DBLink), KEGGCompoundPack.dblinkObj)
            Yield (GetType(KeyValuePair), TupleModel)
            Yield (GetType(Reference), ReferenceModel)
        End Function

        Friend Shared Function ReferenceModel() As Dictionary(Of String, NilImplication)
            Return New Dictionary(Of String, NilImplication) From {
                {NameOf(Reference.DOI), NilImplication.MemberDefault},
                {NameOf(Reference.Title), NilImplication.MemberDefault},
                {NameOf(Reference.Journal), NilImplication.MemberDefault},
                {NameOf(Reference.Reference), NilImplication.MemberDefault},
                {NameOf(Reference.Authors), NilImplication.MemberDefault}
            }
        End Function

        Friend Shared Function TupleModel() As Dictionary(Of String, NilImplication)
            Return New Dictionary(Of String, NilImplication) From {
                {NameOf(KeyValuePair.Key), NilImplication.MemberDefault},
                {NameOf(KeyValuePair.Value), NilImplication.MemberDefault}
            }
        End Function

        Private Function GetPathwayModel() As Dictionary(Of String, NilImplication)
            Return New Dictionary(Of String, NilImplication) From {
                {NameOf(Pathway.EntryId), NilImplication.MemberDefault},
                {NameOf(Pathway.compound), NilImplication.MemberDefault},
                {NameOf(Pathway.description), NilImplication.MemberDefault},
                {NameOf(Pathway.disease), NilImplication.MemberDefault},
                {NameOf(Pathway.drugs), NilImplication.MemberDefault},
                {NameOf(Pathway.genes), NilImplication.MemberDefault},
                {NameOf(Pathway.KOpathway), NilImplication.MemberDefault},
                {NameOf(Pathway.modules), NilImplication.MemberDefault},
                {NameOf(Pathway.name), NilImplication.MemberDefault},
                {NameOf(Pathway.organism), NilImplication.MemberDefault},
                {NameOf(Pathway.otherDBs), NilImplication.MemberDefault},
                {NameOf(Pathway.pathwayMap), NilImplication.MemberDefault},
                {NameOf(Pathway.references), NilImplication.MemberDefault}
            }
        End Function

        Shared Sub New()
            Call MsgPackSerializer.DefaultContext.RegisterSerializer(New KEGGPathwayPack)
        End Sub

        Public Shared Function ReadKeggDb(file As String) As Pathway()
            Using buffer As Stream = file.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                Return ReadKeggDb(buffer)
            End Using
        End Function

        Public Shared Function ReadKeggDb(file As Stream) As Pathway()
            Return MsgPackSerializer.Deserialize(Of Pathway())(file)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pathways"></param>
        ''' <param name="file"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' data will be auto flush to <paramref name="file"/>.
        ''' </remarks>
        Public Shared Function WriteKeggDb(pathways As IEnumerable(Of Pathway), file As Stream) As Boolean
            Try
                Call MsgPackSerializer.SerializeObject(pathways.ToArray, file)
                Call file.Flush()
            Catch ex As Exception
                Call App.LogException(ex)
                Return False
            End Try

            Return True
        End Function
    End Class
End Namespace
