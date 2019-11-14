#Region "Microsoft.VisualBasic::f1b0a9b60caba80251032eb4b0fff537, CLI_tools\c2\Reconstruction\Genome\Promoter\SubjectPromoter.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '         Class SubjectPromoter
    ' 
    '             Properties: GetExported
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Function: Export, Performance
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Reconstruction : Partial Class Promoters

        ''' <summary>
        ''' Exporting the promoter sequence in the subject MeatCyc database.
        ''' </summary>
        ''' <remarks>导出100bp的片段长度</remarks>
        Public Class SubjectPromoter : Inherits c2.Reconstruction.Operation

            Dim SubjectGenome As Promoters.PromoterFinder.Chromosome
            Dim Exported As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile

            Public ReadOnly Property GetExported As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
                Get
                    Return Exported
                End Get
            End Property

            Sub New(Entity As OperationSession)
                Call MyBase.New(Entity)
                Me.SubjectGenome = New Promoters.PromoterFinder.Chromosome(Subject.Database.WholeGenome)
            End Sub

            Public Overrides Function Performance() As Integer
                Dim Promoters = MyBase.Subject.GetPromoters '获取Subject数据库中的启动子表
                Dim LQuery = From Promoter As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Promoter In Promoters
                             Let fsa As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken = Export(Promoter)
                             Where Not fsa Is Nothing
                             Select fsa '
                Me.Exported = LQuery.ToArray
                Return Exported.Count
            End Function

            ''' <summary>
            ''' 导出转录起始位点开始到-35区右端上游的75bp的片段区域
            ''' </summary>
            ''' <param name="Promoter"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Private Function Export(Promoter As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Promoter) As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken
                Dim TSS As Long = Val(Promoter.AbsolutePlus1Pos)  '获取转录起始位点
                If TSS = 0 Then Return Nothing '数据错误

                Dim fsa As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken = New LANS.SystemsBiology.SequenceModel.FASTA.FastaToken
                Dim dr As Integer = Promoter.Direction
                '获取启动子的序列
                If dr = -1 Then '启动子位于互补链 -35_right + 75
                    Dim sp As Long = Val(Promoter.Minus35Right) + 15
                    Dim l = sp - TSS
                    If l < 0 Then
                        l = 60
                        fsa.SequenceData = Mid(SubjectGenome.Complement, TSS, l)
                        ' fsa.Attributes = New String() {"gi", "66571685", "gb", "AAY47095.1", Promoter.UniqueId, String.Format("Dr:=-1 TSS={0} *SP={1}", TSS, sp)}
                    Else
                        fsa.SequenceData = Mid(SubjectGenome.Complement, TSS, l)
                        '  fsa.Attributes = New String() {"gi", "66571685", "gb", "AAY47095.1", Promoter.UniqueId, String.Format("Dr:=-1 TSS={0} SP={1}", TSS, sp)}
                    End If
                    fsa.SequenceData = fsa.SequenceData.Reverse.ToArray
                ElseIf dr = 1 Then  '启动子位于正义链 -35_left - 75
                    Dim sp As Long = Val(Promoter.Minus35Left) - 15
                    If sp < 0 Then
                        sp = TSS - 60
                        fsa.SequenceData = Mid(SubjectGenome.Sequence, sp, 60)
                        '  fsa.Attributes = New String() {"gi", "66571685", "gb", "AAY47095.1", Promoter.UniqueId, String.Format("Dr:=1 TSS={0} *SP={1}", TSS, sp)}
                    Else
                        fsa.SequenceData = Mid(SubjectGenome.Sequence, sp, TSS - sp)
                        '  fsa.Attributes = New String() {"gi", "66571685", "gb", "AAY47095.1", Promoter.UniqueId, String.Format("Dr:=1 TSS={0} SP={1}", TSS, sp)}
                    End If
                Else '无法确定的
                    Return Nothing
                End If
                fsa.Attributes = New String() {Promoter.Identifier}

                Return fsa
            End Function
        End Class
    End Class
End Namespace
