Namespace Assembly.MetaCyc.Schema.Reflection

    ''' <summary>
    ''' 寻找MetaCyc数据库之中的任意两个对象之间的连接关系
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PathRoute

        Dim Database As MetaCyc.File.FileSystem.DatabaseLoadder
        Dim TableSchemas As TableSchema()

        ''' <summary>
        ''' MetaCyc数据库的数据文件夹
        ''' </summary>
        ''' <param name="MetaCycDir"></param>
        ''' <remarks></remarks>
        Sub New(MetaCycDir As String)
            Me.Database = MetaCyc.File.FileSystem.DatabaseLoadder.CreateInstance(MetaCycDir)
            Call InitializeSchema()
        End Sub

        Sub New(MetaCyc As MetaCyc.File.FileSystem.DatabaseLoadder)
            Me.Database = MetaCyc
            Call InitializeSchema()
        End Sub

        Private Sub InitializeSchema()
            Me.TableSchemas = New TableSchema() {
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.BindReaction), File.DataFiles.Slots.Object.Tables.bindrxns),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.Compound), File.DataFiles.Slots.Object.Tables.compounds),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.DNABindSite), File.DataFiles.Slots.Object.Tables.dnabindsites),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.Enzrxn), File.DataFiles.Slots.Object.Tables.enzrxns),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.Gene), File.DataFiles.Slots.Object.Tables.genes),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.Pathway), File.DataFiles.Slots.Object.Tables.pathways),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.Promoter), File.DataFiles.Slots.Object.Tables.promoters),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.ProteinFeature), File.DataFiles.Slots.Object.Tables.proteinfeatures),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.Protein), File.DataFiles.Slots.Object.Tables.proteins),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.ProtLigandCplxe), File.DataFiles.Slots.Object.Tables.protligandcplxes),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.Reaction), File.DataFiles.Slots.Object.Tables.reactions),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.Regulation), File.DataFiles.Slots.Object.Tables.regulation),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.Regulon), File.DataFiles.Slots.Object.Tables.regulons),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.Terminator), File.DataFiles.Slots.Object.Tables.terminators),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.TransUnit), File.DataFiles.Slots.Object.Tables.transunits),
                New TableSchema(GetType(MetaCyc.File.DataFiles.Slots.tRNA), File.DataFiles.Slots.Object.Tables.trna)}
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="objA">对象A的UniqueId属性</param>
        ''' <param name="objB">对象B的UniqueId属性</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 程序会首先尝试查找A-->B的最短路线，假若没有查找到，则会尝试查找B-->A的最短路线
        ''' </remarks>
        Public Function GetPath(objA As String, objB As String) As Integer
            Throw New NotImplementedException
        End Function

        Public Overrides Function ToString() As String
            Return Database.Database.ToString
        End Function
    End Class
End Namespace