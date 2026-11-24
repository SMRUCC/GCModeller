Imports System.Runtime.CompilerServices

Namespace Writer

    ''' <summary>
    ''' 便捷的落盘扩展方法：直接对数据模型集合或者读取到的数据集合执行写盘操作
    ''' </summary>
    Public Module DatFileExtensions

        ''' <summary>
        ''' 将一组数据模型对象写出为对应的 BioCyc 平面文件(文件名由 ``XrefAttribute`` 推导)
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="data"></param>
        ''' <param name="dataDir"></param>
        ''' <param name="meta"></param>
        ''' <returns></returns>
        <Extension>
        Public Function SaveDat(Of T As Model)(data As IEnumerable(Of T), dataDir As String, meta As FileMeta) As String
            Return New BioCycDatFile(Of T)(meta, data).Save(dataDir)
        End Function

        ''' <summary>
        ''' 将一个已经读取到内存之中的 BioCyc 数据集合重新写盘(可用于数据库的二次编辑或者格式转换)
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="data"></param>
        ''' <param name="dataDir"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Save(Of T As Model)(data As AttrDataCollection(Of T), dataDir As String) As String
            Dim meta As FileMeta = data.fileMeta

            If meta Is Nothing Then
                meta = New FileMeta
            End If

            Return New BioCycDatFile(Of T)(meta, data.features).Save(dataDir)
        End Function

    End Module

End Namespace
