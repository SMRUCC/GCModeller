#Region "Microsoft.VisualBasic::72e84d30246b3f901f230ea6d5b2cac4, analysis\Metagenome\microbiomeGWAS\C\dExp2.vb"

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

    ' Module DExp2
    ' 
    '     Sub: dExp2
    ' 
    ' /********************************************************************************/

#End Region

Public Module DExp2

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="[Dim]"></param>
    ''' <param name="Xv">Xv is the input dist matrix, Dim*Dim dimension</param>
    ''' <param name="nCol"></param>
    ''' <param name="simIdxV">simIdxV is a Dim * nCol matrix, each column is a simulation index vector</param>
    ''' <param name="len"></param>
    ''' <param name="result">result is a vector with length = len, corresponding to the 7 definitions and dmean</param>
    Public Sub dExp2(ByRef [Dim] As Integer, Xv As Double(), ByRef nCol As Integer, simIdxV As Integer(), ByRef len As Integer, ByRef result As Double())
        Dim N__1 As Integer = [Dim]
        Dim nSim As Integer = nCol
        ' int n = *len;
        Dim iSim As Integer
        Dim ii As Integer
        Dim jj As Integer
        Dim i As Integer
        Dim j As Integer
        Dim k As Integer
        Dim l As Integer
        Dim m As Integer
        Dim n__2 As Integer
        Dim tmp As Double

        'Initilize the result vector to all 0
        For i = 0 To len - 1
            result(i) = 0
        Next

        'Calculate mean		
        For i = 0 To N__1 - 2
            For j = i + 1 To N__1 - 1
                result(0) += Xv(i * N__1 + j)
            Next
        Next
        result(0) = result(0) / (CDbl(N__1) * (N__1 - 1) / 2)
        ' minus mean from Xv
        For i = 0 To N__1 - 2
            For j = i + 1 To N__1 - 1
                Xv(i * N__1 + j) -= result(0)
                Xv(j * N__1 + i) -= result(0)
            Next
        Next

        For jj = 0 To nSim - 1
            ' ii, ii+1, ii+2, ii+3
            ' for(ii = 0; ii + 3 < N; ii = ii + 4)
            ii = 0
            While ii < N__1 - 3
                iSim = jj * N__1 + ii
                i = simIdxV(iSim)
                j = simIdxV(iSim + 1)
                k = simIdxV(iSim + 2)
                l = simIdxV(iSim + 3)
                tmp = Xv(i * N__1 + j) * Xv(j * N__1 + k) * Xv(k * N__1 + l)
                ' result[6] += Xv[i*N+j] * Xv[j*N+k] * Xv[k*N+l];
                result(6) += tmp
                ' result[12] += Xv[i*N+j] * Xv[i*N+j] * Xv[j*N+k] * Xv[k*N+l];
                ' result[13] += Xv[i*N+j] * Xv[j*N+k] * Xv[j*N+k] * Xv[k*N+l];
                ' result[16] += Xv[i*N+j] * Xv[j*N+k] * Xv[k*N+l] * Xv[i*N+l];
                result(12) += Xv(i * N__1 + j) * tmp
                result(13) += Xv(j * N__1 + k) * tmp
                result(16) += Xv(i * N__1 + l) * tmp

                tmp = Xv(i * N__1 + j) * Xv(i * N__1 + k) * Xv(i * N__1 + l)
                ' result[7] += Xv[i*N+j] * Xv[i*N+k] * Xv[i*N+l];
                result(7) += tmp
                ' result[14] += Xv[i*N+j] * Xv[i*N+j] * Xv[i*N+k] * Xv[i*N+l];
                ' result[15] += Xv[i*N+j] * Xv[j*N+k] * Xv[i*N+k] * Xv[i*N+l];
                result(14) += Xv(i * N__1 + j) * tmp
                result(15) += Xv(j * N__1 + k) * tmp

                result(17) += Xv(i * N__1 + j) * Xv(i * N__1 + j) * Xv(k * N__1 + l) * Xv(k * N__1 + l)
                ii = ii + 4
            End While
        Next
        tmp = (N__1 \ 4) * CDbl(nSim)
        result(6) = result(6) / tmp
        result(7) = result(7) / tmp
        result(12) = result(12) / tmp
        result(13) = result(13) / tmp
        result(14) = result(14) / tmp
        result(15) = result(15) / tmp
        result(16) = result(16) / tmp
        result(17) = result(17) / tmp


        For jj = 0 To nSim - 1
            ' ii, ii+1, ii+2, ii+3, ii + 4
            ' for(ii = 0; ii + 4 < N; ii = ii + 5)
            ii = 0
            While ii < N__1 - 4
                iSim = jj * N__1 + ii
                i = simIdxV(iSim)
                j = simIdxV(iSim + 1)
                k = simIdxV(iSim + 2)
                l = simIdxV(iSim + 3)
                m = simIdxV(iSim + 4)

                result(18) += Xv(i * N__1 + j) * Xv(j * N__1 + k) * Xv(k * N__1 + l) * Xv(l * N__1 + m)

                tmp = Xv(i * N__1 + j) * Xv(i * N__1 + k) * Xv(i * N__1 + l)
                ' result[18] += Xv[i*N+j] * Xv[i*N+k] * Xv[i*N+l] * Xv[i*N+m];
                result(19) += tmp * Xv(i * N__1 + m)
                ' result[19] += Xv[i*N+j] * Xv[i*N+k] * Xv[i*N+l] * Xv[l*N+m];
                result(20) += tmp * Xv(l * N__1 + m)

                result(21) += Xv(i * N__1 + j) * Xv(i * N__1 + k) * Xv(l * N__1 + m) * Xv(l * N__1 + m)
                ii = ii + 5
            End While
        Next
        tmp = (N__1 \ 5) * CDbl(nSim)
        result(18) = result(18) / tmp
        result(19) = result(19) / tmp
        result(20) = result(20) / tmp
        result(21) = result(21) / tmp

        For jj = 0 To nSim - 1
            ' ii, ii+1, ii+2, ii+3, ii + 4, ii + 5
            ' for(ii = 0; ii + 5 < N; ii = ii + 6)
            ii = 0
            While ii < N__1 - 5
                iSim = jj * N__1 + ii
                i = simIdxV(iSim)
                j = simIdxV(iSim + 1)
                k = simIdxV(iSim + 2)
                l = simIdxV(iSim + 3)
                m = simIdxV(iSim + 4)
                n__2 = simIdxV(iSim + 5)


                result(22) += Xv(i * N__1 + j) * Xv(i * N__1 + k) * Xv(l * N__1 + m) * Xv(l * N__1 + n__2)
                ii = ii + 6
            End While
        Next
        tmp = (N__1 \ 6) * CDbl(nSim)
        result(22) = result(22) / tmp
    End Sub
End Module
