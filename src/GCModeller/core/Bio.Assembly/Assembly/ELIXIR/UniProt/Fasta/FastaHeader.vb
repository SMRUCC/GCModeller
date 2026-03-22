#Region "Microsoft.VisualBasic::f8d9c614804556f20be0527de990ec6f, core\Bio.Assembly\Assembly\ELIXIR\UniProt\UniprotFasta.vb"

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

'   Total Lines: 169
'    Code Lines: 79 (46.75%)
' Comment Lines: 72 (42.60%)
'    - Xml Docs: 95.83%
' 
'   Blank Lines: 18 (10.65%)
'     File Size: 8.30 KB


'     Class FastaHeader
' 
'         Properties: EntryName, GN, OrgnsmSpName, PE, ProtName
'                     SV, UniprotID
' 
'         Function: ToString
' 
'     Class UniprotFasta
' 
'         Properties: UniProtHeader, UniprotID
' 
'         Function: CreateObject, LoadFasta, ParseHeader, SimpleHeaderParser
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Assembly.Uniprot

    Public Class FastaHeader : Implements INamedValue

        ''' <summary>
        ''' UniqueIdentifier Is the primary accession number of the UniProtKB entry.
        ''' </summary>
        ''' <returns></returns>
        Public Property UniprotID As String Implements INamedValue.Key
        ''' <summary>
        ''' EntryName Is the entry name of the UniProtKB entry.
        ''' </summary>
        ''' <returns></returns>
        Public Property EntryName As String
        ''' <summary>
        ''' OrganismName Is the scientific name of the organism of the UniProtKB entry.
        ''' </summary>
        ''' <returns></returns>
        Public Property OrgnsmSpName As String
        ''' <summary>
        ''' GeneName Is the first gene name of the UniProtKB entry. If there Is no gene name, OrderedLocusName Or ORFname, the GN field Is Not listed. GeneName(基因名称)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property GN As String
        ''' <summary>
        ''' ProteinExistence Is the numerical value describing the evidence for the existence of the protein.
        ''' </summary>
        ''' <returns></returns>
        Public Property PE As String
        ''' <summary>
        ''' SequenceVersion Is the version number of the sequence.
        ''' </summary>
        ''' <returns></returns>
        Public Property SV As String

        Public Property OX As String

        ''' <summary>
        ''' ProteinName Is the recommended name of the UniProtKB entry as annotated in the RecName field. For UniProtKB/TrEMBL entries without a RecName field, the SubName field Is used. 
        ''' In case of multiple SubNames, the first one Is used. The 'precursor' attribute is excluded, 'Fragment' is included with the name if applicable.
        ''' </summary>
        ''' <returns></returns>
        Public Property ProtName As String

        Public Overrides Function ToString() As String
            Return String.Format("sp|{0}|{1} {2} OS={3} GN={4} PE={5} SV={6}", EntryName, UniprotID, ProtName, OrgnsmSpName, GN, PE, SV)
        End Function

        Public Shared Function Parse(title As String) As FastaHeader
            Try
                If title Is Nothing Then
                    Return New FastaHeader
                Else
                    title = title.Trim("-"c, "&"c, " "c, "|"c, "+"c)
                End If

                Return UniprotFasta.ParseHeader(title, Nothing)
            Catch ex As Exception
                Return New FastaHeader
            End Try
        End Function
    End Class

End Namespace
