#Region "Microsoft.VisualBasic::b280fe6af188f9c94d44c2447e08807e, RNA-Seq\Rockhopper\Java\IGV_Ops.vb"

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

    '     Module IGV_Ops
    ' 
    '         Function: createBatchFileToLoadData, createGenomeRegistry, getGenomeDisplayName
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

    Public Module IGV_Ops

        ''' <summary>
        ''' Creates a genome registry for IGV. File is named genomes.txt.
        ''' </summary>
        Public Function createGenomeRegistry(genomeRegistryFile As Oracle.Java.IO.File, genomes As List(Of Genome)) As Boolean
            Try
                Dim writer As New PrintWriter(genomeRegistryFile)
                writer.println("<Server-Side Genome List>")
                For Each genome As Genome In genomes
                    writer.println(getGenomeDisplayName(genome.name) & vbTab & genome.baseFileName & ".genome" & vbTab & genome.name.Replace(" "c, "_"c))
                Next
                writer.close()
                Return True
            Catch ex As FileNotFoundException
                Return False
            End Try
        End Function

        '   ''' <summary>
        '   ''' Creates a genome archive file for use by IGV. This method
        '   ''' utilizes IGV code.
        '   ''' </summary>
        '   Public Shared Function createGenomeArchive(genomeArchiveFileName As String, genomeName As String, fastaFileName As String, geneFileName As String) As Boolean
        '	Try
        '           Call (New GenomeImporter()).createGenomeArchive(New Oracle.Java.IO.File(genomeArchiveFileName), genomeName.Replace(" "c, "_"c), getGenomeDisplayName(genomeName), fastaFileName, New Oracle.Java.IO.File(geneFileName), Nothing,
        '               Nothing)
        '           Return True
        '	Catch ex As IOException
        '		Return False
        '	End Try
        'End Function

        ''' <summary>
        ''' Create a batch script to execute when IGV launches to load
        ''' the sequencing reads data.
        ''' </summary>
        Public Function createBatchFileToLoadData(batchFileName As String, genomeName As String, dataFiles As String, rockhopperFiles As String) As Boolean
            Try
                Dim writer As New PrintWriter(New Oracle.Java.IO.File(batchFileName))
                writer.println("new")
                writer.println("genome " & genomeName.Replace(" "c, "_"c))
                If dataFiles.Length > 0 Then
                    writer.println("load " & dataFiles)
                End If
                If rockhopperFiles.Length > 0 Then
                    writer.println("load " & rockhopperFiles)
                End If
                writer.close()
                Return True
            Catch ex As FileNotFoundException
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Returns the name of the genome to display in IGV's drop-down menu.
        ''' </summary>
        Private Function getGenomeDisplayName(genomeName As String) As String
            genomeName = genomeName.Trim()
            Dim indexOfSpace As Integer = genomeName.IndexOf(" "c)
            If indexOfSpace >= 0 Then
                genomeName = genomeName(0) & "." & genomeName.Substring(indexOfSpace)
            End If
            Return genomeName
        End Function

    End Module

End Namespace
