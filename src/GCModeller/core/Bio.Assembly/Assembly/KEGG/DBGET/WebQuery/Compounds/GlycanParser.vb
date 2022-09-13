#Region "Microsoft.VisualBasic::1a490e57d01b9dbd232bb43d5611dbb2, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\WebQuery\Compounds\GlycanParser.vb"

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


    ' Code Statistics:

    '   Total Lines: 50
    '    Code Lines: 43
    ' Comment Lines: 0
    '   Blank Lines: 7
    '     File Size: 1.95 KB


    '     Module GlycanParser
    ' 
    '         Function: __parseOrthology, ParseGlycan
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.WebQuery.Compounds
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers

Namespace Assembly.KEGG.DBGET.WebQuery.Compounds

    Module GlycanParser

        Public Function ParseGlycan(html As WebForm) As Glycan
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

        Private Function __parseOrthology(html$) As KeyValuePair()
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
    End Module
End Namespace
