Public Module Release

    ''' <summary>
    ''' Release the clustal program files from this assembly module resources data.
    ''' (将本模块资源数据之中的Clustal程序释放至目标文件夹之中)
    ''' </summary>
    ''' <param name="DIR"></param>
    ''' <returns>返回clustal程序的路径</returns>
    ''' <remarks></remarks>
    Public Function ReleasePackage(DIR As String) As String
        On Error Resume Next

        Call FileIO.FileSystem.CreateDirectory(DIR)

        Call My.Resources.clustalo.FlushStream(path:=DIR & "/clustalo.exe")
        Call My.Resources.libgcc_s_dw2_1.FlushStream(path:=DIR & "/libgcc_s_dw2-1.dll")
        Call My.Resources.libgomp_1.FlushStream(path:=DIR & "/libgomp-1.dll")
        Call My.Resources.libstdc___6.FlushStream(path:=DIR & "/libstdc++-6.dll")
        Call My.Resources.mingwm10.FlushStream(path:=DIR & "/mingwm10.dll")
        Call My.Resources.pthreadGC2.FlushStream(path:=DIR & "/pthreadGC2.dll")

        Return DIR & "/clustalo.exe"
    End Function
End Module
