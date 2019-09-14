#Region "Microsoft.VisualBasic::5a1e4ecad7a3e418c1ce902c60c2128f, analysis\Metagenome\microbiomeGWAS\C\parsePlink.vb"

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

    ' Module ParsePlink
    ' 
    '     Sub: parsePlink
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO

Public Module ParsePlink

    ' plinkBed, the file name of plink bed file
    ' NumSample, Number of Samples in Plink
    ' NumSNP, Number of SNPs in plink
    ' distMat, distance matrix NumSample * NumSample
    ' E, environment vector with length = NumSample
    ' result, 12 * NSNP, including
    ' 1:5		#0, #1, #2, #NA MAF of G
    ' 6:10		#0, #1, #2, #NA MAF of GE
    ' 11		SM
    ' 12		SI
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="plinkBed">plinkBed, the file name of plink bed file</param>
    ''' <param name="NumSample">NumSample, Number of Samples in Plink</param>
    ''' <param name="NumSNP">NumSNP, Number of SNPs in plink</param>
    ''' <param name="distMat">distMat, distance matrix NumSample * NumSample</param>
    ''' <param name="E">E, environment vector with length = NumSample</param>
    ''' <param name="result">result, 12 * NSNP, including
    ''' 
    ''' ```
    ''' 1:5		#0, #1, #2, #NA MAF of G
    ''' 6:10	#0, #1, #2, #NA MAF of GE
    ''' 11		SM
    ''' 12		SI
    ''' ```
    ''' </param>
    Public Sub parsePlink(plinkBed As String, ByRef NumSample As Integer, ByRef NumSNP As Integer, distMat As Double(), E As Integer(), result As Double())
        Dim NSam As Integer = NumSample
        Dim NSNP As Integer = NumSNP
        Dim Gmap As Integer() = New Integer(4 * 256 - 1) {}
        Dim G As Integer() = New Integer(NSam - 1) {}
        Dim GE As Integer() = New Integer(NSam - 1) {}
        Dim buffer As Integer
        ' note: 1 byte
        Dim tmp As Integer
        Dim sampleOffSet As Integer = NSam Mod 4
        Dim i As Integer
        Dim j As Integer
        Dim k As Integer
        Dim iResult As Integer
        Dim GDiff As Integer
        Dim dExp As Double = 0
        Dim rowResult As Integer = 12

        'initialize the result to all 0s

        For i = 0 To NSNP - 1
            For j = 0 To rowResult - 1
                result(i * rowResult + j) = 0
            Next
        Next

        ' minus mean from dExp
        For i = 0 To NSam - 2
            For j = i + 1 To NSam - 1
                dExp += distMat(i * NSam + j)
            Next
        Next

        dExp = dExp / (CDbl(NSam) * (NSam - 1) / 2)

        For i = 0 To NSam - 2
            For j = i + 1 To NSam - 1
                distMat(i * NSam + j) = distMat(i * NSam + j) - dExp
                distMat(j * NSam + i) = distMat(j * NSam + i) - dExp
            Next
        Next

        ' creat a map for a single byte to 4 genotype in the same order with plink.fam file
        ' counts are the number of Allele A1 in bim file
        For i = 0 To 255
            buffer = CByte(i)
            k = i * 4
            For j = 0 To 3
                tmp = (buffer >> (j * 2)) And 3
                Select Case tmp
                    Case 0
                        Gmap(k + j) = 2
                        Exit Select
                    Case 2
                        Gmap(k + j) = 1
                        Exit Select
                    Case 3
                        Gmap(k + j) = 0
                        Exit Select
                    Case Else
                        Gmap(k + j) = -9
                        Exit Select
                End Select
            Next
        Next


        Dim ptr_plink As New FileStream(plinkBed, FileMode.Open)
        If ptr_plink Is Nothing Then
            Console.Write("Unable to open file!")
        End If
        ptr_plink.Seek(3, SeekOrigin.Begin)

        For j = 0 To NSNP - 1
            ' fseek(ptr_plink, 3 + (NSam/4+1) * j, SEEK_SET);
            iResult = j * rowResult
            i = 0
            While i < NSam - 3
                buffer = ptr_plink.ReadByte
                ' tmp = tmp * 4
                tmp = (buffer << 2)
                For k = 0 To 3
                    G(i + k) = Gmap(tmp + k)
                    Select Case G(i + k)
                        Case 0
                            result(iResult) = result(iResult) + 1
                            Exit Select
                        Case 1
                            result(iResult + 1) = result(iResult + 1) + 1
                            Exit Select
                        Case 2
                            result(iResult + 2) = result(iResult + 2) + 1
                            Exit Select
                        Case Else
                            result(iResult + 3) = result(iResult + 3) + 1
                            Exit Select
                    End Select

                    Select Case E(i + k)
                        Case 0
                            GE(i + k) = (If(G(i + k) = -9, -9, 0))
                            Exit Select
                        Case 1
                            GE(i + k) = G(i + k)
                            Exit Select
                        Case Else
                            GE(i + k) = -9
                            Exit Select
                    End Select

                    Select Case GE(i + k)
                        Case 0
                            result(iResult + 5) = result(iResult + 5) + 1
                            Exit Select
                        Case 1
                            result(iResult + 6) = result(iResult + 6) + 1
                            Exit Select
                        Case 2
                            result(iResult + 7) = result(iResult + 7) + 1
                            Exit Select
                        Case Else
                            result(iResult + 8) = result(iResult + 8) + 1
                            Exit Select
                    End Select
                Next
                i = i + 4
            End While
            If sampleOffSet > 0 Then
                buffer = ptr_plink.ReadByte
                ' tmp = tmp * 4
                tmp = (buffer << 2)
                For k = 0 To sampleOffSet - 1
                    G(i + k) = Gmap(tmp + k)
                    Select Case G(i + k)
                        Case 0
                            result(iResult) = result(iResult) + 1
                            Exit Select
                        Case 1
                            result(iResult + 1) = result(iResult + 1) + 1
                            Exit Select
                        Case 2
                            result(iResult + 2) = result(iResult + 2) + 1
                            Exit Select
                        Case Else
                            result(iResult + 3) = result(iResult + 3) + 1
                            Exit Select
                    End Select

                    Select Case E(i + k)
                        Case 0
                            GE(i + k) = (If(G(i + k) = -9, -9, 0))
                            Exit Select
                        Case 1
                            GE(i + k) = G(i + k)
                            Exit Select
                        Case Else
                            GE(i + k) = -9
                            Exit Select
                    End Select

                    Select Case GE(i + k)
                        Case 0
                            result(iResult + 5) = result(iResult + 5) + 1
                            Exit Select
                        Case 1
                            result(iResult + 6) = result(iResult + 6) + 1
                            Exit Select
                        Case 2
                            result(iResult + 7) = result(iResult + 7) + 1
                            Exit Select
                        Case Else
                            result(iResult + 8) = result(iResult + 8) + 1
                            Exit Select
                    End Select
                Next
            End If

            result(iResult + 4) = (result(iResult + 1) + result(iResult + 2) + result(iResult + 2)) / (NSam + NSam - result(iResult + 3) - result(iResult + 3))
            result(iResult + 9) = (result(iResult + 6) + result(iResult + 7) + result(iResult + 7)) / (NSam + NSam - result(iResult + 8) - result(iResult + 8))

            For i = 0 To NSam - 2
                For k = i + 1 To NSam - 1
                    If (G(i) <> -9) AndAlso (G(k) <> -9) Then
                        GDiff = Math.Abs(G(i) - G(k))
                        ' if(GDiff > 0){
                        ' 	result[iResult + 10] += distMat[i*NSam + k];
                        ' 	if(GDiff > 1)
                        ' 		result[iResult + 10] += distMat[i*NSam + k];
                        ' }
                        Select Case GDiff
                            Case 0
                            Case 2
                                result(iResult + 10) += distMat(i * NSam + k)
                            Case Else
                                result(iResult + 10) += distMat(i * NSam + k)
                        End Select
                    End If

                    If (GE(i) <> -9) AndAlso (GE(k) <> -9) Then
                        GDiff = Math.Abs(GE(i) - GE(k))
                        ' if(GDiff > 0){
                        ' 	result[iResult + 11] += distMat[i*NSam + k];
                        ' 	if(GDiff > 1)
                        ' 		result[iResult + 11] += distMat[i*NSam + k];
                        ' }
                        Select Case GDiff
                            Case 0
                            Case 2
                                result(iResult + 11) += distMat(i * NSam + k)
                            Case Else
                                result(iResult + 11) += distMat(i * NSam + k)
                        End Select

                    End If
                Next
            Next
        Next
    End Sub
End Module
