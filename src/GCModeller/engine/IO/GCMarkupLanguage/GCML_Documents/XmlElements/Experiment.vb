#Region "Microsoft.VisualBasic::5f09ad6677ca9c451161e628d3ac89f9, GCModeller\engine\IO\GCMarkupLanguage\GCML_Documents\XmlElements\Experiment.vb"

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

    '   Total Lines: 48
    '    Code Lines: 27
    ' Comment Lines: 13
    '   Blank Lines: 8
    '     File Size: 1.55 KB


    '     Class Experiment
    ' 
    ' 
    '         Enum Types
    ' 
    '             [Mod], ChangeTo, Decay, Decrease, Increase
    '             Multiplying
    ' 
    ' 
    ' 
    '         Class PeriodicBahaviors
    ' 
    '             Properties: Interval, PeriodicBehavior, TargetAction, TICKS, TriggedCondition
    ' 
    '             Function: ToString
    ' 
    '  
    ' 
    '     Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

Namespace GCML_Documents.ComponentModels

    ''' <summary>
    ''' 所有的参数都可以被<see cref="Microsoft.VisualBasic.CommandLine.CommandLine">命令行解析器所解析</see>
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Experiment

        Public Enum Types As Integer
            Increase
            Decrease
            Multiplying
            Decay
            [Mod]
            ChangeTo
        End Enum

        ''' <summary>
        ''' Target Action List
        ''' </summary>
        ''' <remarks></remarks>
        <Column("Actions")> Public Property TargetAction As String

        ''' <summary>
        ''' The start time of this disturb.
        ''' (这个干扰动作的开始时间)
        ''' </summary>
        ''' <remarks></remarks>
        <Column("Trigger")> <XmlAttribute> Public Property TriggedCondition As String
        <Column("PeriodicBehavior")> <XmlAttribute> Public Property PeriodicBehavior As String

        Public Class PeriodicBahaviors
            <XmlAttribute> Public Property TICKS As Integer
            <XmlAttribute> Public Property Interval As Integer

            Public Overrides Function ToString() As String
                Return Me.GetXml
            End Function
        End Class

        Public Overrides Function ToString() As String
            Return TargetAction
        End Function
    End Class
End Namespace
