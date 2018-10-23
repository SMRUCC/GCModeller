Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application

Public Module MotifTree

    ''' <summary>
    ''' 使用``!values``字典键来获取目标位点上的所有mapping结果
    ''' </summary>
    ''' <param name="mappings"></param>
    ''' <returns></returns>
    <Extension>
    Public Function BuildTree(mappings As IEnumerable(Of BlastnMapping)) As BinaryTree(Of Location, BlastnMapping)
        Dim tree As New AVLTree(Of Location, BlastnMapping)(AddressOf compares, )

        For Each map As BlastnMapping In mappings
            Call tree.Add(map.MappingLocation, map, False)
        Next

        Return tree.root
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

    Public Function ExtractSites(tree As BinaryTree(Of Location, BlastnMapping)) As IEnumerable(Of (loci As NucleotideLocation, maps As BlastnMapping()))
        Return tree _
            .PopulateNodes _
            .Select(Function(cluster)
                        Dim maps As BlastnMapping() = TryCast(cluster!values, IEnumerable(Of BlastnMapping)).ToArray
                        Dim loci As NucleotideLocation = maps _
                            .Select(Function(map) map.MappingLocation) _
                            .getLoci

                        Return (loci, maps)
                    End Function)
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
