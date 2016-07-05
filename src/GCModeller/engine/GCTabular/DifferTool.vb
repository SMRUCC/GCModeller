#Region "Microsoft.VisualBasic::b076fe42b1f71d2673bc291d169584e6, ..\GCModeller\engine\GCTabular\DifferTool.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

