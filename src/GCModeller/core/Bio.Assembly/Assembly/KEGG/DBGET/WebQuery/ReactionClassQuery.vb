#Region "Microsoft.VisualBasic::5d70af656b537d8c7e49651b0417e20e, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\WebQuery\ReactionClassQuery.vb"

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

    '   Total Lines: 143
    '    Code Lines: 116
    ' Comment Lines: 6
    '   Blank Lines: 21
    '     File Size: 6.28 KB


    '     Class ReactionClassQuery
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: getRCNumber, ParseHtml, rcURL
    ' 
    '     Module ReactionClassWebQuery
    ' 
    '         Function: DownloadReactionClass, GetReactionClass, ParseReactionClass, removesECLink
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers
Imports r = System.Text.RegularExpressions.Regex

Namespace Assembly.KEGG.DBGET.WebQuery

    Public Class ReactionClassQuery : Inherits WebQuery(Of BriteHEntry.ReactionClass)

        Public Sub New(<CallerMemberName>
                       Optional cache As String = Nothing,
                       Optional interval As Integer = -1,
                       Optional offline As Boolean = False)

            MyBase.New(
                url:=AddressOf rcURL,
                contextGuid:=AddressOf getRCNumber,
                parser:=AddressOf ParseHtml,
                prefix:=Nothing,
                cache:=cache,
                interval:=interval,
                offline:=offline
            )
        End Sub

        Private Shared Function getRCNumber(entry As BriteHEntry.ReactionClass) As String
            Return entry.RCNumber
        End Function

        Private Shared Function rcURL(entry As BriteHEntry.ReactionClass) As String
            Return $"https://www.kegg.jp/dbget-bin/www_bget?rc:{entry.RCNumber}"
        End Function

        Private Shared Function ParseHtml(html$, schema As Type) As Object
            Return ReactionClassWebQuery.ParseReactionClass(html)
        End Function
    End Class

    Public Module ReactionClassWebQuery

        ''' <summary>
        ''' The function returns a id list which failure to download its content data.
        ''' </summary>
        ''' <param name="export"></param>
        ''' <param name="cache"></param>
        ''' <returns></returns>
        Public Function DownloadReactionClass(export$, Optional cache$ = Nothing) As IEnumerable(Of String)
            Dim web As New ReactionClassQuery(cache Or $"{export}/.cache/".When(cache.StringEmpty))
            Dim numbers As BriteHEntry.ReactionClass() = BriteHEntry.ReactionClass _
                .LoadFromResource _
                .ToArray
            Dim failures As New List(Of String)

            Using progressbar As New ProgressBar("Download reaction class numbers...", 1, True)
                Dim progress As New ProgressProvider(progressbar, numbers.Length)
                Dim save$
                Dim rcnumber As ReactionClass

                For Each number As BriteHEntry.ReactionClass In numbers
                    save = $"{export}/{number.GetPathComponents}/{number.RCNumber}.xml"
                    rcnumber = web.Query(Of ReactionClass)(number, ".html")

                    If rcnumber Is Nothing Then
                        failures.Add(number.RCNumber)
                    Else
                        Call rcnumber _
                            .GetXml _
                            .SaveTo(save)
                    End If

                    Call progressbar.SetProgress(progress.StepProgress, $"{number.RCNumber}, ETA={progress.ETA.FormatTime}")
                Next
            End Using

            Return failures
        End Function

        Public Function GetReactionClass(id As String, Optional cache$ = "./br08204/") As ReactionClass
            Static handler As New Dictionary(Of String, ReactionClassQuery)

            Return handler.ComputeIfAbsent(
                key:=cache,
                lazyValue:=Function()
                               Return New ReactionClassQuery(cache)
                           End Function) _
                .Query(Of ReactionClass)(New BriteHEntry.ReactionClass With {.RCNumber = id}, ".html")
        End Function

        Friend Function ParseReactionClass(html As String) As ReactionClass
            Dim web As New WebForm(html)
            Dim reactantPairs = WebForm.parseList(web.GetValue("Reactant pair").FirstOrDefault, "<a href="".+?"">.+?</a>") _
                .Select(Function(name) name.name.Split("_"c)) _
                .Where(Function(tuple) tuple.Length = 2) _
                .Select(Function(tuple)
                            Return New ReactionCompoundTransform With {
                                .from = tuple(0),
                                .[to] = tuple(1)
                            }
                        End Function) _
                .ToArray
            Dim reactions = WebForm.parseList(web.GetValue("Reaction").FirstOrDefault, "<a href="".+?"">.+?</a>")
            Dim enzymes = WebForm.parseList(web.GetValue("Enzyme").FirstOrDefault, "<a href="".+?"">.+?</a>")
            Dim pathways = WebForm.parseList(web.GetValue("Pathway").FirstOrDefault, "<a href="".+?"">.+?</a>")
            Dim orthology = WebForm.parseList(web.GetValue("Orthology").FirstOrDefault.removesECLink, "<a href="".+?"">.+?</a>")
            Dim [class] As New ReactionClass With {
                .entryId = web("Entry")(Scan0).StripHTMLTags.Match("RC\d+"),
                .definition = web("Definition").JoinBy("") _
                    .SplitBy("</th>") _
                    .Last _
                    .StripHTMLTags _
                    .TrimNewLine _
                    .Trim,
                .reactantPairs = reactantPairs,
                .reactions = reactions,
                .enzymes = enzymes,
                .orthology = orthology,
                .pathways = pathways
            }

            Return [class]
        End Function

        <Extension>
        Private Function removesECLink(html As String) As String
            Dim EClinks = r.Matches(html, "<a href[=]""/dbget-bin/www_bget[?]ec[:].+?"">.+?</a>").ToArray
            Dim sb As New StringBuilder(html)
            Dim ECnumber As String

            For Each link As String In EClinks
                ECnumber = link.GetValue
                sb.Replace(link, ECnumber)
            Next

            Return sb.ToString
        End Function
    End Module
End Namespace
