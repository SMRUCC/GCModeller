#Region "Microsoft.VisualBasic::9040fa9de06809cc9926706f1f6f4dd4, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Files\ProteinFeature.vb"

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

    '   Total Lines: 31
    '    Code Lines: 22
    ' Comment Lines: 5
    '   Blank Lines: 4
    '     File Size: 1.44 KB


    '     Class ProteinFeatures
    ' 
    '         Properties: AttributeList
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.MetaCyc.Schema.Reflection

Namespace Assembly.MetaCyc.File.DataFiles

    ''' <summary>
    ''' Protein features (for example, active sites), This file lists all the protein 
    ''' features (such as active sites) in the PGDB. /* protein-features.dat */
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ProteinFeatures : Inherits DataFile(Of Slots.ProteinFeature)

        Public Overrides ReadOnly Property AttributeList As String()
            Get
                Return {
                    "UNIQUE-ID", "TYPES", "COMMON-NAME", "ALTERNATE-SEQUENCE",
                    "ATTACHED-GROUP", "CATALYTIC-ACTIVITY", "CITATIONS",
                    "COMMENT", "COMMENT-INTERNAL", "COMPONENT-OF", "CREDITS",
                    "DATA-SOURCE", "DBLINKS", "DOCUMENTATION", "FEATURE-OF",
                    "HIDE-SLOT?", "HOMOLOGY-MOTIF", "INSTANCE-NAME-TEMPLATE",
                    "LEFT-END-POSITION", "LINKAGE-TYPE", "MEMBER-SORT-FN",
                    "POSSIBLE-FEATURE-STATES", "RESIDUE-NUMBER", "RESIDUE-TYPE",
                    "RIGHT-END-POSITION", "SYNONYMS", "TEMPLATE-FILE"
                }
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}  {1} frame object records.", DbProperty.ToString, FrameObjects.Count)
        End Function
    End Class
End Namespace
