#Region "Microsoft.VisualBasic::79da10eb8452ef1e0263fdfe1b929168, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Files\Enzrxns.vb"

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

    '   Total Lines: 34
    '    Code Lines: 21
    ' Comment Lines: 9
    '   Blank Lines: 4
    '     File Size: 1.86 KB


    '     Class Enzrxns
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
    ''' Frames in the class Enzymatic-Reactions describe attributes of an enzyme with respect 
    ''' to a particular reaction. For reactions that are catalyzed by more than one enzyme, 
    ''' or for enzymes that catalyze more than one reaction, multiple Enzymatic-Reactions 
    ''' frames are created, one for each enzyme/reaction pair. For example, Enzymatic-Reactions 
    ''' frames can represent the fact that two enzymes that catalyze the same reaction may be 
    ''' controlled by different activators and inhibitors.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Enzrxns : Inherits DataFile(Of Slots.Enzrxn)

        Public Overrides ReadOnly Property AttributeList As String()
            Get
                Return {
                    "UNIQUE-ID", "TYPES", "COMMON-NAME", "ALTERNATIVE-COFACTORS", "ALTERNATIVE-SUBSTRATES",
                    "BASIS-FOR-ASSIGNMENT", "CITATIONS", "COFACTOR-BINDING-COMMENT", "COFACTORS",
                    "COFACTORS-OR-PROSTHETIC-GROUPS", "COMMENT", "COMMENT-INTERNAL", "CREDITS", "DATA-SOURCE",
                    "DBLINKS", "DOCUMENTATION", "ENZYME", "HIDE-SLOT?", "INSTANCE-NAME-TEMPLATE", "KCAT",
                    "KM", "MEMBER-SORT-FN", "PH-OPT", "PHYSIOLOGICALLY-RELEVANT?", "PROSTHETIC-GROUPS",
                    "REACTION", "REACTION-DIRECTION", "REGULATED-BY", "REQUIRED-PROTEIN-COMPLEX",
                    "SPECIFIC-ACTIVITY", "SYNONYMS", "TEMPERATURE-OPT", "TEMPLATE-FILE", "VMAX"
                }
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}  {1} frame object records.", DbProperty.ToString, FrameObjects.Count)
        End Function
    End Class
End Namespace
