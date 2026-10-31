Imports System.IO
Imports System.Runtime.CompilerServices

Namespace ARACNE

    ''' <summary>
    ''' Zip file storage of the fitted <see cref="AracneNetwork"/> object.
    ''' </summary>
    Public Module StorageFile

        ''' <summary>
        ''' Save the fitted AracneNetwork object to a zip archive file
        ''' </summary>
        ''' <param name="aracne"></param>
        ''' <param name="file">file stream of the target zip archive to save model</param>
        ''' 
        <Extension>
        Public Sub Save(aracne As AracneNetwork, file As Stream)

        End Sub

        ''' <summary>
        ''' Load AracneNetwork object from zip archive file
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        Public Function Load(file As Stream) As AracneNetwork

        End Function
    End Module
End Namespace