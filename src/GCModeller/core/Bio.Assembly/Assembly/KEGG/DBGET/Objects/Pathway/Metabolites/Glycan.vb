#Region "Microsoft.VisualBasic::277738347653512c6c03fabf00aa2237, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\Glycan.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
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
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.HtmlParser
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports SMRUCC.genomics.ComponentModel.EquaionModel

Namespace Assembly.KEGG.DBGET.bGetObject

    <XmlRoot("KEGG.Glycan", Namespace:="http://www.kegg.jp/dbget-bin/www_bget?gl:glycan_id")>
    Public Class Glycan : Inherits Compound
        Implements ICompoundObject

        Public Property Composition As String
        Public Property Mass As String

        Dim _DBLinks As DBLinks

        Public Property Reactions As String()

        Const URL = "http://www.kegg.jp/dbget-bin/www_bget?gl:{0}"

        Public Overloads Shared Function Download(Id As String) As Glycan
            Return DownloadFrom(url:=String.Format(URL, Id))
        End Function

        Const show_pathway As String = "<a href="".*/kegg-bin/show_pathway\?.+?"">.+?</a>"
        Const show_module As String = "<a href="".*/kegg-bin/show_module\?.+?"">.+?</a>"

        Public Overloads Shared Function DownloadFrom(url As String) As Glycan
            Dim WebForm As New WebForm(url)
            Dim pathways = WebForm.GetValue("Pathway").FirstOrDefault
            Dim modules = WebForm.GetValue("Module").FirstOrDefault
            Dim Compound As New Glycan With {
                .Entry = Regex.Match(WebForm.GetValue("Entry").FirstOrDefault, "[GC]\d+").Value
            }
            Compound.CommonNames = MetabolitesDBGet.GetCommonNames(WebForm.GetValue("Name").FirstOrDefault())
            Compound.Composition = WebForm.GetValue("Composition").FirstOrDefault.Replace("<br>", "")
            Compound.Reactions = WebForm.GetValue("Reaction").FirstOrDefault.GetLinks
            Compound.Pathway = LinqAPI.Exec(Of String) <=
 _
                From x As KeyValuePair
                In InternalWebFormParsers.WebForm.parseList(pathways, show_pathway)
                Select String.Format("[{0}] {1}", x.Key, x.Value)

            Compound.Module = LinqAPI.Exec(Of String) <=
 _
                From x As KeyValuePair
                In InternalWebFormParsers.WebForm.parseList(modules, show_module)
                Select String.Format("[{0}] {1}", x.Key, x.Value)

            Compound._DBLinks = MetabolitesDBGet.GetDBLinks(WebForm.GetValue("Other DBs").FirstOrDefault)
            Compound.Mass = Val(WebForm.GetValue("Mass").FirstOrDefault)

            If Compound.CommonNames.IsNullOrEmpty Then
                Compound.CommonNames = New String() {Compound.Composition}
            End If

            Return Compound
        End Function

        Public Function ToCompound() As Compound
            Return New Compound With {
                .Entry = Entry,
                .CommonNames = CommonNames,
                .DbLinks = DbLinks,
                .Formula = Me.Composition,
                .KEGG_reaction = Reactions,
                .Module = Me.Module,
                .MolWeight = Val(Mass),
                .Pathway = Pathway
            }
        End Function
    End Class
End Namespace
