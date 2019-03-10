#Region "Microsoft.VisualBasic::56a34ac7c992432107af12876e3450d5, RNA-Seq\Rockhopper\Java\Parameters.vb"

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

    '     Class Parameters
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: outputParametersToFile
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic

'
' * Copyright 2013 Brian Tjaden
' *
' * This file is part of Rockhopper.
' *
' * Rockhopper is free software: you can redistribute it and/or modify
' * it under the terms of the GNU General Public License as published by
' * the Free Software Foundation, either version 3 of the License, or
' * any later version.
' *
' * Rockhopper is distributed in the hope that it will be useful,
' * but WITHOUT ANY WARRANTY; without even the implied warranty of
' * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' * GNU General Public License for more details.
' *
' * You should have received a copy of the GNU General Public License
' * (in the file gpl.txt) along with Rockhopper.  
' * If not, see <http://www.gnu.org/licenses/>.
' 

Namespace Java

    Public Class Parameters

        Private ReadOnly fileName As String = "parameters.txt"

        ' Default values
        Private ReadOnly mismatches_DEFAULT As Double = Peregrine.percentMismatches
        Private ReadOnly minReadLength_DEFAULT As Double = Peregrine.percentSeedLength
        Private ReadOnly singleEndOrientation_DEFAULT As Boolean = Peregrine.singleEndOrientationReverseComplement
        Private ReadOnly matePairOrientation_DEFAULT As String = Peregrine.pairedEndOrientation
        Private ReadOnly numProcessors_DEFAULT As Integer = 0
        Private ReadOnly maxPairedEndLength_DEFAULT As Integer = Peregrine.maxPairedEndLength
        Private ReadOnly verboseOutput_DEFAULT As Boolean = verbose
        Private ReadOnly outputSAM_DEFAULT As Boolean = False
        Private ReadOnly isStrandSpecific_DEFAULT As Boolean = Not unstranded
        Private ReadOnly testDifferentialExp_DEFAULT As Boolean = computeExpression
        Private ReadOnly predictOperons_DEFAULT As Boolean = computeOperons
        Private ReadOnly transcriptBoundaries_DEFAULT As Boolean = computeTranscripts
        Private ReadOnly expressionParameter_DEFAULT As Double = transcriptSensitivity
        Private ReadOnly minReadsMapping_DEFAULT As Integer = Assembler.MIN_READS_MAPPING
        Private ReadOnly minTranscriptLength_DEFAULT As Integer = 2 * Assembler.k
        Private ReadOnly minSeedExpression_DEFAULT As Integer = Assembler.minSeedExpression
        Private ReadOnly minExtendExpression_DEFAULT As Integer = Assembler.minExpression
        Private ReadOnly outputDIR_DEFAULT As String = output_DIR

        ' Editable values
        Public mismatches As Double
        Public minReadLength As Double
        Public singleEndOrientation As Boolean
        Public matePairOrientation As String
        Public numProcessors As Integer
        Public maxPairedEndLength As Integer
        Public verboseOutput As Boolean
        Public outputSAM As Boolean
        Public isStrandSpecific As Boolean
        Public testDifferentialExp As Boolean
        Public predictOperons As Boolean
        Public transcriptBoundaries As Boolean
        Public expressionParameter As Double
        Public minReadsMapping As Integer
        Public minTranscriptLength As Integer
        Public minSeedExpression As Integer
        Public minExtendExpression As Integer
        Public outputDIR As String

        Private f As Oracle.Java.IO.File

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        Public Sub New()
            ' Initialize to default values
            mismatches = mismatches_DEFAULT
            minReadLength = minReadLength_DEFAULT
            singleEndOrientation = singleEndOrientation_DEFAULT
            matePairOrientation = matePairOrientation_DEFAULT
            numProcessors = numProcessors_DEFAULT
            maxPairedEndLength = maxPairedEndLength_DEFAULT
            verboseOutput = verboseOutput_DEFAULT
            outputSAM = outputSAM_DEFAULT
            isStrandSpecific = isStrandSpecific_DEFAULT
            testDifferentialExp = testDifferentialExp_DEFAULT
            predictOperons = predictOperons_DEFAULT
            transcriptBoundaries = transcriptBoundaries_DEFAULT
            expressionParameter = expressionParameter_DEFAULT
            minReadsMapping = minReadsMapping_DEFAULT
            minTranscriptLength = minTranscriptLength_DEFAULT
            minSeedExpression = minSeedExpression_DEFAULT
            minExtendExpression = minExtendExpression_DEFAULT
            outputDIR = outputDIR_DEFAULT

            ' Read in from file previous parameter values.
            Dim reader As Scanner = Nothing
            Try
                Dim f As New Oracle.Java.IO.File(StartupDirectory + fileName)
                If Not f.exists() Then
                    Return
                End If
                reader = New Scanner(f)
                ' Do nothing. Use default parameter values.
            Catch ex As FileNotFoundException
                ' Do nothing. Use default parameter values.

            End Try
            If reader IsNot Nothing Then
                Try
                    Dim version As String = reader.nextLine()
                    If Not version.Equals(version) Then
                        Throw New Exception()
                    End If
                    Dim mismatches_TEMP As Double = Convert.ToDouble(reader.nextLine())
                    Dim minReadLength_TEMP As Double = Convert.ToDouble(reader.nextLine())
                    Dim singleEndOrientation_TEMP As Boolean = CBool(Convert.ToBoolean(reader.nextLine()))
                    Dim matePairOrientation_TEMP As String = reader.nextLine()
                    Dim numProcessors_TEMP As Integer = Convert.ToInt32(reader.nextLine())
                    Dim maxPairedEndLength_TEMP As Integer = Convert.ToInt32(reader.nextLine())
                    Dim verboseOutput_TEMP As Boolean = CBool(Convert.ToBoolean(reader.nextLine()))
                    Dim outputSAM_TEMP As Boolean = CBool(Convert.ToBoolean(reader.nextLine()))
                    Dim isStrandSpecific_TEMP As Boolean = CBool(Convert.ToBoolean(reader.nextLine()))
                    Dim testDifferentialExp_TEMP As Boolean = CBool(Convert.ToBoolean(reader.nextLine()))
                    Dim predictOperons_TEMP As Boolean = CBool(Convert.ToBoolean(reader.nextLine()))
                    Dim transcriptBoundaries_TEMP As Boolean = CBool(Convert.ToBoolean(reader.nextLine()))
                    Dim expressionParameter_TEMP As Double = Convert.ToDouble(reader.nextLine())
                    Dim minReadsMapping_TEMP As Integer = Convert.ToInt32(reader.nextLine())
                    Dim minTranscriptLength_TEMP As Integer = Convert.ToInt32(reader.nextLine())
                    Dim minSeedExpression_TEMP As Integer = Convert.ToInt32(reader.nextLine())
                    Dim minExtendExpression_TEMP As Integer = Convert.ToInt32(reader.nextLine())
                    Dim outputDIR_TEMP As String = reader.nextLine()
                    reader.close()

                    mismatches = mismatches_TEMP
                    minReadLength = minReadLength_TEMP
                    singleEndOrientation = singleEndOrientation_TEMP
                    matePairOrientation = matePairOrientation_TEMP
                    numProcessors = numProcessors_TEMP
                    maxPairedEndLength = maxPairedEndLength_TEMP
                    verboseOutput = verboseOutput_TEMP
                    outputSAM = outputSAM_TEMP
                    isStrandSpecific = isStrandSpecific_TEMP
                    testDifferentialExp = testDifferentialExp_TEMP
                    predictOperons = predictOperons_TEMP
                    transcriptBoundaries = transcriptBoundaries_TEMP
                    expressionParameter = expressionParameter_TEMP
                    minReadsMapping = minReadsMapping_TEMP
                    minTranscriptLength = minTranscriptLength_TEMP
                    minSeedExpression = minSeedExpression_TEMP
                    minExtendExpression = minExtendExpression_TEMP
                    outputDIR = outputDIR_TEMP
                Catch e As Exception
                End Try
            End If
        End Sub

        Private Sub outputParametersToFile()
            Try
                Dim f As New Oracle.Java.IO.File(StartupDirectory)
                If Not f.exists() Then
                    f.mkdir()
                End If
                Dim writer As New PrintWriter(New Oracle.Java.IO.File(StartupDirectory + fileName))
                writer.println(version)
                writer.println(mismatches)
                writer.println(minReadLength)
                writer.println(singleEndOrientation)
                writer.println(matePairOrientation)
                writer.println(numProcessors)
                writer.println(maxPairedEndLength)
                writer.println(verboseOutput)
                writer.println(outputSAM)
                writer.println(isStrandSpecific)
                writer.println(testDifferentialExp)
                writer.println(predictOperons)
                writer.println(transcriptBoundaries)
                writer.println(expressionParameter)
                writer.println(minReadsMapping)
                writer.println(minTranscriptLength)
                writer.println(minSeedExpression)
                writer.println(minExtendExpression)
                writer.println(outputDIR)
                writer.close()
                ' Do nothing if parameter file could not be written.
            Catch ex As FileNotFoundException
            End Try
        End Sub

    End Class

End Namespace
