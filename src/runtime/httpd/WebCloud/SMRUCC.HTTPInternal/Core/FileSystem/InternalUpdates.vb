Imports System.Runtime.CompilerServices

Namespace Core.Cache

    ''' <summary>
    ''' 文件内存缓存的更新策略
    ''' 
    ''' 1. 先遍历已缓存的文件，将被删除的文件从内存中删除释放
    ''' 2. 再遍历已缓存的文件，比较文件的最后写日期，将发生更新的文件重新缓存到内存中
    ''' 3. 然后遍历文件夹，添加新的文件
    ''' </summary>
    Module InternalUpdates

        <Extension>
        Public Sub RunCacheUpdate(vfs As VirtualFileSystem)

        End Sub
    End Module
End Namespace