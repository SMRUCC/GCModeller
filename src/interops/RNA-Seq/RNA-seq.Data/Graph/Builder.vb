#Region "Microsoft.VisualBasic::dd69f0bb5ed09fd8897e396821fd6d2d, RNA-Seq\RNA-seq.Data\Graph\Builder.vb"

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

    '   Total Lines: 23
    '    Code Lines: 16 (69.57%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 7 (30.43%)
    '     File Size: 576 B


    '     Class Builder
    ' 
    '         Properties: Graph
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.SequenceModel.FQ

Namespace Graph

    Public MustInherit Class Builder

        Protected ReadOnly g As New NetworkGraph

        Public ReadOnly Property Graph As NetworkGraph
            Get
                Return g
            End Get
        End Property

        Sub New(reads As IEnumerable(Of FastQ))
            Call ProcessReads(reads)
        End Sub

        Protected MustOverride Sub ProcessReads(reads As IEnumerable(Of FastQ))

    End Class
End Namespace
