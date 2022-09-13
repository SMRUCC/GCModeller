#Region "Microsoft.VisualBasic::ebd1294140271ba85297b83da58f2cf8, GCModeller\core\Bio.Assembly\Assembly\KEGG\Medical\Extensions.vb"

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

    '   Total Lines: 85
    '    Code Lines: 62
    ' Comment Lines: 11
    '   Blank Lines: 12
    '     File Size: 3.38 KB


    '     Module Extensions
    ' 
    '         Function: BuildDrugCompoundMaps, CompoundsDrugs, DrugTargetGenes
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Collection
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
        ''' Using remarks the ``same as`` map compound to drug
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
                              Function(v)
                                  Return v.Select(Function(g) g.Item2) _
                                           .GroupBy(Function(d) d.Entry) _
                                           .Select(Function(dr) dr.First) _
                                           .ToArray
                              End Function)
            Return compoundDrugs
        End Function

        <Extension>
        Public Function BuildDrugCompoundMaps(drugs As IEnumerable(Of Drug), groups As IEnumerable(Of DrugGroup)) As Dictionary(Of String, String())
            Dim maps As New Dictionary(Of String, String())
            Dim DGmaps = groups.ToDictionary

            For Each drug As Drug In drugs
                Dim theSameAs$ = drug.TheSameAs

                If theSameAs.StringEmpty Then
                    ' 可能是drug group
                    Dim DGid$ = drug.RemarksTable.TryGetValue("Chemical group")

                    If Not DGid.StringEmpty Then
                        Dim DG = DGmaps.TryGetValue(DGid)

                        If Not DG Is Nothing Then
                            Dim members = DG.Members _
                                .Where(Function(s) s.First = "C"c) _
                                .Select(Function(s) s.Split.First) _
                                .ToArray

                            maps.Add(drug.Entry, members)
                        End If
                    End If
                Else
                    maps.Add(drug.Entry, theSameAs.Split)
                End If
            Next

            Return maps
        End Function
    End Module
End Namespace
