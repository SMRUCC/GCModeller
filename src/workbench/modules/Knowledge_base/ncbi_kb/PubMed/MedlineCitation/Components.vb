﻿#Region "Microsoft.VisualBasic::63e22542fa9a135caf39a5000b909bef, modules\Knowledge_base\ncbi_kb\PubMed\MedlineCitation\Components.vb"

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

    '   Total Lines: 26
    '    Code Lines: 21 (80.77%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (19.23%)
    '     File Size: 702 B


    '     Class MedlineJournalInfo
    ' 
    '         Properties: Country, ISSNLinking, MedlineTA, NlmUniqueID
    ' 
    '     Class Chemical
    ' 
    '         Properties: NameOfSubstance, RegistryNumber
    ' 
    '     Class RegisterObject
    ' 
    '         Properties: MajorTopicYN, UI, Value
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace PubMed

    Public Class MedlineJournalInfo

        Public Property Country As String
        Public Property MedlineTA As String
        Public Property NlmUniqueID As String
        Public Property ISSNLinking As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class

    Public Class Chemical

        Public Property RegistryNumber As String
        Public Property NameOfSubstance As RegisterObject

        Public Overrides Function ToString() As String
            Return NameOfSubstance.ToString
        End Function

    End Class

    Public Class RegisterObject

        <XmlAttribute>
        Public Property UI As String
        <XmlAttribute>
        Public Property MajorTopicYN As String
        <XmlText>
        Public Property Value As String

        Public Overrides Function ToString() As String
            Return $"[{UI}] {Value}"
        End Function

    End Class
End Namespace
