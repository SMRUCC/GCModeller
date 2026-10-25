Imports System.IO
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.SGrid.GRN

Namespace IO

    Public Module ZipPersists

        ''' <summary>
        ''' persists save the fitted model as zip archive file stream
        ''' </summary>
        ''' <param name="model"></param>
        ''' <param name="file"></param>
        ''' 
        <Extension>
        Public Sub Save(model As SSystemNetwork, file As Stream)

        End Sub

        ''' <summary>
        ''' Load SSystemNetwork model from an opened zip archive file stream
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        Public Function Load(file As Stream) As SSystemNetwork

        End Function
    End Module
End Namespace