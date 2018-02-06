#Region "Microsoft.VisualBasic::eec806231333e76ab6c9cdbfaed09d93, ..\GCModeller\data\functional_DATA\CARD\Fasta.vb"

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

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.ComponentModel.Loci
Imports r = System.Text.RegularExpressions.Regex

Public Class SeqHeader : Implements INamedValue

    Public Property AccessionID As String Implements INamedValue.Key
    Public Property ARO As String
    Public Property name As String
    ''' <summary>
    ''' scientific name.(菌株或者物种的名称)
    ''' </summary>
    ''' <returns></returns>
    Public Property species As String
    ''' <summary>
    ''' 只针对核酸序列存在这个属性的值
    ''' </summary>
    ''' <returns></returns>
    <Column("loci", GetType(LocationParser))>
    Public Property loci As NucleotideLocation

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
            .loci = loc,
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
