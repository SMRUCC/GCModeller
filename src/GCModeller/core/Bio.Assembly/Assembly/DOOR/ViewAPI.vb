﻿#Region "Microsoft.VisualBasic::bb933c97a923e9b067ae59915cfab1d3, core\Bio.Assembly\Assembly\DOOR\ViewAPI.vb"

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

    '   Total Lines: 24
    '    Code Lines: 19 (79.17%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (20.83%)
    '     File Size: 1007 B


    '     Module ViewAPI
    ' 
    '         Function: GenerateLstIdString, GetFirstGene
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace Assembly.DOOR

    Public Module ViewAPI

        Public Function GetFirstGene(Operon As KeyValuePair(Of String, OperonGene())) As OperonGene
            If Operon.Value.First.Location.Strand = Strands.Forward Then
                Return (From Gene In Operon.Value Select Gene Order By Gene.Location.Left Ascending).First
            Else
                Return (From Gene In Operon.Value Select Gene Order By Gene.Location.Left Descending).First
            End If
        End Function

        Public Function GenerateLstIdString(Operon As KeyValuePairObject(Of String, OperonGene())) As String
            If Operon.Value.Count = 1 Then
                Return Operon.Value.First.Synonym
            End If
            Return String.Join("; ", (From GeneObject In Operon.Value Select GeneObject.Synonym).ToArray)
        End Function

    End Module
End Namespace
