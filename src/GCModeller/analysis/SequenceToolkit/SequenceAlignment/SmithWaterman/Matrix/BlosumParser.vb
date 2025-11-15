#Region "Microsoft.VisualBasic::511f958996e1edb3be56219b1c0b7bcb, analysis\SequenceToolkit\SmithWaterman\Matrix\BlosumParser.vb"

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

'   Total Lines: 59
'    Code Lines: 38 (64.41%)
' Comment Lines: 15 (25.42%)
'    - Xml Docs: 86.67%
' 
'   Blank Lines: 6 (10.17%)
'     File Size: 2.16 KB


' Module BlosumParser
' 
'     Function: LoadFromStream, LoadMatrix
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace BestLocalAlignment

    ''' <summary>
    ''' Parser for the NCBI blosum matrix file
    ''' </summary>
    Public Module BlosumParser

        ''' <summary>
        ''' Load Blosum matrix from the text file, and this Blosum matrix file which is available downloads from NCBI FTP site.
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
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

            ' skip the comment information 
            ' lines
            Do While tokens.Read(i).First = "#"c
            Loop

            Dim matrix() = LinqAPI.Exec(Of NamedValue(Of Integer())) _
                                                                     _
                () <= From line As String
                      In tokens.Skip(i)
                      Where Not String.IsNullOrWhiteSpace(line)
                      Let row As String() = line.Split()
                      Let base As String = row(Scan0)
                      Let score As Integer() = row.Skip(1) _
                          .Where(Function(x)
                                     Return Not String.IsNullOrWhiteSpace(x)
                                 End Function) _
                          .Select(Function(x) CInt(Val(x))) _
                          .ToArray
                      Select New NamedValue(Of Integer()) With {
                          .Name = base,
                          .Value = score
                      }

            Return New Blosum(matrix.Keys.ToArray) With {
                .matrix = matrix _
                    .Select(Function(r) r.Value) _
                    .ToArray
            }
        End Function
    End Module
End Namespace