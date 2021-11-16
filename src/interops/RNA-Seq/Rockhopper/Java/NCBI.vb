#Region "Microsoft.VisualBasic::898108d3f5690ffbfc9e0bd8bad6a37c, RNA-Seq\Rockhopper\Java\NCBI.vb"

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

    '     Class NCBI
    ' 
    '         Properties: valid_Genome, valid_Rockhopper
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: createRepliconDictionary, downloadFile, establishServerConnection, getRepliconDir, getRepliconFileName
    '                   getRepliconNames, prepGenomeFiles, readInRepliconList
    ' 
    '         Sub: Main
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Oracle.Java.net
Imports Microsoft.VisualBasic

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

    ''' <summary>
    ''' Maintains a list of bacterial genomes and their annotations available from NCBI.
    ''' </summary>
    Public Class NCBI

        ''' <summary>
        '''***************************************
        ''' **********   CLASS VARIABLES   **********
        ''' </summary>

        Public Shared HASH_SIZE As Integer = 3
        Private Shared fileName As String = "replicons.txt"
        Private Shared Genome_url As String = "http://cs.wellesley.edu/~btjaden/genomes/"
        Private Shared Rockhopper_url As String = "http://cs.wellesley.edu/~btjaden/Rockhopper/"
        Public Shared genome_DIR As String = output_DIR + "genomes/"



        ''' <summary>
        '''******************************************
        ''' **********   INSTANCE VARIABLES   **********
        ''' </summary>

        Private Genome_valid As Boolean
        ' We are able to establish a connection to the server.
        Private Rockhopper_valid As Boolean
        ' We are able to establish a connection to the server.
        Private repliconList As List(Of String)
        Private repliconDictionary As Dictionary(Of String, Dictionary(Of Integer, Boolean))



        ''' <summary>
        '''************************************
        ''' **********   CONSTRUCTORS   **********
        ''' </summary>

        Public Sub New()
            Rockhopper_valid = establishServerConnection(Rockhopper_url & fileName)
            repliconList = readInRepliconList()
            repliconDictionary = createRepliconDictionary(repliconList)
        End Sub



        ''' <summary>
        '''***********************************************
        ''' **********   PUBLIC INSTANCE METHODS   **********
        ''' </summary>

        ''' <summary>
        ''' Returns true if a connection to the Genome server has been successfully established.
        ''' Returns false otherwise.
        ''' </summary>
        Public Overridable ReadOnly Property valid_Genome() As Boolean
            Get
                Return Me.Genome_valid
            End Get
        End Property

        ''' <summary>
        ''' Returns true if a connection to the NCBI server has been successfully established.
        ''' Returns false otherwise.
        ''' </summary>
        Public Overridable ReadOnly Property valid_Rockhopper() As Boolean
            Get
                Return Me.Rockhopper_valid
            End Get
        End Property

        ''' <summary>
        ''' Return a list of all replicon names that contain
        ''' the substring s.
        ''' </summary>
        Public Overridable Function getRepliconNames(s As String) As List(Of String)
            s = s.ToLower()
            Dim reps As New List(Of String)()
            If s.Length < HASH_SIZE Then
                Return reps
            End If
            Dim seed As String = s.Substring(0, HASH_SIZE)
            If Not repliconDictionary.ContainsKey(seed) Then
                Return reps
            End If
            Dim list As Dictionary(Of Integer, Boolean) = repliconDictionary(seed)
            For Each i As Integer In list.Keys
                Dim repliconName As String = StringSplit(repliconList(CInt(i)), vbTab, True)(0)
                If repliconName.ToLower().IndexOf(s) >= 0 Then
                    reps.Add(repliconName)
                End If
            Next
            Collections.Sort(reps)
            Return reps
        End Function

        ''' <summary>
        ''' Return the name of a FASTA file on the Genome server corresponding
        ''' to the given replicon name.
        ''' </summary>
        Public Overridable Function getRepliconFileName(repliconName As String) As String
            repliconName = repliconName.ToLower()
            If repliconName.Length < HASH_SIZE Then
                Return ""
            End If
            Dim seed As String = repliconName.Substring(0, HASH_SIZE)
            If Not repliconDictionary.ContainsKey(seed) Then
                Return ""
            End If
            Dim list As Dictionary(Of Integer, Boolean) = repliconDictionary(seed)
            For Each i As Integer In list.Keys
                Dim parse_repliconInfo As String() = StringSplit(repliconList(CInt(i)), vbTab, True)
                If repliconName.ToUpper() = parse_repliconInfo(0).ToUpper() Then
                    Return parse_repliconInfo(1)
                End If
            Next
            Return ""
        End Function

        ''' <summary>
        ''' Ensures the appropriate genomes files (*.fna, *.ptt, *.rnt) are
        ''' available locally for the specified replicon. If not, the
        ''' files are copied from the server and created locally.
        ''' Returns true if successful, false otherwise (in which
        ''' case the user must specifify the files locally).
        ''' </summary>
        Public Overridable Function prepGenomeFiles(repliconName As String) As Boolean
            Dim successful As Boolean = True
            Dim serverFileName As String = getRepliconFileName(repliconName)
            If serverFileName.Length = 0 Then
                Return False
            End If
            Dim slashIndex As Integer = serverFileName.LastIndexOf("/")
            Dim periodIndex As Integer = serverFileName.LastIndexOf(".")
            Dim baseFileName As String = serverFileName.Substring(slashIndex + 1, periodIndex - (slashIndex + 1))
            Dim replicon_DIR As String = getRepliconDir(repliconName)
            Dim resultsDIR As New Oracle.Java.IO.File(output_DIR)
            If Not resultsDIR.exists() Then
                resultsDIR.mkdir()
            End If
            Dim genDIR As New Oracle.Java.IO.File(genome_DIR)
            If Not genDIR.exists() Then
                genDIR.mkdir()
            End If
            Dim repliconDIR As New Oracle.Java.IO.File(genome_DIR & replicon_DIR)
            If Not repliconDIR.exists() Then
                repliconDIR.mkdir()
            End If
            Dim genomeFile As New Oracle.Java.IO.File(genome_DIR & replicon_DIR & baseFileName & ".fna")
            If Not genomeFile.exists() Then
                successful = downloadFile(serverFileName, genome_DIR & replicon_DIR & baseFileName & ".fna")
            End If
            Dim geneFile As New Oracle.Java.IO.File(genome_DIR & replicon_DIR & baseFileName & ".ptt")
            If Not geneFile.exists() Then
                downloadFile(serverFileName.Substring(0, serverFileName.Length - 4) & ".ptt", genome_DIR & replicon_DIR & baseFileName & ".ptt")
            End If
            Dim rnaFile As New Oracle.Java.IO.File(genome_DIR & replicon_DIR & baseFileName & ".rnt")
            If Not rnaFile.exists() Then
                downloadFile(serverFileName.Substring(0, serverFileName.Length - 4) & ".rnt", genome_DIR & replicon_DIR & baseFileName & ".rnt")
            End If
            Return successful
        End Function



        ''' <summary>
        '''************************************************
        ''' **********   PRIVATE INSTANCE METHODS   **********
        ''' </summary>

        ''' <summary>
        ''' Returns true if we can connect to the servers, false otherwise.
        ''' </summary>
        Private Function establishServerConnection(url As String) As Boolean
            Try
                Dim reader As New Scanner(New URL(url).openStream())
                Return True
            Catch e As IOException
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Read in the list of replicons either from a local file or from
        ''' the Rockhopper server. If there is no local file or its file
        ''' size differs from that of the server file, then a new local
        ''' file is created. Returns an empty list if neither a local
        ''' file nor the server file could be found/accessed.
        ''' </summary>
        Private Function readInRepliconList() As List(Of String)
            Dim repliconList As New List(Of String)()

            Try
                ' Local file
                Dim resultsDIR As New Oracle.Java.IO.File(output_DIR)
                If Not resultsDIR.exists() Then
                    resultsDIR.mkdir()
                End If
                Dim genDIR As New Oracle.Java.IO.File(genome_DIR)
                If Not genDIR.exists() Then
                    genDIR.mkdir()
                End If
                Dim repFile As New Oracle.Java.IO.File(genome_DIR & fileName)
                Dim fileLength As Long = 0
                If repFile.exists() Then
                    fileLength = repFile.length
                End If

                ' Server file
                Dim serverLength As Long = 0
                If valid_Rockhopper Then
                    serverLength = (New URL(Rockhopper_url & fileName)).openConnection().contentLength
                End If

                ' Neither local file nor server file is available.
                If Not repFile.exists() AndAlso Not valid_Rockhopper Then
                    Return repliconList
                End If

                ' Use appropriate file (local or server) to read in replicon information.
                Dim reader As Scanner
                Dim writer As PrintWriter = Nothing
                If serverLength = fileLength Then
                    reader = New Scanner(repFile)
                Else
                    ' Read from server rather than local file. Create new local file.
                    reader = New Scanner(New URL(Rockhopper_url & fileName).openStream())
                    writer = New PrintWriter(repFile)
                End If
                While reader.hasNextLine()
                    Dim repliconInfo As String = reader.nextLine()
                    repliconList.Add(repliconInfo)
                    If serverLength <> fileLength Then
                        writer.println(repliconInfo)
                    End If
                End While
                reader.close()
                If serverLength <> fileLength Then
                    writer.close()
                End If
            Catch e As IOException
                Output("Could not process file " & genome_DIR & fileName & " and file " & Rockhopper_url & fileName & vbLf)
            End Try
            Return repliconList
        End Function

        ''' <summary>
        ''' Create dictionary of replicon tokens. Values are indices in ArrayList.
        ''' </summary>
        Private Function createRepliconDictionary(repliconList As List(Of String)) As Dictionary(Of String, Dictionary(Of Integer, Boolean))
            Dim repliconDictionary As New Dictionary(Of String, Dictionary(Of Integer, Boolean))()
            For i As Integer = 0 To repliconList.Count - 1
                Dim name As String = StringSplit(repliconList(i), vbTab, True)(0).ToLower()
                For j As Integer = 0 To name.Length - HASH_SIZE
                    Dim key As String = name.Substring(j, HASH_SIZE)
                    If Not repliconDictionary.ContainsKey(key) Then
                        repliconDictionary.Add(key, New Dictionary(Of Integer, Boolean)())
                    End If
                    repliconDictionary(key).Add(i, True)
                Next
            Next
            Return repliconDictionary
        End Function

        ''' <summary>
        ''' Retrieves a file from a server and stores it locally.
        ''' Returns true on success, false otherwise.
        ''' </summary>
        Private Function downloadFile(serverFileName As String, localFileName As String) As Boolean
            Try
                'System.setProperty("java.net.preferIPv4Stack", "true");  // Windows firewall bug
                Dim reader As New Scanner(New URL(serverFileName).openStream())
                Dim writer As New PrintWriter(New Oracle.Java.IO.File(localFileName))
                While reader.hasNextLine()
                    writer.println(reader.nextLine())
                End While
                reader.close()
                writer.close()
                Return True
            Catch e As IOException
                Return False
            End Try
        End Function



        ''' <summary>
        '''********************************************
        ''' **********   PUBLIC CLASS METHODS   **********
        ''' </summary>

        Public Shared Function getRepliconDir(repliconName As String) As String
            For i As Integer = 0 To repliconName.Length - 1
                Dim ch As Char = repliconName(i)
                If Not Char.IsLetterOrDigit(ch) Then
                    repliconName = repliconName.Replace(ch, "_"c)
                End If
            Next
            Return repliconName & "/"
        End Function



        ''' <summary>
        '''***********************************
        ''' **********   MAIN METHOD   **********
        ''' </summary>

        Private Shared Sub Main(args As String())

        End Sub

    End Class

End Namespace
