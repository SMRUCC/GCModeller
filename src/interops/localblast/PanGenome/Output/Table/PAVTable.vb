#Region "Microsoft.VisualBasic::5e1de7e58086de0505bf69f1c029c140, localblast\PanGenome\Output\Table\PAVTable.vb"

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

    '   Total Lines: 53
    '    Code Lines: 44 (83.02%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (16.98%)
    '     File Size: 1.78 KB


    ' Class PAVTable
    ' 
    '     Properties: Category, ClusterGenes, Dispensable, FamilyID, PAV
    '                 SingleCopyOrtholog, Size
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.ComponentModel.Annotation

Public Class PAVTable : Implements IDynamicMeta(Of Integer), IOrthologyCluster, INamedValue

    Public Property FamilyID As String Implements IOrthologyCluster.FamilyID, INamedValue.Key
    Public Property PAV As Dictionary(Of String, Integer) Implements IDynamicMeta(Of Integer).Properties
    Public Property ClusterGenes As String() Implements IOrthologyCluster.GeneCluster
    Public Property Category As GeneCategoryType
    Public Property Dispensable As Boolean
    Public Property SingleCopyOrtholog As Boolean

    Default Public Property GenomeData(name As String) As Integer
        Get
            If PAV Is Nothing OrElse Not PAV.ContainsKey(name) Then
                Return 0
            Else
                Return PAV(name)
            End If
        End Get
        Set(value As Integer)
            If PAV Is Nothing Then
                PAV = New Dictionary(Of String, Integer)
            End If

            PAV(name) = value
        End Set
    End Property

    Public ReadOnly Property Size As Integer
        Get
            Return ClusterGenes.TryCount
        End Get
    End Property

    Sub New()
    End Sub

    Sub New(copy As PAVTable)
        FamilyID = copy.FamilyID
        PAV = New Dictionary(Of String, Integer)(copy.PAV)
        ClusterGenes = copy.ClusterGenes.ToArray
        Category = copy.Category
        Dispensable = copy.Dispensable
        SingleCopyOrtholog = copy.SingleCopyOrtholog
    End Sub

    Public Overrides Function ToString() As String
        Return FamilyID
    End Function

End Class

