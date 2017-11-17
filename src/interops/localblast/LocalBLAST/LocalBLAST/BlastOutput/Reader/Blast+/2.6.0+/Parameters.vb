#Region "Microsoft.VisualBasic::e69382313450df9161ff719231fe8bc6, ..\localblast\LocalBLAST\LocalBLAST\BlastOutput\Reader\Blast+\2.2.28\Parameters.vb"

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

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Scripting.Runtime

Namespace LocalBLAST.BLASTOutput.BlastPlus

    Public Structure ParameterSummary

        Dim Database, PostedDate As String
        ''' <summary>
        ''' Number of letters in database
        ''' </summary>
        ''' <remarks></remarks>
        Dim Charts As String
        Dim SequenceCounts As String

        Dim Matrix As String
        Dim GapPenaltiesExistence, GapPenaltiesExtension As Double
        ''' <summary>
        ''' Neighboring words threshold
        ''' </summary>
        ''' <remarks></remarks>
        Dim NWThreshold As Double
        ''' <summary>
        ''' Window for multiple hits
        ''' </summary>
        ''' <remarks></remarks>
        Dim Window As Double

        Public Shared Function TryParse(text As String) As ParameterSummary
            Dim Database As String = Mid(text.Match("Database[:].+$", RegexOptions.Multiline), 11).Trim
            Dim PostedDate As String = Mid(text.Match("Posted date[:].+$", RegexOptions.Multiline), 15).Trim
            Dim Charts As String = Mid(text.Match("Number of letters in database[:].+$", RegexOptions.Multiline), 31).Trim
            Dim SequenceCounts As String = Mid(text.Match("Number of sequences in database[:].+$", RegexOptions.Multiline), 33).Trim
            Dim Matrix As String = Mid(text.Match("Matrix[:].+$", RegexOptions.Multiline), 9).Trim
            Dim GapPenaltiesExistence As Double = text.Match("Existence[:]\s*\d+[,]").RegexParseDouble
            Dim GapPenaltiesExtension As Double = text.Match(", Extension[:]\s*\d+").RegexParseDouble
            Dim NWThreshold As Double = text.Match("Neighboring words threshold[:]\s*\d+").RegexParseDouble
            Dim Window As Double = text.Match("Window for multiple hits[:]\s*\d+").RegexParseDouble

            Return New ParameterSummary With {
                .Database = Database,
                .Charts = Charts,
                .GapPenaltiesExistence = GapPenaltiesExistence,
                .GapPenaltiesExtension = GapPenaltiesExtension,
                .Matrix = Matrix,
                .NWThreshold = NWThreshold,
                .PostedDate = PostedDate,
                .SequenceCounts = SequenceCounts,
                .Window = Window
            }
        End Function
    End Structure
End Namespace
