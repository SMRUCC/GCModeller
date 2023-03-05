#Region "Microsoft.VisualBasic::33be84876d7d68e7bfe34f5582ae7be1, Bio.Repository\KEGG\MessagePack\KEGGCompoundPack.vb"

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

    '     Class KEGGCompoundPack
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CompoundObj, dblinkObj, entryObj, GetObjectSchema, (+2 Overloads) ReadKeggDb
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

        ''' <summary>
        ''' <see cref="NamedValue"/>
        ''' </summary>
        ''' <returns></returns>
        Friend Shared Function entryObj() As Dictionary(Of String, NilImplication)
            Return New Dictionary(Of String, NilImplication) From {
                {NameOf(NamedValue.name), NilImplication.MemberDefault},
                {NameOf(NamedValue.text), NilImplication.MemberDefault}
            }
        End Function

        ''' <summary>
        ''' <see cref="DBLink"/>
        ''' </summary>
        ''' <returns></returns>
        Protected Friend Shared Function dblinkObj() As Dictionary(Of String, NilImplication)
            Return New Dictionary(Of String, NilImplication) From {
                {NameOf(DBLink.DBName), NilImplication.MemberDefault},
                {NameOf(DBLink.Entry), NilImplication.MemberDefault}
            }
        End Function

        Shared Sub New()
            Call MsgPackSerializer.DefaultContext.RegisterSerializer(New KEGGCompoundPack)
        End Sub

        Public Shared Function ReadKeggDb(file As String) As Compound()
            Using buffer As Stream = file.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                Return ReadKeggDb(buffer)
            End Using
        End Function

        Public Shared Function ReadKeggDb(file As Stream) As Compound()
            Return MsgPackSerializer.Deserialize(Of Compound())(file)
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
        Public Shared Function WriteKeggDb(cpds As IEnumerable(Of Compound), file As Stream) As Boolean
            Try
                Call MsgPackSerializer.SerializeObject(cpds.GroupBy(Function(c) c.entry).Select(Function(c) c.First).ToArray, file)
                Call file.Flush()
            Catch ex As Exception
                Call App.LogException(ex)
                Return False
            End Try

            Return True
        End Function
    End Class
End Namespace
