Imports LANS.SystemsBiology.Assembly
Imports LANS.SystemsBiology.DatabaseServices.Regprecise
Imports LANS.SystemsBiology.Toolkits.RNA_Seq
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' 编译调控模型
''' </summary>
''' <remarks></remarks>
Public Module RegulationNetworkFromFootprints

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data">请使用经过<see cref="DocumentFormat.RegulatesFootprints">Group操作</see>之后的调控数据</param>
    ''' <param name="PccMatrix"></param>
    ''' <remarks></remarks>
    Public Sub CompileFootprintsData(data As Generic.IEnumerable(Of DocumentFormat.RegulatesFootprints),
                                     PccMatrix As PccMatrix,
                                     OperonData As LANS.SystemsBiology.Assembly.DOOR.OperonView, _
 _
                                     ByRef TranscriptUnits As List(Of CsvTabular.FileStream.TranscriptUnit),
                                     ByRef Motifs As List(Of CsvTabular.FileStream.MotifSite),
                                     ByRef Regulators As List(Of FileStream.Regulator))

        'Dim FootprintData = (From item In data.AsParallel
        '                     Where Not String.IsNullOrEmpty(item.ORF) AndAlso Not item.MotifLocation = SegmentRelationships.DownStream
        '                     Select item
        '                     Group By item.ORF Into Group).ToArray  '首先按照ORF分组，将每一个ORF看作为一个转录单元，假若该ORF为操纵子之内的结构基因，则后面的基因都被看作为该转录单元的成员

        'Dim LQuery = (From ItemRegulation In FootprintData.AsParallel
        '              Let OperonId = ItemRegulation.Group.First.DoorId
        '              Let OperonGenes As String() = ItemRegulation.Group.First.StructGenes
        '              Let TUModel As FileStream.TranscriptUnit = New FileStream.TranscriptUnit With
        '                                                         {
        '                                                             .BasalValue = 5, .OperonGenes = OperonGenes,
        '                                                             .OperonId = OperonId,
        '                                                             .PromoterGene = ItemRegulation.ORF,
        '                                                             .RegulatorK = 1
        '                                                         }
        '              Let motifObjects = (From item
        '                                  In ItemRegulation.Group
        '                                  Select regulation = item, motifObject = New FileStream.MotifSite With
        '                                                                          {
        '                                                                              .ORF = ItemRegulation.ORF,
        '                                                                              .Position = item.Distance,
        '                                                                              .Regulators = item.Regulators.ToList,
        '                                                                              .TU_MODEL_GUID = TUModel.TU_GUID,
        '                                                                              .MotifName = item.MotifId}).ToArray
        '              Let motifIdList = (From item In motifObjects Select item.motifObject.Internal_GUID).ToArray
        '              Let RegulatorList = (From item In motifObjects Select (From p As Integer
        '                                                                     In item.regulation.Regulators.Sequence
        '                                                                     Let id As String = item.regulation.Regulators(p)
        '                                                                     Select New FileStream.Regulator With
        '                                                                            {
        '                                                                                .ProteinId = id,
        '                                                                                .RegulatesMotif = item.motifObject.Internal_GUID,
        '                                                                                .Pcc = Val(item.regulation.corr(p))
        '                                                                            }).ToArray).ToArray
        '              Let Modified = TUModel.InvokeSet(NameOf(TUModel.Motifs), motifIdList)
        '              Select motifObjects, TUModel, RegulatorList).ToArray

        'TranscriptUnits = (From item In LQuery Select item.TUModel).ToList
        'Motifs = (From item In LQuery Select (From n In item.motifObjects Select n.motifObject).ToArray).ToArray.MatrixToList
        'Regulators = (From item In LQuery Select item.RegulatorList.MatrixToVector).ToArray.MatrixToVector.ToList
    End Sub

    ''' <summary>
    ''' 赋值给<see cref="FileStream.Regulator.Effectors"></see>属性
    ''' </summary>
    ''' <param name="Regulators"></param>
    ''' <param name="Effectors"></param>
    ''' <param name="Regprecise"></param>
    ''' <remarks></remarks>
    Public Function MappingEffector(ByRef Regulators As List(Of FileStream.Regulator),
                                    Effectors As List(Of MetaCyc.Schema.EffectorMap),
                                    Regprecise As RegpreciseMPBBH()) As List(Of FileStream.Regulator)

        Dim RegulatorGroupedChunk = (From item In Regulators Select item Group item By item.ProteinId Into Group).ToArray
        Dim LQuery = (From item In RegulatorGroupedChunk.AsParallel
                      Let hits_effector As String() = (From besthit As LANS.SystemsBiology.DatabaseServices.Regprecise.RegpreciseMPBBH
                                                       In Regprecise
                                                       Where String.Equals(besthit.QueryName, item.ProteinId)
                                                       Let effector_cpds = besthit.Effectors
                                                       Select effector_cpds).ToArray.MatrixToVector.Distinct.ToArray
                      Let mapped_effector = (From id As String
                                             In hits_effector
                                             Select (From efc As MetaCyc.Schema.EffectorMap
                                                     In Effectors
                                                     Where String.Equals(efc.Effector, id)
                                                     Select efc).ToArray).ToArray.MatrixToVector
                      Select mapped_effector, item).ToArray
        Dim setValue = New SetValue(Of FileStream.Regulator)().GetSet(NameOf(FileStream.Regulator.Effectors))

        Regulators = LinqAPI.MakeList(Of FileStream.Regulator) <=
            From item
            In LQuery.AsParallel
            Let mapped_effector = (From n In item.mapped_effector Select n.KEGGCompound).ToArray
            Select From regulator As FileStream.Regulator
                   In item.item.Group.ToArray
                   Select setValue(regulator, mapped_effector)

        Return Regulators
    End Function

    Public Sub TCS__RR(ByRef Regulators As List(Of FileStream.Regulator), MisT2 As MiST2.MiST2)
        Dim setValue = New SetValue(Of FileStream.Regulator)() _
            .GetSet(NameOf(FileStream.Regulator.TCS_RR))

        Regulators =
            LinqAPI.MakeList(Of FileStream.Regulator) <=
                From regulator As FileStream.Regulator
                In Regulators
                Let b As Boolean = MisT2.IsRR(regulator.ProteinId)
                Select setValue(regulator, b)
    End Sub
End Module
