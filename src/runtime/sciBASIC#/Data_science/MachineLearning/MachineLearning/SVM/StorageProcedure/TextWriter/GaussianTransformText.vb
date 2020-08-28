Imports System.Globalization
Imports System.IO

Namespace SVM

    Module GaussianTransformText

        ''' <summary>
        ''' Saves the transform to the disk.  The samples are not stored, only the 
        ''' statistics.
        ''' </summary>
        ''' <param name="stream">The destination stream</param>
        ''' <param name="transform">The transform</param>
        Public Sub Write(stream As Stream, transform As GaussianTransform)
            Dim output As StreamWriter = New StreamWriter(stream)
            output.WriteLine(transform._means.Length)

            For i = 0 To transform._means.Length - 1
                output.WriteLine("{0} {1}", transform._means(i), transform._stddevs(i))
            Next

            output.Flush()
        End Sub

        ''' <summary>
        ''' Reads a GaussianTransform from the provided stream.
        ''' </summary>
        ''' <param name="stream">The source stream</param>
        ''' <returns>The transform</returns>
        Public Function Read(stream As Stream) As GaussianTransform
            Dim input As StreamReader = New StreamReader(stream)
            Dim length As Integer = Integer.Parse(input.ReadLine(), CultureInfo.InvariantCulture)
            Dim means = New Double(length - 1) {}
            Dim stddevs = New Double(length - 1) {}

            For i = 0 To length - 1
                Dim parts As String() = input.ReadLine().Split()
                means(i) = Double.Parse(parts(0), CultureInfo.InvariantCulture)
                stddevs(i) = Double.Parse(parts(1), CultureInfo.InvariantCulture)
            Next

            Return New GaussianTransform(means, stddevs)
        End Function

        ''' <summary>
        ''' Saves the transform to the disk.  The samples are not stored, only the 
        ''' statistics.
        ''' </summary>
        ''' <param name="filename">The destination filename</param>
        ''' <param name="transform">The transform</param>
        Public Sub Write(filename As String, transform As GaussianTransform)
            Dim output = File.Open(filename, FileMode.Create)

            Try
                Write(output, transform)
            Finally
                output.Close()
            End Try
        End Sub

        ''' <summary>
        ''' Reads a GaussianTransform from the provided stream.
        ''' </summary>
        ''' <param name="filename">The source filename</param>
        ''' <returns>The transform</returns>
        Public Function Read(filename As String) As GaussianTransform
            Dim input = File.Open(filename, FileMode.Open)

            Try
                Return Read(input)
            Finally
                input.Close()
            End Try
        End Function
    End Module
End Namespace