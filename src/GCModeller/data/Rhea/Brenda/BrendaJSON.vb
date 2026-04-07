#Region "Microsoft.VisualBasic::dc15c7b0dc8f01e90466bca80b17d736, data\Rhea\Brenda\BrendaJSON.vb"

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

    '   Total Lines: 93
    '    Code Lines: 77 (82.80%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 16 (17.20%)
    '     File Size: 3.25 KB


    ' Class BrendaJSON
    ' 
    '     Properties: data, release, version
    ' 
    ' Class BrendaEnzymeData
    ' 
    '     Properties: activating_compound, application, cloned, cofactor, crystallization
    '                 expression, general_information, general_stability, ic50_value, id
    '                 inhibitor, ki_value, km_value, localization, metals_ions
    '                 molecular_weight, natural_substrates_products, ph_optimum, ph_range, posttranslational_modification
    '                 protein, protein_variants, purification, reaction, reaction_type
    '                 recommended_name, reference, renatured, specific_activity, substrates_products
    '                 subunits, synonyms, systematic_name, temperature_optimum, temperature_range
    '                 temperature_stability, turnover_number
    ' 
    '     Function: ToString
    ' 
    ' Class ReferenceData
    ' 
    '     Properties: authors, id, journal, pages, pmid
    '                 title, vol, year
    ' 
    '     Function: ToString
    ' 
    ' Class ProteinData
    ' 
    '     Properties: accessions, comment, id, organism, references
    '                 source
    ' 
    ' Class ValueData
    ' 
    '     Properties: comment, proteins, references, value
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Public Class BrendaJSON

    Public Property release As String
    Public Property version As String
    Public Property data As Dictionary(Of String, BrendaEnzymeData)

End Class

Public Class BrendaEnzymeData

    Public Property id As String
    Public Property recommended_name As String
    Public Property systematic_name As String
    Public Property reaction As ValueData()
    Public Property synonyms As ValueData()
    Public Property protein As Dictionary(Of String, ProteinData)
    Public Property reaction_type As ValueData()
    Public Property localization As ValueData()
    Public Property natural_substrates_products As ValueData()
    Public Property substrates_products As ValueData()
    Public Property turnover_number As ValueData()
    Public Property km_value As ValueData()
    Public Property ph_optimum As ValueData()
    Public Property ph_range As ValueData()
    Public Property specific_activity As ValueData()
    Public Property temperature_optimum As ValueData()
    Public Property temperature_range As ValueData()
    Public Property cofactor As ValueData()
    Public Property activating_compound As ValueData()
    Public Property inhibitor As ValueData()
    Public Property metals_ions As ValueData()
    Public Property molecular_weight As ValueData()
    Public Property posttranslational_modification As ValueData()
    Public Property subunits As ValueData()
    Public Property application As ValueData()
    Public Property protein_variants As ValueData()
    Public Property cloned As ValueData()
    Public Property crystallization As ValueData()
    Public Property purification As ValueData()
    Public Property renatured As ValueData()
    Public Property general_stability As ValueData()
    Public Property temperature_stability As ValueData()
    Public Property reference As Dictionary(Of String, ReferenceData)
    Public Property ki_value As ValueData()
    Public Property expression As ValueData()
    Public Property general_information As ValueData()
    Public Property ic50_value As ValueData()

    Public Overrides Function ToString() As String
        Return $"[{id}] {recommended_name}"
    End Function
End Class

Public Class ReferenceData

    Public Property id As String
    Public Property title As String
    Public Property authors As String()
    Public Property journal As String
    Public Property year As String
    Public Property pages As String
    Public Property vol As String
    Public Property pmid As String

    Public Overrides Function ToString() As String
        Return title
    End Function

End Class

Public Class ProteinData

    Public Property id As String
    Public Property organism As String
    Public Property references As String()
    Public Property comment As String
    Public Property source As String
    Public Property accessions As String()

End Class

Public Class ValueData

    Public Property value As String
    Public Property proteins As String()
    Public Property references As String()
    Public Property comment As String

    Public Overrides Function ToString() As String
        Return value
    End Function

End Class

