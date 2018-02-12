#Region "Microsoft.VisualBasic::f3eed2e97a14501e932eae1a05e85b01, analysis\ProteinTools\Interactions.BioGRID\TabularFiles\OSPREY_DATASET.vb"

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

    ' Class OSPREY_DATASET
    ' 
    '     Properties: ExperimentalSystem, GeneA, GeneB, PubmedID, ScreenNameA
    '                 ScreenNameB, Source
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON
''' <summary>
''' ``BIOGRID-OSPREY_DATASETS-3.4.138.osprey.zip``
''' 
''' This zip archive contains multiple files formatted In Osprey Network Visualization 
''' System Custom Network format containing all interaction And associated annotation 
''' data from the BIOGRID separated into seperate Organisms And Experimental Systems 
''' To be used In Osprey.
''' </summary>
Public Class OSPREY_DATASET

    Public Property GeneA As String
    Public Property GeneB As String
    Public Property ScreenNameA As String
    Public Property ScreenNameB As String
    Public Property ExperimentalSystem As String
    Public Property Source As String
    Public Property PubmedID As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
