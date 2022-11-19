#Region "Microsoft.VisualBasic::daaa7ae23d7bdd3534e04c6ca4d6ef68, GCModeller\core\Bio.Assembly\Assembly\NCBI\SeqDump\NTheader.vb"

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

    '   Total Lines: 84
    '    Code Lines: 65
    ' Comment Lines: 3
    '   Blank Lines: 16
    '     File Size: 2.75 KB


    '     Structure NTheader
    ' 
    '         Function: ParseId, (+2 Overloads) ParseNTheader, ToString, Trim
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Assembly.NCBI.SequenceDump

    ''' <summary>
    ''' The fasta header of the nt database.
    ''' </summary>
    Public Structure NTheader

        Public Const AccessionId$ = "\S+?\d+\.\d+"

        Public gi As String
        Public db As String
        Public uid As String
        Public description As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Shared Function ParseId(title$) As String
            Dim id$ = Regex.Match(title, AccessionId).Value
            Return id
        End Function

        Public Shared Function ParseNTheader(attrs$(), Optional throwEx As Boolean = True) As IEnumerable(Of NTheader)
            Dim splits$()() = attrs.Skip(1).Split(4%)
            Dim out As New List(Of NTheader)
            Dim trimGI As Boolean = splits.Length > 1

            Try
                For Each b As String() In splits
                    If b.Length = 1 Then
                        Dim x As NTheader = out.Last

                        out(out.Count - 1) = New NTheader With {
                            .db = x.db,
                            .description = x.description & "|" & b(Scan0),
                            .gi = x.gi,
                            .uid = x.uid
                        }

                        Continue For
                    End If

                    Dim descr$ = Strings.Trim(b(3))

                    out += New NTheader With {
                        .gi = b(0),
                        .db = b(1),
                        .uid = b(2),
                        .description = If(trimGI, Trim(descr), descr)
                    }
                Next
            Catch ex As Exception
                ex = New Exception(attrs.JoinBy("|"), ex)

                If throwEx Then
                    Throw ex
                Else
                    Call App.LogException(ex)
                    Call ex.PrintException
                End If
            End Try

            Return out
        End Function

        Public Shared Function ParseNTheader(fa As FastaSeq, Optional throwEx As Boolean = True) As IEnumerable(Of NTheader)
            Return ParseNTheader(fa.Headers, throwEx)
        End Function

        Private Shared Function Trim(s As String) As String
            Dim gi$ = Mid(s, s.Length - 1)
            If String.Equals("gi", gi$, StringComparison.OrdinalIgnoreCase) Then
                s = Mid(s, 1, s.Length - 2)
            End If
            Return s
        End Function
    End Structure
End Namespace
