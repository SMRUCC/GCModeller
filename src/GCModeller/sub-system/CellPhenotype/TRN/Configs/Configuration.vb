#Region "Microsoft.VisualBasic::39b3e100b1c82d205bd9734950f62422, sub-system\CellPhenotype\TRN\Configs\Configuration.vb"

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

    '     Class Configuration
    ' 
    '         Properties: BasalThreshold, Enzyme_Decays, OCS_Default_EffectValue, OCS_NONE_Effector, Regulator_Decays
    '                     SiteSpecificDynamicsRegulations
    ' 
    '         Function: DefaultValue, GetConfigures, LoadConfiguration, (+2 Overloads) Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Text
Imports Oracle.Java.IO.Properties

Namespace TRN

    <Comment("This configuration file will configure the dynamics parameter for te DFL network gene object node. changes the paramter value in this file will modify the dynamics behavior of the gene expression regulation network.", 0)>
    <Comment("If you have any question about this configuration file, please contact the author via:" & vbCrLf &
             "    Twitter:   twitter.com/xieguigang" & vbCrLf &
             "    Gmail:     xie.guigang@gmail.com ", 1)>
    <Comment("Please notice that all of the parameter configuring in this configuration file is between 0 to 1.", 2)>
    Public Class Configuration : Implements ISaveHandle

        <Comment("This property settings up the regulator decays rate, the value of the property should be a positive number, the bigger of this property it is, the more fast of the regulator will decays.", 0)>
        Public Property Regulator_Decays As String
        <Comment("This property settings up the enzyme decays rate, the value of the property should be a positive number, the bigger of this property it is, the more fast of the enzyme will decays.", 0)>
        Public Property Enzyme_Decays As String

        ''' <summary>
        ''' 这个参数值调整调控事件的发生概率阈值的高低，则阈值越低，即调控事件越容易发生
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <Comment("The smaller of this property it is, the more easily of the regulation event will happening", 0)>
        Public Property SiteSpecificDynamicsRegulations As String
        ''' <summary>
        ''' 本底表达水平，数值越高，则表达量越高
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <Comment("This property value specific the gene basal expression rate when there is none regulation event happens on the target gene, and this property value should be a positive value, the bigger of this property value it is the more fast of the target gene express will be.", 0)>
        Public Property BasalThreshold As String

        ''' <summary>
        ''' 没有在模型之中找到代谢物的合成的代谢途径，则可能为第二信使或者其他未知的原因，则在模型之中以很低的概率产生调控效应，这个参数配置产生活性的概率的高低
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property OCS_NONE_Effector As String

        ''' <summary>
        ''' 默认的调控值
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <Comment("The bigger of this value, the more bigger weight of the OCS regulator will be.", 0)>
        Public Property OCS_Default_EffectValue As String

        Public Function GetConfigures() As Configs
            Dim data As Configs = LoadMapping(Of Configs, Configuration)(Me)
            Return data
        End Function

        ''' <summary>
        ''' 从文件之中读取配置数据
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MappingsIgnored>
        Public Shared Function LoadConfiguration(Path As String) As Configuration
            Return Oracle.Java.IO.Properties.Properties.Load(Path).FillObject(Of Configuration)()
        End Function

        ''' <summary>
        ''' Get the default configuration data for the DFL network simulator.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DefaultValue() As Configuration
            Return New Configuration With {
                .BasalThreshold = 0.2,
                .Enzyme_Decays = 0.1,
                .Regulator_Decays = 0.2,
                .SiteSpecificDynamicsRegulations = 0.4,
                .OCS_Default_EffectValue = 0.2,
                .OCS_NONE_Effector = 0.2
            }
        End Function

        Public Function Save(Path As String, encoding As Text.Encoding) As Boolean Implements ISaveHandle.Save
            Return Me.ToConfigDoc.SaveTo(Path, encoding)
        End Function

        Public Function Save(Path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(Path, encoding.CodePage)
        End Function
    End Class
End Namespace
