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
        Dim tokens$() = doc.lTokens
        Dim i%

        Do While tokens.Read(i).First = "#"c
        Loop

        Dim matrix%()() = LinqAPI.Exec(Of Integer()) _
 _
            () <= From line As String
                  In tokens.Skip(i)
                  Where Not String.IsNullOrWhiteSpace(line)
                  Select __toVector(line)

        Return New Blosum() With {
            .Matrix = matrix
        }
    End Function

    Private Function __toVector(line As String) As Integer()
        Dim array%() = LinqAPI.Exec(Of Integer) _
 _
            () <= From x As String
                  In line.Split.Skip(1)
                  Where Not String.IsNullOrWhiteSpace(x)
                  Select CInt(Val(x))

        Return array
    End Function
End Module
