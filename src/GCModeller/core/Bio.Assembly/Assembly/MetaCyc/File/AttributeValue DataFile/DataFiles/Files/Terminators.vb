#Region "Microsoft.VisualBasic::8417f2204e18f51a31ad13eb60bce31d, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Files\Terminators.vb"

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

    '   Total Lines: 21
    '    Code Lines: 18
    ' Comment Lines: 0
    '   Blank Lines: 3
    '     File Size: 875 B


    '     Class Terminators
    ' 
    '         Properties: AttributeList
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Assembly.MetaCyc.File.DataFiles

    Public Class Terminators : Inherits DataFile(Of Slots.Terminator)

        Public Overrides ReadOnly Property AttributeList As String()
            Get
                Return {
                    "UNIQUE-ID", "TYPES", "COMMON-NAME", "CITATIONS", "COMMENT",
                    "COMMENT-INTERNAL", "COMPONENT-OF", "CREDITS", "DATA-SOURCE",
                    "DBLINKS", "DOCUMENTATION", "HIDE-SLOT?",
                    "INSTANCE-NAME-TEMPLATE", "LEFT-END-POSITION", "MEMBER-SORT-FN",
                    "RIGHT-END-POSITION", "SYNONYMS", "TEMPLATE-FILE"
                }
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}  {1} frame object records.", DbProperty.ToString, FrameObjects.Count)
        End Function
    End Class
End Namespace
