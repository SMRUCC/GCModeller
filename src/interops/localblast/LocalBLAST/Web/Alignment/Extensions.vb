#Region "Microsoft.VisualBasic::3f7c4d41de26ed2c77947e5cd624441e, localblast\LocalBLAST\Web\Alignment\Extensions.vb"

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

    '     Module Extensions
    ' 
    '         Function: ExportOrderByGI, GetHitsEntryList, TopBest
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports r = System.Text.RegularExpressions.Regex

Namespace NCBIBlastResult.WebBlast

    <HideModuleName>
    Public Module Extensions

        ''' <summary>
        ''' 导出绘制的顺序
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>这里不能够使用并行拓展</remarks>
        ''' 
        <Extension>
        Public Function ExportOrderByGI(table As AlignmentTable) As String()
            Dim LQuery As String() = (From hit As HitRecord
                                      In table.Hits
                                      Select hit.GI.FirstOrDefault
                                      Distinct).ToArray
            Return LQuery
        End Function

        Const LOCUS_ID As String = "(emb|gb|dbj)\|[a-z]+\d+"

        <Extension>
        Public Function GetHitsEntryList(table As AlignmentTable) As String()
            Dim LQuery As String() =
                LinqAPI.Exec(Of String) <= From hit As HitRecord
                                           In table.Hits
                                           Let hitID As String = r.Match(hit.SubjectIDs, LOCUS_ID, RegexICSng).Value
                                           Where Not String.IsNullOrEmpty(hitID)
                                           Select hitID.Split(CChar("|")).Last
                                           Distinct
            Return LQuery
        End Function

        <Extension>
        Public Iterator Function TopBest(raw As IEnumerable(Of HitRecord)) As IEnumerable(Of HitRecord)
            Dim gg = From x As HitRecord In raw Select x Group x By x.QueryID Into Group

            For Each groups In gg
                Dim orders = From x As HitRecord
                             In groups.Group
                             Select x
                             Order By x.Identity Descending

                Yield orders.First
            Next
        End Function
    End Module
End Namespace
