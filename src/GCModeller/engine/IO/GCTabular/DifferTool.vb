#Region "Microsoft.VisualBasic::2c281217d4110809d764cf417596b71a, engine\IO\GCTabular\DifferTool.vb"

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

    ' Class DifferTool
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: MergeMetaCycCompounds
    ' 
    '     Sub: Invoke
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem
Imports SMRUCC.genomics.Assembly.MetaCyc.Schema

Public Class DifferTool

    Dim _MetaCyc As DatabaseLoadder
    Dim _KEGG_Compounds As bGetObject.Compound()
    Dim _KEGG_Reactions As bGetObject.Reaction()

    Sub New(MetaCyc As DatabaseLoadder, KEGGCompounds As bGetObject.Compound(), KEGGReactions As bGetObject.Reaction())
        Me._MetaCyc = MetaCyc
        Me._KEGG_Compounds = KEGGCompounds
        Me._KEGG_Reactions = KEGGReactions
    End Sub

    Public Sub Invoke()

    End Sub

    Private Shared Function MergeMetaCycCompounds() As EffectorMap()
        Throw New NotImplementedException
    End Function
End Class
