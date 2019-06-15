#Region "Microsoft.VisualBasic::0e88f3f737dd5745b04d4103c7cee656, Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\Metabolites\Glycan.vb"

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

    '     Class Glycan
    ' 
    '         Properties: Composition, CompoundId, Mass, Orthology
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: Download, DownloadFrom, GetLinkDbRDF, ToCompound
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.WebQuery.Compounds
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace Assembly.KEGG.DBGET.bGetObject

    <XmlRoot("KEGG.Glycan", Namespace:="http://www.kegg.jp/dbget-bin/www_bget?gl:glycan_id")>
    Public Class Glycan : Inherits Compound

        Public Property Composition As String
        Public Property Mass As String
        Public Property Orthology As KeyValuePair()

        Public ReadOnly Property CompoundId As String()
            Get
                If Remarks.IsNullOrEmpty Then
                    Return {}
                End If

                Dim sameAs$ = Remarks.Select(Function(s)
                                                 Return s.GetTagValue(":"c, trim:=True)
                                             End Function) _
                                     .Where(Function(t) t.Name = "Same as") _
                                     .FirstOrDefault _
                                     .Value

                If sameAs.StringEmpty Then
                    Return {}
                Else
                    Return sameAs.Split _
                                 .Select(AddressOf Trim) _
                                 .Where(Function(id) id.First = "C"c) _
                                 .ToArray
                End If
            End Get
        End Property

        Sub New()
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(links As DBLinks)
            MyBase._DBLinks = links
        End Sub

        Const URL = "http://www.kegg.jp/dbget-bin/www_bget?gl:{0}"

        ''' <summary>
        ''' 使用glycan编号来下载数据模型
        ''' </summary>
        ''' <param name="ID$"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Function Download(ID$) As Glycan
            Return DownloadFrom(url:=String.Format(URL, ID))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Function GetLinkDbRDF(glycan As Glycan) As IEnumerable(Of LinkDB.Relationship)
            If InStr(glycan.Entry, ":") > 0 Then
                Return LinkDB.Relationship.GetLinkDb(glycan.Entry)
            Else
                Return LinkDB.Relationship.GetLinkDb($"gl:{glycan.Entry}")
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Function DownloadFrom(url As String) As Glycan
            Return GlycanParser.ParseGlycan(New WebForm(url))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ToCompound() As Compound
            Return New Compound With {
                .Entry = Entry,
                .CommonNames = CommonNames,
                .DbLinks = DbLinks,
                .Formula = Me.Composition,
                .reactionId = reactionId,
                .Module = Me.Module,
                .MolWeight = Val(Mass),
                .Pathway = Pathway,
                .Enzyme = Enzyme,
                .ExactMass = .MolWeight,
                .Remarks = .Remarks
            }
        End Function
    End Class
End Namespace
