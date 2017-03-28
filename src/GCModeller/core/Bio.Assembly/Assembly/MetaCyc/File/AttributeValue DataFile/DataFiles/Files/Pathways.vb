#Region "Microsoft.VisualBasic::c2be6b0d5383de1e01f138357fda728b, ..\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Files\Pathways.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports SMRUCC.genomics.Assembly.MetaCyc.Schema.Reflection

Namespace Assembly.MetaCyc.File.DataFiles

    ''' <summary>
    ''' Frames in class Pathways encode metabolic and signaling pathways.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Pathways : Inherits DataFile(Of Slots.Pathway)

        Public Overrides ReadOnly Property AttributeList As String()
            Get
                Return {
                    "UNIQUE-ID", "TYPES", "COMMON-NAME", "CITATIONS", "CLASS-INSTANCE-LINKS",
                    "COMMENT", "CREDITS", "DBLINKS", "ENZYME-USE", "HYPOTHETICAL-REACTIONS",
                    "IN-PATHWAY", "NET-REACTION-EQUATION", "PATHWAY-INTERACTIONS", "PATHWAY-LINKS",
                    "POLYMERIZATION-LINKS", "PREDECESSORS", "PRIMARIES", "REACTION-LAYOUT",
                    "REACTION-LIST", "SPECIES", "SUB-PATHWAYS", "SUPER-PATHWAYS", "SYNONYMS"
                }
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}  {1} frame object records.", DbProperty.ToString, FrameObjects.Count)
        End Function
    End Class
End Namespace
