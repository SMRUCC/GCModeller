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