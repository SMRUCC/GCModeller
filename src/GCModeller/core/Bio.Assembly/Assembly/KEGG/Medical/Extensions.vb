#Region "Microsoft.VisualBasic::4d019fd3e5afc41ff8c5e071a87813b6, ..\core\Bio.Assembly\Assembly\KEGG\Medical\Extensions.vb"

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

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Linq

Namespace Assembly.KEGG.Medical

    Public Module Extensions

        Const hsa$ = "HSA[:]\d+(\s\d+)*"

        ''' <summary>
        ''' 从<see cref="Drug.Targets"/>属性之中解析出``geneID``列表
        ''' </summary>
        ''' <param name="drug"></param>
        ''' <returns></returns>
        <Extension>
        Public Function DrugTargetGenes(drug As Drug) As String()
            Dim geneTargets$() = drug.Targets _
                .SafeQuery _
                .Select(Function(gene) Regex.Match(gene, hsa, RegexICSng).Value) _
                .Where(Function(s) Not s.StringEmpty) _
                .ToArray
            Dim geneIDs$() = geneTargets _
                .Select(Function(id) id.GetTagValue(":").Value.Split) _
                .IteratesALL _
                .Distinct _
                .ToArray
            Return geneIDs
        End Function

        ''' <summary>
        ''' Using remarks the same as map drug to compounds
        ''' </summary>
        ''' <param name="drugs"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CompoundsDrugs(drugs As IEnumerable(Of Drug)) As Dictionary(Of String, Drug())
            Dim compoundDrugs = drugs _
                .Select(Function(dr) New Tuple(Of String, Drug)(dr.TheSameAs, dr)) _
                .Where(Function(dr) Not dr.Item1.StringEmpty) _
                .GroupBy(Function(k) k.Item1) _
                .ToDictionary(Function(k) k.Key,
                              Function(v) v.Select(Function(g) g.Item2) _
                                           .GroupBy(Function(d) d.Entry) _
                                           .Select(Function(dr) dr.First) _
                                           .ToArray)
            Return compoundDrugs
        End Function
    End Module
End Namespace
