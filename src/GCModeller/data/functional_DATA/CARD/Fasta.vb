Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.ComponentModel.Loci
Imports r = System.Text.RegularExpressions.Regex

Public Class SeqHeader : Implements INamedValue

    Public Property AccessionID As String Implements INamedValue.Key
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