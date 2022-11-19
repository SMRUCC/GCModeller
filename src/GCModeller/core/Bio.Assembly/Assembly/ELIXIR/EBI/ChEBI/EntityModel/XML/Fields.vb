#Region "Microsoft.VisualBasic::ab5efc62bff7a5fc7c237e0d196a49f9, GCModeller\core\Bio.Assembly\Assembly\ELIXIR\EBI\ChEBI\EntityModel\XML\Fields.vb"

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

    '   Total Lines: 98
    '    Code Lines: 66
    ' Comment Lines: 11
    '   Blank Lines: 21
    '     File Size: 2.82 KB


    '     Class CompoundOrigin
    ' 
    '         Properties: componentAccession, componentText, SourceAccession, SourceType, speciesAccession
    '                     speciesText
    ' 
    '         Function: ToString
    ' 
    '     Class OntologyParents
    ' 
    '         Properties: chebiId, chebiName, cyclicRelationship, status, type
    ' 
    '         Function: ToString
    ' 
    '     Class DatabaseLinks
    ' 
    '         Properties: data, type
    ' 
    '         Function: ToString
    ' 
    '     Class ChemicalStructures
    ' 
    '         Properties: [structure], defaultStructure, dimension, type
    ' 
    '     Class Synonyms
    ' 
    '         Properties: data, source, type
    ' 
    '         Function: ToString
    ' 
    '     Class RegistryNumbers
    ' 
    '         Properties: data, source, type
    ' 
    '         Function: ToString
    ' 
    '     Class Formulae
    ' 
    '         Properties: data, source
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.ELIXIR.EBI.ChEBI.XML

    Public Class CompoundOrigin
        Public Property speciesText As String
        Public Property speciesAccession As String
        Public Property componentText As String
        Public Property componentAccession As String
        Public Property SourceType As String
        Public Property SourceAccession As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class OntologyParents
        Public Property chebiName As String
        Public Property chebiId As String
        Public Property type As String
        Public Property status As String
        Public Property cyclicRelationship As Boolean

        Public Overrides Function ToString() As String
            Return chebiName
        End Function
    End Class

    Public Class DatabaseLinks

        Public Property data As String
        Public Property type As String

        Public Const HMDB_accession$ = "HMDB accession"
        Public Const KEGG_COMPOUND_accession$ = "KEGG COMPOUND accession"
        Public Const KEGG_DRUG_accession$ = "KEGG DRUG accession"

        Public Overrides Function ToString() As String
            Return $"[{type}] {data}"
        End Function
    End Class

    Public Class ChemicalStructures
        Public Property [structure] As String
        Public Property type As String
        Public Property dimension As String
        Public Property defaultStructure As String
    End Class

    Public Class Synonyms

        Public Property data As String
        Public Property source As String
        Public Property type As String

        Public Overrides Function ToString() As String
            Return $"[{source}] {data}"
        End Function
    End Class

    Public Class RegistryNumbers

        Public Property data As String
        Public Property source As String
        Public Property type As String

        Public Const Type_Reaxys$ = "Reaxys Registry Number"
        Public Const Type_Beilstein$ = "Beilstein Registry Number"
        Public Const Type_CAS$ = "CAS Registry Number"

        Public Overrides Function ToString() As String
            Return data
        End Function

    End Class

    ''' <summary>
    ''' 分子式
    ''' </summary>
    Public Class Formulae

        ''' <summary>
        ''' 分子式字符串
        ''' </summary>
        ''' <returns></returns>
        Public Property data As String
        ''' <summary>
        ''' 分子式的数据来源
        ''' </summary>
        ''' <returns></returns>
        Public Property source As String

        Public Overrides Function ToString() As String
            Return data
        End Function
    End Class
End Namespace
