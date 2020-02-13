Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components

Namespace v15

    ''' <summary>
    ''' BIOM netCDF I/O
    ''' </summary>
    Public Module CDF

        Public Function ReadFile(biom As String) As v10.BIOMDataSet(Of Double)
            Using cdf As New netCDFReader(biom)
                Dim attributes = cdf.globalAttributes.ToDictionary(Function(a) a.name, Function(a As attribute)
                                                                                           Return a.
                                                                                       End Function)
            End Using
        End Function
    End Module

End Namespace

