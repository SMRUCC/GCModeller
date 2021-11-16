#Region "Microsoft.VisualBasic::efcd1b85f56b17deddb1e25755e308dc, meme_suite\MEME\Workflows\PromoterParser\ParserLocus.vb"

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

    '     Delegate Function
    ' 
    ' 
    '     Enum GetLocusTags
    ' 
    '         DOOR_InitX, locus, UniDOOR
    ' 
    ' 
    ' 
    '     Module ParserLocus
    ' 
    '         Function: [GetType], __locusItSelf, CreateMethod
    '         Class __DOORHelper
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Function: __initX, InitX, UniDOOR
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.DOOR

Namespace Workflows.PromoterParser

    ''' <summary>
    ''' 根据操纵子之中的某一个基因编号按照规则返回某些基因的编号
    ''' </summary>
    ''' <param name="locus"></param>
    ''' <returns></returns>
    Public Delegate Function IGetLocusTag(locus As String) As String()

    Public Enum GetLocusTags As Byte
        ''' <summary>
        ''' 不进行任何关联，则只返回本locus自身
        ''' </summary>
        locus
        UniDOOR
        DOOR_InitX
    End Enum

    Public Module ParserLocus

        ''' <summary>
        ''' locus/union/initx
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Public Function [GetType](name As String) As GetLocusTags
            Select Case name.ToLower
                Case "locus"
                    Return GetLocusTags.locus
                Case "union"
                    Return GetLocusTags.UniDOOR
                Case "initx"
                    Return GetLocusTags.DOOR_InitX
                Case Else
                    Throw New NotImplementedException(name)
            End Select
        End Function

        Public Function CreateMethod(DOOR As DOOR, type As GetLocusTags) As IGetLocusTag
            Select Case type
                Case GetLocusTags.locus
                    Return AddressOf __locusItSelf
                Case GetLocusTags.UniDOOR
                    Return AddressOf New __DOORHelper(DOOR).UniDOOR
                Case GetLocusTags.DOOR_InitX
                    Return AddressOf New __DOORHelper(DOOR).InitX
                Case Else
                    Throw New NotSupportedException
            End Select
        End Function

        Private Class __DOORHelper

            ReadOnly __DOOR As DOOR

            Sub New(DOOR As DOOR)
                __DOOR = DOOR
            End Sub

            ''' <summary>
            ''' 返回基因以及操纵子的第一个基因，在返回的数据之中操纵子的第一个基因总是在第一个元素之中的
            ''' </summary>
            ''' <param name="locus"></param>
            ''' <returns></returns>
            Public Function UniDOOR(locus As String) As String()
                Return {__initX(locus), locus}
            End Function

            Private Function __initX(locus As String) As String
                Dim gene As OperonGene = __DOOR.GetGene(locus)
                If gene Is Nothing Then
                    Call $"locus_id {locus} not contains in database???".__DEBUG_ECHO
                    Return locus
                End If
                Dim operon As Operon = __DOOR.DOOROperonView.GetOperon(gene.OperonID)
                Dim firstGene As OperonGene = operon.InitialX
                Return firstGene.Synonym
            End Function

            Public Function InitX(locus As String) As String()
                Return {__initX(locus)}
            End Function
        End Class

        Private Function __locusItSelf(s As String) As String()
            Return {s}
        End Function
    End Module
End Namespace
