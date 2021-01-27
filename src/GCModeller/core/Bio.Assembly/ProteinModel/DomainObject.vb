#Region "Microsoft.VisualBasic::744c3ca2a911bb5df37b304d570c99b8, Bio.Assembly\ProteinModel\DomainObject.vb"

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

    '     Class DomainObject
    ' 
    '         Properties: BitScore, EValue, Location, Name, Position
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: CopyTo, GetInteractionDomains, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.NCBI.CDD
Imports SMRUCC.genomics.ComponentModel.Loci
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports System.Xml.Serialization

Namespace ProteinModel

    ''' <summary>
    ''' Domain identifier + Domain Location
    ''' </summary>
    Public Class DomainObject : Inherits SmpFile
        Implements INamedValue
        Implements IMotifDomain

        <XmlAttribute>
        Public Overrides Property Name As String Implements IMotifDomain.Id

        Public Property Position As Location Implements IMotifDomain.location
        Public Property EValue As Double
        Public Property BitScore As String
        ''' <summary>
        ''' 百分比位置
        ''' </summary>
        ''' <returns></returns>
        Public Property Location As Position

        Public Overrides Function ToString() As String
            Return Me.AsMetaString()
        End Function

        ''' <summary>
        ''' 获取与本结构域相互作用的结构域的ID
        ''' </summary>
        ''' <param name="DOMINE"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetInteractionDomains(DOMINE As DOMINE.Database) As String()
            Dim Interactions = DOMINE.Interaction
            Dim LQuery = LinqAPI.Exec(Of String) <=
                From Interaction As DOMINE.Tables.Interaction
                In Interactions
                Let DomainId As String = Interaction.GetInteractionDomain(MyBase.Name)
                Where Not String.IsNullOrEmpty(DomainId)
                Select DomainId '

            Return LQuery
        End Function

        Sub New(SmpFile As SmpFile)
            MyBase.Name = SmpFile.Name
            MyBase.CommonName = SmpFile.CommonName
            MyBase.Describes = SmpFile.Describes
            MyBase.SequenceData = SmpFile.SequenceData
            MyBase.ID = SmpFile.ID
            MyBase.Title = SmpFile.Title
        End Sub

        Public Function CopyTo(Of T As DomainObject)() As T
            Dim Target As T = Activator.CreateInstance(Of T)()
            Target.Name = Name
            Target.BitScore = BitScore
            Target.CommonName = CommonName
            Target.Describes = Describes
            Target.EValue = EValue
            Target.Position = Position
            Target.SequenceData = SequenceData
            Target.ID = ID
            Target.Title = Title

            Return Target
        End Function

        Sub New()
        End Sub
    End Class
End Namespace
