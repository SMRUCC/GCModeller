#Region "Microsoft.VisualBasic::e6a5f33853e83e357c1eb3c24f3f048c, engine\GCModeller\EngineSystem\Engine\Config\Configurations.vb"

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

    '     Class Configurations
    ' 
    '         Properties: ActionScripts, CommitLoopsInterval, CultivationMediums, DataStorageUrl, DefaultCompartmentId
    '                     DumpData, ExperimentData, ExpressionRegulationNetwork, FilePath, GeneMutations
    '                     Initial_pH, Initial_Temperature, KernelCycles, MetabolismModel, mRNA_LambdaWeight
    '                     rRNA_LambdaWeight, ScriptMountPoint, SuppressErrorMessage, SuppressPeriodicMessage, SuppressWarnMessage
    '                     TriggerScripts, TrimMetabolismMetabolites, tRNA_LambdaWeight
    ' 
    '         Function: DefaultValue, Load, (+2 Overloads) Save, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Oracle.Java.IO.Properties
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Text
Imports System.Text

Namespace EngineSystem.Engine.Configuration

    ''' <summary>
    ''' You should using method <see cref="Configurations.Load"></see> for data loading, because this is from the requirement of the property <see cref="Configurations.FilePath"></see>.
    ''' (对于本对象类型，务必要使用<see cref="Configurations.Load"></see>方法进行加载，因为<see cref="Configurations.FilePath"></see>属性需要在后面记载数据的时候被使用到)
    ''' </summary>
    ''' <remarks></remarks>
    <Comment(
       "--------------------------------------------------------------------------------------------------" & vbCrLf &
       "              Configuration data for gcmodeller virtual cell simulation engine." & vbCrLf &
       "--------------------------------------------------------------------------------------------------", 0)>
    <Comment(
       "If you have any question about the drawing script and this configuration file, " & vbCrLf &
       "please contact the author via: " & vbCrLf &
       "     Gmail:    xie.guigang@gmail.com" & vbCrLf &
       "     Twitter:  @xieguigang(https://twitter.com/xieguigang)", 1)>
    <Comment("Please notice that some configuration data in this configuration data file is as the same as input from " & vbCrLf &
       "the command line arguments does, if you have assign the argument data from the command line, " & vbCrLf &
       "then the commandline argument will override the corresponding configuration in this configuration file.", 3)>
    Public Class Configurations
        Implements ISaveHandle, IFileReference

        ''' <summary>
        ''' 仅针对MYSQL数据存储服务有效的一个配置参数，用于指示计算引擎想数据库服务器提交数据的时间间隔
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>建议模型越大，本属性的值设置得越小，这样子不会出现内存溢出的错误</remarks>
        ''' 
        <Comment(
            "This property is using for the MySQL data storage service, using for specific the " & vbCrLf &
            "calculation data commit cycle interval from the GCModeller to the database server. " & vbCrLf &
            "The larger the model file it is, the smaller this property is, and this may avoid " & vbCrLf &
            "the memory overflow exception if the GCModeller was running on an 32bit platform" & vbCrLf &
            "(as its memory limit is 2 GB for an application and system can not handle the memory " & vbCrLf &
            "large then 3 GB.)", 0)>
        <DumpNode> <XmlAttribute> Public Property CommitLoopsInterval As Integer

        ''' <summary>
        ''' The total simulation time, kernel cycles.(内核循环的次数，即本属性值表示总的模拟计算的时间)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <Comment("This property specifc how long does the gcmodeller running the simulation experiment. And briefly for the total simulation time, kernel cycles.", 0)>
        <DumpNode> <XmlAttribute> Public Property KernelCycles As Long
        ''' <summary>
        ''' Csv data file file path.(CSV格式的实验数据文件的文件路径)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DumpNode> Public Property ExperimentData As String

        <Comment("In the transportation reaction, there are some metabolite is not specific the compartment id, so that this problem " & vbCrLf &
            "will be crash the engine system, so we need a default compartment id value for those meabolite." & vbCrLf &
            "A typical value is the metacyc compartment id: you can set this property of value:  " & vbCrLf &
            "  ""CCO-IN""  - cell metabolism compartment" & vbCrLf &
            "  ""CCO-OUT"" - the cultivation mediums compartment.", 0)>
        Public Property DefaultCompartmentId As String

        ''' <summary>
        ''' 基因突变数据，可以为一个列表或者指向一个Csv文件的文件路径
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Comment("Can be a mutation description list or a file path of the gene mutations data." & vbCrLf &
            "Example for the mutation description list:" & vbCrLf &
            "     GeneId|mutation_factor; GeneId|mutation_factor" & vbCrLf &
            "Example for the mutation data csv file is that" & vbCrLf &
            "Csv HeadLine:   GeneId,Factor,Comment" & vbCrLf &
            "Mutation data:  GeneId1,Factor1,Something to comment", 0)>
        Public Property GeneMutations As String

        Public Property CultivationMediums As String
        Public Property Initial_pH As Double
        Public Property Initial_Temperature As Double

        <Comment("gcmodeller support dynamic load external assembly library as plugin, so that you can using another mathematics model without " & vbCrLf &
            "modify the code of the gcmodeller.", 0)>
        <Comment("The value of this property can be a library assembly file name(if you have not register the model in gcmodeller) " & vbCrLf &
            "or the register name(if you already have register the external mathematics model in the gcmodeller) for the target model.", 1)>
        <Comment("If you don't know what to do, please leave this property value blank.", 2)>
        Public Property ExpressionRegulationNetwork As String
        Public Property MetabolismModel As String

        <Comment("This property is a boolean value, the value format can be(value no sensitive to the character case):   " & vbCrLf &
            "TRUE:    T/True/1/y/Yes" & vbCrLf &
            "False:   F/False/0/n/no" & vbCrLf &
            "Any other value or EMPTY value will be treated as Boolean value FALSE", 0)>
        Public Property TrimMetabolismMetabolites As String
        Public Property SuppressErrorMessage As String
        Public Property SuppressWarnMessage As String
        Public Property SuppressPeriodicMessage As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Comment("There are two type of this url property:" & vbCrLf &
            "CSV://[DirPath] for direct saveing the simulation data into some csv table file in the directory specific " & vbCrLf &
            "by the [DirPath] parameter" & vbCrLf &
            "MYSQL://[MySQL_URL] for transferring the simulation data into a database server, this is much useful for the large data set, " & vbCrLf &
            "and this type of option consumed less memory resource, so that when your computer have a memory less than 4GB," & vbCrLf &
            "please chose this type of storage url.", 0)>
        Public Property DataStorageUrl As String

        <Comment("The relative path of the directory which contains the trigger shellscript using for the experiment system in the gcmodeller.", 0)>
        Public Property TriggerScripts As String
        Public Property ActionScripts As String
        Public Property ScriptMountPoint As String
        Public Property DumpData As String

        'Public Property ATP_Compensate As String

        Public Property mRNA_LambdaWeight As String
        Public Property rRNA_LambdaWeight As String
        Public Property tRNA_LambdaWeight As String
        Public Property FilePath As String Implements IFileReference.FilePath

        Public Overrides Function ToString() As String
            Return Me.ToConfigDoc
        End Function

        Public Shared Function DefaultValue() As Configurations
            Return New Configurations With {
                .CommitLoopsInterval = 1,
                .ExperimentData = "",
                .KernelCycles = 20,
                .DefaultCompartmentId = "CCO-IN",
                .Initial_pH = 7,
                .Initial_Temperature = 28,
                .DataStorageUrl = "CSV://@Desktop/vcell/",
                .TrimMetabolismMetabolites = "true",
                .TriggerScripts = "./Experiments/Triggers/",
                .ActionScripts = "./Experiments/Actions/",
                .ScriptMountPoint = "./Scripts/",
                .SuppressErrorMessage = "True",
                .SuppressWarnMessage = "True",
                .SuppressPeriodicMessage = "True",
                .DumpData = "@Desktop/vcell",
                .mRNA_LambdaWeight = "0.3",
                .rRNA_LambdaWeight = "0.2",
                .tRNA_LambdaWeight = "0.1"
            }
        End Function

        ''' <summary>
        ''' 假若目标文件不存在，则会返回一个默认的文件数据，假若不希望返回默认数据，请将参数<paramref name="returnDefaul"></paramref>设置为False.
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <param name="returnDefaul">当目标文件<paramref name="Path"></paramref>不存在的时候，是否返回默认的配置数据，默认值为返回该默认数据</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Load(Path As String, Optional returnDefaul As Boolean = True) As Configurations
            Dim Data = Path.LoadConfiguration(Of Configurations)()

            If Data Is Nothing Then
                If returnDefaul Then
                    Data = DefaultValue()
                    Call Data.Save(Path)
                Else
                    Return Nothing
                End If
            End If
            Call Data.FilePath.SetValue(Path)
            Return Data
        End Function

        Public Function Save(Path As String, encoding As Text.Encoding) As Boolean Implements ISaveHandle.Save
            Return Me.ToConfigDoc.SaveTo(Path, encoding)
        End Function

        Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(path, encoding.CodePage)
        End Function
    End Class
End Namespace
