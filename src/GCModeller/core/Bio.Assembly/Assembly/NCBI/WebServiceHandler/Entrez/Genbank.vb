#Region "Microsoft.VisualBasic::9a0834b44a05368cd18b0bb22438f1f6, GCModeller\core\Bio.Assembly\Assembly\NCBI\WebServiceHandler\Entrez\Genbank.vb"

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

    '   Total Lines: 55
    '    Code Lines: 32
    ' Comment Lines: 12
    '   Blank Lines: 11
    '     File Size: 2.00 KB


    '     Module Genbank
    ' 
    '         Function: GetMetaInfo, getUid
    ' 
    '         Sub: Fetch
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

Namespace Assembly.NCBI.Entrez

    ''' <summary>
    ''' The NCBI genbank data api handler
    ''' </summary>
    Public Module Genbank

        ' https://www.ncbi.nlm.nih.gov/sviewer/viewer.cgi?
        ' tool=portal
        ' save=file
        ' log$=seqview
        ' db=nuccore
        ' report=gbwithparts
        ' id=229599883
        ' withparts=on
        ' showgi=1

        Const sviewerApi$ = "https://www.ncbi.nlm.nih.gov/sviewer/viewer.cgi?tool=portal&save=file&log$=seqview&db=nuccore&report=gbwithparts"

        Public Sub Fetch(accessionID$, saveAs$, Optional full As Boolean = True, Optional showgi As Boolean = True)
            Dim parameters As New Dictionary(Of String, String)

            parameters!id = getUid(accessionID)
            parameters!withparts = "on" Or "off".When(Not full)
            parameters!showgi = 0 Or 1.When(showgi)

            Dim query$ = parameters.Select(Function(a) $"{a.Key}={a.Value}").JoinBy("&")
            Dim url = $"{sviewerApi}&{query}"

            Call url.DownloadFile(saveAs)
        End Sub

        Public Function GetMetaInfo(accessionID As String) As Dictionary(Of String, String)
            Dim url$ = $"https://www.ncbi.nlm.nih.gov/nuccore/{accessionID}"
            Dim html$ = url.GET
            Dim meta As Dictionary(Of String, String) = html.ParseHtmlMeta

            Return meta
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function getUid(accessionID As String) As String
            Return GetMetaInfo(accessionID) _
                .Where(Function(meta)
                           Return meta.Key.TextEquals("ncbi_uidlist")
                       End Function) _
                .FirstOrDefault _
                .Value
        End Function
    End Module
End Namespace
