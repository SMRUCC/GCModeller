#Region "Microsoft.VisualBasic::9d7aaef94e28a4037959ab9480911f96, CARMEN\WebHandler.vb"

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

    ' Module WebHandler
    ' 
    '     Properties: lstOrganisms, lstPathways
    ' 
    '     Function: LoadList, Reconstruct
    ' 
    '     Sub: DownloadResult
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.xConsole
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.MIME.Markup.HTML
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

''' <summary>
''' Comparative Analysis and Reconstruction of MEtabolic Networks
''' </summary>
<Package("CARMEN.WebHandler",
                  Description:="Comparative Analysis and Reconstruction of MEtabolic Networks<br />
                  KGML-based model reconstruction of metabolic pathways<br /><br />             
After a CARMEN analysis run has finished, you can directly download various SBML files (Level 2 Version 1 and Version 4, Level 3 Version 1). 
                  Subsequently, the SBML file can be visualized by SBML-compliant tools. 
<br />
Additionally, a SVG (Scalable Vector Graphics) representation of the metabolic reconstruction is rendered on the fly, displaying the selected metabolic pathways. 
                  The SVG image can be downloaded or reviewed in most recent browsers directly or via the freely available Adobe SVG plugin.",
                  Url:="https://carmen.cebitec.uni-bielefeld.de/cgi-bin/execute.cgi",
                  Category:=APICategories.ResearchTools,
                  Publisher:="amethyst.asuka@gcmodeller.org")>
Public Module WebHandler

    ''' <summary>
    ''' https://carmen.cebitec.uni-bielefeld.de/cgi-bin/execute.cgi
    ''' </summary>
    Const URL As String = "https://carmen.cebitec.uni-bielefeld.de/cgi-bin/execute.cgi"
    ''' <summary>
    ''' https://carmen.cebitec.uni-bielefeld.de/cgi-bin/results.cgi
    ''' </summary>
    Const EXEC_Reconstruct As String = "https://carmen.cebitec.uni-bielefeld.de/cgi-bin/results.cgi"

    Public ReadOnly Property lstOrganisms As String()
    Public ReadOnly Property lstPathways As Dictionary(Of String, String)

    <ExportAPI("__Init()")>
    Public Function LoadList(Optional fromOnline As Boolean = False) As Boolean
        Dim webPage As String = If(fromOnline, URL.GET, My.Resources.CARMEN)
        Dim Pages As String() = Regex.Matches(webPage, "<select name.+?</select>").ToArray
        Dim Session As Dictionary(Of String, HtmlElement()) =
            Pages.Select(Function(x) New With {
            .name = Regex.Match(x, "<select name="".+?""").Value,
            .lst = Regex.Matches(x, "<option value="".+?"">.+?</option>").ToArray}) _
            .ToDictionary(Function(x) Regex.Match(x.name, """.+?""").Value.GetString,
                          Function(x)
                              Return x.lst.Select(Function(s) HtmlElement.SingleNodeParser(s)).ToArray
                          End Function)
        _lstOrganisms = Session("organism").Select(Function(x) x("value").Value).ToArray
        _lstPathways = Session("pathways").ToDictionary(
            Function(x) x("value").Value,
            Function(x) x.HtmlElements(Scan0).InnerText)

        Return True
    End Function

    Const OP_organism As String = "organism"
    Const OP_pathways As String = "pathways"

    '<meta http-equiv="refresh" content="10; URL=results.cgi?result=CARMEN_KIw91T_2-1_CellDesigner.sbml&organism=Xanthomonas_campestris_8004_uid57595">

    <ExportAPI("Reconstruct")>
    Public Function Reconstruct(sp As String, pathway As String, outDIR As String) As Boolean
        Dim reqparm As Specialized.NameValueCollection =
            New Dictionary(Of String, String) From {
                {OP_organism, sp},
                {OP_pathways, pathway}
        }.BuildReqparm

        Dim result As String = EXEC_Reconstruct.POST(reqparm)

        result = Regex.Match(result, "<meta http-equiv=""refresh"".+?>", RegexOptions.IgnoreCase Or RegexOptions.Singleline).Value

        Call $"{sp}:{pathway}  ==> {result}".__DEBUG_ECHO

        result = Regex.Match(result, "URL=.+", RegexOptions.IgnoreCase).Value
        result = Mid(result, 5)
        result = Mid(result, 1, Len(result) - 2)

        Dim url As String = "https://carmen.cebitec.uni-bielefeld.de/cgi-bin/" & result
        Dim Spinner As New Spinner(txt:="^b[Working {0}]^! Server Running ^gReconstruction^!....")
        Call Spinner.RunTask(75)
        Call Threading.Thread.Sleep(60 * 1000 * 2)  ' 服务器需要一些时间完成计算，在这里等待2分钟
        Call DownloadResult(url, outDIR)
        Call Spinner.Break()
        Call "JOB Done!".__DEBUG_ECHO
        Return True
    End Function

    Public Sub DownloadResult(url As String, outDIR As String)
        Dim WebPage As String = url.GET
        Dim Downloads As String() = Regex.Matches(WebPage, "href=""/out/CARMEN.+?""", RegexOptions.IgnoreCase) _
            .ToArray(Function(href) href.href)

        For Each file As String In Downloads
            Dim path As String = outDIR & "/" & FileIO.FileSystem.GetFileInfo(file).Name
            Dim href As String = "https://carmen.cebitec.uni-bielefeld.de" & file
            Call $"{href} ===> {path.ToFileURL}".__DEBUG_ECHO
            Call href.GetDownload(path)
        Next
    End Sub
End Module
