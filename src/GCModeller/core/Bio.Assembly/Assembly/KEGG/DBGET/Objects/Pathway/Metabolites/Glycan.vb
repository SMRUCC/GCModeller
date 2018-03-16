#Region "Microsoft.VisualBasic::53c257bfeeed62f0a6744843e48dfbcb, core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\Metabolites\Glycan.vb"

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
    '         Properties: Composition, Mass, Orthology
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: __parseOrthology, Download, DownloadFrom, ToCompound
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.HtmlParser
Imports Microsoft.VisualBasic.Text.Xml.Models
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
                .reactionId = base.reactionId,
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
