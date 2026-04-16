#Region "Microsoft.VisualBasic::a3493affb9c62b881d11ef97d95ec205, localblast\PanGenome\Output\Table\SVTable.vb"

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
    '    Code Lines: 47 (64.38%)
    ' Comment Lines: 16 (21.92%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (13.70%)
    '     File Size: 2.11 KB


    ' Class SVData
    ' 
    '     Properties: CopyNumber, Description, FamilyID, GenomeName, Median
    '                 RelatedGenes, SV_ID, Type
    ' 
    ' Class SVTable
    ' 
    '     Properties: Category, ClusterSize, Dispensable, SingleCopyOrtholog
    ' 
    '     Constructor: (+3 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.Serialization
Imports SMRUCC.genomics.ComponentModel.Annotation

<DataContract> Public Class SVData : Implements IOrthologyCluster

    Public Property SV_ID As String
    Public Property Type As SVType
    ''' <summary>
    ''' 发生变异的基因组
    ''' </summary>
    ''' <returns></returns>
    Public Property GenomeName As String
    ''' <summary>
    ''' 关联的基因家族ID
    ''' </summary>
    ''' <returns></returns>
    Public Property FamilyID As String Implements IOrthologyCluster.FamilyID
    ''' <summary>
    ''' 涉及的基因ID
    ''' </summary>
    ''' <returns></returns>
    Public Property RelatedGenes As String() Implements IOrthologyCluster.GeneCluster
    Public Property CopyNumber As Integer
    Public Property Median As Double
    ''' <summary>
    ''' 描述信息
    ''' </summary>
    ''' <returns></returns>
    Public Property Description As String

End Class

Public Class SVTable : Inherits SVData

    Public Property Category As GeneCategoryType
    Public Property Dispensable As Boolean
    Public Property SingleCopyOrtholog As Boolean

    Public ReadOnly Property ClusterSize As Integer
        Get
            Return RelatedGenes.TryCount
        End Get
    End Property

    Sub New()
    End Sub

    Sub New(sv As StructuralVariation)
        SV_ID = sv.SV_ID
        Type = sv.Type
        GenomeName = sv.GenomeName
        FamilyID = sv.FamilyID
        RelatedGenes = sv.RelatedGenes
        Description = sv.Description
        CopyNumber = sv.CopyNumber
        Median = sv.Median
    End Sub

    Sub New(copy As SVTable)
        SV_ID = copy.SV_ID
        Type = copy.Type
        GenomeName = copy.GenomeName
        FamilyID = copy.FamilyID
        RelatedGenes = copy.RelatedGenes.ToArray
        CopyNumber = copy.CopyNumber
        Median = copy.Median
        Description = copy.Description
        Category = copy.Category
        Dispensable = copy.Dispensable
        SingleCopyOrtholog = copy.SingleCopyOrtholog
    End Sub

End Class

