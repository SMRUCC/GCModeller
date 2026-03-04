#Region "Microsoft.VisualBasic::ae789282944dbd2d30c9d8ad5ec9d4f3, analysis\Metagenome\Metagenome\Enterotype.vb"

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

'   Total Lines: 60
'    Code Lines: 46 (76.67%)
' Comment Lines: 9 (15.00%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 5 (8.33%)
'     File Size: 2.49 KB


' Module Enterotype
' 
'     Function: JSD, PAMclustering
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Math.Correlations
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.Metagenomics

''' <summary>
''' Protocol module to produce enterotype clusters
''' </summary>
Public Module Enterotype

    ''' <summary>
    ''' First, the abundances of classified genera are used to 
    ''' produce a JSD matrix between samples.
    ''' </summary>
    ''' <param name="abundances"></param>
    ''' <returns>A JSD correlation matrix between samples.</returns>
    <Extension>
    Public Iterator Function JSD(abundances As IEnumerable(Of DataSet), Optional parallel As Boolean = True) As IEnumerable(Of DataSet)
        Dim matrix As DataSet() = abundances.ToArray
        Dim taxonomy As String() = matrix.PropertyNames
        Dim jsdMatrix = From sample As DataSet
                        In matrix.Populate(parallel:=parallel)
                        Let P As Double() = sample(taxonomy)
                        Let jsdi As Dictionary(Of String, Double) = matrix.ToDictionary(
                            Function(another) another.ID,
                            Function(another)
                                Dim Q As Double() = another(taxonomy)
                                Return Correlations.JSD(P, Q)
                            End Function)
                        Select New DataSet With {
                            .ID = sample.ID,
                            .Properties = jsdi
                        }

        For Each sample As DataSet In jsdMatrix
            Yield sample
        Next
    End Function

    <Extension>
    Public Function PAMclustering(JSD As IEnumerable(Of DataSet), Optional maxIterations% = 1000) As EntityClusterModel()
        Dim JSDMatrix = JSD.ToArray
        Dim sampleNames$() = JSDMatrix.PropertyNames
        Dim entities As ClusterEntity() = JSDMatrix _
            .Select(Function(d)
                        Return New ClusterEntity With {
                            .uid = d.ID,
                            .entityVector = d(sampleNames)
                        }
                    End Function) _
            .ToArray
        Dim clusters = entities.DoKMedoids(3, maxIterations)
        Dim result = clusters _
            .Select(Function(c) c.ToDataModel(sampleNames)) _
            .ToArray

        Return result
    End Function

    <Extension>
    Public Function BuildClusterTree(table As IEnumerable(Of OTUTable),
                                     Optional equals As Double = 0.85,
                                     Optional gt As Double = 0.6) As NetworkGraph

        Dim jsd As New OTUCosineComparer(table, equals, gt)
        Dim tree As BTreeCluster = jsd.OTU_ids.BTreeCluster(alignment:=jsd)
        Dim g As NetworkGraph = tree.MakeTreeGraph(
            metadata:=Function(id)
                          Return MakeMetadata(otu:=jsd.GetObject(id))
                      End Function)
        Return g
    End Function

    Private Function MakeMetadata(otu As OTUTable) As Dictionary(Of String, String)
        Dim data As New Dictionary(Of String, String)
        Dim tax As Taxonomy = otu.taxonomy

        Call data.Add("text", otu.taxonomy.BIOMTaxonomyString)
        Call data.Add("value", otu.Vector.Sum)

        Call data.Add(NameOf(Taxonomy.kingdom), tax.kingdom)
        Call data.Add(NameOf(Taxonomy.phylum), tax.phylum)
        Call data.Add(NameOf(Taxonomy.class), tax.class)
        Call data.Add(NameOf(Taxonomy.order), tax.order)
        Call data.Add(NameOf(Taxonomy.family), tax.family)
        Call data.Add(NameOf(Taxonomy.genus), tax.genus)
        Call data.Add(NameOf(Taxonomy.species), tax.species)

        Call data.Add(NameOf(Taxonomy.scientificName), tax.scientificName)
        Call data.Add(NameOf(Taxonomy.ncbi_taxid), tax.ncbi_taxid)

        Return data
    End Function
End Module
