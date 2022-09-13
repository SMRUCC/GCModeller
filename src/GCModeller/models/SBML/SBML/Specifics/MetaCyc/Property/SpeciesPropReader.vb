#Region "Microsoft.VisualBasic::7745d8054e781eb502f119fa9e1512eb, GCModeller\models\SBML\SBML\Specifics\MetaCyc\Property\SpeciesPropReader.vb"

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

    '   Total Lines: 37
    '    Code Lines: 32
    ' Comment Lines: 0
    '   Blank Lines: 5
    '     File Size: 1.69 KB


    '     Class SpeciesPropReader
    ' 
    '         Properties: BIOCYC, CAS, CHARGE, CHEBI, CHEMSPIDER
    '                     DRUGBANK, FORMULA, HMDB, INCHI, KEGG
    '                     METABOLIGHTS, PUBCHEM
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Model.SBML.Components

Namespace Specifics.MetaCyc

    Public Class SpeciesPropReader : Inherits ReaderBase(Of SpeciesProperties)

        Sub New(note As Notes)
            Call MyBase.New(note.Properties, PropertyParser.SpeciesKeyMaps)

            Me.BIOCYC = __getValue(SpeciesProperties.BIOCYC)
            Me.CAS = __getValue(SpeciesProperties.CAS)
            Me.CHARGE = __getValue(SpeciesProperties.CHARGE)
            Me.CHEBI = __getValue(SpeciesProperties.CHEBI)
            Me.CHEMSPIDER = __getValue(SpeciesProperties.CHEMSPIDER)
            Me.DRUGBANK = __getValue(SpeciesProperties.DRUGBANK)
            Me.FORMULA = __getValue(SpeciesProperties.FORMULA)
            Me.HMDB = __getValue(SpeciesProperties.HMDB)
            Me.INCHI = __getValue(SpeciesProperties.INCHI)
            Me.KEGG = __getValue(SpeciesProperties.KEGG)
            Me.METABOLIGHTS = __getValue(SpeciesProperties.METABOLIGHTS)
            Me.PUBCHEM = __getValue(SpeciesProperties.PUBCHEM)
        End Sub

        Public ReadOnly Property BIOCYC As String
        Public ReadOnly Property INCHI As String
        Public ReadOnly Property CHEBI As String
        Public ReadOnly Property HMDB As String
        Public ReadOnly Property CHEMSPIDER As String
        Public ReadOnly Property PUBCHEM As String
        Public ReadOnly Property DRUGBANK As String
        Public ReadOnly Property CAS As String
        Public ReadOnly Property METABOLIGHTS As String
        Public ReadOnly Property KEGG As String
        Public ReadOnly Property FORMULA As String
        Public ReadOnly Property CHARGE As String
    End Class
End Namespace
