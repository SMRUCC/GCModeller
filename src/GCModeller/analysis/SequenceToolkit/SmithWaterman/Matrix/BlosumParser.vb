#Region "Microsoft.VisualBasic::1868a31ab18e89b44bec54451981b70e, analysis\SequenceToolkit\SmithWaterman\Matrix\BlosumParser.vb"

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

    ' Module BlosumParser
    ' 
    '     Function: __toVector, LoadFromStream, LoadMatrix
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Linq
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions

''' <summary>
''' Parser for the NCBI blosum matrix file
''' </summary>
Public Module BlosumParser

    ''' <summary>
    ''' Load Blosum matrix from the text file, and this Blosum matrix file which is available downloads from NCBI FTP site.
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    Public Function LoadMatrix(path As String) As Blosum
        Return LoadFromStream(FileIO.FileSystem.ReadAllText(path))
    End Function

    ''' <summary>
    ''' Load Blosum matrix from the text file, and this Blosum matrix file which is available downloads from NCBI FTP site.
    ''' </summary>
    ''' <param name="doc"></param>
    ''' <returns></returns>
    Public Function LoadFromStream(doc As String) As Blosum
        Dim tokens$() = doc.LineTokens
        Dim i%

        Do While tokens.Read(i).First = "#"c
        Loop

        Dim matrix%()() = LinqAPI.Exec(Of Integer()) _
 _
            () <= From line As String
                  In tokens.Skip(i)
                  Where Not String.IsNullOrWhiteSpace(line)
                  Select scoreVector(line)

        Return New Blosum() With {
            .Matrix = matrix
        }
    End Function

    Private Function scoreVector(line As String) As Integer()
        Dim array%() = LinqAPI.Exec(Of Integer) _
 _
            () <= From x As String
                  In line.Split.Skip(1)
                  Where Not String.IsNullOrWhiteSpace(x)
                  Select CInt(Val(x))

        Return array
    End Function
End Module
