Namespace Settings

    ''' <summary>
    ''' The configuration model of the GCModeller docker environment.
    ''' </summary>
    Public Class Docker

        ''' <summary>
        ''' Docker容器对象的image编号
        ''' </summary>
        ''' <returns></returns>
        Public Property ImageID As String
        ''' <summary>
        ''' 应用程序的文件夹路径
        ''' </summary>
        ''' <returns></returns>
        Public Property AppHome As String

#Region "FileSystem Mount"
        ''' <summary>
        ''' 数据共享文件夹的宿主机上面的路径
        ''' </summary>
        ''' <returns></returns>
        Public Property Local As String
        ''' <summary>
        ''' 数据共享文件夹再虚拟机之中的路径
        ''' </summary>
        ''' <returns></returns>
        Public Property Virtual As String
#End Region

    End Class
End Namespace