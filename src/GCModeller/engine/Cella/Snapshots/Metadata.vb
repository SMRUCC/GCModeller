
''' <summary>
''' 结果快照数据的元数据信息
''' </summary>
''' <remarks>
''' 结果快照数据在硬盘上的文件列表为：
''' 
''' ```
''' metadata.json
''' frame_1.json
''' frame_2.json
''' frame_3.json
''' ...
''' ```
''' 
''' 其中，metadata.json就是本对象的json序列化结果，而frame_xxx.json文件则是具体的数据帧快照<see cref="TimeFrameSnapshot"/>对象的json序列化结果
''' </remarks>
Public Class Metadata

    ''' <summary>
    ''' 总时间大小
    ''' </summary>
    ''' <returns></returns>
    Public Property total_time As Double
    ''' <summary>
    ''' 每一帧时间的数据快照对应的时间点，例如：
    ''' 
    ''' frame_1.json -> [0] 0.0min
    ''' frame_2.json -> [1] 1.0min
    ''' frame_3.json -> [2] 2.0min
    ''' </summary>
    ''' <returns></returns>
    Public Property time_frames As Double()

    ''' <summary>
    ''' 使用一个一维数组来表示一个三维空间的三维数组 boolean(,,)，<see cref="width"/>, <see cref="height"/>, <see cref="depth"/>标记了这个三维数组的维度信息。
    ''' 在这个三维空间中，采用boolean来标记模拟的空间形状，false表示空（不存在任何数据），true表示对应的位置是模拟环境空间的一部分
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' 1D index = (x * HEIGHT + y) * DEPTH + z
    ''' </remarks>
    Public Property shape As Boolean()

    ''' <summary>
    ''' width of the <see cref="shape"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property width As Integer
    ''' <summary>
    ''' height of the <see cref="shape"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property height As Integer
    ''' <summary>
    ''' depth of the <see cref="shape"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property depth As Integer
    ''' <summary>
    ''' 进行模拟计算的细胞的种类信息，具体的虚拟细胞实例会依照这个细胞元数据信息进行实例化
    ''' </summary>
    ''' <returns></returns>
    Public Property cells As Dictionary(Of String, CellMetadata)
    ''' <summary>
    ''' [pathway_id => molecule_id array]
    ''' </summary>
    ''' <returns></returns>
    Public Property pathways As Dictionary(Of String, String())

End Class

Public Class CellMetadata

    Public Property taxonomy As String
    ''' <summary>
    ''' [gene_id => GO terms]
    ''' </summary>
    ''' <returns></returns>
    Public Property genes As Dictionary(Of String, String())
    ''' <summary>
    ''' [gene_id => EC numbers]
    ''' </summary>
    ''' <returns></returns>
    Public Property ec_numbers As Dictionary(Of String, String())

End Class