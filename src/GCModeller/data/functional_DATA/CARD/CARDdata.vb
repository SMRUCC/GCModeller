#Region "Microsoft.VisualBasic::606209514b425e83984bb0a3b0bfe68a, ..\GCModeller\data\functional_DATA\CARD\CARDdata.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2018 GPL3 Licensed
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

Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports r = System.Text.RegularExpressions.Regex

''' <summary>
''' the Comprehensive Antibiotic Resistance Database
''' > https://card.mcmaster.ca/about
''' </summary>
Public Module CARDdata

    Public Iterator Function FastaParser(directory As String) As IEnumerable(Of SeqHeader)
        For Each fasta As FastaToken In StreamIterator.SeqSource(handle:=directory, debug:=True)
            Dim headers$() = fasta.Attributes

            If headers.Length = 4 Then
                ' protien
                Yield SeqHeader.ProteinHeader(headers)
            Else
                ' nucleotide
                Yield SeqHeader.NucleotideHeader(headers)
            End If
        Next
    End Function
End Module

Public Class SeqHeader

    Public Property AccessionID As String
    Public Property ARO As String
    Public Property name As String
    Public Property species As String
    ''' <summary>
    ''' 只针对核酸序列存在这个属性的值
    ''' </summary>
    ''' <returns></returns>
    <Column("loci", GetType(LocationParser))>
    Public Property Location As NucleotideLocation

    Public Overrides Function ToString() As String
        Return name
    End Function

    Public Shared Function ProteinHeader(headers$()) As SeqHeader
        Dim sp$ = r.Match(headers(3), "\[.+\]").Value
        Dim name$ = headers(3).Replace(sp, "").Trim

        Return New SeqHeader With {
            .AccessionID = headers(1),
            .ARO = headers(2),
            .name = name,
            .species = sp.GetStackValue("[", "]")
        }
    End Function

    Public Shared Function NucleotideHeader(headers$()) As SeqHeader
        Dim strand As Strands = headers(2).GetStrand
        Dim range As IntRange = headers(3)
        Dim loc As New NucleotideLocation(range, strand)
        Dim sp$ = r.Match(headers(5), "\[.+\]").Value
        Dim name$ = headers(5).Replace(sp, "").Trim

        Return New SeqHeader With {
            .AccessionID = headers(1),
            .Location = loc,
            .ARO = headers(4),
            .name = name,
            .species = sp.GetStackValue("[", "]")
        }
    End Function

    Public Class LocationParser
        Implements IParser

        Public Overloads Function ToString(obj As Object) As String Implements IParser.ToString
            If obj Is Nothing Then
                Return ""
            Else
                Return DirectCast(obj, NucleotideLocation).ToString
            End If
        End Function

        Public Function TryParse(cell As String) As Object Implements IParser.TryParse
            Return NucleotideLocation.Parse(cell)
        End Function
    End Class
End Class