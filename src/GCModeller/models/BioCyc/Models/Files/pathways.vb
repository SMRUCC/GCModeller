#Region "Microsoft.VisualBasic::b29f96c37cbb5bee5b50ece1e30899f5, models\BioCyc\Models\pathways.vb"

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

    '   Total Lines: 41
    '    Code Lines: 27 (65.85%)
    ' Comment Lines: 4 (9.76%)
    '    - Xml Docs: 75.00%
    ' 
    '   Blank Lines: 10 (24.39%)
    '     File Size: 1.21 KB


    ' Class pathways
    ' 
    '     Properties: enzymesNotUsed, inPathway, pathwayLinks, predecessors, primaries
    '                 reactionLayout, reactionList, subPathways, superPathways
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

''' <summary>
''' Pathways, including relationships among reactions
''' </summary>
''' 
<Xref("pathways.dat")>
Public Class pathways : Inherits Model

    <AttributeField("IN-PATHWAY")>
    Public Property inPathway As String()
    <AttributeField("SUPER-PATHWAYS")>
    Public Property superPathways As String()
    <AttributeField("SUB-PATHWAYS")>
    Public Property subPathways As String()

    <AttributeField("PATHWAY-LINKS")>
    Public Property pathwayLinks As String()

    <AttributeField("PREDECESSORS")>
    Public Property predecessors As String()

    <AttributeField("PRIMARIES")>
    Public Property primaries As String()

    <AttributeField("REACTION-LAYOUT")>
    Public Property reactionLayout As String()

    <AttributeField("REACTION-LIST")>
    Public Property reactionList As String()

    <AttributeField("ENZYMES-NOT-USED")>
    Public Property enzymesNotUsed As String()

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return If(commonName, uniqueId)
    End Function

End Class
