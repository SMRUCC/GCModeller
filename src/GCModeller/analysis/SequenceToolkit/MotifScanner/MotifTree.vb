#Region "Microsoft.VisualBasic::698be716fdb1407a6a34bce994e20459, analysis\SequenceToolkit\MotifScanner\MotifTree.vb"

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

    ' Module MotifTree
    ' 
    '     Function: BuildTree, compares, ExtractSites, (+2 Overloads) FilterMotifs, FilterMotifsQuantile
    '               getLoci
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.NtMapping

Public Module MotifTree

    ''' <summary>
    ''' 使用``!values``字典键来获取目标位点上的所有mapping结果
    ''' </summary>
    ''' <param name="mappings"></param>
    ''' <returns></returns>
    <Extension>
    Public Function BuildTree(mappings As IEnumerable(Of BlastnMapping)) As NaiveBinaryTree(Of Location, BlastnMapping)
        Dim tree As New NaiveBinaryTree(Of Location, BlastnMapping)(AddressOf compares, )

        For Each map As BlastnMapping In mappings
            Call tree.insert(map.MappingLocation, map, append:=True)
        Next

        Return tree
    End Function

    ''' <summary>
    ''' 计算两个片段的overlap的区域的占比
    ''' </summary>
    ''' <param name="a"></param>
    ''' <param name="b"></param>
    ''' <returns></returns>
    Private Function compares(a As Location, b As Location) As Integer
        If Not a.IsOverlapping(b) Then
NOT_EQUALS:
            Dim xa = (a.Left + a.Right) / 2
            Dim xb = (b.Left + b.Right) / 2

            If xa < xb Then
                Return -1
            Else
                Return 1
            End If
        Else
            '      a
            ' |---------|
            '     |-----------|
            '           b
            Dim d1 = a.Right - b.Left
            Dim d2 = b.Right - a.Left

            If Math.Min(d1, d2) / Math.Max(a.Length, b.Length) >= 0.5 Then
                Return 0
            Else
                GoTo NOT_EQUALS
            End If
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="tree">
    ''' 因为当节点非常多的时候，直接使用二叉树的结构可能会因为很多层次的递归而造成stack overflow的问题
    ''' 所以在这里直接使用AVLTree对象的ToArray来避免这种情况的出现
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function ExtractSites(tree As NaiveBinaryTree(Of Location, BlastnMapping)) As IEnumerable(Of (loci As NucleotideLocation, maps As BlastnMapping()))
        Return tree _
            .GetAllNodes _
            .AsParallel _
            .Select(Function(cluster)
                        Dim maps As BlastnMapping() = TryCast(cluster!values, IEnumerable(Of BlastnMapping)).ToArray
                        Dim loci As NucleotideLocation = maps _
                            .Select(Function(map) map.MappingLocation) _
                            .getLoci

                        Return (loci, maps)
                    End Function)
    End Function

    ''' <summary>
    ''' 可能会有多个家族的调控因子作用在该位点上
    ''' </summary>
    ''' <param name="sites"></param>
    ''' <param name="getFamily"></param>
    ''' <param name="getTarget"></param>
    ''' <param name="hits%"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Iterator Function FilterMotifs(sites As IEnumerable(Of (loci As NucleotideLocation, maps As BlastnMapping())),
                                          getFamily As Func(Of String, String),
                                          getTarget As Func(Of String, String),
                                          Optional hits% = 2,
                                          Optional isQuantileHits As Boolean = False) As IEnumerable(Of NamedValue(Of NucleotideLocation))

        Dim familyHits As Integer

        If isQuantileHits Then
            familyHits = 2
        Else
            familyHits = hits
        End If

        For Each siteCluster In sites.Where(Function(site) site.maps.Length >= hits)
            Dim loci As NucleotideLocation = siteCluster.loci
            Dim maps As BlastnMapping() = siteCluster _
                .maps _
                .Where(Function(map) map.MappingLocation.Strand = loci.Strand) _
                .GroupBy(Function(site) site.ReadQuery) _
                .Select(Function(g) g.First) _
                .ToArray
            Dim familyGroups = maps _
                .GroupBy(Function(map) getFamily(map.ReadQuery)) _
                .Where(Function(family) family.Count >= familyHits) _
                .ToArray

            If familyGroups.Length = 0 Then
                Continue For
            End If

            For Each family In familyGroups
                Dim targets$() = family _
                    .Select(Function(site) getTarget(site.ReadQuery)) _
                    .ToArray

                Yield New NamedValue(Of NucleotideLocation) With {
                    .Description = targets.GetJson,
                    .Name = family.Key,
                    .Value = family _
                        .Select(Function(site) site.MappingLocation) _
                        .getLoci
                }
            Next
        Next
    End Function

    ''' <summary>
    ''' 可能会有多个家族的调控因子作用在该位点上
    ''' </summary>
    ''' <param name="sites"></param>
    ''' <param name="getFamily"></param>
    ''' <param name="getTarget"></param>
    ''' <param name="hitsExpression"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function FilterMotifs(sites As IEnumerable(Of (loci As NucleotideLocation, maps As BlastnMapping())),
                                 getFamily As Func(Of String, String),
                                 getTarget As Func(Of String, String),
                                 hitsExpression$) As IEnumerable(Of NamedValue(Of NucleotideLocation))

        If hitsExpression.IsPattern("\d+") Then
            Return sites.FilterMotifs(
                getFamily, getTarget,
                hits:=hitsExpression.ParseInteger,
                isQuantileHits:=False
            )
        Else
            Dim quantile# = Val(hitsExpression.GetTagValue(":").Value)
            Return sites.FilterMotifsQuantile(getFamily, getTarget, quantile:=quantile)
        End If
    End Function

    ''' <summary>
    ''' 可能会有多个家族的调控因子作用在该位点上
    ''' </summary>
    ''' <param name="sites"></param>
    ''' <param name="getFamily"></param>
    ''' <param name="getTarget"></param>
    ''' <param name="quantile"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function FilterMotifsQuantile(sites As IEnumerable(Of (loci As NucleotideLocation, maps As BlastnMapping())),
                                         getFamily As Func(Of String, String),
                                         getTarget As Func(Of String, String),
                                         Optional quantile# = 0.65) As IEnumerable(Of NamedValue(Of NucleotideLocation))
        Dim data = sites.ToArray
        Dim mapLength As QuantileEstimationGK = data _
            .Select(Function(d) CDbl(d.maps.Length)) _
            .GKQuantile
        Dim hitsBase% = mapLength.Query(quantile)

        Return data.FilterMotifs(
            getFamily, getTarget,
            hits:=hitsBase,
            isQuantileHits:=True
        )
    End Function

    <Extension>
    Private Function getLoci(locations As IEnumerable(Of NucleotideLocation)) As NucleotideLocation
        Dim sites As NucleotideLocation() = locations.ToArray
        Dim topStrain = sites.Select(Function(l) l.Strand).TopMostFrequent
        Dim locis = sites.Where(Function(l) l.Strand = topStrain).ToArray
        Dim left% = locis.Select(Function(l) l.Left).Average
        Dim right% = locis.Select(Function(l) l.Right).Average

        Return New NucleotideLocation(left, right, topStrain)
    End Function
End Module
