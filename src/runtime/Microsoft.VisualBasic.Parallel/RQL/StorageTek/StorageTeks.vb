
Namespace StorageTek

    ''' <summary>
    ''' Entity storage technology.(实体对象所存储的方法)
    ''' </summary>
    Public Enum StorageTeks As Integer
        ''' <summary>
        ''' 使用的是Linq数据源
        ''' </summary>
        Linq
        ''' <summary>
        ''' Individual files in a directory.(以单独的文件的形式保存在一个文件夹之中)
        ''' </summary>
        DIR = 2
        ''' <summary>
        ''' Csv rows.(Csv文件的行映射为某一个实体对象)(需要外部插件)
        ''' </summary>
        Tabular = 4
        ''' <summary>
        ''' Xml文件之中的List之中的某一个对象映射为某一个实体对象
        ''' </summary>
        Xml = 8
        ''' <summary>
        ''' Json文件之中的list之中的某一个对象映射为某一个实体对象
        ''' </summary>
        Json = 16
        ''' <summary>
        ''' 实体对象是存储在MySQL数据库的某一张表之中的.(需要外部插件)
        ''' </summary>
        SQL = 32
        ''' <summary>
        ''' 二进制类型的数据源
        ''' </summary>
        ''' <remarks>例如图片，声音，视频等</remarks>
        Binary = 64
    End Enum
End Namespace