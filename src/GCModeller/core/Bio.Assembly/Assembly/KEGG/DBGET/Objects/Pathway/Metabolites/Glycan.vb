#Region "Microsoft.VisualBasic::3687d4ef78b5a2933f41bbe3369fbe75, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\Metabolites\Glycan.vb"

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

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
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
        Public Property Orthology As KeyValuePair()

        Sub New()
        End Sub

        Sub New(links As DBLinks)
            MyBase._DBLinks = links
        End Sub

        Const URL = "http://www.kegg.jp/dbget-bin/www_bget?gl:{0}"

        ''' <summary>
        ''' 使用glycan编号来下载数据模型
        ''' </summary>
        ''' <param name="ID$"></param>
        ''' <returns></returns>
        Public Overloads Shared Function Download(ID$) As Glycan
            Return DownloadFrom(url:=String.Format(URL, ID))
        End Function

        Public Overloads Shared Function DownloadFrom(url As String) As Glycan
            Dim html As New WebForm(url)
            Dim base As Compound = html.ParseCompound
            Dim gl As New Glycan(base._DBLinks) With {
                .Entry = base.Entry,
                .CommonNames = base.CommonNames,
                .Remarks = base.Remarks,
                .Pathway = base.Pathway,
                .MolWeight = base.MolWeight,
                .Module = base.Module,
                .KEGG_reactions = base.KEGG_reactions,
                .ExactMass = base.ExactMass,
                .Formula = base.Formula,
                .Enzyme = base.Enzyme,
                .Composition = html.GetText("Composition"),
                .Mass = html.GetText("Mass"),
                .Orthology = __parseOrthology(html("Orthology").FirstOrDefault)
            }

            Return gl
        End Function

        Private Shared Function __parseOrthology(html$) As KeyValuePair()
            Dim divs = html.Strip_NOBR.DivInternals
            Dim out As New List(Of KeyValuePair)

            For Each o In divs.SlideWindows(2, 2)
                out += New KeyValuePair With {
                    .Key = o(0).StripHTMLTags(stripBlank:=True),
                    .Value = o(1).StripHTMLTags(stripBlank:=True)
                }
            Next

            Return out
        End Function

        Public Function ToCompound() As Compound
            Return New Compound With {
                .Entry = Entry,
                .CommonNames = CommonNames,
                .DbLinks = DbLinks,
                .Formula = Me.Composition,
                .KEGG_reactions = KEGG_reactions,
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
