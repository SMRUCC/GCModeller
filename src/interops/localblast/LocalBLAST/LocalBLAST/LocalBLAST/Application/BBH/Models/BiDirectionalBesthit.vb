#Region "Microsoft.VisualBasic::0e66bf3889ca9b8a39b4376565a9630c, localblast\LocalBLAST\LocalBLAST\LocalBLAST\Application\BBH\Models\BiDirectionalBesthit.vb"

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


' Code Statistics:

'   Total Lines: 97
'    Code Lines: 50 (51.55%)
' Comment Lines: 36 (37.11%)
'    - Xml Docs: 94.44%
' 
'   Blank Lines: 11 (11.34%)
'     File Size: 3.89 KB


'     Class BiDirectionalBesthit
' 
'         Properties: description, forward, identities, length, level
'                     NullValue, positive, reverse, term
' 
'         Function: ShadowCopy, ToString
'         Delegate Function
' 
'             Function: MatchDescription
' 
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models.KeyValuePair
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract

Namespace LocalBLAST.Application.BBH

    ''' <summary>
    ''' Best hit result from the binary direction blastp result.(最佳双向比对结果，BBH，直系同源)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class BiDirectionalBesthit : Inherits I_BlastQueryHit
        Implements IKeyValuePair, IQueryHits

        ''' <summary>
        ''' Functional annotiation for protein <see cref="BiDirectionalBesthit.QueryName"></see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Property description As String

        ''' <summary>
        ''' Category term annotiation for protein <see cref="BiDirectionalBesthit.QueryName"></see>, like COG/KO, etc
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property term As String
        ''' <summary>
        ''' Protein length annotiation for protein <see cref="BiDirectionalBesthit.QueryName"></see>.(本蛋白质实体对象的序列长度)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property length As String
        Public Property level As Levels = Levels.NA

        <Ignored>
        Public ReadOnly Property identities As Double Implements IQueryHits.identities
            Get
                Return (forward + reverse) / 2
            End Get
        End Property

        Public Property positive As Double

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property forward As Double
        Public Property reverse As Double

        Public Shared ReadOnly Property NullValue As BiDirectionalBesthit
            Get
                Return New BiDirectionalBesthit
            End Get
        End Property

        ''' <summary>
        '''
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ToString() As String
            Return String.Format("{0} <==> {1}", QueryName, HitName)
        End Function

        Public Function ShadowCopy(Of T As {New, BiDirectionalBesthit})() As T
            Return New T With {
                .QueryName = QueryName,
                .HitName = HitName,
                .term = term,
                .description = description,
                .length = length
            }
        End Function

        ''' <summary>
        ''' Get gene function description from the specific locus_tag
        ''' </summary>
        ''' <param name="locusId"></param>
        ''' <returns></returns>
        Public Delegate Function GetDescriptionHandle(locusId As String) As String

        Public Shared Function MatchDescription(data As BiDirectionalBesthit(), sourceDescription As GetDescriptionHandle) As BiDirectionalBesthit()
            Dim setValue = New SetValue(Of BiDirectionalBesthit) <= NameOf(BiDirectionalBesthit.description)
            Dim LQuery As BiDirectionalBesthit() =
                LinqAPI.Exec(Of BiDirectionalBesthit) <= From bbh As BiDirectionalBesthit
                                                         In data.AsParallel
                                                         Let desc As String = sourceDescription(locusId:=bbh.QueryName)
                                                         Select setValue(bbh, desc)
            Return LQuery
        End Function
    End Class
End Namespace
