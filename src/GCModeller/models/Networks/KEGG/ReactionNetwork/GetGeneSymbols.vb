#Region "Microsoft.VisualBasic::dff517caadd8c39779ab6e01cec8fac6, GCModeller\models\Networks\KEGG\ReactionNetwork\GetGeneSymbols.vb"

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

    '   Total Lines: 84
    '    Code Lines: 73
    ' Comment Lines: 5
    '   Blank Lines: 6
    '     File Size: 3.53 KB


    '     Module GetGeneSymbols
    ' 
    '         Function: GetGeneSymbols, IdDesc
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq

Namespace ReactionNetwork

    <HideModuleName>
    Module GetGeneSymbols

        <Extension>
        Private Function IdDesc(src As IEnumerable(Of String)) As String()
            Return src _
                .GroupBy(Function(id) id) _
                .OrderByDescending(Function(g) g.Count) _
                .Keys _
                .ToArray
        End Function

        ''' <summary>
        ''' 返回的id列表已经是经过降序排序之后的结果，可以直接取第一个得到最多的id
        ''' </summary>
        ''' <param name="reactions"></param>
        ''' <returns></returns>
        <Extension>
        Public Function GetGeneSymbols(reactions As IEnumerable(Of ReactionTable)) As (label As String, KO As String(), EC As String(), keggRid As String(), geneSymbols As String())
            Dim models As String() = reactions _
                .Select(Function(r)
                            Return r.geneNames.JoinIterates(r.KO).JoinIterates(r.EC).JoinIterates(r.entry)
                        End Function) _
                .IteratesALL _
                .Select(Function(s) s.StringSplit("[;,]")) _
                .IteratesALL _
                .Select(AddressOf Strings.Trim) _
                .Where(Function(s) Not s.StringEmpty) _
                .ToArray
            Dim KO = models.Where(Function(id) id.IsPattern("K\d+")).IdDesc
            Dim EC = models _
                .Select(Function(id) id.Match("\d+\.([-]|(\d+))(\.([-]|(\d+))){3}")) _
                .Where(Function(id) Not id.StringEmpty) _
                .IdDesc
            Dim keggRid As String() = models _
                .Select(Function(id) id.Match("R\d+")) _
                .Where(Function(id) Not id.StringEmpty) _
                .ToArray
            Dim allId As String() = KO.JoinIterates(EC).JoinIterates(keggRid).ToArray
            Dim geneSymbols = models _
                .AsParallel _
                .Where(Function(line) line.InStrAny(allId) = -1) _
                .Where(Function(id) id.Match("\d+\.([-]|(\d+))(\.([-]|(\d+)))*", RegexICSng).StringEmpty) _
                .ToArray
            Dim middleNode As String

            If models.Length = 1 Then
                middleNode = models(Scan0)
            Else
                If geneSymbols.IsNullOrEmpty Then
                    If EC.IsNullOrEmpty Then
                        If KO.IsNullOrEmpty Then
                            middleNode = keggRid.First
                        Else
                            middleNode = KO.First
                        End If
                    Else
                        middleNode = EC _
                            .GroupBy(Function(id)
                                         Return id.Split("."c).Take(3).JoinBy(".")
                                     End Function) _
                            .OrderByDescending(Function(g) g.Count) _
                            .First _
                            .Key & ".-"
                    End If
                Else
                    middleNode = geneSymbols _
                        .GroupBy(Function(name) name.ToLower) _
                        .OrderByDescending(Function(g) g.Count) _
                        .First _
                        .First
                End If
            End If

            Return (middleNode, KO, EC, keggRid, geneSymbols)
        End Function
    End Module
End Namespace
