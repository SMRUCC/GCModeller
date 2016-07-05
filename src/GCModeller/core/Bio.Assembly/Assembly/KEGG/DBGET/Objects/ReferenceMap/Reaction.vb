#Region "Microsoft.VisualBasic::5a33ed2b1c0e9215677e3c8a35810b4d, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\ReferenceMap\Reaction.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Assembly.KEGG.DBGET.ReferenceMap

    <XmlType("KEGG-RefRxnDef", Namespace:="http://code.google.com/p/genome-in-code/kegg/reference_reaction")>
    Public Class ReferenceReaction : Inherits bGetObject.Reaction

        ''' <summary>
        ''' 酶分子的直系同源的参考序列
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SSDBs As KeyValuePairObject(Of String, QueryEntry())()

        Const ENTRY_PATTERN As String = "<a href=""/dbget-bin/www_bget\?ko:K\d+"

        Public Overloads Shared Function Download(Entry As WebServices.ListEntry) As ReferenceMap.ReferenceReaction
            Dim WebForm As New WebForm(Url:=Entry.Url)

            If WebForm.Count = 0 Then
                Return Nothing
            End If

            Dim refReaction = __webFormParser(Of ReferenceMap.ReferenceReaction)(WebForm)
            Dim sValue As String = WebForm("Orthology").FirstOrDefault

            If Not String.IsNullOrEmpty(sValue) Then
                Dim OrthologyEntries = (From m As Match
                                        In Regex.Matches(sValue, ENTRY_PATTERN, RegexOptions.IgnoreCase)
                                        Select m.Value.Split(CChar(":")).Last).ToArray
                Dim GeneList = (From EntryID As String
                                In OrthologyEntries
                                Select New KeyValuePairObject(Of String, QueryEntry()) With {
                                    .Key = EntryID,
                                    .Value = DBGET.bGetObject.SSDB.API.HandleDownload(KO_ID:=EntryID)}).ToArray
                refReaction.SSDBs = GeneList
            End If

            Return refReaction
        End Function
    End Class
End Namespace
