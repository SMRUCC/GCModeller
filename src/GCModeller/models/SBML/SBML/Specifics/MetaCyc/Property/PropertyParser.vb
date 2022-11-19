#Region "Microsoft.VisualBasic::a9fedf8a6a6225ee139a4af0a94a83fd, GCModeller\models\SBML\SBML\Specifics\MetaCyc\Property\PropertyParser.vb"

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

    '   Total Lines: 80
    '    Code Lines: 60
    ' Comment Lines: 12
    '   Blank Lines: 8
    '     File Size: 2.61 KB


    '     Enum FluxProperties
    ' 
    '         BIOCYC, ConfidenceLevel, ECNumber, GENE_ASSOCIATION, SUBSYSTEM
    ' 
    '  
    ' 
    ' 
    ' 
    '     Enum SpeciesProperties
    ' 
    '         BIOCYC, CAS, CHARGE, CHEBI, CHEMSPIDER
    '         DRUGBANK, FORMULA, HMDB, INCHI, KEGG
    '         METABOLIGHTS, PUBCHEM
    ' 
    '  
    ' 
    ' 
    ' 
    '     Module PropertyParser
    ' 
    '         Properties: FluxKeyMaps, SpeciesKeyMaps
    ' 
    '         Function: GetEcList, GetGenes
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Linq

Namespace Specifics.MetaCyc

    Public Enum FluxProperties
        BIOCYC
        ECNumber
        SUBSYSTEM
        GENE_ASSOCIATION
        ConfidenceLevel
    End Enum

    Public Enum SpeciesProperties
        BIOCYC
        INCHI
        CHEBI
        HMDB
        CHEMSPIDER
        PUBCHEM
        DRUGBANK
        CAS
        METABOLIGHTS
        KEGG
        FORMULA
        CHARGE
    End Enum

    Public Module PropertyParser

        Public ReadOnly Property SpeciesKeyMaps As IReadOnlyDictionary(Of SpeciesProperties, String) =
            New Dictionary(Of SpeciesProperties, String) From {
 _
                {SpeciesProperties.BIOCYC, "BIOCYC"},
                {SpeciesProperties.CAS, "CAS"},
                {SpeciesProperties.CHARGE, "CHARGE"},
                {SpeciesProperties.CHEBI, "CHEBI"},
                {SpeciesProperties.CHEMSPIDER, "CHEMSPIDER"},
                {SpeciesProperties.DRUGBANK, "DRUGBANK"},
                {SpeciesProperties.FORMULA, "FORMULA"},
                {SpeciesProperties.HMDB, "HMDB"},
                {SpeciesProperties.INCHI, "INCHI"},
                {SpeciesProperties.KEGG, "KEGG"},
                {SpeciesProperties.METABOLIGHTS, "METABOLIGHTS"},
                {SpeciesProperties.PUBCHEM, "PUBCHEM"}
        }

        Public ReadOnly Property FluxKeyMaps As IReadOnlyDictionary(Of FluxProperties, String) =
            New Dictionary(Of FluxProperties, String) From {
 _
                {FluxProperties.BIOCYC, "BIOCYC"},
                {FluxProperties.ECNumber, "EC Number"},
                {FluxProperties.SUBSYSTEM, "SUBSYSTEM"},
                {FluxProperties.GENE_ASSOCIATION, "GENE_ASSOCIATION"},
                {FluxProperties.ConfidenceLevel, "Confidence level"}
        }

        ''' <summary>
        ''' Example: 
        ''' GENE_ASSOCIATION: (XC_3424) or (XC_4096)
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Function GetGenes(value As String) As String()
            Dim ms As String() = Regex.Matches(value, "\(.+?\)").ToArray
            ms = ms.Select(Function(s) Mid(s, 2, s.Length - 2)).ToArray
            Return ms
        End Function

        ''' <summary>
        ''' Example:
        ''' EC Number: 2.3.1.85/2.3.1.86/4.2.1.59
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Function GetEcList(value As String) As String()
            Return value.Split("/"c)
        End Function
    End Module
End Namespace
