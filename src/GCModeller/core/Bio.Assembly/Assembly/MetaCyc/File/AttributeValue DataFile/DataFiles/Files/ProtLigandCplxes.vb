#Region "Microsoft.VisualBasic::d6e33f7df4f52328a558ecdd3ba83b7c, ..\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Files\ProtLigandCplxes.vb"

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
    ''' The file lists all the complexes of proteins with small-molecule ligands in the PGDB.
    ''' (在本文件中列出了本菌种内的所有与小分子配基所形成的蛋白质复合物)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ProtLigandCplxes : Inherits DataFile(Of Slots.ProtLigandCplxe)

        Public Overrides ReadOnly Property AttributeList As String()
            Get
                Return {
                    "UNIQUE-ID", "TYPES", "COMMON-NAME", "ABBREV-NAME", "AROMATIC-RINGS",
                    "ATOM-CHARGES", "CATALYZES", "CITATIONS", "COFACTORS-OF",
                    "COFACTORS-OR-PROSTHETIC-GROUPS-OF", "COMMENT", "COMMENT-INTERNAL",
                    "COMPONENT-COEFFICIENTS", "COMPONENT-OF", "COMPONENTS", "CONSENSUS-SEQUENCE",
                    "CREDITS", "DATA-SOURCE", "DBLINKS", "DNA-FOOTPRINT-SIZE", "DOCUMENTATION",
                    "ENZYME-NOT-USED-IN", "GO-TERMS", "HAS-NO-STRUCTURE?", "HIDE-SLOT?", "IN-MIXTURE",
                    "ISOZYME-SEQUENCE-SIMILARITY", "LOCATIONS", "MEMBER-SORT-FN", "MODIFIED-FORM",
                    "MOLECULAR-WEIGHT", "MOLECULAR-WEIGHT-EXP", "MOLECULAR-WEIGHT-KD",
                    "MOLECULAR-WEIGHT-SEQ", "N+1-NAME", "N-1-NAME", "N-NAME", "NEIDHARDT-SPOT-NUMBER",
                    "PI", "PROSTHETIC-GROUPS-OF", "RADICAL-ATOMS", "REGULATED-BY", "REGULATES",
                    "SPECIES", "STRUCTURE-BONDS", "SUPERATOMS", "SYMMETRY", "SYNONYMS", "TEMPLATE-FILE"
                }
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}  {1} frame object records.", DbProperty.ToString, FrameObjects.Count)
        End Function
    End Class
End Namespace
