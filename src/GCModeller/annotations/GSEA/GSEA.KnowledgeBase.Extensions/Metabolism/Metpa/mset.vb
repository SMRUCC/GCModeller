#Region "Microsoft.VisualBasic::9c88ba752731e931b70cb3a5cfa25504, annotations\GSEA\GSEA.KnowledgeBase.Extensions\Metabolism\Metpa\mset.vb"

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

    '   Total Lines: 73
    '    Code Lines: 46
    ' Comment Lines: 13
    '   Blank Lines: 14
    '     File Size: 2.48 KB


    '     Class mset
    ' 
    '         Properties: clusterId, kegg_id, metaboliteNames
    ' 
    '         Function: ToClusterModel, ToString
    ' 
    '     Class msetList
    ' 
    '         Properties: list
    ' 
    '         Function: CountUnique, GetClusters
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.ComponentModel.Annotation

Namespace Metabolism.Metpa

    ''' <summary>
    ''' a molecule collection
    ''' </summary>
    Public Class mset

        Public Property metaboliteNames As String()
        Public Property kegg_id As String()

        ''' <summary>
        ''' the pathway id
        ''' </summary>
        ''' <returns></returns>
        Public Property clusterId As String

        Public Function ToClusterModel(id As String) As Cluster
            Dim molecules As BackgroundGene() = New BackgroundGene(kegg_id.Length - 1) {}

            For i As Integer = 0 To molecules.Length - 1
                molecules(i) = New BackgroundGene With {
                    .accessionID = _kegg_id(i),
                    .[alias] = {_kegg_id(i)},
                    .locus_tag = New NamedValue(_kegg_id(i), _metaboliteNames(i)),
                    .name = _metaboliteNames(i),
                    .term_id = {}
                }
            Next

            Return New Cluster With {
                .ID = id,
                .names = clusterId,
                .description = clusterId,
                .members = molecules
            }
        End Function

        Public Overrides Function ToString() As String
            Return $"[{clusterId}] has {kegg_id.Length} compounds. ({metaboliteNames.Take(3).JoinBy("; ")}...)"
        End Function

    End Class

    ''' <summary>
    ''' the molecule collection for each pathway cluster
    ''' </summary>
    ''' <remarks>
    ''' the molecule collection save in vector data model <see cref="mset"/>
    ''' </remarks>
    Public Class msetList

        Public Property list As Dictionary(Of String, mset)

        Public Shared Function CountUnique(Of T As PathwayBrief)(models As T()) As Integer
            Return Aggregate cpd As NamedValue(Of String)
                   In models.Select(Function(a) a.GetCompoundSet).IteratesALL
                   Group By cpd.Name Into Group
                   Into Count
        End Function

        Public Iterator Function GetClusters() As IEnumerable(Of Cluster)
            For Each mapId As String In list.Keys
                Yield list(mapId).ToClusterModel(mapId)
            Next
        End Function

    End Class
End Namespace
