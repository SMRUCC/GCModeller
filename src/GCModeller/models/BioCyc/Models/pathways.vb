#Region "Microsoft.VisualBasic::e5a8e02b80ff7535e4fcb85dcc99f0e4, GCModeller\models\BioCyc\Models\pathways.vb"

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

    '   Total Lines: 39
    '    Code Lines: 25
    ' Comment Lines: 4
    '   Blank Lines: 10
    '     File Size: 1.07 KB


    ' Class pathways
    ' 
    '     Properties: enzymesNotUsed, inPathway, pathwayLinks, predecessors, primaries
    '                 reactionLayout, reactionList, subPathways, superPathways
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

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

    Public Overrides Function ToString() As String
        Return If(commonName, uniqueId)
    End Function

End Class
