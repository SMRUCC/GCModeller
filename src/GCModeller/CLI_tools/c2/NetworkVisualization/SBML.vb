Namespace NetworkVisualization

    Public Class SBML : Implements System.IDisposable

        Dim MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder

        Sub New(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder)
            Me.MetaCyc = MetaCyc
        End Sub

        Public Function Export() As LANS.SystemsBiology.Assembly.SBML.Level2.XmlFile
            Return New LANS.SystemsBiology.Assembly.SBML.Level2.XmlFile With
                   {
                       .Model = New LANS.SystemsBiology.Assembly.SBML.Level2.Elements.Model With
                                {
                                    .listOfSpecies = GetSpecies(),
                                    .listOfReactions = GetNetwork()}}
        End Function

        Private Function GetSpecies() As Generic.List(Of LANS.SystemsBiology.Assembly.SBML.Level2.Elements.Specie)
            Dim LQuery = From Compound As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Compound
                         In MetaCyc.GetCompounds
                         Select New LANS.SystemsBiology.Assembly.SBML.Level2.Elements.Specie With
                                {
                                    .ID = Compound.Identifier, .name = Compound.CommonName} '
            Return LQuery.ToList
        End Function

        Private Function GetNetwork() As Generic.List(Of LANS.SystemsBiology.Assembly.SBML.Level2.Elements.Reaction)
            Dim LQuery = From Reaction As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Reaction
                          In MetaCyc.GetReactions
                          Select New LANS.SystemsBiology.Assembly.SBML.Level2.Elements.Reaction With {
                              .id = Reaction.Identifier, .name = Reaction.CommonName,
                              .reversible = Reaction.ReactionDirection,
                              .listOfProducts = (From s As String In Reaction.Right
                                                 Select New LANS.SystemsBiology.Assembly.SBML.Level2.Elements.Reaction.speciesReference With
                                                        {
                                                            .species = s, .stoichiometry = 1}).ToList,
                              .listOfReactants = (From s As String In Reaction.Left
                                                  Select New LANS.SystemsBiology.Assembly.SBML.Level2.Elements.Reaction.speciesReference With
                                                         {
                                                             .species = s, .stoichiometry = 1}).ToList
                          } '
            Return LQuery.ToList
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' 检测冗余的调用

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO:  释放托管状态(托管对象)。
                End If

                ' TODO:  释放非托管资源(非托管对象)并重写下面的 Finalize()。
                ' TODO:  将大型字段设置为 null。
            End If
            Me.disposedValue = True
        End Sub

        ' TODO:  仅当上面的 Dispose(ByVal disposing As Boolean)具有释放非托管资源的代码时重写 Finalize()。
        'Protected Overrides Sub Finalize()
        '    ' 不要更改此代码。    请将清理代码放入上面的 Dispose(ByVal disposing As Boolean)中。
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' Visual Basic 添加此代码是为了正确实现可处置模式。
        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。    请将清理代码放入上面的 Dispose (disposing As Boolean)中。
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace