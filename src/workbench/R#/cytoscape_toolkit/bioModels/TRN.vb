﻿#Region "Microsoft.VisualBasic::ab9aafced8b603fbdacd3ba36acdd80b, G:/GCModeller/src/workbench/R#/cytoscape_toolkit//bioModels/TRN.vb"

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

    '   Total Lines: 18
    '    Code Lines: 12
    ' Comment Lines: 4
    '   Blank Lines: 2
    '     File Size: 597 B


    ' Module TRN
    ' 
    '     Function: ExpressionConnections
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Model.Network.Regulons

''' <summary>
''' Transcription Regulation Network Builder Tools
''' </summary>
''' 
<Package("bioModels.TRN")>
Module TRN

    <ExportAPI("fpkm.connections")>
    Public Function ExpressionConnections(fpkm As DataSet(), Optional cutoff# = 0.65) As Connection()
        Return fpkm.CorrelationNetwork(cutoff).ToArray
    End Function
End Module
