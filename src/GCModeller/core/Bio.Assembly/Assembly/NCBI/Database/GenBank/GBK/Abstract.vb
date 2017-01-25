Namespace Assembly.NCBI.GenBank.GBFF

    ''' <summary>
    ''' Genbank数据库文件的构件
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class IgbComponent

        ''' <summary>
        ''' Link to the genbank raw object.(这个构件对象所处在的``genbank``数据库对象.)
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend gb As File
    End Class
End Namespace