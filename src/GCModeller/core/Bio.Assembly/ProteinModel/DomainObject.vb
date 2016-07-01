#Region "Microsoft.VisualBasic::85e190c1e546b07d07c2d1428338bfe5, ..\GCModeller\core\Bio.Assembly\ProteinModel\DomainObject.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.NCBI.CDD
Imports SMRUCC.genomics.ComponentModel.Loci
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace ProteinModel

    ''' <summary>
    ''' Domain identifier + Domain Location
    ''' </summary>
    Public Class DomainObject : Inherits SmpFile
        Implements sIdEnumerable

        Public Property Position As Location
        Public Property EValue As Double
        Public Property BitScore As String
        ''' <summary>
        ''' 百分比位置
        ''' </summary>
        ''' <returns></returns>
        Public Property Location As Position

        Public Overrides Function ToString() As String
            Call Me.Position.Normalization()
            Return $"{Identifier}({Position.Left}|{Position.Right})"
        End Function

        ''' <summary>
        ''' 获取与本结构域相互作用的结构域的ID
        ''' </summary>
        ''' <param name="DOMINE"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetInteractionDomains(DOMINE As DOMINE.Database) As String()
            Dim Interactions = DOMINE.Interaction
            Dim LQuery = From Interaction As DOMINE.Tables.Interaction
                         In Interactions
                         Let DomainId As String = Interaction.GetInteractionDomain(MyBase.Identifier)
                         Where Not String.IsNullOrEmpty(DomainId)
                         Select DomainId '
            Return LQuery.ToArray
        End Function

        Sub New(SmpFile As SmpFile)
            MyBase.Identifier = SmpFile.Identifier
            MyBase.CommonName = SmpFile.CommonName
            MyBase.Describes = SmpFile.Describes
            MyBase.SequenceData = SmpFile.SequenceData
            MyBase.Id = SmpFile.Id
            MyBase.Title = SmpFile.Title
        End Sub

        Public Function CopyTo(Of T As DomainObject)() As T
            Dim Target As T = Activator.CreateInstance(Of T)()
            Target.Identifier = Identifier
            Target.BitScore = BitScore
            Target.CommonName = CommonName
            Target.Describes = Describes
            Target.EValue = EValue
            Target.Position = Position
            Target.SequenceData = SequenceData
            Target.Id = Id
            Target.Title = Title

            Return Target
        End Function

        Sub New()
        End Sub
    End Class
End Namespace
