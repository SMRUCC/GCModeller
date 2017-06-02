Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.Expressions
Imports SMRUCC.genomics.Assembly.EBI.ChEBI.Database.IO.StreamProviders.Tsv.Tables
Imports SMRUCC.genomics.Assembly.EBI.ChEBI.XML

Namespace Assembly.EBI.ChEBI

    Public Module Extensions

        ''' <summary>
        ''' Gets the numeirc chebi main ID
        ''' </summary>
        ''' <param name="chebi"></param>
        ''' <returns></returns>
        <Extension>
        Public Function MainID(chebi As ChEBIEntity) As Long
            Return CLng(Val(chebi.chebiId.Split(":"c).Last))
        End Function

        ''' <summary>
        ''' 从用户所提供的有限的信息之中获取得到chebi编号结果
        ''' </summary>
        ''' <param name="chebi">可以使用<see cref="DATA.LoadNameOfDatabaseFromTsv"/>函数来构建出数据库参数</param>
        ''' <param name="id$"></param>
        ''' <param name="idtype"></param>
        ''' <param name="mass#">假设这个值是不会有大问题的</param>
        ''' <param name="names$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function FindChEBI(chebi As [NameOf], id$, idtype As AccessionTypes, Optional mass# = -1, Optional names$() = Nothing) As IEnumerable(Of String)
            Dim chebiIDlist$() = {""}
            Dim acc As Accession() = chebi.MatchByID(id, idtype, chebiIDlist(0))

            If chebiIDlist(Scan0).StringEmpty Then
                ' 按照编号没有查找结果
                ' 则尝试按照名字和质量来模糊查找
                acc = names _
                    .SafeQuery _
                    .Select(Function(name) chebi.MatchByName(name, fuzzy:=True)) _
                    .IteratesALL _
                    .Distinct _
                    .ToArray
                chebiIDlist = acc _
                    .Select(Of String)("COMPOUND_ID") _
                    .Distinct _
                    .ToArray
            Else
            End If

            If mass > 0 Then
                ' 这些所获取得到的编号可能是一种物质的不同的化学形式
                ' 也可能是字符串模糊匹配出错了
                ' 之后还需要依靠mass来确定一个符合结果要求的编号
re0:            Dim comfirm As New List(Of String)

                For Each id$ In chebiIDlist
                    Dim chemical = chebi.GetChemicalDatas(chebiID:=id)
                    Dim MASS_find = Val(chemical.TryGetValue("MASS")?.CHEMICAL_DATA)

                    If Math.Abs(MASS_find - mass) <= 0.5 Then
                        comfirm += id
                    End If
                Next

                chebiIDlist = comfirm
            End If

            Return chebiIDlist
        End Function

        '''' <summary>
        '''' 从一个chebi编号查找出其他的编号，相当于查找``Secondary ChEBI IDs``关联结果
        '''' </summary>
        '''' <param name="chebiID$"></param>
        '''' <returns></returns>
        'Public Function FoundOthersChebiID(chebi As [NameOf], chebiID$) As String()

        'End Function

        ''' <summary>
        ''' 查找目标和指定的编号是否为``Secondary ChEBI IDs``关联结果
        ''' </summary>
        ''' <param name="compound"></param>
        ''' <param name="chebiID$">
        ''' 可能有两种形式：
        ''' 
        ''' 带有前缀的：CHEBI:4108
        ''' 没有前缀的：4108
        ''' </param>
        ''' <returns></returns>
        <Extension> Public Function TheSameAs(compound As ChEBIEntity, chebiID$) As Boolean
            If chebiID.IndexOf(":"c) = -1 Then
                chebiID = "CHEBI:" & chebiID
            End If

            With compound
                If .chebiId.TextEquals(chebiID) Then
                    Return True
                End If

                For Each id$ In .SecondaryChEBIIds
                    If id.TextEquals(chebiID) Then
                        Return True
                    End If
                Next
            End With

            Return False
        End Function

        ''' <summary>
        ''' 这个函数会返回主ID和副ID，都是纯数字形式的编号，没有``chebi:``前缀的
        ''' </summary>
        ''' <param name="compound"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function IDlist(compound As ChEBIEntity) As IEnumerable(Of String)
            Yield compound.chebiId.Split(":"c).Last

            For Each id As String In compound.SecondaryChEBIIds.SafeQuery
                Yield id.Split(":"c).Last
            Next
        End Function
    End Module
End Namespace