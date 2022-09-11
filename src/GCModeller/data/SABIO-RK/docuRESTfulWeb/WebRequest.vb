#Region "Microsoft.VisualBasic::b620d3bb68fed094b24b778e8b573852, GCModeller\data\SABIO-RK\docuRESTfulWeb\WebRequest.vb"

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

    '   Total Lines: 68
    '    Code Lines: 49
    ' Comment Lines: 9
    '   Blank Lines: 10
    '     File Size: 2.73 KB


    ' Module WebRequest
    ' 
    '     Function: QueryByECNumber, QueryByECNumbers, QueryUsing_KEGGId
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Data.SABIORK.SBML
Imports r = System.Text.RegularExpressions.Regex
Imports sbXML = SMRUCC.genomics.Model.SBML.Level3.XmlFile(Of SMRUCC.genomics.Data.SABIORK.SBML.SBMLReaction)

Public Module WebRequest

    Public Const KEGG_QUERY_ENTRY As String = "http://sabiork.h-its.org/sabioRestWebServices/reactions/reactionIDs?q=KeggReactionID:"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="IdList"></param>
    ''' <param name="ExportDir"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 因为会存在非常多的废弃的id编号，所以这个函数应该会被废弃掉
    ''' </remarks>
    Public Iterator Function QueryUsing_KEGGId(IdList As String(), ExportDir As String) As IEnumerable(Of String)
        For Each Id As String In IdList
            Dim url As String = (KEGG_QUERY_ENTRY & Id)
            Dim PageContent = url.GET
            Dim Entries As String() = (From m As Match In r.Matches(PageContent, "<SabioReactionID>\d+</SabioReactionID>") Select m.Value.GetValue).ToArray

            For Each Entry In Entries
                Dim File = String.Format("{0}/{1}-{2}.sbml", ExportDir, Id, Entry)

                url = SabiorkSBML.URL_SABIORK_KINETIC_LAWS_QUERY & Entry
                Call url.GET.SaveTo(File)
            Next
        Next
    End Function

    <Extension>
    Public Iterator Function QueryByECNumbers(enzymes As htext, export$) As IEnumerable(Of String)
        Dim ecNumbers As BriteHText() = enzymes.Hierarchical _
            .EnumerateEntries _
            .ToArray
        Dim saveXml As String
        Dim cache$ = $"{export}/.cache"
        Dim q As Dictionary(Of QueryFields, String)
        Dim ECNumber As String

        For Each id As BriteHText In ecNumbers
            saveXml = id.BuildPath(export)
            ECNumber = id.parent.classLabel.Split.First
            q = New Dictionary(Of QueryFields, String) From {
                {QueryFields.ECNumber, ECNumber}
            }

            Call docuRESTfulWeb.searchKineticLaws(q, cache) _
                .GetXml _
                .SaveTo(saveXml)
        Next
    End Function

    Public Function QueryByECNumber(ECNumber As String, Optional cache$ = "./.cache") As sbXML
        Dim q As New Dictionary(Of QueryFields, String) From {
            {QueryFields.ECNumber, ECNumber}
        }
        Dim xml As sbXML = docuRESTfulWeb.searchKineticLaws(q, cache)

        Return xml
    End Function
End Module
